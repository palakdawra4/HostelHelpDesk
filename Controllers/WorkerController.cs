using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.DTO;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    public class WorkerController : Controller
    {
        private readonly HostelComplaintsDB _DB;
        private readonly Auth _auth;

        public WorkerController(HostelComplaintsDB DB, Auth auth)
        {
            _DB = DB;
            _auth = auth;
        }

        [HttpGet("WorkerLogin")]
        public async Task<IActionResult> WorkerLogin(string Email, string Password)
        {
            User? worker = await _DB.Workers.FirstOrDefaultAsync(x => x.Email == Email);
            if (worker == null)
            {
                return BadRequest("User Not Found");
            }
            if (!_auth.VerifyPassowrd(Password, worker.PasswordHash, worker.PasswordSalt))
            {
                return BadRequest("Incorrect Password");
            }
            string token = _auth.CreateToken(worker.Email, "worker");
            return Ok(token);
        }

        [HttpGet("GetAllWorkerCompaint")]
        public async Task<ActionResult<GetWorkerComplaints[]>> GetAllWorkerCompaint(string email)
        {
            var worker = await _DB.Workers.FirstOrDefaultAsync(s => s.Email == email);
            var complaints = await _DB.Complaints.Where(a => a.WorkerId == worker.Id).ToListAsync();
            List<GetWorkerComplaints> getWorkerComplaints = new List<GetWorkerComplaints>();
            foreach (var complaint in complaints)
            {
                var hostel = await _DB.Hostels.FirstOrDefaultAsync(c => c.Id == complaint.HostelId);
                var stu = await _DB.Students.FirstOrDefaultAsync(c => c.Id == complaint.StudentId);
                var room = await _DB.Rooms.FirstOrDefaultAsync(x => x.Id == complaint.RoomId);
                var timeslot = await _DB.TimeSlots.FindAsync(complaint.TimeSlotId);
                var comp = new GetWorkerComplaints()
                {
                    Complaint_No = complaint.Complaint_No,
                    HostelName = hostel.HostelName,
                    StudentName = stu.FirstName,
                    RoomNo = room.RoomNo,
                    Description = complaint.Description,
                    status = complaint.Status,
                    timeslot = timeslot.StartTime.ToString("HH:mm")
                };
                getWorkerComplaints.Add(comp);
            }
            return Ok(getWorkerComplaints);
        }

        [HttpPut("UpdateComplaint"), Authorize(Roles = "worker,student")]
        public async Task<IActionResult> UpdateComplaint(Guid complaintId, bool status)
        {
            Complaint? complaint = await _DB.Complaints.FirstOrDefaultAsync(c => c.Id == complaintId);

            if (complaint == null)
            {
                return NotFound();
            }
            if (status)
            {
                complaint.Status = ComplaintStatus.GetStatus(ComplaintStatus.ComplaintStatusDesc.CS102);
            }
            else
            {
                complaint.Status = ComplaintStatus.GetStatus(ComplaintStatus.ComplaintStatusDesc.CS101);
            }

            await _DB.SaveChangesAsync();

            return Ok();
        }

    }
}
