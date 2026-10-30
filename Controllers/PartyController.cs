using Microsoft.AspNetCore.Mvc;
using Ember2023.Auth;
using Ember2023.Classes.DBs;
using LiteDB;
using System;
using System.Collections.Generic;

namespace Ember2023.Controllers
{
    [ApiController]
    [Route("/api/party")]
    public class PartyController : ControllerBase
    {
        private static readonly LiteDatabase PartyDb =
            new(Path.Combine(Program.dataDir, "DBs", "Parties.db"));
        private static readonly ILiteCollection<PartyEntry> Parties =
            PartyDb.GetCollection<PartyEntry>("Parties");
        private static readonly ILiteCollection<PartyMemberEntry> PartyMembers =
            PartyDb.GetCollection<PartyMemberEntry>("PartyMembers");

        static PartyController()
        {
            Parties.EnsureIndex(p => p.LeaderAccountId);
            PartyMembers.EnsureIndex(pm => pm.PartyId);
            PartyMembers.EnsureIndex(pm => pm.AccountId);
            PartyInvites.EnsureIndex(i => i.PartyId);
            PartyInvites.EnsureIndex(i => i.InvitedAccountId);
            PartyInvites.EnsureIndex(i => i.InviterAccountId);
        }

        public class PartyEntry
        {
            [BsonId]
            public string Id { get; set; } = Guid.NewGuid().ToString("N");
            public string PartyId { get; set; } = Guid.NewGuid().ToString("N");
            public long LeaderAccountId { get; set; }
            public string Name { get; set; } = "";
            public bool IsPublic { get; set; } = true;
            public int MaxMembers { get; set; } = 8;
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        }

        public class PartyMemberEntry
        {
            [BsonId]
            public string Id { get; set; } = Guid.NewGuid().ToString("N");
            public string PartyId { get; set; } = "";
            public long AccountId { get; set; }
            public string Role { get; set; } = "member";
            public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        }

        public class CreatePartyRequest
        {
            public string? Name { get; set; }
            public bool IsPublic { get; set; } = true;
            public int MaxMembers { get; set; } = 8;
        }

        [HttpGet("v1/mine")]
        public IActionResult GetMyParty()
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var membership = PartyMembers.FindOne(pm => pm.AccountId == (long)id);
            if (membership == null)
                return Ok(new { inParty = false });

            var party = Parties.FindOne(p => p.PartyId == membership.PartyId);
            if (party == null)
                return NotFound();

            var members = PartyMembers.Find(pm => pm.PartyId == party.PartyId).ToList();
            var memberDetails = new List<object>();

            foreach (var m in members)
            {
                var player = PlayerDB.Players.FindById(m.AccountId);
                memberDetails.Add(new
                {
                    accountId = m.AccountId,
                    username = player?.Player?.Username ?? "Unknown",
                    role = m.Role,
                    joinedAt = m.JoinedAt.ToString("O")
                });
            }

            return Ok(new
            {
                inParty = true,
                partyId = party.PartyId,
                name = party.Name,
                isPublic = party.IsPublic,
                leaderAccountId = party.LeaderAccountId,
                maxMembers = party.MaxMembers,
                memberCount = members.Count,
                members = memberDetails
            });
        }

        [HttpPost("v1/create")]
        public IActionResult CreateParty([FromBody] CreatePartyRequest request)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            long accountId = (long)id;

            // Leave existing party first
            var existing = PartyMembers.FindOne(pm => pm.AccountId == accountId);
            if (existing != null)
            {
                Parties.DeleteMany(p => p.PartyId == existing.PartyId);
                PartyMembers.DeleteMany(pm => pm.PartyId == existing.PartyId);
            }

            var party = new PartyEntry
            {
                LeaderAccountId = accountId,
                Name = request.Name ?? "Ember Party",
                IsPublic = request.IsPublic,
                MaxMembers = request.MaxMembers
            };
            Parties.Insert(party);

            PartyMembers.Insert(new PartyMemberEntry
            {
                PartyId = party.PartyId,
                AccountId = accountId,
                Role = "leader",
                JoinedAt = DateTime.UtcNow
            });

