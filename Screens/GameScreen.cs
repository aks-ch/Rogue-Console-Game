using RogueConsoleGame.Enums;
using RogueConsoleGame.Interfaces;
using RogueConsoleGame.World;

namespace RogueConsoleGame.Screens;

public class GameScreen(GameManager gameManager) : IScreen
{
    public readonly GameManager GameManager = gameManager;
    
    public MapTree? MapTree { get; private set; }
    
    // Execute the screen
    public void Draw()
    {
        if (MapTree == null)
        {
            // Get player name
            Console.CursorVisible = true;
            GameManager.ColorConsoleWrite(ConsoleColor.Cyan, "Please enter your name: ");
            string? name = Console.ReadLine();
            name = string.IsNullOrEmpty(name) ? "Player" : name;
            
            MapTree = new MapTree(GameManager, GameManager.Difficulty, name);
            
            if (MapTree.Failed)
            {
                Clear();
                GameManager.CurrentGameState = GameState.Menu;
            }
            
            return;
        }
        
        // Continue if no change
        MapTree.Update();
        
        // Check for victory/defeat
        if (MapTree.Victory)
        {
            Clear();
            GameManager.CurrentGameState = GameState.Victory;
        }
        else if (MapTree.Player.Health <= 0)
        {
            Clear();
            GameManager.CurrentGameState = GameState.Defeat;
        }
    }

    /// <summary>
    /// Clear everything.
    /// </summary>
    private void Clear()
    {
        MapTree = null;
    }
}