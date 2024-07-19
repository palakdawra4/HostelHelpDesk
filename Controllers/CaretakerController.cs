using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.DTO;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CaretakerController : Controller
    {
        private readonly HostelComplaintsDB _DB;
        private readonly Auth _auth;
        public CaretakerController(HostelComplaintsDB DB, Auth auth)
        {
            _DB = DB;
            _auth = auth;
        }

        [HttpGet("CaretakerLogin")]
        public async Task<IActionResult> CaretakerLogin(string Email, string Password)
        {
            User? caretaker = await _DB.Caretakers.FirstOrDefaultAsync(x => x.Email == Email);
            if (caretaker == null)
            {
                return BadRequest("User Not Found");
            }
            if (!_auth.VerifyPassowrd(Password, caretaker.PasswordHash, caretaker.PasswordSalt))
            {
                return BadRequest("Incorrect Password");
            }
            string token = _auth.CreateToken(caretaker.Email, "caretaker");
            return Ok(token);
        }

        [HttpGet("GetAllCaretakerCompaint")]
        public async Task<ActionResult<GetCaretakerComplaints[]>> GetAllCaretakerCompaint(string email)
        {
            var caretaker = await _DB.Caretakers.FirstOrDefaultAsync(s => s.Email == email);
            var complaints = await _DB.Complaints.Where(a => a.WorkerId == caretaker.Id).ToListAsync();
            List<GetCaretakerComplaints> getCaretakerComplaints = new List<GetCaretakerComplaints>();
            foreach (var complaint in complaints)
            {
                var worker = await _DB.Workers.FirstOrDefaultAsync(c => c.Id == complaint.WorkerId);
                var stu = await _DB.Students.FirstOrDefaultAsync(c => c.Id == complaint.StudentId);
                var room = await _DB.Rooms.FirstOrDefaultAsync(x => x.Id == complaint.RoomId);
                var timeslot = await _DB.TimeSlots.FindAsync(complaint.TimeSlotId);
                var comp = new GetCaretakerComplaints()
                {
                    Complaint_No = complaint.Complaint_No,
                    Worker = worker.FirstName,
                    StudentName = stu.FirstName,
                    RoomNo = room.RoomNo,
                    Description = complaint.Description,
                    status = complaint.Status,
                    timeslot = timeslot.StartTime.ToString("HH:mm")

                };
                getCaretakerComplaints.Add(comp);
            }
            return Ok(getCaretakerComplaints);
        }



    }
}
