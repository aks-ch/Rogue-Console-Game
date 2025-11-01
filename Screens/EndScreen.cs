using RogueConsoleGame.Enums;
using RogueConsoleGame.Interfaces;

namespace RogueConsoleGame.Screens;

public class EndScreen(GameManager gameManager) : IScreen
{
    public bool Victory { private get; set; }
    // Execute the screen.
    public void Draw()
    {
        GameManager.ColorConsoleWriteLine(ConsoleColor.Cyan, "Game Over");
        if (Victory)
        {
            GameManager.ColorConsoleWriteLine(ConsoleColor.Green, "You Win! c:");
        }
        else
        {
            GameManager.ColorConsoleWriteLine(ConsoleColor.Red, "You Lose! :c");
        }
        Console.WriteLine("Press ENTER key to continue...");
        ConsoleKey key;
        do
        {
            key = Console.ReadKey(true).Key;
        } while (key != ConsoleKey.Enter);
        gameManager.CurrentGameState = GameState.Menu;
    }
}