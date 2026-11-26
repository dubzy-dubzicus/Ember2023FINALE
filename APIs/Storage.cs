namespace asdfasdf.APIs;

public static class Storage
{
    public static void Init(WebApplication app)
    {
        app.MapPost("/upload", async (HttpRequest request) =>
        {
            var file = request.Form.Files.FirstOrDefault();

            string fileName;

            if (Matchmaking.currentInstance != null)
            {
                fileName = Matchmaking.currentInstance.roomId + "-" + Guid.NewGuid() + ".room";
            }
            else
            {
                fileName = Guid.NewGuid() + ".room";
            }
            
            using (var stream = new FileStream("wwwroot/room/" + fileName, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Results.Ok(new {Filename = fileName});
        });
    }
}