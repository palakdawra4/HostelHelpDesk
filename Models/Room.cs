namespace WebApplication1.Models
{
    public class Room
    {
        public Guid Id { get; set; }
        public required string RoomNo { get; set; }
        public Hostel Hostel { get; set; }
        public Guid HostelId { get; set; }

    }
}
