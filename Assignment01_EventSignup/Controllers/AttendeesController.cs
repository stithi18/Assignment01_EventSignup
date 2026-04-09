using Assignment01_EventSignup.Data;
using Assignment01_EventSignup.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment01_EventSignup.Controllers
{
    public class AttendeesController : Controller
    {
        private readonly AppDbContext _context;

        public AttendeesController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize]
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
            if (ModelState.IsValid)
            {
                _context.Attendees.Add(attendee);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Events", new { id = attendee.EventId });
            }

            ViewBag.Event = await _context.Events.FindAsync(attendee.EventId);
            return View(attendee);
        }

        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> Delete(int id)
        {
            var attendee = await _context.Attendees
                .Include(a => a.Event)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (attendee == null)
            {
                return NotFound();
            }

            return View(attendee);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var attendee = await _context.Attendees.FindAsync(id);
            if (attendee == null)
            {
                return NotFound();
            }

            int eventId = attendee.EventId;

            _context.Attendees.Remove(attendee);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Events", new { id = eventId });
        }
    }
}