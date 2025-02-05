using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace apibot;

public abstract class Base
{
    protected readonly HttpClient HttpClient;

    protected Base(HttpClient httpClient)
    {
        HttpClient = httpClient;
    }

    public abstract Task<GameResponse> CreateGameAsync(string baseUrl, string key, string level);
    public abstract Task CloseGameAsync(string baseUrl, string key);
    public abstract Task<List<GameListItem>> ListGamesAsync(string baseUrl, bool running, int take);
    public abstract Task<StartGameResponse> StartGameAsync(string baseUrl, string key);
    public abstract Task<bool> JoinGameAsync(string baseUrl, string key, string gameId);
    public abstract Task<GameStateResponse> GetGameStateAsync(string baseUrl, string key);
    public abstract Task<MoveResponse> MoveAsync(string baseUrl, string key, int direction);

}
