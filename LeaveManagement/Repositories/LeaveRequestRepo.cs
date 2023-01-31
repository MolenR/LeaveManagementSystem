﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
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
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILeaveAllocationRepo _leaveAllocationRepo;
    private readonly AutoMapper.IConfigurationProvider _configurationProvider;
    private readonly IEmailSender _emailSender;
    private readonly UserManager<Employee> _userManager;

    public LeaveRequestRepo(
        ApplicationDbContext context, 
        IMapper mapper, 
        IHttpContextAccessor httpContextAccessor,
        ILeaveAllocationRepo leaveAllocationRepo,
        AutoMapper.IConfigurationProvider configurationProvider,
        IEmailSender emailSender,
        UserManager<Employee> userManager
        ) : base(context)
    {
        _context = context;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _leaveAllocationRepo = leaveAllocationRepo;
        _configurationProvider = configurationProvider;
        _emailSender = emailSender;
        _userManager = userManager;
    }

    public async Task ChangeApprovalStatus(int leaveRequestId, bool approved)
    {
        var leaveRequest = await GetAsync(leaveRequestId);
        leaveRequest.Approved = approved;
        
        if (approved)
        {
            var allocation = await _leaveAllocationRepo.GetEmployeeAllocation(leaveRequest.RequestEmployeeId, leaveRequest.LeaveTypeId);
            int daysRequested = (int)(leaveRequest.EndDate - leaveRequest.StartDate).TotalDays;
            allocation.NumberOfDays -= daysRequested;
            await _leaveAllocationRepo.UpdateAsync(allocation);
        }

        await UpdateAsync(leaveRequest);

        var user = await _userManager.FindByIdAsync(leaveRequest.RequestEmployeeId);

        var approvalStatus = approved ? "Approved" : "Declined";
       
        await _emailSender.SendEmailAsync(user?.Email,
            $"Your request from: {leaveRequest.StartDate} till {leaveRequest.EndDate}",
            $"Leave Request: {approvalStatus}.");
    }

    public async Task<bool> CreateLeaveRequest(LeaveRequestCreateViewModel model)
    {
        var user = await _userManager.GetUserAsync(_httpContextAccessor?.HttpContext?.User);
        var leaveAllocation = await _leaveAllocationRepo.GetEmployeeAllocation(user?.Id, model.LeaveTypeId);
        
        int daysRequested = (int)(model.EndDate.Value - model.StartDate.Value).TotalDays;

        if(daysRequested > leaveAllocation?.NumberOfDays) 
        {
            return false;
        }   

        var leaveRequest = _mapper.Map<LeaveRequest>(model);
        leaveRequest.DateRequested = DateTime.Now;
        leaveRequest.RequestEmployeeId = user.Id;

        await AddAsync(leaveRequest);

        await _emailSender.SendEmailAsync(user?.Email,
            "Leave Request Submitted.",
            $"Leave request from: {leaveRequest.StartDate} untill {leaveRequest.EndDate}");

        return true;
    }
    
    public async Task CancelLeaveRequest(int leaveRequestId)
    {
        var leaveRequest = await GetAsync(leaveRequestId);

        leaveRequest.Cancelled = true;
        
        await UpdateAsync(leaveRequest);

        var user = await _userManager.FindByIdAsync(leaveRequest?.RequestEmployeeId);

        await _emailSender.SendEmailAsync(user?.Email,
            $"Request from: {leaveRequest.StartDate} till {leaveRequest.EndDate}",
            "Leave Request Cancelled.");
    }

    public async Task<AdminLeaveRequestViewModel> GetAdminLeaveRequestList()
    {
        var leaveRequests = await _context.LeaveRequests
            .Include(request => request.LeaveType)
            .ProjectTo<LeaveRequestViewModel>(_configurationProvider)
            .ToListAsync();
        
        var model = new AdminLeaveRequestViewModel
        {
            TotalRequests = leaveRequests.Count,
            ApprovedRequests = leaveRequests.Count(approve => approve.Approved == true),
            PendingRequests = leaveRequests.Count(pending => pending.Approved == null),
            RejectedRequests = leaveRequests.Count(rejected => rejected.Approved == false),
            LeaveRequests = leaveRequests
        };

        foreach(var leaveRequest in model.LeaveRequests)
        {
            string? requestEmployeeId = leaveRequest.RequestEmployeeId;
            leaveRequest.Employee = _mapper.Map<EmployeeListViewModel>(await _userManager.FindByIdAsync(requestEmployeeId));
        }

        return model;
    }

    public async Task<List<LeaveRequest>> GetAllAsync(string employeeId)
    {
        return await _context.LeaveRequests.Where(q => q.RequestEmployeeId == employeeId)
            .ToListAsync();
    }


    public async Task<LeaveRequestViewModel?> GetLeaveRequestAsync(int? id)
    {
        LeaveRequest? leaveRequest = await _context.LeaveRequests
            .Include(leave => leave.LeaveType)
            .FirstOrDefaultAsync(request => request.Id == id);

        var model = _mapper.Map<LeaveRequestViewModel>(leaveRequest);
        
        model.Employee = _mapper.Map<EmployeeListViewModel>(await _userManager
            .FindByIdAsync(leaveRequest.RequestEmployeeId));

        return model;
    }
    
    public async Task<EmployeeLeaveRequestViewModel> GetLeaveDetails()
    {
        Employee? user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
        var allocations = (await _leaveAllocationRepo.GetEmployeeAllocations(user.Id)).LeaveAllocations;
        var requests = _mapper.Map<List<LeaveRequestViewModel>>(await GetAllAsync(user.Id));

        var model = new EmployeeLeaveRequestViewModel(allocations, requests);

        return model;
    }

}
