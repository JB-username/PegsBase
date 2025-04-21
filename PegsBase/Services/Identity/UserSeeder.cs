using Microsoft.AspNetCore.Identity;
using PegsBase.Models.Constants;
using PegsBase.Models.Identity;

namespace PegsBase.Services.Identity
{
    public class UserSeeder
    {
        public static async Task SeedUsersAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            await CreateUserWithRole(userManager, "jb@ampelongames.com", "Master@123!", Roles.Master, "Johan", "Bender");
        }

        private static async Task CreateUserWithRole(
            UserManager<ApplicationUser> userManager,
            string email,
            string password,
            string role,
            string firstName,
            string lastName)
        {
            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new ApplicationUser
                {
                    Email = email,
                    EmailConfirmed = true,
                    UserName = email,
                    FirstName = firstName,
                    LastName = lastName
                };

                user.GenerateNormalizedName(); // ✅ Call after setting name

                var result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
                else
                {
                    throw new Exception($"Failed creating user with email {user.Email}. Errors: {string.Join(",", result.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}
