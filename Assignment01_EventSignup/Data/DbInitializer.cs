using Assignment01_EventSignup.Models;
using Microsoft.EntityFrameworkCore;

namespace Assignment01_EventSignup.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.Migrate();

            if (context.Events.Any())
            {
                return;
            }

            var events = new List<Event>
            {
                new Event
                {
                    Title = "Routing Workshop",
                    Description = "Learn attribute routing and MVC patterns.",
                    Date = DateTime.Now.AddDays(7),
                    Location = "Algonquin College - T Building",
                    BannerUrl = "https://via.placeholder.com/900x300.png?text=Routing+Workshop"
                },
                new Event
                {
                    Title = "Tech Conference 2026",
                    Description = "A full-day conference about cloud, AI, and enterprise apps.",
                    Date = DateTime.Now.AddDays(17),
                    Location = "Ottawa Convention Centre",
                    BannerUrl = "https://via.placeholder.com/900x300.png?text=Tech+Conference+2026"
                },
                new Event
                {
                    Title = "EF Core Bootcamp",
                    Description = "Hands-on EF Core, Azure SQL and CRUD development.",
                    Date = DateTime.Now.AddDays(27),
                    Location = "Online",
                    BannerUrl = "https://via.placeholder.com/900x300.png?text=EF+Core+Bootcamp"
                }
            };

            context.Events.AddRange(events);
            context.SaveChanges();

            var attendees = new List<Attendee>
            {
                new Attendee { Name = "Alice Smith", Email = "alice@example.com", EventId = events[0].Id },
                new Attendee { Name = "Bob Jones", Email = "bob@example.com", EventId = events[0].Id },

                new Attendee { Name = "Charlie Brown", Email = "charlie@example.com", EventId = events[1].Id },
                new Attendee { Name = "Diana Prince", Email = "diana@example.com", EventId = events[1].Id },

                new Attendee { Name = "Ethan Hunt", Email = "ethan@example.com", EventId = events[2].Id },
                new Attendee { Name = "Fiona Green", Email = "fiona@example.com", EventId = events[2].Id }
            };

            context.Attendees.AddRange(attendees);
            context.SaveChanges();
        }
    }
}