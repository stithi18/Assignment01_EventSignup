using System.ComponentModel.DataAnnotations;

namespace Assignment01_EventSignup.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        public string Location { get; set; } = string.Empty;

        public List<Attendee> Attendees { get; set; } = new();
    }
}