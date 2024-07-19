using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTO
{
    public class AddTimeslot
    {
        [RegularExpression(@"^([01]?[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Invalid time format")]
        public string StartTime {  get; set; }
        [RegularExpression(@"^([01]?[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Invalid time format")]
        public string EndTime { get; set; }
    }
}
