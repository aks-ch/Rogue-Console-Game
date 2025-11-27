using RogueConsoleGame.Interfaces;

namespace RogueConsoleGame.World.GameObjects;

/// <summary>
/// Represents a wall object in the game world.
/// </summary>
public class Wall: IVisible
{
    public bool IsVisible { get; set; }
    public char Symbol { get; private set; }
    
    private GameManager _gameManager;

    public Wall(GameManager gameManager)
    {
        _gameManager = gameManager;
    }

    /// <summary>
    /// Checks for adjacent walls and modifies its own symbol accordingly.
    /// </summary>
    public void CheckSymbol()
    {
        throw new NotImplementedException();
    }
}