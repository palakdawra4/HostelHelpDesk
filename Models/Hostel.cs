using Microsoft.AspNetCore.Antiforgery;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Hostel
    {
        public Guid Id { get; set; } 
        public required string HostelName { get; set; }
    }
}
