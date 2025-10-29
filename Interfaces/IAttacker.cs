namespace RogueConsoleGame.Interfaces;

public interface IAttacker
{
    /// <summary>
    /// The strength of the attacker.
    /// </summary>
    protected float Strength { get; }

    /// <summary>
    /// Attack a damagable.
    /// </summary>
    public void Attack(IDamageable damageable);
}