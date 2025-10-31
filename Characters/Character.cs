using RogueConsoleGame.Interfaces;
using RogueConsoleGame.Records;
using RogueConsoleGame.Screens;

namespace RogueConsoleGame.Characters;

public abstract class Character : IDamageable, IAttacker
{
    public Vector2 Position;
    
    public float Health { get; set; }
    public float MaxHealth { get; }
    public float Strength { get; }
    
    public char Symbol { get; }
    
    protected GameScreen Game;
    protected GameManager GameManager;
    
    /// <summary>
    /// Constructor a new character.
    /// </summary>
    /// <param name="game">The game manager that this character falls under.</param>
    /// <param name="symbol">The symbol this character is represented as on the board.</param>
    /// <param name="maxHealth">The maximum health of the character.</param>
    /// <param name="strength">The strength of the character.</param>
    public Character(GameScreen game, char symbol, float maxHealth, float strength)
    {
        Symbol = symbol;
        Health = maxHealth;
        MaxHealth = maxHealth;
        Strength = strength;
        
        Game = game;
        GameManager = game.GameManager;
        
        Position = new Vector2(0, 0);
        RandomizePosition();
    }
    
    // Take Damage
    public void TakeDamage(float damage)
    {
        Health = (Health - damage) < 0 ? 0 : Health - damage;
    }

    // Attack another damageable
    public void Attack(IDamageable damageable)
    {
        damageable.TakeDamage(Strength);
    }

    /// <summary>
    /// Randomize the position of the character so that it is not in a space already occupied.
    /// </summary>
    protected virtual void RandomizePosition()
    {
        do
        {
            Position = new Vector2(GameManager.Seed.Next(0, GameManager.GameWidth),
                GameManager.Seed.Next(0, GameManager.GameHeight));
        } while (Game.Enemies.ContainsKey(Position) || Position == Game.PlayerPosition);
    }

    /// <summary>
    /// Checks if a position is out of the map or not.
    /// </summary>
    /// <param name="position">The position to check.</param>
    /// <returns>True if outside map. False if inside.</returns>
    protected bool PositionOutOfBounds(Vector2 position)
    {
        if (position.X < 0 ||
            position.X >= Game.GameManager.GameWidth ||
            position.Y < 0 ||
            position.X >= Game.GameManager.GameHeight)
        {
            return true;
        }
        return false;
    }
}