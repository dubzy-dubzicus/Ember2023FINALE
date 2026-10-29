using Microsoft.AspNetCore.Mvc;
using Ember2023.Auth;
using Ember2023.Classes.DBs;
using LiteDB;
using System;
using System.Collections.Generic;
using PDBC = Ember2023.Classes.DBs.DBClasses.PlayerDBClasses;

namespace Ember2023.Controllers
{
    [ApiController]
    [Route("/api/moderation")]
    public class ModerationController : ControllerBase
    {
        private static readonly LiteDatabase ModDb =
            new(Path.Combine(Program.dataDir, "DBs", "Moderation.db"));
        private static readonly ILiteCollection<ReportEntry> Reports =
            ModDb.GetCollection<ReportEntry>("Reports");
        private static readonly ILiteCollection<BanEntry> Bans =
            ModDb.GetCollection<BanEntry>("Bans");

        static ModerationController()
        {
            Reports.EnsureIndex(r => r.ReporterAccountId);
            Reports.EnsureIndex(r => r.TargetAccountId);
            Reports.EnsureIndex(r => r.Status);
            Bans.EnsureIndex(b => b.AccountId);
            Bans.EnsureIndex(b => b.BannedBy);
        }

        public class ReportEntry
        {
            [BsonId]
            public string Id { get; set; } = Guid.NewGuid().ToString("N");
            public long ReporterAccountId { get; set; }
            public long TargetAccountId { get; set; }
            public string Reason { get; set; } = "";
            public string Category { get; set; } = "general"; // harassment, cheating, inappropriate_content, spam, other
            public string? RoomId { get; set; }
            public string? EvidenceUrl { get; set; }
            public string Status { get; set; } = "pending"; // pending, reviewed, resolved, dismissed
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime? ReviewedAt { get; set; }
            public long? ReviewedBy { get; set; }
        }

        public class BanEntry
        {
            [BsonId]
            public string Id { get; set; } = Guid.NewGuid().ToString("N");
            public long AccountId { get; set; }
            public long BannedBy { get; set; }
            public string Reason { get; set; } = "";
            public string Type { get; set; } = "temporary"; // temporary, permanent
            public int DurationHours { get; set; } = 24;
            public DateTime BannedAt { get; set; } = DateTime.UtcNow;
            public DateTime? ExpiresAt { get; set; }
            public bool IsActive { get; set; } = true;
        }

        public class CreateReportRequest
        {
            public long TargetAccountId { get; set; }
            public string Reason { get; set; } = "";
            public string Category { get; set; } = "general";
            public string? RoomId { get; set; }
            public string? EvidenceUrl { get; set; }
        }

        [HttpPost("v1/report")]
        public IActionResult CreateReport([FromBody] CreateReportRequest request)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            if (id == request.TargetAccountId)
                return BadRequest(new { error = "Cannot report yourself" });

            var report = new ReportEntry
            {
                ReporterAccountId = (long)id,
                TargetAccountId = request.TargetAccountId,
                Reason = request.Reason,
                Category = request.Category,
                RoomId = request.RoomId,
                EvidenceUrl = request.EvidenceUrl,
                Status = "pending",
                CreatedAt = DateTime.UtcNow
            };
            Reports.Insert(report);

            return Ok(new
            {
                success = true,
                reportId = report.Id,
                status = report.Status
            });
        }

        [HttpGet("v1/reports")]
        public IActionResult GetReports([FromQuery] string status = "pending", [FromQuery] int limit = 50)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var reporter = PlayerDB.Players.FindById((long)id);
            if (reporter == null || reporter.PlayerRoles == null || !reporter.PlayerRoles.Contains(PDBC.PlayerRoles.Developer))
                return Forbid();

            var reports = Reports.Find(r => r.Status == status)
                .OrderByDescending(r => r.CreatedAt)
                .Take(limit)
                .ToList();

