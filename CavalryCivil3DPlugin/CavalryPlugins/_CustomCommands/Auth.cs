using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

public class OAuthFlow
{
    private readonly string clientId = "YOUR_CLIENT_ID";
    private readonly string clientSecret = "YOUR_CLIENT_SECRET";
    private readonly string redirectUri = "http://localhost:5000/callback";
    private readonly string authUrl = "https://developer.api.autodesk.com/authentication/v2/authorize";
    private readonly string tokenUrl = "https://developer.api.autodesk.com/authentication/v2/token";
    private readonly string scope = "data:read data:create data:write";

    public void StartAuthFlow()
    {
        var url = $"{authUrl}?response_type=code&client_id={clientId}&redirect_uri={HttpUtility.UrlEncode(redirectUri)}&scope={HttpUtility.UrlEncode(scope)}";
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
        Console.WriteLine("Please login in browser and paste the ?code= from the redirect URI here:");
        Console.Write("Code: ");
        string code = Console.ReadLine();
        Task.Run(() => ExchangeCodeForToken(code)).Wait();
    }

    private async Task ExchangeCodeForToken(string code)
    {
        using var client = new HttpClient();
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("client_secret", clientSecret),
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("redirect_uri", redirectUri)
        });

        var response = await client.PostAsync(tokenUrl, content);
        var responseContent = await response.Content.ReadAsStringAsync();

        Console.WriteLine("Access Token Response:");
        Console.WriteLine(responseContent);
    }
}
