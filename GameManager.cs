using System.Text.Json;
using RogueConsoleGame.Enums;
using RogueConsoleGame.Records;
using RogueConsoleGame.Screens;

namespace RogueConsoleGame;

/// <summary>
/// Create and manage an instance of the game.
/// </summary>
public class GameManager
{
    public bool IsRunning;

    public int GameWidth;
    public int GameHeight;
    public int EnemyCount;

    public char PlayerChar = '☻';
    public char EmptyChar = '·';
    public Enemy[] Enemies;

    public Random Seed = new Random();

    public int MinGameWidth { get; }
    public int MaxGameWidth { get; }

    public int MinGameHeight { get; }
    public int MaxGameHeight { get; }

    public int MinEnemyCount { get; }
    public int MaxEnemyCount { get; }

    private MenuScreen MenuScreen { get; }
    private GameScreen GameScreen { get; }
    private EndScreen EndScreen { get; }

    public GameState CurrentGameState { get; set; }

    /// <summary>
    /// Initialize this game manager's game.
    /// </summary>
    public GameManager()
    {
        // Enemies checked first
        try
        {
            string enemyJson = File.ReadAllText("./Data/Enemies.json");
            Enemies = JsonSerializer.Deserialize<Enemy[]>(enemyJson);
            if (Enemies == null || Enemies.Length == 0) throw new Exception("No enemies found!");
        }
        catch (Exception e)
        {
            Console.WriteLine("An error occured when reading the Data/Enemies.json file!");
            throw;
        }
        
        MinGameWidth = 15;
        MaxGameWidth = 24;
        MinGameHeight = 20;
        MaxGameHeight = 40;
        MinEnemyCount = 3;
        MaxEnemyCount = 10;

        GameWidth = MaxGameWidth;
        GameHeight = MaxGameHeight;
        EnemyCount = MaxEnemyCount;

        MenuScreen = new MenuScreen(this);
        GameScreen = new GameScreen(this);
        EndScreen = new EndScreen(this);
    }

    /// <summary>
    /// Run the game in this instance of the game manager.
    /// </summary>
    public void Run()
    {
        IsRunning = true;
        CurrentGameState = GameState.Menu;

        // Execute the applicable screen
        while (IsRunning)
        {
            Console.Clear();
            switch (CurrentGameState)
            {
                case GameState.Menu:
                    MenuScreen.Draw();
                    break;
                case GameState.Game:
                    GameScreen.Draw();
                    break;
                case GameState.Victory:
                    EndScreen.Draw();
                    break;
                case GameState.Defeat:
                    EndScreen.Draw();
                    break;
                default:
                    throw new Exception("Invalid Game State!");
            }
        }
    }

    /// <summary>
    /// Output the guide of the game.
    /// </summary>
    public static void DrawGuide()
    {
        Console.WriteLine();
        ColorConsoleWriteLine(ConsoleColor.Cyan, "Incomplete Guide.");
        // throw new NotImplementedException("Guide not implemented");
    }

    /// <summary>
    /// Use Console.Write() with colored text without having to write three lines of code.
    /// </summary>
    /// <param name="color">The ConsoleColor you wish to use with your output.</param>
    /// <param name="text">The output.</param>
    public static void ColorConsoleWrite(ConsoleColor color, string text)
    {
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ResetColor();
    }

    /// <summary>
    /// Use Console.WriteLine() with colored text without having to write three lines of code.
    /// </summary>
    /// <param name="color">The ConsoleColor you wish to use with your output.</param>
    /// <param name="text">The output.</param>
    public static void ColorConsoleWriteLine(ConsoleColor color, string text)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }
}