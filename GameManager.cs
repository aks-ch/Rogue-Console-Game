using System.Diagnostics;
using System.Text;
using RogueConsoleGame.Enums;
using RogueConsoleGame.Screens;

namespace RogueConsoleGame;

/// <summary>
/// Create and manage an instance of the game.
/// </summary>
public class GameManager
{
    public bool IsRunning;

    public char EmptyChar = '·';
    
    // according to directionality
    public char[] WallChars { get; } = ['╬', '║', '║', '║', '═', '╚', '╔', '╠', '═', '╝', '╗', '╣', '═', '╩', '╦', '╬'];

    public Random Seed = new Random();
    public int Difficulty { get; set; }

    public int MinGameWidth { get; }
    public int MaxGameWidth { get; }

    public int MinGameHeight { get; }
    public int MaxGameHeight { get; }

    public int MinEnemyCount { get; }
    public int MaxEnemyCount { get; }

    public GameState CurrentGameState { get; set; }
    public GameState PreviousGameState { get; private set; }
    public DataInitializer DataInitializer { get; } = new DataInitializer();

    private MenuScreen MenuScreen { get; }
    private GameScreen GameScreen { get; }
    private EndScreen EndScreen { get; }

    /// <summary>
    /// Initialize this game manager's game.
    /// </summary>
    public GameManager()
    {
        MinGameWidth = 20;
        MaxGameWidth = 40;
        MinGameHeight = 15;
        MaxGameHeight = 24;
        MinEnemyCount = 3;
        MaxEnemyCount = 6;

        MenuScreen = new MenuScreen(this);
        GameScreen = new GameScreen(this);
        EndScreen = new EndScreen(this);
    }

    /// <summary>
    /// Run the game in this instance of the game manager.
    /// </summary>
    public void Run()
    {
        const int timePerFrame = 33;
        Stopwatch stopwatch = new Stopwatch();
        
        IsRunning = true;
        CurrentGameState = GameState.Menu;
        PreviousGameState = CurrentGameState;

        // Execute the applicable screen
        while (IsRunning)
        {
            // frame cap
            stopwatch.Restart();
            
            // Carry out correct clearing depending on current game state and previous state
            if (CurrentGameState == PreviousGameState && PreviousGameState == GameState.Game) Console.SetCursorPosition(0, 0);
            else Console.Clear();
            Console.CursorVisible = false;
            
            // Update previous state
            PreviousGameState = CurrentGameState;
            
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
            
            // frame cap
            stopwatch.Stop();
            if (timePerFrame - stopwatch.ElapsedMilliseconds <= 0) continue;
            int timeToSleep = timePerFrame - (int)stopwatch.ElapsedMilliseconds;
            if (timeToSleep > 0) Thread.Sleep(timeToSleep);
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
        ColorConsoleWriteLine(ConsoleColor.Green, $"- Symbol    : {gameManager.DataInitializer.Player.Symbol}");
        ColorConsoleWriteLine(ConsoleColor.Green, $"- MaxHealth : {gameManager.DataInitializer.Player.MaxHealth}");
        ColorConsoleWriteLine(ConsoleColor.Green, $"- Strength  : {gameManager.DataInitializer.Player.Strength}");
        ColorConsoleWriteLine(ConsoleColor.Green, $"- Heal Cooldown : {gameManager.DataInitializer.Player.HealCooldown}");
        ColorConsoleWriteLine(ConsoleColor.Green, $"- Heal Factor   : {gameManager.DataInitializer.Player.HealFactor}");
        Console.WriteLine("Initialized Enemy stats:");
        int i = 0;
        foreach (var enemy in gameManager.DataInitializer.Enemies)
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
    /// Use Console.Write() with colored text without having to write three lines of code.
    /// </summary>
    /// <param name="color">The ConsoleColor you wish to use with your output.</param>
    /// <param name="text">The output.</param>
    public static void ColorConsoleWrite(ConsoleColor color, StringBuilder text)
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