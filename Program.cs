using System;
using System.Net.Http;
using System.Threading.Tasks;
using apibot;

namespace apibot;

internal class Program
{
    private static async Task Main(string[] args)
    {
        using var httpClient = new HttpClient();
        var bot = new Communication(httpClient);

        var key = "key";
        var baseUrl = "https://botventure.htl-neufelden.at";

        string currentGameId = null;
        bool exit = false; // Flag to control when to exit the main loop

        while (!exit)
        {
            DisplayMainMenu(); // Show main menu options
            var mainMenuKey = Console.ReadKey(true).Key;

            switch (mainMenuKey)
            {
                case ConsoleKey.D1:
                    currentGameId = await HandleCreateGame(bot, baseUrl, key); // Handle creating a game
                    break;
                case ConsoleKey.D2:
                    await HandleStartGame(bot, baseUrl, key, currentGameId); // Start the game if one exists
                    break;
                case ConsoleKey.D3:
                    await HandleCloseGame(bot, baseUrl, key); // Close the current game
                    currentGameId = null;
                    break;
                case ConsoleKey.D4:
                    currentGameId = await HandleJoinGame(bot, baseUrl, key); // Attempt to join a game
                    break;
                case ConsoleKey.D5:
                    if (currentGameId != null)
                    {
                        await HandleGameMenu(bot, baseUrl, key, currentGameId); // Enter game menu if a game is active
                    }
                    else
                    {
                        Console.WriteLine("\nNo game created or joined yet.\nPress ENTER to return to the main menu...");
                        Console.ReadLine();
                    }
                    break;
                case ConsoleKey.Escape:
                    exit = true; // Exit the program
                    break;
            }
        }

        Console.Clear();
        Console.WriteLine("Exiting program. Goodbye!");
    }

    // Displays the main menu options
    private static void DisplayMainMenu()
    {
        Console.Clear();
        Console.WriteLine("=== My Bot Menu ===");
        Console.WriteLine("1) Create a new game");
        Console.WriteLine("2) Start the game");
        Console.WriteLine("3) Close the existing game");
        Console.WriteLine("4) Join a game from the list");
        Console.WriteLine("5) Game Menu (View State & Move)");
        Console.WriteLine("\nChoose an option or press Esc to exit:");
    }

    // Handles the creation of a new game
    private static async Task<string> HandleCreateGame(Communication bot, string baseUrl, string key)
    {
        Console.Clear();
        Console.WriteLine("Choose a level to create (press 1-5) or Esc to go back...");
        Console.WriteLine("[1] level0\n[2] level1\n[3] level2\n[4] level3\n[5] level8");

        string chosenLevel = null;
        while (chosenLevel == null) // Loop until a valid level is chosen
        {
            var levelKey = Console.ReadKey(true).Key;
            chosenLevel = levelKey switch
            {
                ConsoleKey.D1 => "level0",
                ConsoleKey.D2 => "level1",
                ConsoleKey.D3 => "level2",
                ConsoleKey.D4 => "level3",
                ConsoleKey.D5 => "level8",
                ConsoleKey.Escape => null,
                _ => null // Invalid input; prompt again
            };
            if (levelKey == ConsoleKey.Escape) return null; // Exit if Escape is pressed
        }

        try
        {
            var response = await bot.CreateGameAsync(baseUrl, key, chosenLevel);
            Console.WriteLine($"\nGame created successfully!\n - ID: {response.Id}\n - Level: {response.Level}");
            Console.WriteLine("\nPress ENTER to return to main menu...");
            Console.ReadLine();
            return response.Id;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError creating the game: {ex.Message}");
            Console.WriteLine("\nPress ENTER to return to main menu...");
            Console.ReadLine();
            return null;
        }
    }

    // Starts an existing game if possible
    private static async Task HandleStartGame(Communication bot, string baseUrl, string key, string currentGameId)
    {
        Console.Clear();
        if (currentGameId == null)
        {
            Console.WriteLine("No game created or joined yet. Please create or join a game first.");
        }
        else
        {
            try
            {
                var startResp = await bot.StartGameAsync(baseUrl, key);
                Console.WriteLine($"\nGame started successfully!\nNow: {startResp.Now}\nStartAt: {startResp.StartAt}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError starting the game: {ex.Message}");
            }
        }
        Console.WriteLine("\nPress ENTER to return to main menu...");
        Console.ReadLine();
    }

