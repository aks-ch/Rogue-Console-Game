using RogueConsoleGame.DataTypes;
using RogueConsoleGame.Interfaces;

namespace RogueConsoleGame.World.GameObjects.Characters;

public abstract class Character : GameObject, IDamageable, IAttacker
{
    public double Health { get; set; }
    public int MaxHealth { get; }
    public double Strength { get; }
    
    protected readonly GameManager GameManager;
    
    /// <summary>
    /// Constructor a new character.
    /// </summary>
    /// <param name="map">The map that this character falls under.</param>
    /// <param name="symbol">The symbol this character is represented as on the board.</param>
    /// <param name="maxHealth">The maximum health of the character.</param>
    /// <param name="strength">The strength of the character.</param>
    public Character(Map map, Vector2 position, char symbol, int maxHealth, double strength) : base(map, position)
    {
        Symbol = symbol;
        Health = maxHealth;
        MaxHealth = maxHealth;
        Strength = strength;
        
        GameManager = map.GameManager;
    }
    
    // Take Damage
    public virtual void TakeDamage(double damage)
    {
        Health = (Health - damage) < 0 ? 0 : Math.Round(Health - damage, 1);
    }

    // Attack another damageable
    public void Attack(IDamageable damageable)
    {
        damageable.TakeDamage(Strength);
    }
}