using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace asdfasdf.APIs;

public static class API
{
    public static ApiResponse SuccessResponse;
    
    public static void Init(WebApplication app)
    {
        SuccessResponse = new()
        {
            success = true,
            error = string.Empty
        };
        
        if (!File.Exists("ServerData/CharadeWords.json"))
        {
            List<CharadeWord> cw = new();
            
            cw.Add(new(0, "Lemres"));
            cw.Add(new(1, "your mom"));
            cw.Add(new(2, "F"));
            cw.Add(new(3, "Gamer"));
            cw.Add(new(4, "Gaming chair"));
            cw.Add(new(5, "Gaming keyboard"));
            cw.Add(new(6, "Oculus Quest 4"));
            cw.Add(new(7, "Nintendo Switch 3"));
            cw.Add(new(8, "PlayStation 6"));
            cw.Add(new(9, "GTA 7"));
            cw.Add(new(10, "Black Shoe"));
            cw.Add(new(11, "Xbox 720"));
            cw.Add(new(12, "wii phone"));
            cw.Add(new(13, "Valve Index 2"));
            cw.Add(new(14, "BONEWORKS"));
            cw.Add(new(15, "BONELAB"));
            cw.Add(new(16, "Sonic"));
            cw.Add(new(17, "Fuck you you don't get a word"));
            cw.Add(new(18, "Rec room 2"));
            cw.Add(new(19, "4DCharades"));
            cw.Add(new(20, "ONE TWO THREE FOUR BADADADADADADAA BBBBB"));
            cw.Add(new(21, "puyo puyo"));
            cw.Add(new(22, "t-spin"));
            cw.Add(new(23, "garry's mod"));
            cw.Add(new(24, "Merge conflict"));
            cw.Add(new(25, "Hi. I hate you. I hope you die."));
            cw.Add(new(26, "The entirety of North Korea"));
            cw.Add(new(27, "Gaming controller"));
            cw.Add(new(28, "the"));
            cw.Add(new(29, "ENA"));
            cw.Add(new(30, "N"));
            cw.Add(new(31, "No peeking!"));
            
            File.WriteAllText("ServerData/CharadeWords.json", JsonSerializer.Serialize(cw));
        }

        if (!File.Exists("ServerData/RoomFilters.json"))
        {
            RoomFilters filters = new();
            filters.PinnedFilters = new();
            filters.PopularFilters = new();
            filters.TrendingFilters = new();
            
            filters.PinnedFilters.Add("recroomoriginal");
            filters.PinnedFilters.Add("community");
            filters.PinnedFilters.Add("featured");
            filters.PinnedFilters.Add("quest");
            filters.PinnedFilters.Add("pvp");
            filters.PinnedFilters.Add("hangout");
            filters.PinnedFilters.Add("game");
            filters.PinnedFilters.Add("art");
            filters.PinnedFilters.Add("store");
            filters.PinnedFilters.Add("tutorial");
            filters.PinnedFilters.Add("fandom");
            filters.PinnedFilters.Add("performance");
            filters.PinnedFilters.Add("action");
            filters.PinnedFilters.Add("horror");
            
            filters.PopularFilters.Add("pvp");
            filters.PopularFilters.Add("quest");
            filters.PopularFilters.Add("game");
            filters.PopularFilters.Add("hangout");
            filters.PopularFilters.Add("art");

            filters.TrendingFilters.Add("roleplay");
            filters.TrendingFilters.Add("nomp");
            filters.TrendingFilters.Add("rp");
            filters.TrendingFilters.Add("casual");
            filters.TrendingFilters.Add("fun");
            filters.TrendingFilters.Add("action");
            filters.TrendingFilters.Add("military");
            filters.TrendingFilters.Add("sports");
            
            File.WriteAllText("ServerData/RoomFilters.json", JsonSerializer.Serialize(filters));
        }
        
        app.MapGet("/api/gameconfigs/v1/all", () => File.ReadAllText("gameconfig.json"));

        app.MapGet("/api/customAvatarItems/v1/isCreationAllowedForAccount", () =>
        {
            app.Logger.LogWarning("(API) /api/customAvatarItems/v1/isCreationAllowedForAccount STUB");

            CustomAvatarItems_CreationAllowed creationAllowed = new()
            {
                success = true
            };
            
            return creationAllowed;
        });

        app.MapGet("/api/customAvatarItems/v1/me", () => {
            app.Logger.LogWarning("(API) /api/customAvatarItems/v1/me STUB");

            return "[]";
        });

        app.MapPost("/api/customAvatarItems/v1/bulk", () =>
        {
            app.Logger.LogWarning("(API) /api/customAvatarItems/v1/bulk STUB");

            // List<BulkCustomAvatarItem> avatarItems = new();
            //
            // BulkCustomAvatarItem avatarItem = new()
            // {
            //     Accessibility = 2,
            //     CreatedAt = "2024-11-06T20:55:26.184Z",
            //     CreatorAccountId = 1,
            //     CustomAvatarItemId = "e396c94f-5e76-438f-9ed5-c6174011d239",
            //     CurrentSaves = Array.Empty<Object>(),
            //     ForceCannotPublish = false,
            //     IsFeatured = false,
            //     IsRecRoomApproved = true,
            //     ModifiedAt = "2024-11-06T20:55:26.184Z",
            //     Name = "(UGCTee_Shirt)",
            //     OutfitType = 101,
            //     PreviewOrientation = 0,
            //     Price = 0,
            //     RankedEntityId = "e396c94f-5e76-438f-9ed5-c6174011d239",
            //     Tags = Array.Empty<Object>(),
            // };
            //
            // BulkCustomAvatarItem avatarItem2 = new()
            // {
            //     Accessibility = 2,
            //     CreatedAt = "2024-11-05T23:44:26.452Z",
            //     CreatorAccountId = 1,
            //     CustomAvatarItemId = "8c03db9e-ea46-48f9-a114-3afd18454e31",
            //     CurrentSaves = Array.Empty<Object>(),
            //     ForceCannotPublish = false,
            //     IsFeatured = false,
            //     IsRecRoomApproved = true,
            //     ModifiedAt = "2024-11-05T23:44:26.452Z",
            //     Name = "(UGCPulloverHoodie_Shirt)",
            //     OutfitType = 101,
            //     PreviewOrientation = 0,
            //     Price = 0,
            //     RankedEntityId = "8c03db9e-ea46-48f9-a114-3afd18454e31",
            //     Tags = Array.Empty<Object>(),
            // };
            //
            // avatarItems.Add(avatarItem);
            // avatarItems.Add(avatarItem2);

            return Results.Created("/api/customAvatarItems/v1/bulk", "[]");
        });

        app.MapGet("/api/versioncheck/v4", (string v, int p) =>
        {
            app.Logger.LogInformation("VersionCheck: " + v + " " + p);

            VersionCheckObject ver = new()
            {
                VersionStatus = 0,
                UpdateNotificationStage = 0,
                IsVersionIslanded = false,
                IsCrossPlayDisabled = false,
            };

            return ver;
        });

        // It's also like this on the actual API
        app.MapGet("/api/versioncheck/islandedversions", () => "[]");

        app.MapGet("/api/keepsakes/categories", () =>
        {
            app.Logger.LogWarning("(API) /api/keepsakes/categories STUB");

            return File.ReadAllText("keepsakes_categories.json");
        });

        app.MapGet("/api/progressionEvents/event/18", () =>
        {
            app.Logger.LogWarning("(API) /api/progressionEvents/event/18 STUB");
            
            ProgressionEvent progressionEvent = new()
            {
                ActiveExperiment = "liveops_2025q4_yeti",
                BoostDailyGameplayMinutesLimit = 0,
                BoosXpMultiplier = 0,
                CollectionEndTime = DateTime.Now.AddMonths(-1),
                EventInfoImageName = "DefaultProfileImage.png",
                EventLeaderboardId = 18,
                InitialCurrencyGrant = 300,
                IsEnabled = false,
                Name = "Yetis challenge",
                ProgressionEventId = 18,
                PurchasableXpBoostId = "e4a7b4d4-f609-4008-9874-f6cd01b896ea",
                StartTime = DateTime.Now.AddMonths(-2),
                UsesBoost = false
            };
            
            return progressionEvent;
        });

        app.MapGet("/api/players/v2/progression/bulk", (int id) =>
        {
            app.Logger.LogWarning("(API) /api/players/v2/progression/bulk STUB\n" + id);

            List<Progression> progressions = new();
            
            Progression progression = new()
            {
                Level = 50,
                PlayerId = Accounts.account.accountId,
                XP = 0
            };
            
            progressions.Add(progression);

            return progressions;
        });

        app.Map("/api/PlayerReporting/v1/moderationBlockDetails", () =>
        {
            // var a = await request.ReadFormAsync();
            // var a1 = a.ElementAt(0);
            
            app.Logger.LogWarning("(API) /api/PlayerReporting/v1/moderationBlockDetails STUB");

            ModerationBlockDetail block = new()
            {
                ReportCategory = ReportCategory.Moderator,
                Duration = 0,
                GameSessionId = 0,
                IsHostKick = false,
                PlayerIdReporter = 0,
                IsBan = false,
                IsVoiceModAutoban = false,
                IsDeviceBan = false,
                IsWarning = false,
                ShowCreatorCodeOfConduct = false
            };
            
            // ModerationBlockDetail block = new()
            // {
            //     ReportCategory = ReportCategory.Moderator,
            //     Duration = 555,
            //     GameSessionId = 0,
            //     IsHostKick = false,
            //     PlayerIdReporter = 1,
            //     IsBan = true,
            //     IsVoiceModAutoban = false,
            //     IsDeviceBan = true,
            //     IsWarning = false,
            //     ShowCreatorCodeOfConduct = false
            // };

            return Results.Created("/api/PlayerReporting/v1/moderationBlockDetails", block);
        });

        app.MapGet("/api/PlayerReporting/v1/voteToKickReasons", () =>
        {
            app.Logger.LogWarning("(API) /api/PlayerReporting/v1/voteToKickReasons STUB");

            return "[]";
        });

        app.MapPost("/api/PlayerReporting/v1/hile", () =>
        {
            return false;
        });

        app.MapGet("/api/banappeal/generateCode", () =>
        {
            ApiResponse response = new()
            {
                success = false,
                error = "Lemres"
            };

            return response;
        });

        app.MapGet("/outfits/me", () =>
        {
            app.Logger.LogWarning("(API) /outfits/me STUB");

            Outfit outfit = new()
            {
                Accessibility = 0,
                DataVersion = 0,
                LegacyData = new()
                {
                    FaceFeatures = "{\"ver\":6,\"eyeId\":\"AjGMoJhEcEehacRZjUMuDg\",\"eyePos\":{\"x\":0.0,\"y\":0.0},\"eyeScl\":0.05000000074505806,\"mouthId\":\"FrZBRanXEEK29yKJ4jiMjg\",\"mouthPos\":{\"x\":0.0,\"y\":0.0},\"mouthScl\":0.05000000074505806,\"hairPrimaryColorId\":\"\",\"hairSecondaryColorId\":\"5e51ac88-e365-418b-a303-8737fd2e6bc8\",\"hairPatternId\":\"\",\"beardColorId\":\"5e51ac88-e365-418b-a303-8737fd2e6bc8\",\"beardSecondaryColorId\":\"5e51ac88-e365-418b-a303-8737fd2e6bc8\",\"beardPatternId\":\"\",\"faceShapeId\":\"yR4oYZr_AUSynXCgwS2lGw\",\"bodyShapeId\":\"bY1RGIph0kiAxbd6Shn9tQ\",\"useHatAnchorParams\":true,\"useHelmetHair\":1,\"hideEars\":true,\"hatAnchorParams\":{\"NormalizedPosition\":{\"x\":0.5,\"y\":0.5},\"HemisphereOffsets\":{\"x\":0.0,\"y\":0.0,\"z\":0.0},\"HemisphereRotations\":{\"x\":0.0,\"y\":0.0,\"z\":0.0}}}",
                    HairColor = "5e51ac88-e365-418b-a303-8737fd2e6bc8",
                    SelectionsV1 = "193a3bf9-abc0-4d78-8d63-92046908b1c5,,0;b6rLwzD4NkKV7xKn9ZYVkA,PRLBrEEZhUumm3VnYVJP6Q,1;5cd08cfb-c729-4c30-96d9-6a99bb934d91,,1;071f179a-dde2-407f-92b2-f0c16fcb4e39,VudVdOAjZUibHmtFeNkvSw,0;ecc1dbe6-ca06-4564-b2a6-30956194d1e9,dee70c38-7a99-4c2b-9181-665f1bf75aca,0b2395e1-ebcc-47e9-aaf1-faf9e9cec4cd,,2;ecc1dbe6-ca06-4564-b2a6-30956194d1e9,f8b0cfe8-e129-4578-8bb5-f60af5d38599,0b2395e1-ebcc-47e9-aaf1-faf9e9cec4cd,,3",
                    SelectionsV2 = "{\"selections\":[{\"PrefabGuid\":\"193a3bf9-abc0-4d78-8d63-92046908b1c5\",\"CombinationGuid\":\"\",\"BodyPart\":0,\"UgcOutfitData\":{\"BaseAvatarItemColor\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"CustomAvatarItemId\":\"\"}},{\"PrefabGuid\":\"b6rLwzD4NkKV7xKn9ZYVkA\",\"CombinationGuid\":\"PRLBrEEZhUumm3VnYVJP6Q\",\"BodyPart\":1,\"UgcOutfitData\":{\"BaseAvatarItemColor\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"CustomAvatarItemId\":\"\"}},{\"PrefabGuid\":\"5cd08cfb-c729-4c30-96d9-6a99bb934d91\",\"CombinationGuid\":\"\",\"BodyPart\":1,\"UgcOutfitData\":{\"BaseAvatarItemColor\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"CustomAvatarItemId\":\"\"}},{\"PrefabGuid\":\"071f179a-dde2-407f-92b2-f0c16fcb4e39\",\"CombinationGuid\":\"VudVdOAjZUibHmtFeNkvSw\",\"BodyPart\":0,\"UgcOutfitData\":{\"BaseAvatarItemColor\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"CustomAvatarItemId\":\"\"}},{\"PrefabGuid\":\"ecc1dbe6-ca06-4564-b2a6-30956194d1e9\",\"CombinationGuid\":\"dee70c38-7a99-4c2b-9181-665f1bf75aca,0b2395e1-ebcc-47e9-aaf1-faf9e9cec4cd,\",\"BodyPart\":2,\"UgcOutfitData\":{\"BaseAvatarItemColor\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"CustomAvatarItemId\":\"\"}},{\"PrefabGuid\":\"ecc1dbe6-ca06-4564-b2a6-30956194d1e9\",\"CombinationGuid\":\"f8b0cfe8-e129-4578-8bb5-f60af5d38599,0b2395e1-ebcc-47e9-aaf1-faf9e9cec4cd,\",\"BodyPart\":3,\"UgcOutfitData\":{\"BaseAvatarItemColor\":{\"r\":0.0,\"g\":0.0,\"b\":0.0,\"a\":0.0},\"CustomAvatarItemId\":\"\"}}]}",
                    SkinColor = "85343b16-d58a-4091-96d8-083a81fb03ae"
                },
                Slot = 0
            };
            
            return outfit;
        });

        app.MapGet("/api/playerReputation/v2/bulk", (string id) =>
        {
            app.Logger.LogWarning("(API) /api/playerReputation/v2/bulk STUB");

            List<BulkPlayerReputation> reps = new();

            BulkPlayerReputation rep = new()
            {
                AccountId = Accounts.account.accountId,
                CheerCreative = 0,
                CheerCredit = 10,
                CheerGeneral = 0,
                CheerGreatHost = 0,
                CheerHelpful = 0,
                CheerSportsman = 0,
                IsCheerful = true,
                Noteriety = 0,
                SelectedCheer = 0
            };
            
            reps.Add(rep);

            return reps;
        });

        app.Map("/api/relationships/v2/get", () => "[]");

        app.MapGet("/api/statsigUserProperties", () => "{\"success\" : true}");

        app.MapPost("/api/gamesight/event", () =>
        {
            ApiResponseNoValue response = new()
            {
                success = true
            };

            return Results.Created("/api/gamesight/event", response);
        });

        app.MapGet("/api/config/v1/amplitude", () =>
        {
            AmplitudeConfig config = new()
            {
                AmplitudeKey = "93941036bd6a7243bf5c628535f41d63",
                UseRudderStack = false,
                RudderStackKey = "23NiJHIgu3koaGNCZIiuYvIQNCu",
                UseStatSig = true,
                StatSigKey = "client-SBZkOrjD3r1Cat3f3W8K6sBd11WKlXZXIlCWj6l4Aje",
                StatSigEnvironment = 0
            };

            return config;
        });

        app.MapGet("/api/config/v1/backtrace", () =>
        {
            BacktraceConfig config = new()
            {
                ReportBudget = 125,
                FilterType = 0,
                SampleRate = 1,
                LogLineCount = 50,
                CaptureNativeCrashes = 0,
                AMRThresholdMS = 0,
                MessageCount = 1000,
                MessageRegex = "^.*$",
                VersionRegex = ".*"
            };
            
            return config;
        });

        app.MapGet("/api/config/v2", () =>
        {
            app.Logger.LogWarning("(API) /api/config/v2 STUB");

            return File.ReadAllText("wwwroot/config/config.json");
        });

        app.MapGet("/api/quickPlay/v1/getandclear", () =>
        {
            app.Logger.LogWarning("(API) /api/quickPlay/v1/getandclear STUB");

            return "";
        });

        app.MapGet("/api/messages/v2/get", () =>
        {
            app.Logger.LogWarning("(API) /api/messages/v2/get STUb");

            return "[]";
        });

        app.MapGet("/api/images/v2/named", () =>
        {
            app.Logger.LogWarning("(API) /api/images/v2/named STUB");

            return "[]";
        });

        app.MapGet("/api/communityboard/v2/current", () =>
        {
            return File.ReadAllText("communityboard.json");
        });

        app.MapGet("/api/playerevents/v1/all", () =>
        {
            app.Logger.LogWarning("(API) /api/playerevents/v1/all STUB");

            PlayerEvents events = new()
            {
                Created = Array.Empty<object>(),
                Responses = Array.Empty<object>()
            };

            return events;
        });

        app.MapGet("/api/customAvatarItems/v1/isCreationEnabled", () =>
        {
            return true;
        });

        app.MapGet("/api/customAvatarItems/v1/isRenderingEnabled", () =>
        {
            return true;
        });

        app.MapGet("/api/config/v1/azurespeech", () =>
        {
            var config = new
            {
                Key = "dce8de5b297747d9b5bddcc7f19e8c5b",
                Region = "eastus",
                Enabled = false,
            };

            return config;
        });

        app.MapGet("/api/announcement/v1/get", () =>
        {
            app.Logger.LogWarning("(API) /api/announcement/v1/get STUB");

            return "[]";
        });

        app.MapPost("/api/sanitize/v1", ([FromBody] SanitizeRequest request) =>
        {
            return Results.Ok(JsonSerializer.Serialize(request.Value));
        });

        app.MapPost("/api/sanitize/v1/isPure", () =>
        {
            var result = new
            {
                IsPure = true
            };

            return result;
        });

        app.MapGet("/api/testcasemanagement/v1/testpasssummary", () =>
        {
            app.Logger.LogWarning("(API) /api/testcasemanagement/v1/testpasssummary STUB");

            return "[]";
        });
        
        app.MapGet("/api/testcasemanagement/v1/testplans", () =>
        {
            app.Logger.LogWarning("(API) /api/testcasemanagement/v1/testplans STUB");

            return "[]";
        });

        app.MapPost("/api/rooms/v1/verifyRole", () =>
        {
            app.Logger.LogWarning("(API) /api/rooms/v1/verifyRole STUB");

            return true;
        });

        app.MapGet("/api/inventions/v2/mine", () =>
        {
            app.Logger.LogWarning("(API) /api/inventions/v2/mine STUB");

            return "[]";
        });

        app.MapGet("/api/apple/musicpromotion/active", () =>
        {
            return true;
        });

        app.MapGet("/api/inventions/v2/search", (int skip, int take) =>
        {
            app.Logger.LogWarning("(API) /api/inventions/v2/search STUB");
            
            return "[]";
        });

        app.MapGet("/api/inventions/v1/room", (int id) =>
        {
            app.Logger.LogWarning("(API) /api/inventions/v1/room STUB");

            return "[]";
        });

        app.MapGet("/api/keepsakes/globalconfig", () =>
        {
            var response = new
            {
                KeepsakeFeatureEnabled = true,
                KeepsakeRoomLimit = 10,
                SocialXpBoostEnabled = false
            };

            return response;
        });

        app.MapGet("/api/keepsakes/rooms/{roomId}", (int roomId) =>
        {
            app.Logger.LogWarning("(API) /api/keepsakes/rooms/{{roomId}} STUB");

            return Results.NoContent();
        });

        app.MapGet("/api/activities/charades/v1/words/{activityName}", (string activityName) =>
        {
            return File.ReadAllText("ServerData/CharadeWords.json");
        });

        app.MapGet("/api/images/v4/room/{roomId}", (int roomId, int sort, int filter, int take, int skip) =>
        {
            app.Logger.LogWarning("(API) /api/images/v4/room/{{roomId}} STUB");

            return "[]";
        });

        app.MapGet("/api/playerevents/v1/room/{roomId}", () =>
        {
            app.Logger.LogWarning("(API) /api/playerevents/v1/room/{{roomId}} STUB");

            return "[]";
        });

        app.MapGet("/api/rooms/v1/filters", () =>
        {
            return File.ReadAllText("ServerData/RoomFilters.json");
        });

        app.MapGet("/api/images/v6", (string name) =>
        {
            app.Logger.LogWarning("(API) /api/images/v6 STUB");
            
            return API.SuccessResponse;
        });
    }
}