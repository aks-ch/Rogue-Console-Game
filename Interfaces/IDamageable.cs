namespace RogueConsoleGame.Interfaces;

public interface IDamageable
{
    /// <summary>
    /// Current health of the damageable.
    /// </summary>
    protected float Health { get; set; }
    
    /// <summary>
    /// Maximum health of the damageable.
    /// </summary>
    protected float MaxHealth { get; }
    
    /// <summary>
    /// Take damage.
    /// </summary>
    public void TakeDamage();
}