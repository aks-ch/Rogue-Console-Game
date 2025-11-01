namespace RogueConsoleGame.Interfaces;

public interface IAttacker
{
    /// <summary>
    /// The strength of the attacker.
    /// </summary>
    public double Strength { get; }

    /// <summary>
    /// Attack a damageable.
    /// </summary>
    /// <param name="damageable">The damageable to attack.</param>
    public void Attack(IDamageable damageable);
}