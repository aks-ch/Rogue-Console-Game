namespace RogueConsoleGame;

/// <summary>
/// Create and manage an instance of the game.
/// </summary>
public class GameManager
{
    
    private int minGameWidth { get; }
    private int maxGameWidth { get; }
    
    private int minGameHeight { get; }
    private int maxGameHeight { get; }
    
    private int minEnemyCount { get; }
    private int maxEnemyCount { get; }
    
    private bool isRunning { get; set; }
    
    public GameManager()
    {
        
        minGameWidth = 15;
        maxGameWidth = 24;
        minGameHeight = 20;
        maxGameHeight = 40;
        minEnemyCount = 3;
        maxEnemyCount = 10;

        throw new NotImplementedException();

    }
    
    /// <summary>
    /// Run the game in this instance of the game manager.
    /// </summary>
    public void Run()
    {
        
        isRunning = true;
        
        while (isRunning)
        {

            throw new NotImplementedException();

        }
        
    }

    /// <summary>
    /// Output the guide of the game.
    /// </summary>
    public static void DrawGuide()
    {
        throw new NotImplementedException();
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