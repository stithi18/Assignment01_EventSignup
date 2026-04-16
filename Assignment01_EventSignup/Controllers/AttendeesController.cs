using Assignment01_EventSignup.Data;
using Assignment01_EventSignup.Hubs;
using Assignment01_EventSignup.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Assignment01_EventSignup.Controllers
{
    public class AttendeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<EventHub> _hubContext;

        public AttendeesController(ApplicationDbContext context, IHubContext<EventHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> Index(int eventId)
        {
            var eventItem = await _context.Events
                .Include(e => e.Attendees)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (eventItem == null)
            {
                return NotFound();
            }

            ViewBag.Event = eventItem;
            return View(eventItem.Attendees.ToList());
        }

        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> Create(int eventId)
        {
            var eventItem = await _context.Events.FindAsync(eventId);
            if (eventItem == null)
            {
                return NotFound();
            }

            ViewBag.Event = eventItem;
            return View(new Attendee { EventId = eventId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> Create(Attendee attendee)
        {
            var eventItem = await _context.Events.FindAsync(attendee.EventId);
            if (eventItem == null)
            {
                return NotFound();
            }

            // Assign UserId before validation because it is required in the model
            attendee.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

            // Remove old validation error for UserId since it is not entered in the form
            ModelState.Remove("UserId");

            if (!ModelState.IsValid)
            {
                ViewBag.Event = eventItem;
                return View(attendee);
            }

            _context.Attendees.Add(attendee);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { eventId = attendee.EventId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Register(int eventId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? "unknown@test.com";
            var fullName = User.Identity?.Name ?? email;

            if (userId == null)
            {
                return Challenge();
            }

            var eventItem = await _context.Events
                .Include(e => e.Attendees)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (eventItem == null)
            {
                return NotFound();
            }

            var alreadyRegistered = await _context.Attendees
                .AnyAsync(a => a.EventId == eventId && a.UserId == userId);

            if (alreadyRegistered)
            {
                TempData["Error"] = "You are already registered for this event.";
                return RedirectToAction("Details", "Events", new { id = eventId });
            }

            var attendee = new Attendee
            {
                EventId = eventId,
                UserId = userId,
                Email = email,
                FullName = fullName
            };

            _context.Attendees.Add(attendee);
            await _context.SaveChangesAsync();

            var updatedAttendees = await _context.Attendees
                .Where(a => a.EventId == eventId)
                .Select(a => new { a.FullName, a.Email })
                .ToListAsync();

            await _hubContext.Clients.Group($"event-{eventId}")
                .SendAsync("AttendeeUpdated", new
                {
                    attendeeCount = updatedAttendees.Count,
                    attendees = updatedAttendees,
                    newAttendeeName = attendee.FullName
                });

            await _hubContext.Clients.User(eventItem.OrganizerUserId)
                .SendAsync("OrganizerNotification",
                    $"{attendee.Email} just registered for your {eventItem.Title}.");

            TempData["Success"] = "Successfully registered.";
            return RedirectToAction("Details", "Events", new { id = eventId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Unregister(int eventId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Challenge();
            }

            var attendee = await _context.Attendees
                .FirstOrDefaultAsync(a => a.EventId == eventId && a.UserId == userId);

            if (attendee == null)
            {
                TempData["Error"] = "Registration record not found.";
                return RedirectToAction("Details", "Events", new { id = eventId });
            }

            _context.Attendees.Remove(attendee);
            await _context.SaveChangesAsync();

            var updatedAttendees = await _context.Attendees
                .Where(a => a.EventId == eventId)
                .Select(a => new { a.FullName, a.Email })
                .ToListAsync();

            await _hubContext.Clients.Group($"event-{eventId}")
                .SendAsync("AttendeeUpdated", new
                {
                    attendeeCount = updatedAttendees.Count,
                    attendees = updatedAttendees,
                    newAttendeeName = ""
                });

            TempData["Success"] = "Successfully unregistered.";
            return RedirectToAction("Details", "Events", new { id = eventId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> Delete(int id)
        {
            var attendee = await _context.Attendees.FindAsync(id);
            if (attendee == null)
            {
                return NotFound();
            }

            int eventId = attendee.EventId;

            _context.Attendees.Remove(attendee);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { eventId });
        }
    }
}