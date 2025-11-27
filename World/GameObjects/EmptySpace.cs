using RogueConsoleGame.Interfaces;

namespace RogueConsoleGame.World.GameObjects;

/// <summary>
/// Represents an empty space in the game world.
/// </summary>
public class EmptySpace: IVisible
{
    public bool IsVisible { get; set; }
    public char Symbol { get; }
    
    public EmptySpace(char symbol)
    {
        Symbol = symbol;
    }
    
}