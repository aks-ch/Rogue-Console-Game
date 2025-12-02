using RogueConsoleGame.DataTypes;
using RogueConsoleGame.Screens;

namespace RogueConsoleGame.World.GameObjects.Characters;

public class Player : Character
{
    /// <summary>
    /// Name of the player.
    /// </summary>
    public string Name;
    
    public bool IsHealing = false;
    
    private bool _interacted = false;
    private int _healCooldown;
    private readonly int _healCooldownMax;
    private readonly double _healFactor;
    
    public Player(GameScreen game, string name, char symbol, int maxHealth, double strength) : base(game, symbol,
        maxHealth, strength)
    {
        Name = name;
        _healCooldownMax = game.GameManager.Player.HealCooldown;
        _healCooldown = _healCooldownMax;
        _healFactor = Math.Round(game.GameManager.Player.HealFactor, 1);
    }

    /// <summary>
    /// Check if input key is relevant.
    /// </summary>
    /// <returns>True if the key input was valid and processed (even if nothing changed with the player). Else False.</returns>
    public bool CheckKey()
    {
        Vector2 newPosition;

        do
        {
            newPosition = Console.ReadKey(true).Key switch
            {
                ConsoleKey.UpArrow => Position with { Y = Position.Y - 1 },
                ConsoleKey.DownArrow => Position with { Y = Position.Y + 1 },
                ConsoleKey.LeftArrow => Position with { X = Position.X - 1 },
                ConsoleKey.RightArrow => Position with { X = Position.X + 1 },
     
                _ => Position
            };
        } while (newPosition == Position);

        // valid key
        ProcessMove(newPosition);
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
            IsHealing = false;
            return;
        }

        if (_healCooldown > 0)
        {
            _healCooldown--;
        }
        else if (Health < MaxHealth)
        {
            IsHealing = true;
            Health = (Health + _healFactor > MaxHealth) ? MaxHealth : Math.Round(Health + _healFactor, 1);
        }
        else 
        {
            IsHealing = false;
        }
    }

    /// <summary>
    /// Process what to do about moving to a new position.
    /// </summary>
    /// <param name="newPosition">The new position.</param>
    private void ProcessMove(Vector2 newPosition)
    {
        // new location outside map
        if (PositionOutOfBounds(newPosition)) return;

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

    /// <summary>
    /// Take damage.
    /// </summary>
    /// <param name="damage">The amount of damage to take.</param>
    public override void TakeDamage(double damage)
    {
        base.TakeDamage(damage);
        _interacted = true;
    }
}