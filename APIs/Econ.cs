using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace asdfasdf.APIs;

public static class Econ
{
    private static List<OutfitSaveRequest> outfits;
    
    public static void Init(WebApplication app)
    {
        if (!File.Exists("ServerData/Accounts/2/Outfits.json"))
        {
            File.WriteAllText("ServerData/Accounts/2/Outfits.json", "[]");
        }

        outfits = JsonSerializer.Deserialize<List<OutfitSaveRequest>>(File.ReadAllText("ServerData/Accounts/2/Outfits.json"));
        
        app.MapGet("/api/avatar/v1/defaultunlocked", () =>
        {
            return File.ReadAllText("avataritems.json");
        });

        app.MapGet("/api/timeLimitedEvents/cbe9582c-6314-4ba3-ad0e-57d220b79aff", () =>
        {
            app.Logger.LogWarning("(econ) /api/timeLimitedEvents/cbe9582c-6314-4ba3-ad0e-57d220b79aff STUB");
            
            Guid ReplicationId = Guid.NewGuid();

            // TimedEventXpSource xpSource = new();
            // TimedEventXpMultiplier xpMultiplier = new()
            // {
            //     XpMultiplier = 2
            // };
            //
            // TimeLimitedEvent limitedEvent = new()
            // {
            //     TimeLimitedEventId = 0,
            //     ReplicationId = ReplicationId,
            //     Name = "An event",
            //     StartTime = DateTime.UtcNow,
            //     EndTime = DateTime.UtcNow.AddHours(24),
            //     Metadata = "stub",
            //     Config = new()
            //     {
            //         CurrencyGrant = new()
            //         {
            //             TimedCurrencyGrantId = 0
            //         },
            //         XpSinks = [new()
            //         {
            //             XpSources = new(new []{xpSource}),
            //             XpMultipliers = new( new []{xpMultiplier}),
            //             GiftMessage = "A Gift"
            //         }],
            //         TeamConfig = new()
            //         {
            //             TimeLimitedEventTeamConfigId = 0,
            //             TimeLimitedEventTeamConfig = new()
            //             {
            //                 TimeLimitedEventTeamConfigId = 0,
            //                 ReplicationId = ReplicationId,
            //                 MaxPlayersPerTeam = 4,
            //                 TeamRegistrationStartTime = DateTime.UtcNow,
            //                 TeamRegistrationEndTime = DateTime.UtcNow.AddHours(24),
            //                 ActiveExperiment = "experiment"
            //             }
            //         }
            //     }
            // };

            return File.ReadAllText("event_cbe9582c-6314-4ba3-ad0e-57d220b79aff.json");
        });

        app.MapGet("/api/avatar/v4/items", () => File.ReadAllText("avataritems.json"));
        app.MapGet("/api/avatar/v1/defaultbaseavataritems", () => File.ReadAllText("defaultbaseavataritems.json"));

        app.MapGet("/api/storefronts/v4/balance/{type}", (int type) =>
        {
            app.Logger.LogWarning("(econ) /api/storefronts/v4/balance/500 STUB");

            List<BalanceResponse> responses = new();
            
            BalanceResponse response = new()
            {
                Balance = 1000,
                CurrencyType = -type,
                BalanceType = StorefrontBalanceType.SteamPurchased
            };
            
            responses.Add(response);
            
            return responses;
        });

        app.MapGet("/api/roomkeys/v1/room", (int roomId) =>
        {
            app.Logger.LogWarning("(econ) /api/roomkeys/v1/room STUB");

            return "[]";
        });

        app.MapGet("/econ/customAvatarItems/v1/owned", (int skip, int take) =>
        {
            app.Logger.LogWarning("(econ) /econ/customAvatarItems/v1/owned STUB");

            OwnedCustomAvatarItems owned = new()
            {
                TotalResults = 0,
                Results = Array.Empty<object>()
            };

            return owned;
        });

        app.MapGet("/api/avatar/v2", (HttpRequest request) =>
        {
            foreach (var user in Auth.currentUsers)
            {
                if (user.token == request.Headers.Authorization[0])
                {
                    return File.ReadAllText("ServerData/Accounts/" + user.id + "/Avatar.json");
                }
            }

            return "{}";
            // return Results.NotFound();
        });

        app.MapGet("/api/objectives/v1/myprogress", () =>
        {
            app.Logger.LogWarning("(econ) /api/objectives/v1/myprogress STUB");

            List<ObjectiveGroup> objectiveGroups = new();
            List<Objective> objectives = new();

            ObjectiveGroup group = new()
            {
                ClearedAt = DateTime.Now.AddYears(1),
                Group = 0,
                IsCompleted = false,
                RequiresCompleteOnServer = false
            };

            Objective objective = new()
            {
                Group = 0,
                HasClaimedReward = false,
                Index = 0,
                IsCompleted = false,
                IsRewarded = false,
                Progress = 0,
                VisualProgress = 0,
            };

            objectiveGroups.Add(group);
            objectives.Add(objective);

            MyProgress myProgress = new()
            {
                ObjectiveGroups = objectiveGroups,
                Objectives = objectives
            };

            return myProgress;
        });

        app.MapPost("/api/objectives/v1/updateobjective", () =>
        {
            app.Logger.LogWarning("(econ) /api/objectives/v1/updateobjective STUB");

            return "";
        });

        app.MapGet("/api/storefronts/v3/giftdropstore/{id}", () =>
        {
            app.Logger.LogWarning("(econ) /api/storefronts/v3/giftdropstore/3 STUB");

            return File.ReadAllText("giftdrop.json");
        });

        app.MapGet("/api/checklist/v1/current", () =>
        {
            app.Logger.LogWarning("(econ) /api/checklist/v1/current STUB");

            List<ChecklistObjective> objectives = new();

            ChecklistObjective objective = new()
            {
                Order = 0,
                Objective = ObjectiveType.AddAFriend,
                Count = 1
            };
            ChecklistObjective objective2 = new()
            {
                Order = 1,
                Objective = ObjectiveType.CheerARoom,
                Count = 5
            };
            
            objectives.Add(objective);
            objectives.Add(objective2);

            return objectives;
        });

        app.MapGet("/api/challenge/v2/getCurrent", () =>
        {
            app.Logger.LogWarning("(econ) /api/challenge/v2/getCurrent STUB");

            return File.ReadAllText("weeklychallenge.json");
        });

        app.MapGet("/api/avatar/v3/saved", (HttpRequest request) =>
        {
            foreach (var user in Auth.currentUsers)
            {
                if (user.token == request.Headers.Authorization[0])
                {
                    return Results.Ok(user.outfits);
                }
            }
            
            return Results.NotFound();
        });

        app.MapPost("/api/avatar/v3/saved/set", ([FromBody] OutfitSaveRequest request, HttpRequest hrequest) =>
        {
            List<OutfitSaveRequest> outfits = null;
            int id = 0;
            
            foreach (var user in Auth.currentUsers)
            {
                if (user.token == hrequest.Headers.Authorization[0])
                {
                    outfits = user.outfits;
                    id = user.id;
                }
            }
            
            if (outfits.Exists(x => x.Slot == request.Slot))
            {
                outfits.Remove(outfits.Find(x => x.Slot == request.Slot));
            }
            
            outfits.Add(request);
            
            File.WriteAllText("ServerData/Accounts/" + id + "/Outfits.json", JsonSerializer.Serialize(outfits));
            
            return Results.Created("/api/avatar/v3/saved/set", request);
        });

        app.MapPost("/api/avatar/v4/saved/set", ([FromBody] OutfitSaveRequest request, HttpRequest hrequest) =>
        {
            List<OutfitSaveRequest> outfits = null;
            int id = 0;
            
            foreach (var user in Auth.currentUsers)
            {
                if (user.token == hrequest.Headers.Authorization[0])
                {
                    outfits = user.outfits;
                    id = user.id;
                }
            }
            
            if (outfits.Exists(x => x.Slot == request.Slot))
            {
                outfits.Remove(outfits.Find(x => x.Slot == request.Slot));
            }
            
            outfits.Add(request);
            
            File.WriteAllText("ServerData/Accounts/" + id + "/Outfits.json", JsonSerializer.Serialize(outfits));

            var response = new
            {
                Success = true,
                Slot = request.Slot
            };
            
            return Results.Created("/api/avatar/v4/saved/set", response);
        });

        app.MapPost("/api/avatar/v2/set", async (HttpRequest request) =>
        {
            var body = new StreamReader(request.Body);
            var requestBody = await body.ReadToEndAsync();
            
            foreach (var user in Auth.currentUsers)
            {
                if (user.token == request.Headers.Authorization[0])
                {
                    await File.WriteAllTextAsync("ServerData/Accounts/" + user.id + "/Avatar.json", requestBody);
                    return Results.Ok();
                }
            }
            
            return Results.NotFound();
        }).DisableAntiforgery();

        app.MapGet("/api/equipment/v2/getUnlocked", () =>
        {
            app.Logger.LogWarning("(econ) /api/equipment/v2/getUnlocked STUB");

            return File.ReadAllText("equipment.json");
        });

        app.MapGet("/api/consumables/v2/getUnlocked", () =>
        {
            app.Logger.LogWarning("(econ) /api/consumables/v2/getUnlocked STUB");

            return File.ReadAllText("consumables.json");
        });

        app.MapGet("/api/avatar/v2/gifts", () =>
        {
            app.Logger.LogWarning("(econ) /api/avatar/v2/gifts STUB");

            List<GiftRow> rows = new();
            
            GiftRow row = new()
            {
                id = 0,
                data = null,
                created_at = DateTime.Now
            };

            rows.Add(row);
            
            return rows;
        });

        app.MapGet("/api/gamerewards/v1/pending", () =>
        {
            app.Logger.LogWarning("(econ) /api/gamerewards/v1/pending STUB");

            return "[]";
        });

        app.MapGet("/api/roomkeys/v1/mine", () =>
        {
            app.Logger.LogWarning("(econ) /api/roomkeys/v1/mine STUB");

            return "[]";
        });

        app.MapGet("/api/roomcurrencies/v1/currencies", () =>
        {
            app.Logger.LogWarning("(econ) /api/roomcurrencies/v1/currencies STUB");
            
            return "[]";
        });

        app.MapGet("/api/roomcurrencies/v1/getAllBalances", () =>
        {
            app.Logger.LogWarning("(econ) /api/roomcurrencies/v1/getAllBalances STUB");
            
            return "[]";
        });

        app.MapGet("/api/roomconsumables/v1/roomConsumable/room/{roomId}", () =>
        {
            app.Logger.LogWarning("/api/roomconsumables/v1/roomConsumable/room/{{roomId}} STUB");

            return "[]";
        });

        app.MapGet("/api/roomconsumables/v1/roomConsumable/room/{roomId}/me", () =>
        {
            app.Logger.LogWarning("/api/roomconsumables/v1/roomConsumable/room/{{roomId}}/me STUB");

            return "[]";
        });

        app.MapPost("/api/CampusCard/v1/UpdateAndGetSubscription", () =>
        {
            CampusCardSubscription value = new()
            {
                canBuySubscription = true,
                subscription = new()
                {
                    CreatedAt = DateTime.Now,
                    ExpirationDate = DateTime.UtcNow.AddDays(30),
                    IsActive = true,
                    IsAutoRenewing = true,
                    Level = 0,
                    ModifiedAt = DateTime.Now,
                    Period = 0,
                    PlatformId = "0",
                    PlatformPurchaseId = "0",
                    PlatformType = 0,
                    RecNetPlayerId = 0,
                    SubscriptionId = 0
                },
                platformAccountSubscribedPlayerId = 0
            };
            
            return Results.Created("/api/CampusCard/v1/UpdateAndGetSubscription", value);
        });

        app.MapGet("/api/itemWishlists/v1/wishlist/me", () =>
        {
            app.Logger.LogWarning("(econ) /api/itemWishlists/v1/wishlist/me STUB");

            return "[]";
        });

        // app.MapGet("/api/subscriptionseasons/v1/seasons/current", () =>
        // {
        //     app.Logger.LogWarning("(econ) /api/subscriptionseasons/v1/seasons STUB");
        //
        //     return "{}";
        // });

        app.MapPost("/api/consumables/v1/consume", () =>
        {
            app.Logger.LogWarning("(econ) /api/consumables/v1/consume STUB");

            return Results.Ok();
        });

        app.MapPost("/subscription/v1/cancel", () =>
        {
            return Results.Ok();
        });
    }
}