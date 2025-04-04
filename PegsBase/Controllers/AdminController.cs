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
    [Authorize(Roles = Roles.Master)]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(
            ApplicationDbContext dbContext, 
            IEmailSender emailSender,
            UserManager<ApplicationUser> user)
        {
            _dbContext = dbContext;
            _emailSender = emailSender;
            _userManager = user;
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

    }
}
