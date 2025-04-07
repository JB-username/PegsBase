using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PegsBase.Data;
using PegsBase.Models;
using PegsBase.Models.Enums;
using PegsBase.Models.Identity;
using PegsBase.Models.JobRequests;
using PegsBase.Models.Settings;
using PegsBase.Models.ViewModels;
using System.Security.Claims;

[Authorize]
public class JobRequestsController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly IOptions<ClientSettings> _clientSettings;

    public JobRequestsController(
        ApplicationDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        IEmailSender emailSender,
        IOptions<ClientSettings> clientSettings)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _emailSender = emailSender;
        _clientSettings = clientSettings;
    }

    public async Task<IActionResult> Index()
    {
        var jobRequests = await _dbContext.JobRequests
            .Include(j => j.CreatedBy)
            .Include(j => j.AssignedTo)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();

        return View(jobRequests);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var users = await _userManager.Users.ToListAsync();
        ViewBag.Users = new SelectList(users, "Id", "Email"); // or FirstName + LastName if you prefer
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateJobRequestViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var users = await _userManager.Users.ToListAsync();
            ViewBag.Users = new SelectList(users, "Id", "Email");
            return View(model);
        }

        var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var job = new JobRequest
        {
            Subject = model.Subject,
            Description = model.Description,
            AssignedToUserId = model.AssignedToUserId,
            TargetDepartment = model.TargetDepartment,
            TargetSection = model.TargetSection,
            CreatedByUserId = senderId,
            CreatedAt = DateTime.UtcNow,
            Status = JobStatus.Pending
        };

        _dbContext.JobRequests.Add(job);
        await _dbContext.SaveChangesAsync();

        // Determine recipients
        var recipients = new List<ApplicationUser>();

        if (!string.IsNullOrEmpty(model.AssignedToUserId))
        {
            var user = await _userManager.FindByIdAsync(model.AssignedToUserId);
            if (user != null) recipients.Add(user);
        }
        else
        {
            // Match department and/or section
            recipients = await _userManager.Users
                .Where(u =>
                    (string.IsNullOrEmpty(model.TargetDepartment) || u.Department == model.TargetDepartment) &&
                    (string.IsNullOrEmpty(model.TargetSection) || u.Section == model.TargetSection))
                .ToListAsync();
        }

        // Filter by whitelisted emails
        var whitelistedEmails = await _dbContext.WhitelistedEmails
            .Select(e => e.Email.ToLower())
            .ToListAsync();

        foreach (var user in recipients)
        {
            if (!whitelistedEmails.Contains(user.Email.ToLower()))
                continue;

            var subject = $"📋 New Job Request: {model.Subject}";
            var message = $@"
                <p>You have received a new job request:</p>
                <p><strong>Subject:</strong> {model.Subject}</p>
                <p><strong>Description:</strong><br />{model.Description}</p>
                <p>This was assigned to you by {User.Identity?.Name}.</p>";

            await _emailSender.SendEmailAsync(user.Email, subject, message);
        }

        TempData["Success"] = "Job request created and notifications sent.";
        return RedirectToAction("MyRequests"); // or "Index" if that's your dashboard
    }
}

