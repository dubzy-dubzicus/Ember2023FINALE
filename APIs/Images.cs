namespace asdfasdf.APIs;

public static class Images
{
    public static void Init(WebApplication app)
    {
        app.MapGet("/img/{name}", async (string name, HttpContext context) =>
        {
            context.Response.Headers.Append("content-signature", "key-id=KEY:RSA:p1.rec.net; data=IWwe/pZ5vWWqNSkSM/54isgDxlZkdrP0sUrppKCbNktO2yCOTjq746xWiiLsueGuVcAGQqkjeRTimxolHckS/YXSYkEJxtiCXbLlsRia2DyAqtWVkGWsfczzFhp/56U66FVzolTspPCvjScOVlGO7dDIK7sJ+ndcRauWjsQsC6g3e7rUc6uwY099a6gy7sw6xr5BFZQSz8wg+fqyHYD/Sc4nQQVOTFZNNASqbJYhpNhEMXRnafCMuLl8a3mkGwvy3t4q2D/7SM48xrGZjEV47qNx1A91KCe28XVToFh4BzwEUU8nZ0d+KwV79MGarLo1cY8igc8FcoThKcovI4ClOg==");

            if (File.Exists("wwwroot/" + name))
            {
                string mime = "image/png";

                if (Path.GetExtension(name) == ".jpg")
                {
                    mime = "image/jpeg";
                }
                
                return Results.File(await File.ReadAllBytesAsync("wwwroot/" + name), mime);
            }
            else
            {
                return Results.File(await File.ReadAllBytesAsync("wwwroot/lemres.png"), "image/png");
            }
        });
    }
}