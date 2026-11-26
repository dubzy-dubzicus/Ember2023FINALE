namespace asdfasdf.APIs;

public static class BugReporting
{
    private static string CurrentADOGUID;
    
    public static void Init(WebApplication app)
    {
        app.MapPost("/ado/bug", async (HttpRequest request) =>
        {
            CurrentADOGUID = Guid.NewGuid().ToString();
            
            await SaveFileFromHttpRequest("ServerData/BugReports/" + "ado_" + CurrentADOGUID + ".txt", request);

            return API.SuccessResponse;
        });

        app.MapPost("/ado/file", async (HttpRequest request) =>
        {
            var file = request.Form.Files.FirstOrDefault();
            
            using (var stream = new FileStream("ServerData/BugReports/ado_" + CurrentADOGUID + "_" + file.FileName, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return API.SuccessResponse;
        });

        app.MapPut("/reports/{id}", async (string id, HttpRequest request) =>
        {
            await SaveFileFromHttpRequest("ServerData/BugReports/" + id + ".json", request);

            return API.SuccessResponse;
        });

        app.MapPut("/reports/{id}/attachments/{fileName}", async (string id, string fileName, HttpRequest request) =>
        {
            await SaveFileFromHttpRequest("ServerData/BugReports/" + id + "_" + fileName, request);

            return API.SuccessResponse;
        });

        app.MapPut("/reports/{id}/userinfo", async (string id, HttpRequest request) =>
        {
            await SaveFileFromHttpRequest("ServerData/BugReports/" + id + "_userinfo.json", request);

            return API.SuccessResponse;
        });

        app.MapPut("/reports/{id}/Complete", (string id) =>
        {
            return API.SuccessResponse;
        });
    }

    private static async Task SaveFileFromHttpRequest(string path, HttpRequest request)
    {
        var body = new StreamReader(request.Body);
        var requestBody = await body.ReadToEndAsync();
            
        await File.WriteAllTextAsync(path, requestBody);
    }
}