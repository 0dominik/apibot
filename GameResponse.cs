using System.Text.Json.Serialization;

namespace apibot;

public class GameResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("level")]
    public string Level { get; set; }
}

public class GameListItem
{
    [JsonPropertyName("level")]
    public string Level { get; set; }
    [JsonPropertyName("description")]
    public string Description { get; set; }
    [JsonPropertyName("host")]
    public string Host { get; set; }
    [JsonPropertyName("amountOfBots")]
    public int AmountOfBots { get; set; }
    [JsonPropertyName("isRunning")]
    public bool IsRunning { get; set; }
    [JsonPropertyName("startedAt")]
    public string StartedAt { get; set; }
    [JsonPropertyName("gameId")]
    public string GameId { get; set; }
}

public class StartGameResponse
{
    [JsonPropertyName("now")]
    public string Now { get; set; }
    [JsonPropertyName("startAt")]
    public string StartAt { get; set; }
}

public class GameStateResponse
{
    [JsonPropertyName("isRunning")]
    public bool IsRunning { get; set; }
    [JsonPropertyName("startAt")]
    public string? StartAt { get; set; }
    [JsonPropertyName("width")]
    public int Width { get; set; }
    [JsonPropertyName("height")]
    public int Height { get; set; }
    [JsonPropertyName("playerY")]
    public int PlayerY { get; set; }
    [JsonPropertyName("playerX")]
    public int PlayerX { get; set; }
    [JsonPropertyName("viewRadius")]
    public int ViewRadius { get; set; }
    [JsonPropertyName("goalPositionY")]
    public int GoalPositionY { get; set; }
    [JsonPropertyName("goalPositionX")]
    public int GoalPositionX { get; set; }
}

public class MoveResponse
{
    [JsonPropertyName("gameOver")]
    public bool GameOver { get; set; }
    [JsonPropertyName("score")]
    public int Score { get; set; }
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    [JsonPropertyName("newPosition")]
    public Position NewPosition { get; set; }
}

public class Position
{
    [JsonPropertyName("left")]
    public int Left { get; set; }

    [JsonPropertyName("top")]
    public int Top { get; set; }
}
