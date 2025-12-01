using RogueConsoleGame.Enums;
using RogueConsoleGame.Interfaces;
using RogueConsoleGame.Records;

namespace RogueConsoleGame.World.GameObjects;

/// <summary>
/// Represents a wall object in the game world.
/// </summary>
public class Wall: GameObject
{
    private GameManager _gameManager;

    /// <summary>
    /// Initialize a wall on the map.
    /// </summary>
    /// <param name="gameManager">The associated game manager.</param>
    /// <param name="position">The position of the wall on the map.</param>
    public Wall(GameManager gameManager, Vector2 position) : base(position)
    {
        _gameManager = gameManager;
    }

    /// <summary>
    /// Checks for adjacent walls and modifies its own symbol accordingly.
    /// </summary>
    /// <param name="map">The map this wall is on.</param>
    public void CheckSymbol(Map map)
    {
        Direction adjacentWalls = 0;
        IVisible[,] grid = map.Grid;
        
        // North
        if (Position.Y - 1 >= 0 &&
            grid[Position.Y - 1, Position.X].IsVisible &&
            grid[Position.Y - 1, Position.X].GetType() == GetType())
        {
            adjacentWalls |= Direction.North;
        }
        
        // South
        if (Position.Y + 1 < map.MapHeight &&
            grid[Position.Y + 1, Position.X].IsVisible &&
            grid[Position.Y + 1, Position.X].GetType() == GetType())
        {
            adjacentWalls |= Direction.South;
        }
        
        // East
        if (Position.X + 1 < map.MapWidth &&
            grid[Position.Y, Position.X + 1].IsVisible &&
            grid[Position.Y, Position.X + 1].GetType() == GetType())
        {
            adjacentWalls |= Direction.East;
        }
        
        // West
        if (Position.X - 1 >= 0 &&
            grid[Position.Y, Position.X - 1].IsVisible &&
            grid[Position.Y, Position.X - 1].GetType() == GetType())
        {
            adjacentWalls |= Direction.West;
        }

        Symbol = _gameManager.WallChars[(int)adjacentWalls];
    }
    
    /// <summary>
    /// Checks whether it is completely surrounded with walls. If yes, this wall does not need to be visible.
    /// </summary>
    /// <param name="map">The map this wall is on.</param>
    public void CheckVisibility(Map map)
    {
        if (map.GetSurroundingCount<Wall>(Position, map.Grid, out _) == 8) IsVisible = false;
    }
}