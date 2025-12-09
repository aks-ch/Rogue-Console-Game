using RogueConsoleGame.DataTypes;

namespace RogueConsoleGame.World.GameObjects.Characters;

public class Player : Character
{
    /// <summary>
    /// Name of the player.
    /// </summary>
    public string Name;
    
    public bool IsHealing;
    public int ExploreRange { get; private set; } = 3;
    public int AttractRange { get; private set; } = 10;
    
    private bool _interacted;
    private int _healCooldown;
    private readonly int _healCooldownMax;
    private readonly double _healFactor;

    public override ConsoleColor Color =>
        Health == MaxHealth
            ? ConsoleColor.DarkCyan
            : IsHealing
                ? ConsoleColor.DarkYellow
                : Health > MaxHealth / 4
                    ? ConsoleColor.DarkGreen
                    : ConsoleColor.DarkRed;

    public Player(Map map, Vector2 position, string name, char symbol, int maxHealth, double strength) : base(map, position, symbol,
        maxHealth, strength)
    {
        Name = name;
        _healCooldownMax = GameManager.DataInitializer.Player.HealCooldown;
        _healCooldown = _healCooldownMax;
        _healFactor = Math.Round(GameManager.DataInitializer.Player.HealFactor, 1);
    }

    /// <summary>
    /// Check if input key is relevant.
    /// </summary>
    public void CheckKey()
    {
        // Don't exit until a key that was processed by the player is entered
        while (true)
        {
            if (!Console.KeyAvailable) continue;
            
            ConsoleKeyInfo info = new ConsoleKeyInfo();

            // Clear buffer
            while (Console.KeyAvailable)
            {
                info = Console.ReadKey(true);
            }

            // Check for valid input
            Vector2 newPosition = info.Key switch
            {
                ConsoleKey.UpArrow => Position with { Y = Position.Y - 1 },
                ConsoleKey.DownArrow => Position with { Y = Position.Y + 1 },
                ConsoleKey.LeftArrow => Position with { X = Position.X - 1 },
                ConsoleKey.RightArrow => Position with { X = Position.X + 1 },
                _ => Position
            };

            // Process only if valid
            if (newPosition == Position) continue;
            
            Position = newPosition;
            break;
        }
    }

    /// <summary>
    /// Attract enemies in a range.
    /// </summary>
    public void Attract()
    {
        Queue<(int Distance, Vector2 Position)> toSearch = [];
        HashSet<Vector2> searched = [];
        
        toSearch.Enqueue((0, Position));

        while (toSearch.Count > 0)
        {
            var next = toSearch.Dequeue();
            
            // Already searched
            if (!searched.Add(next.Position)) continue;

            if (next.Distance >= AttractRange) continue;
            
            Map.GetAdjacentCount(next.Position, Map.Grid, out List<GameObject> adjacent);
            foreach (var space in adjacent)
            {
                if (space is Wall or Hallway) continue;
                if (space is Enemy enemy) enemy.ShouldMove = true;
                
                if (next.Distance < AttractRange && !searched.Contains(space.Position))
                {
                    toSearch.Enqueue((next.Distance + 1, space.Position));
                }
            }
        }
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
    /// Take damage.
    /// </summary>
    /// <param name="damage">The amount of damage to take.</param>
    public override void TakeDamage(double damage)
    {
        base.TakeDamage(damage);
        _interacted = true;
    }

    /// <summary>
    /// Increase the exploration range by a factor.
    /// </summary>
    /// <param name="factor">The factor to increase it by.</param>
    public void IncreaseExploreRange(int factor)
    {
        ExploreRange += factor;
    }
}