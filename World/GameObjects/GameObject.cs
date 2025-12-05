using RogueConsoleGame.DataTypes;
using RogueConsoleGame.Interfaces;

namespace RogueConsoleGame.World.GameObjects;

/// <summary>
/// Default code for a game object.
/// </summary>
/// <param name="position">The position of the game object.</param>
public abstract class GameObject(Vector2 position) : IVisible
{
    public bool IsVisible { get; set; } = true;
    public char Symbol { get; protected set; }
    public Vector2 Position { get; } = position;
}