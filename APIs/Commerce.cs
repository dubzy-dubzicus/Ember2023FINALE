namespace asdfasdf.APIs;

public static class Commerce
{
    public static void Init(WebApplication app)
    {
        app.Map("/purchase/v1/hasspentmoney", () =>
        {
            return false;
        });

        app.MapGet("/purchasecampaign/allcurrent/v2", () =>
        {
            return "[]";
        });

        app.MapGet("/reminder/currentTokenBundles/v2", () =>
        {
            return "[]";
        });
    }
}