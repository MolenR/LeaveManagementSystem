using AutoMapper;
using LeaveManagement.Data;
using LeaveManagement.MVC.Data;
using LeaveManagement.MVC.Interfaces;
using LeaveManagement.MVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.MVC.Repositories;

public class LeaveRequestRepo : GenericRepo<LeaveRequest>, ILeaveRequestRepo
{
    private new readonly ApplicationDbContext context;
    private readonly IMapper mapper;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ILeaveAllocationRepo leaveAllocationRepo;
    //private readonly AutoMapper.IConfigurationProvider configurationProvider;
    private readonly IEmailSender emailSender;
    private readonly UserManager<Employee> userManager;

    public LeaveRequestRepo(
        ApplicationDbContext context, 
        IMapper mapper, 
        IHttpContextAccessor httpContextAccessor,
        ILeaveAllocationRepo leaveAllocationRepo,
        //AutoMapper.IConfigurationProvider configurationProvider,
        IEmailSender emailSender,
        UserManager<Employee> userManager
        ) : base(context)
    {
        this.context = context;
        this.mapper = mapper;
        this.httpContextAccessor = httpContextAccessor;
        this.leaveAllocationRepo = leaveAllocationRepo;
        //this.configurationProvider = configurationProvider;
        this.emailSender = emailSender;
        this.userManager = userManager;
    }

    public async Task ChangeApprovalStatus(int leaveRequestId, bool approved)
    {
        var leaveRequest = await GetAsync(leaveRequestId);
        
        if(leaveRequest == null)
        {
            return;
        }

        leaveRequest.Approved = approved; /* NULL REFERNCE EXCEPTION ID=0 */
        
        if (approved)
        {
            var allocation = await leaveAllocationRepo.GetEmployeeAllocation(leaveRequest.RequestEmployeeId, leaveRequest.LeaveTypeId);
            
            int daysRequested = (int)(leaveRequest.EndDate - leaveRequest.StartDate).TotalDays;
            
            allocation.NumberOfDays -= daysRequested; /* NULL REFERNCE EXCEPTION ID=0 */

            await leaveAllocationRepo.UpdateAsync(allocation);
        }

        await UpdateAsync(leaveRequest);

        var user = await userManager.FindByIdAsync(leaveRequest.RequestEmployeeId);

        var approvalStatus = approved ? "Approved" : "Declined";
       
        await emailSender.SendEmailAsync(user.Email,
            $"Your request from: {leaveRequest.StartDate} till {leaveRequest.EndDate}",
            $"Leave Request: {approvalStatus}.");
    }

    public async Task<bool> CreateLeaveRequest(LeaveRequestCreateViewModel model)
    {
        var user = await userManager.GetUserAsync(httpContextAccessor.HttpContext.User);

        var leaveAllocation = await leaveAllocationRepo.GetEmployeeAllocation(user.Id, model.LeaveTypeId);
        
        if(leaveAllocation == null)
        {
            return false;
        }
        
        int daysRequested = (int)(model.EndDate.Value - model.StartDate.Value).TotalDays;

        if(daysRequested > leaveAllocation.NumberOfDays) 
        {
            return false;
        }   

        var leaveRequest = mapper.Map<LeaveRequest>(model);
        leaveRequest.DateRequested = DateTime.Now;
        leaveRequest.RequestEmployeeId = user.Id;

        await AddAsync(leaveRequest);

        await emailSender.SendEmailAsync(user.Email,
            "Leave Request Submitted.",
            $"Leave request from: {leaveRequest.StartDate} untill {leaveRequest.EndDate}");

        return true;
    }
    
    public async Task CancelLeaveRequest(int leaveRequestId)
    {
        var leaveRequest = await GetAsync(leaveRequestId);

        if(leaveRequest == null)
        {
            return;
        }

        leaveRequest.Cancelled = true;
        
        await UpdateAsync(leaveRequest);

        var user = await userManager.FindByIdAsync(leaveRequest.RequestEmployeeId);

        await emailSender.SendEmailAsync(user.Email,
            $"Request from: {leaveRequest.StartDate} till {leaveRequest.EndDate}",
            "Leave Request Cancelled.");
    }

    public async Task<AdminLeaveRequestViewModel> GetAdminLeaveRequestList()
    {
        var leaveRequests = await context.LeaveRequests.Include(request => request.LeaveType).ToListAsync();
        var model = new AdminLeaveRequestViewModel
        {
            TotalRequests = leaveRequests.Count,
            ApprovedRequests = leaveRequests.Count(approve => approve.Approved == true),
            PendingRequests = leaveRequests.Count(pending => pending.Approved == null),
            RejectedRequests = leaveRequests.Count(rejected => rejected.Approved == false),
            LeaveRequests = mapper.Map<List<LeaveRequestViewModel>>(leaveRequests)
        };

        foreach(var leaveRequest in model.LeaveRequests)
        {
            string? requestEmployeeId = leaveRequest.RequestEmployeeId;
            leaveRequest.Employee = mapper.Map<EmployeeListViewModel>(await userManager.FindByIdAsync(requestEmployeeId));
        }

        return model;
    }

    public async Task<List<LeaveRequest>> GetAllAsync(string employeeId)
    {
        return await context.LeaveRequests.Where(q => q.RequestEmployeeId == employeeId).ToListAsync();
    }


    public async Task<LeaveRequestViewModel?> GetLeaveRequestAsync(int? id)
    {
        var leaveRequest = await context.LeaveRequests
            .Include(leave => leave.LeaveType)
            .FirstOrDefaultAsync(request => request.Id == id);
        
        if(leaveRequest == null )
        {
            return null;
        }

        var model = mapper.Map<LeaveRequestViewModel>(leaveRequest);
        
        model.Employee = mapper.Map<EmployeeListViewModel>(await userManager
            .FindByIdAsync(leaveRequest.RequestEmployeeId));

        return model;
    }
    
    public async Task<EmployeeLeaveRequestViewModel> GetLeaveDetails()
    {
        var user = await userManager.GetUserAsync(httpContextAccessor.HttpContext.User);
        var allocations = (await leaveAllocationRepo.GetEmployeeAllocations(user.Id)).LeaveAllocations;
        var requests = mapper.Map<List<LeaveRequestViewModel>>(await GetAllAsync(user.Id));

        var model = new EmployeeLeaveRequestViewModel(allocations, requests);

        return model;
    }

}
