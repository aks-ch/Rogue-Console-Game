using RogueConsoleGame.Interfaces;
using RogueConsoleGame.DataTypes;

namespace RogueConsoleGame.World.GameObjects;

/// <summary>
/// Represents an empty space in the game world.
/// </summary>
public class EmptySpace : GameObject
{
    /// <summary>
    /// Initialize an empty space on the map.
    /// </summary>
    /// <param name="symbol">The symbol to use for the empty space.</param>
    /// <param name="position">The position of the empty space on the map.</param>
    public EmptySpace(char symbol, Vector2 position) : base(position)
    {
        Symbol = symbol;
    }
}