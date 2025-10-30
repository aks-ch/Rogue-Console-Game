namespace RogueConsoleGame.Interfaces;

public interface IAttacker
{
    /// <summary>
    /// The strength of the attacker.
    /// </summary>
    public float Strength { get; }

    /// <summary>
    /// Attack a damageable.
    /// </summary>
    public void Attack(IDamageable damageable);
}