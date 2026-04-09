using System.ComponentModel.DataAnnotations;

namespace Assignment01_EventSignup.Models
{
    public class Attendee
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public int EventId { get; set; }
        public Event? Event { get; set; }
    }
}