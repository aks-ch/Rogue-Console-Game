using RogueConsoleGame.Enums;
using RogueConsoleGame.Screens;

namespace RogueConsoleGame;

/// <summary>
/// Create and manage an instance of the game.
/// </summary>
public class GameManager
{

    public bool IsRunning;
    
    public int MinGameWidth { get; }
    public int MaxGameWidth { get; }
    
    public int MinGameHeight { get; }
    public int MaxGameHeight { get; }
    
    public int MinEnemyCount { get; }
    public int MaxEnemyCount { get; }

    public MenuScreen MenuS { get; }
    public GameScreen GameS { get; }
    public VictoryScreen VictoryS { get; }
    public DefeatScreen DefeatS { get; }
    
    public Screen CurrentScreen { get; set; }
    
    /// <summary>
    /// Initialize this game manager's game.
    /// </summary>
    public GameManager()
    {
        
        MinGameWidth = 15;
        MaxGameWidth = 24;
        MinGameHeight = 20;
        MaxGameHeight = 40;
        MinEnemyCount = 3;
        MaxEnemyCount = 10;

        MenuS = new MenuScreen(this);
        GameS = new GameScreen(this);
        VictoryS = new VictoryScreen(this);
        DefeatS = new DefeatScreen(this);

    }
    
    /// <summary>
    /// Run the game in this instance of the game manager.
    /// </summary>
    public void Run()
    {
        
        IsRunning = true;
        CurrentScreen = Screen.Menu;
        
        while (IsRunning)
        {

            Console.Clear();
            switch (CurrentScreen)
            {
                
                case Screen.Menu:
                    MenuS.Draw();
                    break;
                case Screen.Game:
                    GameS.Draw();
                    break;
                case Screen.Victory:
                    VictoryS.Draw();
                    break;
                case Screen.Defeat:
                    DefeatS.Draw();
                    break;
                default:
                    throw new Exception("An impossible error became possible! Invalid screen!");
                
            }

        }
        
    }

    /// <summary>
    /// Output the guide of the game.
    /// </summary>
    public static void DrawGuide()
    {
        //throw new NotImplementedException("Guide not implemented");
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