using Assignment01_EventSignup.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Assignment01_EventSignup.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // LOCAL FIX:
            // Delete broken local DB and recreate all tables fresh, including Identity tables.
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            string[] roles = { "Organizer", "Attendee" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            const string organizerEmail = "organizer@test.com";
            const string attendeeEmail = "attendee@test.com";
            const string password = "Password123!";

            var organizerUser = await userManager.FindByEmailAsync(organizerEmail);
            if (organizerUser == null)
            {
                organizerUser = new ApplicationUser
                {
                    UserName = organizerEmail,
                    Email = organizerEmail,
                    EmailConfirmed = true
                };

                var createOrganizer = await userManager.CreateAsync(organizerUser, password);
                if (createOrganizer.Succeeded)
                {
                    await userManager.AddToRoleAsync(organizerUser, "Organizer");
                }
            }
            else if (!await userManager.IsInRoleAsync(organizerUser, "Organizer"))
            {
                await userManager.AddToRoleAsync(organizerUser, "Organizer");
            }

            var attendeeUser = await userManager.FindByEmailAsync(attendeeEmail);
            if (attendeeUser == null)
            {
                attendeeUser = new ApplicationUser
                {
                    UserName = attendeeEmail,
                    Email = attendeeEmail,
                    EmailConfirmed = true
                };

                var createAttendee = await userManager.CreateAsync(attendeeUser, password);
                if (createAttendee.Succeeded)
                {
                    await userManager.AddToRoleAsync(attendeeUser, "Attendee");
                }
            }
            else if (!await userManager.IsInRoleAsync(attendeeUser, "Attendee"))
            {
                await userManager.AddToRoleAsync(attendeeUser, "Attendee");
            }

            if (!await context.Events.AnyAsync() && organizerUser != null)
            {
                context.Events.AddRange(
                    new Event
                    {
                        Title = "Tech Conference 2026",
                        Description = "Annual conference for developers.",
                        Date = DateTime.Now.AddDays(10),
                        Location = "Ottawa",
                        BannerImageUrl = "https://placehold.co/900x300?text=Tech+Conference",
                        OrganizerUserId = organizerUser.Id
                    },
                    new Event
                    {
                        Title = "Career Networking Night",
                        Description = "Meet professionals and students.",
                        Date = DateTime.Now.AddDays(20),
                        Location = "Toronto",
                        BannerImageUrl = "https://placehold.co/900x300?text=Networking+Night",
                        OrganizerUserId = organizerUser.Id
                    }
                );

                await context.SaveChangesAsync();
            }
        }
    }
}