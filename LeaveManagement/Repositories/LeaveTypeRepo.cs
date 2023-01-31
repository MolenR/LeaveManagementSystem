using LeaveManagement.Data;
using LeaveManagement.MVC.Data;
using LeaveManagement.MVC.Interfaces;

namespace LeaveManagement.MVC.Repositories;

public class LeaveTypeRepo : GenericRepo<LeaveType>, ILeaveTypeRepo
{
    public LeaveTypeRepo(ApplicationDbContext context) : base(context)
    {

    }
}