            return Ok(new
            {
                partyId = party.PartyId,
                name = party.Name,
                isPublic = party.IsPublic,
                leaderAccountId = accountId,
                maxMembers = party.MaxMembers
            });
        }

        [HttpPost("v1/join/{partyId}")]
        public IActionResult JoinParty(string partyId)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            long accountId = (long)id;

            var party = Parties.FindOne(p => p.PartyId == partyId);
            if (party == null)
                return NotFound();

            var existing = PartyMembers.FindOne(pm => pm.PartyId == partyId && pm.AccountId == accountId);
            if (existing != null)
                return Ok(new { success = true, alreadyMember = true });

            var memberCount = PartyMembers.Count(pm => pm.PartyId == partyId);
            if (memberCount >= party.MaxMembers)
                return BadRequest(new { error = "Party is full" });

            PartyMembers.Insert(new PartyMemberEntry
            {
                PartyId = partyId,
                AccountId = accountId,
                Role = "member",
                JoinedAt = DateTime.UtcNow
            });

            return Ok(new { success = true, partyId = partyId });
        }

        [HttpPost("v1/leave")]
        public IActionResult LeaveParty()
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            long accountId = (long)id;

            var membership = PartyMembers.FindOne(pm => pm.AccountId == accountId);
            if (membership == null)
                return Ok(new { success = true, notInParty = true });

            var party = Parties.FindOne(p => p.PartyId == membership.PartyId);
            if (party == null)
                return NotFound();

            PartyMembers.DeleteMany(pm => pm.PartyId == membership.PartyId && pm.AccountId == accountId);

            // If leader left, transfer or disband
            if (party.LeaderAccountId == accountId)
            {
                var remaining = PartyMembers.Find(pm => pm.PartyId == membership.PartyId).ToList();
                if (remaining.Count > 0)
                {
                    var newLeader = remaining[0];
                    newLeader.Role = "leader";
                    PartyMembers.Update(newLeader);
                    party.LeaderAccountId = newLeader.AccountId;
                    Parties.Update(party);
                }
                else
                {
                    Parties.DeleteMany(p => p.PartyId == membership.PartyId);
                    PartyMembers.DeleteMany(pm => pm.PartyId == membership.PartyId);
                }
            }

            return Ok(new { success = true });
        }

        [HttpDelete("v1/kick/{targetAccountId}")]
        public IActionResult KickMember(long targetAccountId)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            long callerId = (long)id;

            var membership = PartyMembers.FindOne(pm => pm.AccountId == callerId);
            if (membership == null)
                return BadRequest(new { error = "Not in a party" });

            var party = Parties.FindOne(p => p.PartyId == membership.PartyId);
            if (party == null || party.LeaderAccountId != callerId)
                return Forbid();

            PartyMembers.DeleteMany(pm => pm.PartyId == membership.PartyId && pm.AccountId == targetAccountId);

            return Ok(new { success = true, kickedAccountId = targetAccountId });
        }

        [HttpGet("v1/list")]
        public IActionResult ListPublicParties()
        {
            var parties = Parties.Find(p => p.IsPublic).ToList();
            var results = new List<object>();

            foreach (var p in parties)
            {
                var memberCount = PartyMembers.Count(pm => pm.PartyId == p.PartyId);
                var leader = PlayerDB.Players.FindById(p.LeaderAccountId);
                results.Add(new
                {
                    partyId = p.PartyId,
                    name = p.Name,
                    leaderAccountId = p.LeaderAccountId,
                    leaderUsername = leader?.Player?.Username ?? "Unknown",
                    memberCount = memberCount,
                    maxMembers = p.MaxMembers,
                    isPublic = p.IsPublic
                });
            }

            return Ok(new { Results = results, TotalResults = results.Count });
        }

        // Private LiteDB collection for pending party invites
        private static readonly LiteDatabase InviteDb =
            new(Path.Combine(Program.dataDir, "DBs", "PartyInvites.db"));
        private static readonly ILiteCollection<PartyInviteEntry> PartyInvites =
            InviteDb.GetCollection<PartyInviteEntry>("PartyInvites");

        public class PartyInviteEntry
        {
            [BsonId]
            public string Id { get; set; } = Guid.NewGuid().ToString("N");
            public string InviteId { get; set; } = Guid.NewGuid().ToString("N");
            public string PartyId { get; set; } = "";
            public long InviterAccountId { get; set; }
            public long InvitedAccountId { get; set; }
            public string Status { get; set; } = "pending"; // pending, accepted, declined, expired
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddMinutes(10);
        }

        public class InvitePlayerRequest
        {
            public string PartyId { get; set; } = "";
            public long TargetAccountId { get; set; }
        }

        public class InviteResponseRequest
        {
            public string InviteId { get; set; } = "";
            public bool Accept { get; set; }
        }

        [HttpPost("v1/invite")]
        public IActionResult InvitePlayer([FromBody] InvitePlayerRequest request)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            long inviterId = (long)id;
            var party = Parties.FindOne(p => p.PartyId == request.PartyId);
            if (party == null)
                return NotFound(new { error = "Party not found" });

            if (party.LeaderAccountId != inviterId)
                return StatusCode(403, new { error = "Only party leader can invite" });

            var memberCount = PartyMembers.Count(pm => pm.PartyId == request.PartyId);
            if (memberCount >= party.MaxMembers)
                return BadRequest(new { error = "Party is full" });

            var targetPlayer = PlayerDB.Players.FindById(request.TargetAccountId);
            if (targetPlayer == null)
                return NotFound(new { error = "Target player not found" });

            // Cancel any existing pending invite
            PartyInvites.DeleteMany(i => i.PartyId == request.PartyId && i.InvitedAccountId == request.TargetAccountId && i.Status == "pending");

            var invite = new PartyInviteEntry
            {
                PartyId = request.PartyId,
                InviterAccountId = inviterId,
                InvitedAccountId = request.TargetAccountId,
                Status = "pending",
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            };
            PartyInvites.Insert(invite);

            return Ok(new
            {
                success = true,
                inviteId = invite.InviteId,
                partyId = request.PartyId,
                invitedAccountId = request.TargetAccountId,
                expiresAt = invite.ExpiresAt.ToString("O")
            });
        }

        [HttpGet("v1/invites/pending")]
        public IActionResult GetPendingInvites()
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            long accountId = (long)id;
            var invites = PartyInvites.Find(i => i.InvitedAccountId == accountId && i.Status == "pending" && i.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(i => i.CreatedAt)
                .ToList();

            return Ok(new
            {
                Results = invites.Select(i => new
                {
                    inviteId = i.InviteId,
                    partyId = i.PartyId,
                    inviterAccountId = i.InviterAccountId,
                    inviterUsername = PlayerDB.Players.FindById(i.InviterAccountId)?.Player?.Username ?? "Unknown",
                    createdAt = i.CreatedAt.ToString("O"),
                    expiresAt = i.ExpiresAt.ToString("O")
                }).ToList(),
                TotalResults = invites.Count
            });
        }

        [HttpPost("v1/invite/respond")]
        public IActionResult RespondToInvite([FromBody] InviteResponseRequest request)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            long accountId = (long)id;
            var invite = PartyInvites.FindOne(i => i.InviteId == request.InviteId && i.InvitedAccountId == accountId && i.Status == "pending");

            if (invite == null)
                return NotFound(new { error = "Invite not found or already responded" });

            if (invite.ExpiresAt < DateTime.UtcNow)
            {
                invite.Status = "expired";
                PartyInvites.Update(invite);
                return BadRequest(new { error = "Invite has expired" });
            }

            if (request.Accept)
            {
                var party = Parties.FindOne(p => p.PartyId == invite.PartyId);
                if (party == null)
                {
                    invite.Status = "declined";
                    PartyInvites.Update(invite);
                    return NotFound(new { error = "Party not found" });
                }

                var memberCount = PartyMembers.Count(pm => pm.PartyId == invite.PartyId);
                if (memberCount >= party.MaxMembers)
                {
                    invite.Status = "declined";
                    PartyInvites.Update(invite);
                    return BadRequest(new { error = "Party is full" });
                }

                var existing = PartyMembers.FindOne(pm => pm.PartyId == invite.PartyId && pm.AccountId == accountId);
                if (existing != null)
                {
                    invite.Status = "declined";
                    PartyInvites.Update(invite);
                    return BadRequest(new { error = "Already a member of this party" });
                }

                PartyMembers.Insert(new PartyMemberEntry
                {
                    PartyId = invite.PartyId,
                    AccountId = accountId,
                    Role = "member",
                    JoinedAt = DateTime.UtcNow
                });

                invite.Status = "accepted";
                PartyInvites.Update(invite);

                return Ok(new
                {
                    success = true,
                    partyId = invite.PartyId,
                    action = "accepted"
                });
            }
            else
            {
                invite.Status = "declined";
                PartyInvites.Update(invite);

                return Ok(new
                {
                    success = true,
                    inviteId = invite.InviteId,
                    action = "declined"
                });
            }
        }
    }
}
