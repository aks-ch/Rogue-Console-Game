using RogueConsoleGame.DataTypes;

namespace RogueConsoleGame.World.GameObjects;

/// <summary>
/// Represents an empty space in the game world.
/// </summary>
public sealed class EmptySpace : GameObject
{
    public override ConsoleColor Color => Explored ? ConsoleColor.DarkGray : ConsoleColor.Gray;
    public bool Explored { get; set; }
    
    /// <summary>
    /// Initialize an empty space on the map.
    /// </summary>
    /// <param name="map">The map this empty space is on.</param>
    /// <param name="position">The position of the empty space on the map.</param>
    public EmptySpace(Map map, Vector2 position) : base(map, position)
    {
        Symbol = map.GameManager.EmptyChar;
    }
}