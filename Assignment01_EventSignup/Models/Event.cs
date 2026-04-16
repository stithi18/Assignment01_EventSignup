using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public DateTime Date { get; set; }

        public string? Location { get; set; }

        public string? BannerImageUrl { get; set; }

        [Required]
        public string OrganizerUserId { get; set; } = string.Empty;

        [ForeignKey(nameof(OrganizerUserId))]
        public ApplicationUser? OrganizerUser { get; set; }

        public ICollection<Attendee> Attendees { get; set; } = new List<Attendee>();
    }
}