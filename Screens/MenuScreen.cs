using RogueConsoleGame.Interfaces;

namespace RogueConsoleGame.Screens;

public class MenuScreen : IScreen
{

    private bool haveSettings;
    private int gameHeight;
    private int gameWidth;
    private int enemyCount;

    public MenuScreen()
    {
        haveSettings = false;
        gameWidth = 15;
        gameHeight = 20;
        enemyCount = 3;
        
        throw new NotImplementedException();
        
    }

    /// <summary>
    /// Draw the menu.
    /// </summary>
    public void Draw()
    {
        GameManager.ColorConsoleWriteLine(ConsoleColor.Red, "Welcome to RogueConsoleGame!");
        Console.WriteLine("Please choose one of the listed options by entering the corresponding number:");
        Console.WriteLine("\t1. Start a new game with random settings.");
        Console.WriteLine("\t2. Start a new game with custom settings.");
        Console.WriteLine("\t3. View the guide.");
        Console.WriteLine("\t4. Exit.");
        MenuOptions();
    }

    /// <summary>
    /// Read and handle the key input.
    /// </summary>
    public void MenuOptions()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Select custom game options.
    /// </summary>
    public void CustomGameOptions()
    {
        throw new NotImplementedException();
    }
    
}