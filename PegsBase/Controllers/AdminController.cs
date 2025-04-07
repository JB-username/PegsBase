using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PegsBase.Data;
using PegsBase.Models.Constants;
using PegsBase.Models.Identity;
using System.Diagnostics;
using System.Net.Mail;

namespace PegsBase.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(
            ApplicationDbContext dbContext, 
            IEmailSender emailSender,
            UserManager<ApplicationUser> user,
            RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _emailSender = emailSender;
            _userManager = user;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var whitelist = await _dbContext.WhitelistedEmails.ToListAsync();
            var users = await _userManager.Users.ToListAsync();

            var model = new AdminViewModel
            {
                WhitelistedEmails = whitelist,
                Users = users
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendInvite(string email, string role)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(role))
            {
                TempData["Error"] = "Email and role are both required";
                return RedirectToAction("Index");

            }

            var token = Guid.NewGuid().ToString();

            var invite = new Invite
            {
                Email = email,
                Token = token,
                IsUsed = false,
                CreatedAt = DateTime.UtcNow,
                AssignedRole = role
            };

            _dbContext.Invites.Add(invite);
            await _dbContext.SaveChangesAsync();

            var inviteLink = Url.Page(
                "/Account/Register",
                pageHandler: null,
                values: new { area = "Identity", inviteToken = token },
                protocol: Request.Scheme);

            var subject = "You're invited to join PegsBase!";
            var body = $@"
                        <p>Hello,</p>
                        <p>You are invited to register at PegsBase. Click the link below to get started:</p>
                        <p><a href='{inviteLink}'>{inviteLink}</a></p>
                        <p>If you did not expect this invitation, you can ignore this message.</p>";

            await _emailSender.SendEmailAsync(email, subject, body);

            ViewBag.InviteLink = inviteLink;
            ViewBag.Email = email;

            TempData["Success"] = $"Invite sent to {email}";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddWhitelistedEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                TempData["Error"] = "Email cannot be empty.";
                return RedirectToAction("Index");
            }

            var exists = await _dbContext.WhitelistedEmails.AnyAsync(e => e.Email == email);
            if (exists)
            {
                TempData["Error"] = "Email is already whitelisted.";
                return RedirectToAction("Index");
            }

            _dbContext.WhitelistedEmails.Add(new WhitelistedEmails { Email = email });
            await _dbContext.SaveChangesAsync();

            TempData["Success"] = $"Whitelisted {email}.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveWhitelistedEmail(int id)
        {
            var email = await _dbContext.WhitelistedEmails.FindAsync(id);
            if (email != null)
            {
                _dbContext.WhitelistedEmails.Remove(email);
                await _dbContext.SaveChangesAsync();
                TempData["Success"] = $"Removed {email.Email} from whitelist.";
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin" + "," + "Master")] // or your admin role
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            var roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();

            var userList = new List<AdminUserRow>();

            foreach (var u in users)
            {
                userList.Add(new AdminUserRow
                {
                    Id = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Roles = await _userManager.GetRolesAsync(u),
                    CompanyId = u.CompanyId,
                    JobTitle = u.JobTitle,
                    Department = u.Department,
                    Section = u.Section
                });
            }

            var model = new AdminUsersViewModel
            {
                Users = userList,
                AvailableRoles = roles
            };

            return View(model);
        }


        [Authorize(Roles = "Admin" + "," + "Master")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUserRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var currentRoles = await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, role);

            TempData["Success"] = $"Updated {user.Email} to role {role}";
            return RedirectToAction("Users");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUserInfo(AdminUserRow updatedUser)
        {
            var user = await _userManager.FindByIdAsync(updatedUser.Id);

            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Users");
            }

            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.CompanyId = updatedUser.CompanyId;
            user.JobTitle = updatedUser.JobTitle;

            var result = await _userManager.UpdateAsync(user);

            TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded
                ? "User info updated."
                : string.Join(" ", result.Errors.Select(e => e.Description));

            return RedirectToAction("Users");
        }

    }
}
