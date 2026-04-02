using Assignment01_EventSignup.Data;
using Assignment01_EventSignup.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment01_EventSignup.Controllers
{
    [Route("events/{eventId}/attendees")]
    public class AttendeesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttendeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
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

        [HttpGet("create")]
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

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int eventId, Attendee attendee)
        {
            var eventItem = await _context.Events.FindAsync(eventId);
            if (eventItem == null)
            {
                return NotFound();
            }

            attendee.EventId = eventId;

            if (!ModelState.IsValid)
            {
                ViewBag.Event = eventItem;
                return View(attendee);
            }

            if (string.IsNullOrWhiteSpace(attendee.Id))
            {
                attendee.Id = Guid.NewGuid().ToString();
            }

            _context.Attendees.Add(attendee);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { eventId });
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int eventId, string id)
        {
            var attendee = await _context.Attendees
                .FirstOrDefaultAsync(a => a.Id == id && a.EventId == eventId);

            var eventItem = await _context.Events.FindAsync(eventId);

            if (attendee == null || eventItem == null)
            {
                return NotFound();
            }

            ViewBag.Event = eventItem;
            return View(attendee);
        }

        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int eventId, string id, Attendee attendee)
        {
            if (id != attendee.Id)
            {
                return NotFound();
            }

            var existingAttendee = await _context.Attendees
                .FirstOrDefaultAsync(a => a.Id == id && a.EventId == eventId);

            var eventItem = await _context.Events.FindAsync(eventId);

            if (existingAttendee == null || eventItem == null)
            {
                return NotFound();
            }

            attendee.EventId = eventId;

            if (!ModelState.IsValid)
            {
                ViewBag.Event = eventItem;
                return View(attendee);
            }

            existingAttendee.Name = attendee.Name;
            existingAttendee.Email = attendee.Email;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { eventId });
        }

        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(int eventId, string id)
        {
            var attendee = await _context.Attendees
                .FirstOrDefaultAsync(a => a.Id == id && a.EventId == eventId);

            var eventItem = await _context.Events.FindAsync(eventId);

            if (attendee == null || eventItem == null)
            {
                return NotFound();
            }

            ViewBag.Event = eventItem;
            return View(attendee);
        }

        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int eventId, string id)
        {
            var attendee = await _context.Attendees
                .FirstOrDefaultAsync(a => a.Id == id && a.EventId == eventId);

            if (attendee == null)
            {
                return NotFound();
            }

            _context.Attendees.Remove(attendee);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { eventId });
        }
    }
}