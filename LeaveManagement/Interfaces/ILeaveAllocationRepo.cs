using LeaveManagement.MVC.Data;
using LeaveManagement.MVC.Models;

namespace LeaveManagement.MVC.Interfaces;

public interface ILeaveAllocationRepo : IGenericRepo<LeaveAllocation>
{
    Task LeaveAllocation(int leaveTypeId);
    Task<bool> AllocationExists(string employeeId, int leaveTypeId, int period);
    Task<EmployeeAllocationViewModel> GetEmployeeAllocations(string employeeId);
    Task<LeaveAllocation> GetEmployeeAllocation(string employeeId, int leaveTypeId);
    Task<LeaveAllocationEditViewModel> GetEmployeeAllocation(int id);
    Task<bool> UpdateEmployeeAllocation(LeaveAllocationEditViewModel model);
}
