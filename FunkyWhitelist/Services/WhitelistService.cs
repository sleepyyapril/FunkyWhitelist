using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using Newtonsoft.Json;

namespace FunkyWhitelist.Services;

public class WhitelistService
{
    private HttpClient _httpClient;
    private string ConnectAddress { get; set; }
    private string ApiToken { get; set; }
    
    public WhitelistService(string connectAddress, string apiToken)
    {
        _httpClient = new HttpClient();
        
        ConnectAddress = connectAddress;
        ApiToken = apiToken;
    }


    public async Task<string?> WhitelistUser(string name)
    {
        var whitelistActionBody = new WhitelistActionBody(name);
        var whitelistActionBodyJson = JsonConvert.SerializeObject(whitelistActionBody);
        var httpContent = new StringContent(whitelistActionBodyJson, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"{ConnectAddress}/admin/actions/whitelist", httpContent);

        if (!response.IsSuccessStatusCode)
            return $"Error code {response.StatusCode}";

        return "Success";
    }
}

public class WhitelistActionBody
{
    public string Username { get; set; }

    public WhitelistActionBody() => Username = string.Empty;
    public WhitelistActionBody(string username) => Username = username;
}