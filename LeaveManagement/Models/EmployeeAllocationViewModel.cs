namespace LeaveManagement.MVC.Models;

public class EmployeeAllocationViewModel : EmployeeListViewModel
{
    public List<LeaveAllocationViewModel> LeaveAllocations { get; set; }
}
