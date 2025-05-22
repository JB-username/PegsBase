using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PegsBase.Data;
using PegsBase.Models.Subscriptions;
using System;
using System.Net;


namespace PegsBase.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly SubscriptionApiOptions _opts;

        public SubscriptionController(
            ApplicationDbContext db,
            IOptions<SubscriptionApiOptions> opts)
        {
            _db = db;
            _opts = opts.Value;
        }

        bool ValidateKey() =>
            Request.Headers.TryGetValue("X-Api-Key", out var key)
         && key == _opts.ApiKey;

        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] SubscriptionDto dto)
        {
            if (!ValidateKey()) return Unauthorized();

            var sub = await _db.Subscriptions.FindAsync(dto.CompanyId)
                   ?? new Subscription { CompanyId = dto.CompanyId };

            sub.BaseSeats = dto.BaseSeats;
            sub.ExtraSeats = dto.ExtraSeats;
            sub.ExpiresOn = dto.ExpiresOn;
            sub.GraceDays = dto.GraceDays;
            sub.IsActive = dto.IsActive;

            _db.Subscriptions.Update(sub);
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("status/{companyId:guid}")]
        public async Task<IActionResult> Status(Guid companyId)
        {
            if (!ValidateKey()) return Unauthorized();

            var sub = await _db.Subscriptions.FindAsync(companyId);
            if (sub == null) return NotFound();

            return Ok(new SubscriptionDto(sub));
        }
    }

}
