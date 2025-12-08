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
            GameManager.ColorConsoleWrite(ConsoleColor.Cyan, "Please enter your name: ");
            string? name = Console.ReadLine();
            name = string.IsNullOrEmpty(name) ? "Player" : name;
            
            MapTree = new MapTree(GameManager, 1, name);
            return;
        }
        
        // Check for victory/defeat
        if (MapTree.Player.Health <= 0)
        {
            Clear();
            GameManager.CurrentGameState = GameState.Defeat;
        }
        // missing victory
        
        // Continue if no change
        MapTree.Update();
    }

    /// <summary>
    /// Clear everything.
    /// </summary>
    private void Clear()
    {
        MapTree = null;
    }
}