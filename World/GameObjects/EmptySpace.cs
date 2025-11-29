using RogueConsoleGame.Interfaces;

namespace RogueConsoleGame.World.GameObjects;

/// <summary>
/// Represents an empty space in the game world.
/// </summary>
public class EmptySpace : GameObject
{
    public EmptySpace(char symbol, int y, int x) : base(x, y)
    {
        Symbol = symbol;
    }
}