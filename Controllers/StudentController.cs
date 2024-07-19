using Microsoft.AspNetCore.Authorization;
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
    public class StudentController : Controller
    {
        private readonly HostelComplaintsDB _DB;
        private readonly Auth _auth;
        public StudentController(HostelComplaintsDB DB, Auth auth)
        {
            _DB = DB;
            _auth = auth;
        }

        [HttpGet("StudentLogin")]
        public async Task<IActionResult> StudentLogin(string Email, string Password)
        {
            User? student = await _DB.Students.FirstOrDefaultAsync(x => x.Email == Email);
            if (student == null)
            {
                return BadRequest("User Not Found");
            }
            if (!_auth.VerifyPassowrd(Password, student.PasswordHash, student.PasswordSalt))
            {
                return BadRequest("Incorrect Password");
            }
            string token = _auth.CreateToken(student.Email, "student");
            return Ok(token);
        }

        [HttpGet("GetAllStudentComplaint")]
        public async Task<IActionResult> GetAllStudentComplaint(string email)
        {
            var stu = await _DB.Students.FirstOrDefaultAsync(x => x.Email == email);
            var complaints = await _DB.Complaints.Where(s => s.StudentId == stu.Id).ToListAsync();
            List<GetComplaintDTO> complaintDTOs = new List<GetComplaintDTO>();
            foreach (var complaint in complaints)
            {
                var student = await _DB.Students.FindAsync(complaint.StudentId);
                var worker = await _DB.Workers.FindAsync(complaint.WorkerId);
                var caretaker = await _DB.Caretakers.FindAsync(complaint.CaretakerId);
                var hostel = await _DB.Hostels.FindAsync(complaint.HostelId);
                var timeslot = await _DB.TimeSlots.FindAsync(complaint.TimeSlotId);

                var complaintDTO = new GetComplaintDTO()
                {
                    complaint_no = complaint.Complaint_No,
                    w_name = worker.FirstName,
                    c_name = caretaker.FirstName,
                    type = complaint.Type,
                    description = complaint.Description,
                    timeslot = timeslot.StartTime.ToString("HH:mm")
                    // Add other complaint properties as needed
                };
                complaintDTOs.Add(complaintDTO);
            }

            return Ok(complaintDTOs);
        }

        [HttpPost("RaiseComplaint"), Authorize(Roles = "student")]
        public async Task<IActionResult> RaiseComplaint(AddComplaint addComplaint)
        {
            try
            {
                var stu = await _DB.Students.FindAsync(addComplaint.StudentId);
                if (stu == null)
                {
                    throw new ArgumentException("Student not found");
                }

                var hId = stu.HostelId;

                var caretaker = await _DB.Caretakers.FirstOrDefaultAsync(c => c.HostelId == hId);
                if (caretaker == null)
                {
                    throw new ArgumentException("No caretaker found for the hostel");
                }

                foreach (string type in ComplaintType.GetComplaintType())
                {
                    if (type == addComplaint.Type)
                    {
                        var request = new Complaint()
                        {
                            Id = Guid.NewGuid(),
                            Complaint_No = new int(),
                            StudentId = addComplaint.StudentId,
                            WorkerId = Guid.NewGuid(),
                            CaretakerId = caretaker.Id,
                            TimeSlotId = addComplaint.TimeSlotId,
                            Description = addComplaint.Description,
                            Type = ComplaintType.GetWorkerType(addComplaint.Type),
                            HostelId = hId,
                            RoomId = stu.RoomId
                        };
                        await _DB.Complaints.AddAsync(request);
                        await _DB.SaveChangesAsync();
                        return Ok(request);
                    }
                }
                return BadRequest("Invadid Complaint Type");


            }
            catch (ArgumentException ex)
            {
                // Handle specific exceptions
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                // Handle other unexpected exceptions
                return StatusCode(500, "An error occurred while processing your request");
            }
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
