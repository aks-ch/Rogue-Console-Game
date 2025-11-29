using RogueConsoleGame.Enums;
using RogueConsoleGame.Interfaces;

namespace RogueConsoleGame.World.GameObjects;

/// <summary>
/// Represents a wall object in the game world.
/// </summary>
public class Wall: GameObject
{
    private GameManager _gameManager;

    public Wall(GameManager gameManager, int y, int x) : base(y, x)
    {
        _gameManager = gameManager;
    }

    /// <summary>
    /// Checks for adjacent walls and modifies its own symbol accordingly.
    /// </summary>
    public void CheckSymbol(Map map, int row, int column)
    {
        Direction adjecentWalls = 0;
        IVisible[,] grid = map.Grid;
        
        // North
        if (row - 1 >= 0 && grid[row - 1, column].GetType() == this.GetType())
        {
            adjecentWalls |= Direction.North;
        }
        
        // South
        if (row + 1 < map.MapHeight && grid[row + 1, column].GetType() == this.GetType())
        {
            adjecentWalls |= Direction.South;
        }
        
        // East
        if (column + 1 < map.MapWidth && grid[row, column + 1].GetType() == this.GetType())
        {
            adjecentWalls |= Direction.East;
        }
        
        // West
        if (column - 1 >= 0 && grid[row, column - 1].GetType() == this.GetType())
        {
            adjecentWalls |= Direction.West;
        }

        Symbol = _gameManager.WallChars[(int)adjecentWalls];
    }
}