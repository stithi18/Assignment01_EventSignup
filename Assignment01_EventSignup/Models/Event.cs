using System.ComponentModel.DataAnnotations;

namespace Assignment01_EventSignup.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(150)]
        public string Location { get; set; } = string.Empty;

        public string? BannerUrl { get; set; }

        public List<Attendee> Attendees { get; set; } = new List<Attendee>();
    }
}