using System.Text;
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
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"SS14Token {apiToken}");
        
        ConnectAddress = connectAddress;
        ApiToken = apiToken;
    }


    public async Task<string?> WhitelistUser(string name)
    {
        var whitelistActionBody = new WhitelistActionBody(name, true);
        var whitelistActionBodyJson = JsonConvert.SerializeObject(whitelistActionBody);
        var httpContent = new StringContent(whitelistActionBodyJson, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"http://{ConnectAddress}/admin/actions/whitelist", httpContent);

        if (!response.IsSuccessStatusCode)
            return response.StatusCode.ToString();

        return null;
    }
    
    public async Task<string?> UnwhitelistUser(string name)
    {
        var whitelistActionBody = new WhitelistActionBody(name, false);
        var whitelistActionBodyJson = JsonConvert.SerializeObject(whitelistActionBody);
        var httpContent = new StringContent(whitelistActionBodyJson, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"http://{ConnectAddress}/admin/actions/whitelist", httpContent);

        if (!response.IsSuccessStatusCode)
            return response.StatusCode.ToString();

        return null;
    }
}

public class WhitelistActionBody(string username, bool isAddingWhitelist)
{
    public string Username { get; set; } = username;
    public bool IsAddingWhitelist { get; set; } = isAddingWhitelist; 
}