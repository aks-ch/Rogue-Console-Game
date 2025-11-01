namespace RogueConsoleGame.Interfaces;

public interface IDamageable
{
    /// <summary>
    /// Current health of the damageable.
    /// </summary>
    protected double Health { get; set; }
    
    /// <summary>
    /// Maximum health of the damageable.
    /// </summary>
    protected double MaxHealth { get; }
    
    /// <summary>
    /// Take damage.
    /// </summary>
    /// <param name="damage">The damage to take.</param>
    public void TakeDamage(double damage);
}