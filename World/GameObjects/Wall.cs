using RogueConsoleGame.Enums;
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
    public void CheckSymbol(IVisible[,] grid, int row, int column)
    {
        Direction adjecentWalls = 0;
        
        // North
        if (grid[row - 1, column].GetType() == this.GetType())
        {
            adjecentWalls |= Direction.North;
        }
        
        // South
        if (grid[row + 1, column].GetType() == this.GetType())
        {
            adjecentWalls |= Direction.South;
        }
        
        // East
        if (grid[row, column + 1].GetType() == this.GetType())
        {
            adjecentWalls |= Direction.East;
        }
        
        // West
        if (grid[row, column - 1].GetType() == this.GetType())
        {
            adjecentWalls |= Direction.West;
        }

        throw new NotImplementedException();
    }
}