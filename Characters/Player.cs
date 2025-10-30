using RogueConsoleGame.Screens;
using RogueConsoleGame.Structures;

namespace RogueConsoleGame.Characters;

public class Player : Character
{
    public Player(GameScreen game, char symbol, float maxHealth, float strength) : base(game, symbol,
        maxHealth, strength)
    {
    }

    /// <summary>
    /// Check if input key is relevant.
    /// </summary>
    /// <param name="key">The input key.</param>
    public void CheckKey(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.UpArrow:
                ProcessMove(Position with { Y = Position.Y - 1 });
                break;
        }
    }

    /// <summary>
    /// Process what to do about moving to a new position.
    /// </summary>
    /// <param name="newPosition">The new position.</param>
    private void ProcessMove(Vector2 newPosition)
    {
        if (newPosition.X < 0 ||
            newPosition.X >= Game.GameManager.GameWidth ||
            newPosition.Y < 0 ||
            newPosition.X >= Game.GameManager.GameHeight)
        {
            return;
        }
    }
}