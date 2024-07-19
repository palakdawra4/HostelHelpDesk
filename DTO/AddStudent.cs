using WebApplication1.Models;

namespace WebApplication1.DTO
{
    public class AddStudent: AddUser
    {
        public int RollNo { get; set; }
        public Guid HostelId { get; set; }
        public Guid RoomId { get; set; }
    }
}
