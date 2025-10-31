using RogueConsoleGame.Interfaces;
using RogueConsoleGame.Screens;
using RogueConsoleGame.Structures;

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
    
    public void TakeDamage(float damage)
    {
        Health = (Health - damage) < 0 ? 0 : Health - damage;
    }

    public void Attack(IDamageable damageable)
    {
        damageable.TakeDamage(Strength);
    }

    /// <summary>
    /// Randomize the position of the character.
    /// </summary>
    protected void RandomizePosition()
    {
        do
        {
            Position = new Vector2(GameManager.Seed.Next(0, GameManager.GameWidth),
                GameManager.Seed.Next(0, GameManager.GameHeight));
        } while (Game.CharactersByPosition.ContainsValue(Position));
    }
}