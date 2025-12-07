using RogueConsoleGame.DataTypes;
using RogueConsoleGame.Interfaces;
using RogueConsoleGame.Screens;

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
    public Character(Map map, char symbol, int maxHealth, double strength) : base(map, new Vector2(0, 0))
    {
        Symbol = symbol;
        Health = maxHealth;
        MaxHealth = maxHealth;
        Strength = strength;
        
        GameManager = map.GameManager;
        RandomizePosition();
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

    /// <summary>
    /// Randomize the position of the character accounting for positions not allowed.
    /// </summary>
    /// <param name="bannedPositions">Any banned positions to pass by default.</param>
    protected void RandomizePosition(List<EmptySpace>? bannedPositions = null)
    {
        List<EmptySpace> possiblePositions = Map.GetAll<EmptySpace>();

        // Restrict positions near parent hallway spawn
        if (Map.Parent != null && Map.Grid[Map.Parent.Spawn.Y, Map.Parent.Spawn.X] is EmptySpace space)
        {
            var notAllowed = Map.GetConnected(space, 5);
            
            if (bannedPositions == null) bannedPositions = notAllowed;
            else bannedPositions.AddRange(notAllowed);
        }

        // Remove banned positions
        if (bannedPositions is { Count: > 0 })
        {
            foreach (var bannedPosition in bannedPositions) possiblePositions.Remove(bannedPosition);
        }

        // Select random position
        Position = possiblePositions[GameManager.Seed.Next(0, possiblePositions.Count)].Position;
    }
}