    // Closes the current game
    private static async Task HandleCloseGame(Communication bot, string baseUrl, string key)
    {
        Console.Clear();
        try
        {
            await bot.CloseGameAsync(baseUrl, key);
            Console.WriteLine("\nGame closed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError closing the game: {ex.Message}");
        }
        Console.WriteLine("\nPress ENTER to return to main menu...");
        Console.ReadLine();
    }

    // Allows user to join a game from the available games
    private static async Task<string> HandleJoinGame(Communication bot, string baseUrl, string key)
    {
        Console.Clear();
        try
        {
            var gameList = await bot.ListGamesAsync(baseUrl, running: false, 20);
            if (gameList.Count == 0)
            {
                Console.WriteLine("\nNo joinable games found.");
            }
            else
            {
                Console.WriteLine("\n=== Joinable Games ===");
                for (int i = 0; i < gameList.Count; i++)
                {
                    var g = gameList[i];
                    Console.WriteLine($"{i + 1}) GameId: {g.GameId}\n   Level: {g.Level}\n   Desc:  {g.Description}\n");
                }

                Console.WriteLine("Select a game to join (enter number) or press Esc to go back:");
                while (true)
                {
                    var inputKey = Console.ReadKey(true);

                    if (inputKey.Key == ConsoleKey.Escape) return null;

                    if (int.TryParse(inputKey.KeyChar.ToString(), out var choice) &&
                        choice > 0 && choice <= gameList.Count)
                    {
                        var selectedGame = gameList[choice - 1];
                        try
                        {
                            var joined = await bot.JoinGameAsync(baseUrl, key, selectedGame.GameId);
                            if (joined)
                            {
                                Console.WriteLine($"\nSuccessfully joined the game with ID: {selectedGame.GameId}");
                                return selectedGame.GameId;
                            }
                            else
                            {
                                Console.WriteLine("\nFailed to join the game.");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"\nError joining game: {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("\nInvalid selection. Try again.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError listing or joining games: {ex.Message}");
        }
        Console.WriteLine("\nPress ENTER to return to main menu...");
        Console.ReadLine();
        return null;
    }

    // Handles the game menu for moving and displaying game state
    private static async Task HandleGameMenu(Communication bot, string baseUrl, string key, string currentGameId)
    {
        bool inGameMenu = true;
        Console.Clear();
        Console.WriteLine("=== Game Menu ===");
        Console.WriteLine($"Game ID: {currentGameId}");

        while (inGameMenu)
        {
            try
            {
                var gameState = await bot.GetGameStateAsync(baseUrl, key);

                if (!gameState.IsRunning)
                {
                    Console.WriteLine("\nGame has not started yet. Please wait until the game is started by the host.");
                    Console.WriteLine("Press ENTER to return to the main menu...");
                    Console.ReadLine();
                    return;
                }

                DisplayGameState(gameState);

                Console.WriteLine("Use W/A/S/D or Arrow keys to move. Press Esc to return to the main menu.\n");
                var moveKey = Console.ReadKey(true).Key;
                int? direction = moveKey switch
                {
                    ConsoleKey.W or ConsoleKey.UpArrow => 0,
                    ConsoleKey.D or ConsoleKey.RightArrow => 1,
                    ConsoleKey.S or ConsoleKey.DownArrow => 2,
                    ConsoleKey.A or ConsoleKey.LeftArrow => 3,
                    ConsoleKey.Escape => null,
                    _ => null
                };

                if (direction == null)
                {
                    Console.WriteLine("\nExiting game menu...");
                    inGameMenu = false;
                    break;
                }

                var moveResponse = await bot.MoveAsync(baseUrl, key, direction.Value);
                if (moveResponse.GameOver)
                {
                    Console.WriteLine("\nGame Over! Exiting game menu.");
                    inGameMenu = false;
                    break;
                }
                Console.WriteLine($"Score: {moveResponse.Score}".PadRight(40));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
                Console.WriteLine("Press ENTER to return to the main menu...");
                Console.ReadLine();
                inGameMenu = false;
            }
        }
    }

    // Displays the current game state
    private static void DisplayGameState(GameStateResponse gameState)
    {
        Console.SetCursorPosition(0, 5);
        Console.WriteLine($"Is Running:     {gameState.IsRunning}".PadRight(40));
        Console.WriteLine($"Player Position: ({gameState.PlayerX}, {gameState.PlayerY})".PadRight(40));
        Console.WriteLine($"Goal Position:   ({gameState.GoalPositionX}, {gameState.GoalPositionY})".PadRight(40));
        Console.WriteLine($"View Radius:     {gameState.ViewRadius}".PadRight(40));
        Console.WriteLine($"Width x Height:  {gameState.Width} x {gameState.Height}".PadRight(40));
    }
}
