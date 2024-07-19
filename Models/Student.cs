namespace WebApplication1.Models
{
    public class Student: User
    {
        public int RollNo { get; set; }
        public Hostel? Hostel { get; set; }
        public Guid HostelId { get; set; }
        public Room? Room { get; set; }
        public Guid RoomId { get; set; }
    }
}
