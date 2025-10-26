using RogueConsoleGame.Interfaces;

namespace RogueConsoleGame.Screens;

public class MenuScreen(GameManager gameManager) : IScreen
{

    private bool _haveSettings = false;
    private int _gameHeight = gameManager.MinGameHeight;
    private int _gameWidth = gameManager.MinGameWidth;
    private int _enemyCount = gameManager.MinEnemyCount;

    private bool viewGuide = false;

    /// <summary>
    /// Draw & execute the menu.
    /// </summary>
    public void Draw()
    {
        
        GameManager.ColorConsoleWriteLine(ConsoleColor.Red, "Welcome to RogueConsoleGame!");
        Console.WriteLine( "Please choose one of the listed options by entering the corresponding number:");
        Console.WriteLine( "\t1. Start a new game with random settings.");
        Console.WriteLine( "\t2. Start a new game with custom settings.");
        Console.WriteLine($"\t3. {(viewGuide ? "Hide" : "View")} the guide.");
        Console.WriteLine( "\t4. Exit.");
        if (viewGuide)
        {
            GameManager.DrawGuide();
        }

        switch (Console.ReadKey().Key)
        {
            case ConsoleKey.D1:
                RandomGameOptions();
                break;
            case ConsoleKey.D2:
                CustomGameOptions();
                break;
            case ConsoleKey.D3:
                viewGuide = !viewGuide;
                break;
            case ConsoleKey.D4:
                gameManager.IsRunning  = false;
                break;
        }
        
    }

    /// <summary>
    /// Randomize game options.
    /// </summary>
    private void RandomGameOptions()
    {
        throw new NotImplementedException("Random game options not implemented");
    }

    /// <summary>
    /// Select custom game options.
    /// </summary>
    private void CustomGameOptions()
    {
        throw new NotImplementedException("Custom game options not implemented");
    }
    
}