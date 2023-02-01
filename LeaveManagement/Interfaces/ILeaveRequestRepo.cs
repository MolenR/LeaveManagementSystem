using LeaveManagement.Data;
using LeaveManagement.MVC.Models;

namespace LeaveManagement.MVC.Interfaces;

public interface ILeaveRequestRepo : IGenericRepo<LeaveRequest>
{
    Task<bool> CreateLeaveRequest(LeaveRequestCreateViewModel model);
    
    Task<EmployeeLeaveRequestViewModel> GetLeaveDetails();

    Task<LeaveRequestViewModel?> GetLeaveRequestAsync(int id);

    Task<List<LeaveRequest>> GetAllAsync(string employeeId);

    Task ChangeApprovalStatus(int leaveRequestId, bool approved);

    Task CancelLeaveRequest(int? leaveRequestId);

    Task<AdminLeaveRequestViewModel> GetAdminLeaveRequestList();
}
