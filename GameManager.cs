using System.Text.Json;
using RogueConsoleGame.Characters;
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

    public char EmptyChar = '·';
    public PlayerData Player { get; }
    public EnemyData[] Enemies { get; }
    
    // according to directionality
    public char[] WallChars { get; } = ['╬', '║', '║', '║', '═', '╚', '╔', '╠', '═', '╝', '╗', '╣', '═', '╩', '╦', '╬'];

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
        // Player checked
        string playerJson = File.ReadAllText("./Data/Player.json");
        Player = JsonSerializer.Deserialize<PlayerData>(playerJson) ?? throw new NullReferenceException("Invalid Player JSON!");
        
        // Enemies checked
        string enemyJson = File.ReadAllText("./Data/Enemies.json");
        Enemies = JsonSerializer.Deserialize<EnemyData[]>(enemyJson) ?? throw new NullReferenceException("Invalid Enemies JSON!");
        
        MinGameWidth = 20;
        MaxGameWidth = 40;
        MinGameHeight = 15;
        MaxGameHeight = 24;
        MinEnemyCount = 3;
        MaxEnemyCount = 6;

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
                    EndScreen.Victory = true;
                    EndScreen.Draw();
                    break;
                case GameState.Defeat:
                    EndScreen.Victory = false;
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
    public static void DrawGuide(GameManager gameManager)
    {
        Console.WriteLine();
        ColorConsoleWriteLine(ConsoleColor. Cyan, "Use arrow keys to move the player.");
        ColorConsoleWriteLine(ConsoleColor. Cyan, "Moving into an enemy attacks the enemy. If an enemy tries to move into the player, it attacks the player.");
        Console.WriteLine("Initialized Player stats:");
        ColorConsoleWriteLine(ConsoleColor.Green, $"- Symbol    : {gameManager.Player.Symbol}");
        ColorConsoleWriteLine(ConsoleColor.Green, $"- MaxHealth : {gameManager.Player.MaxHealth}");
        ColorConsoleWriteLine(ConsoleColor.Green, $"- Strength  : {gameManager.Player.Strength}");
        ColorConsoleWriteLine(ConsoleColor.Green, $"- Heal Cooldown : {gameManager.Player.HealCooldown}");
        ColorConsoleWriteLine(ConsoleColor.Green, $"- Heal Factor   : {gameManager.Player.HealFactor}");
        Console.WriteLine("Initialized Enemy stats:");
        int i = 0;
        foreach (var enemy in gameManager.Enemies)
        {
            i++;
            ColorConsoleWriteLine(ConsoleColor.DarkRed, $"Enemy {i}");
            ColorConsoleWriteLine(ConsoleColor.DarkRed, $"- Symbol    : {enemy.Symbol}");
            ColorConsoleWriteLine(ConsoleColor.DarkRed, $"- MaxHealth : {enemy.MaxHealth}");
            ColorConsoleWriteLine(ConsoleColor.DarkRed, $"- Strength  : {enemy.Strength}");
        }
        
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