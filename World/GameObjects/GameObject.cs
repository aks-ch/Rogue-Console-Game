using RogueConsoleGame.Interfaces;
using RogueConsoleGame.Records;

namespace RogueConsoleGame.World.GameObjects;

public abstract class GameObject(int y, int x) : IVisible
{
    public bool IsVisible { get; set; } = true;
    public char Symbol { get; protected set; }
    public Vector2 Position { get; } = new Vector2(x, y);
}