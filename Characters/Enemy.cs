using RogueConsoleGame.Enums;
using RogueConsoleGame.Records;
using RogueConsoleGame.Screens;

namespace RogueConsoleGame.Characters;

public class Enemy : Character
{
    // Enemy Declaration
    public Enemy(GameScreen game, char symbol, int maxHealth, double strength) : base(game, symbol, maxHealth, strength) {}

    /// <summary>
    /// Make the enemy move towards a target location.
    /// </summary>
    /// <param name="target">The target location to move towards.</param>
    /// <exception cref="Exception">The target location is inside the enemy.</exception>
    /// <returns>True if I moved. False if I didn't.</returns>
    public bool ChooseMove(Vector2 target)
    {
        Direction direction = 0;
        
        // Determine direction
        if (target.Y < Position.Y) direction |= Direction.North;
        if (target.Y > Position.Y) direction |= Direction.South;
        if (target.X > Position.X) direction |= Direction.East;
        if (target.X < Position.X) direction |= Direction.West;

        // Invalid direction
        if (direction == 0) throw new Exception("Enemy's target location is inside itself");
        
        bool random = new Random().Next(0, 2) == 1;
        
        // movement
        var newPosition = direction switch
        {
            // Linear directions
            Direction.North => Position with { Y = Position.Y - 1 },
            Direction.South => Position with { Y = Position.Y + 1 },
            Direction.East  => Position with { X = Position.X + 1 },
            Direction.West  => Position with { X = Position.X - 1 },

            // Your diagonal "random choice" logic
            Direction.North | Direction.East => random
                ? Position with { Y = Position.Y - 1 } // North
                : Position with { X = Position.X + 1 }, // East

            Direction.South | Direction.East => random
                ? Position with { Y = Position.Y + 1 } // South
                : Position with { X = Position.X + 1 }, // East

            Direction.South | Direction.West => random
                ? Position with { Y = Position.Y + 1 } // South
                : Position with { X = Position.X - 1 }, // West

            Direction.North | Direction.West => random
                ? Position with { Y = Position.Y - 1 } // North
                : Position with { X = Position.X - 1 }, // West
    
            _ => throw new ArgumentOutOfRangeException()
        };

        return ProcessMove(newPosition);
    }
    
    /// <summary>
    /// Process what to do about moving to a new position.
    /// </summary>
    /// <param name="newPosition">The new position.</param>
    /// <returns>True if the enemy moved. Else false.</returns>
    private bool ProcessMove(Vector2 newPosition)
    {
        // new location outside map or enemy in new location
        if (PositionOutOfBounds(newPosition) || Game.Enemies.ContainsKey(newPosition)) return false;

        // player in new location
        if (Game.PlayerPosition == newPosition)
        {
            if (Game.Player == null) throw new NullReferenceException("The player doesn't exist!");
            Attack(Game.Player);
        }
        // move to location
        else
        {
            Position = newPosition;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Enemy modified implementation of randomizing the position.
    /// </summary>
    protected override void RandomizePosition()
    {
        if (Game.PlayerPosition == null) throw new NullReferenceException("The player position doesn't exist!");
        do
        {
            base.RandomizePosition();
        } while (Math.Abs(Game.PlayerPosition.X - Position.X) < 2 || Math.Abs(Game.PlayerPosition.Y - Position.Y) < 2);
    }
}