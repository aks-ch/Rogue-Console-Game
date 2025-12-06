using RogueConsoleGame.DataTypes;
using RogueConsoleGame.Enums;
using RogueConsoleGame.Interfaces;
using RogueConsoleGame.World;
using RogueConsoleGame.World.GameObjects.Characters;

namespace RogueConsoleGame.Screens;

public class GameScreen(GameManager gameManager) : IScreen
{
    public readonly GameManager GameManager = gameManager;
    
    public MapTree? MapTree { get; private set; }
    
    // Execute the screen
    public void Draw()
    {
        // To be modified
        if (MapTree == null)
        {
            MapTree = new MapTree(GameManager, 1);
            return;
        }
        
        MapTree.ActiveMap.OutputMap();
        MapTree.ActiveMap.Update();
        
        // Checking Victory/Defeat
        if (MapTree.Player.Health <= 0)
        {
            Clear();
            GameManager.CurrentGameState = GameState.Defeat;
            return;
        }
        // Need to add Victory or move this elsewhere
    }

    /// <summary>
    /// Clear everything.
    /// </summary>
    private void Clear()
    {
        MapTree = null;
    }
}