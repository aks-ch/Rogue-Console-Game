using RogueConsoleGame.Screens;
using RogueConsoleGame.Structures;

namespace RogueConsoleGame.Characters;

public class Player : Character
{
    /// <summary>
    /// Name of the player.
    /// </summary>
    public string Name;
    
    public Player(GameScreen game, string name, char symbol, float maxHealth, float strength) : base(game, symbol,
        maxHealth, strength)
    {
        Name = name;
    }

    /// <summary>
    /// Check if input key is relevant.
    /// </summary>
    /// <param name="key">The input key.</param>
    /// <returns>True if the key input was valid and processed (even if nothing changed with the player). Else False.</returns>
    public bool CheckKey(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.UpArrow:
                ProcessMove(Position with { Y = Position.Y - 1 });
                break;
            case ConsoleKey.DownArrow:
                ProcessMove(Position with { Y = Position.Y + 1 });
                break;
            case ConsoleKey.LeftArrow:
                ProcessMove(Position with { X = Position.X - 1 });
                break;
            case ConsoleKey.RightArrow:
                ProcessMove(Position with { X = Position.X + 1 });
                break;
            default:
                return false;
        }
        return true;
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

        if (Game.Enemies.ContainsKey(newPosition))
        {
            Attack(Game.Enemies[newPosition]);
        }
    }
}