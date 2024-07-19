using WebApplication1.Models;

namespace WebApplication1.DTO
{
    public class AddComplaint
    {
        public Guid StudentId { get; set; }
        public required string Type { get; set; }
        public int TimeSlotId { get; set; }
        public string? Description { get; set; }
    }
}