            return Ok(new
            {
                Results = reports.Select(r => new
                {
                    id = r.Id,
                    reporterAccountId = r.ReporterAccountId,
                    targetAccountId = r.TargetAccountId,
                    reason = r.Reason,
                    category = r.Category,
                    roomId = r.RoomId,
                    status = r.Status,
                    createdAt = r.CreatedAt.ToString("O")
                }).ToList(),
                TotalResults = reports.Count
            });
        }

        [HttpPost("v1/reports/{reportId}/resolve")]
        public IActionResult ResolveReport(string reportId, [FromBody] ResolveReportRequest request)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var reporter = PlayerDB.Players.FindById((long)id);
            if (reporter == null || reporter.PlayerRoles == null || !reporter.PlayerRoles.Contains(PDBC.PlayerRoles.Developer))
                return Forbid();

            var report = Reports.FindOne(r => r.Id == reportId);
            if (report == null)
                return NotFound();

            report.Status = request.Status ?? "resolved";
            report.ReviewedAt = DateTime.UtcNow;
            report.ReviewedBy = (long)id;
            Reports.Update(report);

            return Ok(new { success = true, reportId = reportId, status = report.Status });
        }

        [HttpPost("v1/ban")]
        public IActionResult BanAccount([FromBody] BanAccountRequest request)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var banner = PlayerDB.Players.FindById((long)id);
            if (banner == null || banner.PlayerRoles == null || !banner.PlayerRoles.Contains(PDBC.PlayerRoles.Developer))
                return Forbid();

            var existing = Bans.FindOne(b => b.AccountId == request.AccountId && b.IsActive);
            if (existing != null)
                return BadRequest(new { error = "Account is already banned" });

            var ban = new BanEntry
            {
                AccountId = request.AccountId,
                BannedBy = (long)id,
                Reason = request.Reason,
                Type = request.Type ?? "temporary",
                DurationHours = request.DurationHours,
                BannedAt = DateTime.UtcNow,
                ExpiresAt = request.Type == "permanent" ? null : DateTime.UtcNow.AddHours(request.DurationHours),
                IsActive = true
            };
            Bans.Insert(ban);

            return Ok(new
            {
                success = true,
                bannedAccountId = request.AccountId,
                banId = ban.Id,
                expiresAt = ban.ExpiresAt?.ToString("O")
            });
        }

        [HttpDelete("v1/ban/{accountId}")]
        public IActionResult UnbanAccount(long accountId)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var banner = PlayerDB.Players.FindById((long)id);
            if (banner == null || banner.PlayerRoles == null || !banner.PlayerRoles.Contains(PDBC.PlayerRoles.Developer))
                return Forbid();

            foreach (var ban in Bans.Find(b => b.AccountId == accountId).ToList())
            {
                ban.IsActive = false;
                Bans.Update(ban);
            }

            return Ok(new { success = true, unbannedAccountId = accountId });
        }

        [HttpGet("v1/isBanned/{accountId}")]
        public IActionResult IsBanned(long accountId)
        {
            var ban = Bans.FindOne(b => b.AccountId == accountId && b.IsActive);
            if (ban == null)
                return Ok(new { isBanned = false });

            if (ban.ExpiresAt.HasValue && ban.ExpiresAt.Value < DateTime.UtcNow)
            {
                ban.IsActive = false;
                Bans.Update(ban);
                return Ok(new { isBanned = false });
            }

            return Ok(new
            {
                isBanned = true,
                reason = ban.Reason,
                type = ban.Type,
                expiresAt = ban.ExpiresAt?.ToString("O"),
                bannedAt = ban.BannedAt.ToString("O")
            });
        }

        public class ResolveReportRequest
        {
            public string? Status { get; set; }
        }

        public class BanAccountRequest
        {
            public long AccountId { get; set; }
            public string Reason { get; set; } = "";
            public string Type { get; set; } = "temporary";
            public int DurationHours { get; set; } = 24;
        }
    }
}
