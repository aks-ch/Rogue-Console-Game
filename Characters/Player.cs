using RogueConsoleGame.Records;
using RogueConsoleGame.Screens;

namespace RogueConsoleGame.Characters;

public class Player : Character
{
    /// <summary>
    /// Name of the player.
    /// </summary>
    public string Name;
    
    private bool _interacted = false;
    
    private int _healCooldownMax = 5;
    private int _healCooldown;
    private float _healFactor = 0.2F;
    
    public Player(GameScreen game, string name, char symbol, float maxHealth, float strength) : base(game, symbol,
        maxHealth, strength)
    {
        Name = name;
        _healCooldown = _healCooldownMax;
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
    /// Check if healing possible or to reduce/reset the heal cooldown.
    /// </summary>
    public void Heal()
    {
        if (_interacted)
        {
            _healCooldown = _healCooldownMax;
            _interacted = false;
            return;
        }

        if (_healCooldown > 0)
        {
            _healCooldown--;
        }
        else
        {
            Health += _healFactor;
        }
    }

    /// <summary>
    /// Process what to do about moving to a new position.
    /// </summary>
    /// <param name="newPosition">The new position.</param>
    private void ProcessMove(Vector2 newPosition)
    {
        // new location outside map
        if (newPosition.X < 0 ||
            newPosition.X >= Game.GameManager.GameWidth ||
            newPosition.Y < 0 ||
            newPosition.X >= Game.GameManager.GameHeight)
        {
            return;
        }

        // enemy in new location
        if (Game.Enemies.ContainsKey(newPosition))
        {
            Attack(Game.Enemies[newPosition]);
            _interacted = true;
        }
        // move to location
        else
        {
            Position = newPosition;
        }
    }

    public virtual void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        _interacted = true;
    }
}