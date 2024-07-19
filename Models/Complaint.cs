using System.Text.Json.Serialization;
using WebApplication1.DTO;

namespace WebApplication1.Models
{
    public class Complaint
    {
        public Guid Id { get; set; }
        public int Complaint_No { get; set; }
        public Student Student { get; set; }
        public Guid StudentId { get; set; }
        public Worker Worker { get; set; }
        public Guid WorkerId { get; set; }
        public Caretaker Caretaker { get; set; }
        public Guid CaretakerId { get; set; }
        public Hostel Hostel { get; set; }
        public Guid HostelId { get; set; }
        public Room Room { get; set; }
        public Guid RoomId { get; set; }
        public required string Type { get; set; }
        public TimeSlot TimeSlot { get; set; }
        public int TimeSlotId { get; set; }
        public string? Description { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Completed { get; set; }
        public string Status { get; set; } = ComplaintStatus.GetStatus(ComplaintStatus.ComplaintStatusDesc.CS101);
    }
}
