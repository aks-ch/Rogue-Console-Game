using RogueConsoleGame.Interfaces;

namespace RogueConsoleGame.World.GameObjects;

public class EmptySpace: IVisible
{
    public bool IsVisible { get; private set; }
    public char Symbol { get; }
    
    public EmptySpace(char symbol)
    {
        Symbol = symbol;
    }
    
}