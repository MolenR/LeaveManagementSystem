using System.ComponentModel.DataAnnotations.Schema;

namespace LeaveManagement.MVC.Data;

public class LeaveAllocation : BaseData
{
    public int NumberOfDays { get; set; }

    [ForeignKey("LeaveTypeId")]
    public LeaveType LeaveType { get; set; } = null!;
    public int LeaveTypeId { get; set; }
    public string EmployeeId { get; set; } = null!;
    public int Period { get; set; }
}
