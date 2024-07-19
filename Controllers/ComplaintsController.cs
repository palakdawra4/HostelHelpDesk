using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.DTO;
using WebApplication1.Models;
using static WebApplication1.DTO.ComplaintType;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComplaintsController : Controller
    {
        private readonly HostelComplaintsDB _DB;
        public ComplaintsController(HostelComplaintsDB DB)
        {
            _DB = DB;
        }


        [HttpGet("GetAllComplaint")]
        public async Task<IActionResult> GetAllComplaint()
        {
            return Ok(await _DB.Complaints.ToListAsync());
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

                foreach(string type in ComplaintType.GetComplaintType())
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

        [HttpPut("UpdateComplaint"), Authorize(Roles ="worker,student")]
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

        [HttpGet("GetComplaintType")]
        public IActionResult GetComplaintType() 
        {
            return Ok(ComplaintType.GetComplaintType());
        }

        /*[HttpGet("GetWorkerType")]
        public IActionResult GetWorkerType()
        {
            return Ok(ComplaintType.GetWorkerType(value: ComplaintTypeDesc));
        }*/

        [HttpGet("GetTimeSlots")]
        public async Task<IActionResult> GetTimeSlots()
        {
            var timeslots = await _DB.TimeSlots.ToListAsync();
            return Ok(timeslots);
        }

        [HttpPost("AddTimeSlot")]
        public async Task<IActionResult> AddTimeSlot(AddTimeslot timeSlot)
        {
            try
            {
                var timeslot = new TimeSlot()
                {
                    StartTime = TimeOnly.Parse(timeSlot.StartTime),
                    EndTime = TimeOnly.Parse(timeSlot.EndTime)
                };
                await _DB.TimeSlots.AddAsync(timeslot);
                await _DB.SaveChangesAsync();
                return Ok(timeSlot);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }
    }
}
