using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace apibot;

public class Communication : Base
{
    public Communication(HttpClient httpClient) : base(httpClient) { }

    public override async Task<GameResponse> CreateGameAsync(string baseUrl, string key, string level)
    {
        var url = $"{baseUrl}/api/game/{key}/create/{level}";
        var response = await HttpClient.PostAsync(url, new StringContent("", Encoding.UTF8, "text/plain"));
        response.EnsureSuccessStatusCode();
        return await DeserializeResponse<GameResponse>(response);
    }

    public override async Task CloseGameAsync(string baseUrl, string key)
    {
        var url = $"{baseUrl}/api/game/{key}/close";
        var response = await HttpClient.PostAsync(url, new StringContent("", Encoding.UTF8, "text/plain"));
        response.EnsureSuccessStatusCode();
    }

    public override async Task<List<GameListItem>> ListGamesAsync(string baseUrl, bool running, int take)
    {
        var url = $"{baseUrl}/api/game/list/{running.ToString().ToLower()}/{take}";
        var response = await HttpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await DeserializeResponse<List<GameListItem>>(response) ?? new List<GameListItem>();
    }

    public override async Task<StartGameResponse> StartGameAsync(string baseUrl, string key)
    {
        var url = $"{baseUrl}/api/game/{key}/start";
        var response = await HttpClient.PostAsync(url, new StringContent("", Encoding.UTF8, "text/plain"));
        response.EnsureSuccessStatusCode();
        return await DeserializeResponse<StartGameResponse>(response);
    }

    public override async Task<bool> JoinGameAsync(string baseUrl, string key, string gameId)
    {
        var url = $"{baseUrl}/api/game/{key}/join/{gameId}";
        var response = await HttpClient.PostAsync(url, new StringContent("", Encoding.UTF8, "text/plain"));

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Error joining game: {await response.Content.ReadAsStringAsync()}");
        }

        return bool.TryParse(await response.Content.ReadAsStringAsync(), out var success) && success;
    }

    public override async Task<GameStateResponse> GetGameStateAsync(string baseUrl, string key)
    {
        var url = $"{baseUrl}/api/game/{key}/state";
        var response = await HttpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await DeserializeResponse<GameStateResponse>(response);
    }

    public override async Task<MoveResponse> MoveAsync(string baseUrl, string key, int direction)
    {
        var url = $"{baseUrl}/api/Player/{key}/move/{direction}";
        var response = await HttpClient.PostAsync(url, new StringContent("", Encoding.UTF8, "text/plain"));
        response.EnsureSuccessStatusCode();
        return await DeserializeResponse<MoveResponse>(response);
    }

    // Helper method to handle JSON deserialization
    private static async Task<T> DeserializeResponse<T>(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? throw new Exception("Failed to deserialize response.");
    }
}
