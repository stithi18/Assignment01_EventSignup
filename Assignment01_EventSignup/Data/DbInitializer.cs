using Microsoft.AspNetCore.Identity;

namespace Assignment01_EventSignup.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string[] roles = { "Organizer", "Attendee" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            string organizerEmail = "organizer@test.com";
            string attendeeEmail = "attendee@test.com";
            string password = "Password1!";

            var organizer = await userManager.FindByEmailAsync(organizerEmail);
            if (organizer == null)
            {
                organizer = new IdentityUser
                {
                    UserName = organizerEmail,
                    Email = organizerEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(organizer, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(organizer, "Organizer");
                }
            }
            else if (!await userManager.IsInRoleAsync(organizer, "Organizer"))
            {
                await userManager.AddToRoleAsync(organizer, "Organizer");
            }

            var attendee = await userManager.FindByEmailAsync(attendeeEmail);
            if (attendee == null)
            {
                attendee = new IdentityUser
                {
                    UserName = attendeeEmail,
                    Email = attendeeEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(attendee, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(attendee, "Attendee");
                }
            }
            else if (!await userManager.IsInRoleAsync(attendee, "Attendee"))
            {
                await userManager.AddToRoleAsync(attendee, "Attendee");
            }
        }
    }
}