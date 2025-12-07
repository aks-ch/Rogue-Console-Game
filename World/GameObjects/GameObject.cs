using RogueConsoleGame.DataTypes;
using RogueConsoleGame.Interfaces;

namespace RogueConsoleGame.World.GameObjects;

/// <summary>
/// Default code for a game object.
/// </summary>
/// <param name="map">The map this game object belongs to.</param>
/// <param name="position">The position of the game object.</param>
public abstract class GameObject(Map map, Vector2 position) : IVisible
{

    /// <summary>
    /// The map this game object belongs to.
    /// </summary>
    public virtual Map Map { get; set; } = map;
    
    public bool IsVisible { get; set; } = true;
    public virtual char Symbol { get; protected set; }
    
    public virtual ConsoleColor Color { get; set; } = ConsoleColor.White;
    public virtual Vector2 Position { get; set; } = position;
}