namespace WebApplication1.DTO
{
    public class AddStudentsDTO: AddUser
    {
        public int RollNo { get; set; }
        public required string HostelName { get; set; }
        public required string RoomNo { get; set; }
    }
}
