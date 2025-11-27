namespace RogueConsoleGame.Interfaces;

/// <summary>
/// Used to declare something as visible on the map.
/// </summary>
public interface IVisible
{
    public bool IsVisible { get; }
    public char Symbol { get; }
}