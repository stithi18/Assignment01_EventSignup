using Microsoft.AspNetCore.Mvc;
using Assignment01_EventSignup.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assignment01_EventSignup.Controllers
{
    public class EventController : Controller
    {
        // Simulated database (hardcoded events)
        private static List<Event> events = new List<Event>
        {
            new Event
            {
                Id = 1,
                Title = "Career Fair",
                Date = new DateTime(2026, 2, 1),
                Location = "Gym"
            },
            new Event
            {
                Id = 2,
                Title = "Tech Talk",
                Date = new DateTime(2026, 2, 8),
                Location = "Auditorium"
            },
            new Event
            {
                Id = 3,
                Title = "Hack Night",
                Date = new DateTime(2026, 2, 15),
                Location = "Library"
            }
        };

        // STEP 4: Event Manager page
        public IActionResult Index()
        {
            return View(events);
        }

        // STEP 6: Manage Attendees (GET)
        public IActionResult Manage(int id)
        {
            var selectedEvent = events.FirstOrDefault(e => e.Id == id);

            if (selectedEvent == null)
            {
                return NotFound();
            }

            return View(selectedEvent);
        }

        // STEP 8: Signup Attendee (POST)
        [HttpPost]
        public IActionResult Signup(int eventId, string name, string email)
        {
            var selectedEvent = events.FirstOrDefault(e => e.Id == eventId);

            if (selectedEvent != null)
            {
                selectedEvent.Attendees.Add(new Attendee
                {
                    Name = name,
                    Email = email
                });

                TempData["Message"] = "Attendee registered successfully!";
            }

            return RedirectToAction("Manage", new { id = eventId });
        }
    }
}
