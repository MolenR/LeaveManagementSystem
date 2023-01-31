using AutoMapper;
using LeaveManagement.Data;
using LeaveManagement.MVC.Constants;
using LeaveManagement.MVC.Data;
using LeaveManagement.MVC.Interfaces;
using LeaveManagement.MVC.Models;
using LeaveManagement.MVC.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace LeaveManagement.MVC.Repositories;

public class LeaveAllocationRepo : GenericRepo<LeaveAllocation>, ILeaveAllocationRepo
{
    private new readonly ApplicationDbContext context;
    private readonly UserManager<Employee> userManager;
    private readonly ILeaveTypeRepo leaveTypeRepo;
    private readonly IEmailSender emailSender;
    private readonly IMapper mapper;

    public LeaveAllocationRepo(ApplicationDbContext context,
        UserManager<Employee> userManager,
        ILeaveTypeRepo leaveTypeRepo,
        IEmailSender emailSender,
        IMapper mapper) : base(context)
    {
        this.context = context;
        this.userManager = userManager;
        this.leaveTypeRepo = leaveTypeRepo;
        this.emailSender = emailSender;
        this.mapper = mapper;
    }

    public async Task<bool> AllocationExists(string employeeId, int leaveTypeId, int period)
    {
        return await context.LeaveAllocations
            .AnyAsync(
                q => q.EmployeeId == employeeId && 
                q.LeaveTypeId == leaveTypeId && 
                q.Period == period
            );
    }

    public async Task<EmployeeAllocationViewModel> GetEmployeeAllocations(string employeeId)
    {
        var allocations = await context.LeaveAllocations
            .Include(q => q.LeaveType)
            .Where(q => q.EmployeeId == employeeId)
            .ToListAsync();
        
        var employee = await userManager.FindByIdAsync(employeeId);

        var employeeAllocationModel = mapper.Map<EmployeeAllocationViewModel>(employee);
        employeeAllocationModel.LeaveAllocations = mapper.Map<List<LeaveAllocationViewModel>>(allocations);

        return employeeAllocationModel;
    }

    public async Task<LeaveAllocationEditViewModel> GetEmployeeAllocation(int id)
    {
        var allocation = await context.LeaveAllocations
            .Include(q => q.LeaveType)
            .FirstOrDefaultAsync(q => q.Id == id);

        if(allocation == null)
        {
            return null;
        }

        var employee = await userManager.FindByIdAsync(allocation.EmployeeId);

        var model = mapper.Map<LeaveAllocationEditViewModel>(allocation);
        
        model.Employee = mapper.Map<EmployeeListViewModel>(await userManager.FindByIdAsync(allocation.EmployeeId));

        return model;
    }

    /**/
    public async Task LeaveAllocation(int leaveTypeId)
    {
        var employees = await userManager.GetUsersInRoleAsync(Roles.User);
        var period = DateTime.Now.Year;
        var leaveType = await leaveTypeRepo.GetAsync(leaveTypeId);
        var allocations = new List<LeaveAllocation>();
        var employeeNewAllocations = new List<Employee>();

        foreach (var employee in employees)
        {
            if (await AllocationExists(employee.Id, leaveTypeId, period))
                continue;

            allocations.Add(new LeaveAllocation
            {
                EmployeeId = employee.Id,
                LeaveTypeId = leaveTypeId,
                Period = period,
                NumberOfDays = leaveType.DefaultDays
            });
            employeeNewAllocations.Add(employee);

            await AddRangeAsync(allocations);
        }


        foreach (var employee in employeeNewAllocations)
        {
            await emailSender.SendEmailAsync(employee.Email,
            $"Leave ALlocation Posted for {period}", $"Your {leaveType.Name} are set to: {leaveType.DefaultDays}");
        }
    }

    public async Task<bool> UpdateEmployeeAllocation(LeaveAllocationEditViewModel model)
    {
        var leaveAllocation = await GetAsync(model.Id);

        if (leaveAllocation == null)
        {
            return false;
        }

        leaveAllocation.Period = model.Period;
        leaveAllocation.NumberOfDays = model.NumberOfDays;
        await UpdateAsync(leaveAllocation);

        return true;
    }

    public async Task<LeaveAllocation> GetEmployeeAllocation(string employeeId, int leaveTypeId)
    {
        return await context.LeaveAllocations
            .FirstOrDefaultAsync(
                get => get.EmployeeId == employeeId && 
                get.LeaveTypeId == leaveTypeId
            );
    }
}
