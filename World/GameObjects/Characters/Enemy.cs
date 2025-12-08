using RogueConsoleGame.DataTypes;
using RogueConsoleGame.Enums;

namespace RogueConsoleGame.World.GameObjects.Characters;

public class Enemy(Map map, Vector2 position, char symbol, int maxHealth, double strength)
    : Character(map, position, symbol, maxHealth, strength)
{
    /// <summary>
    /// Make the enemy move towards a target location. (NEED TO MODIFY!)
    /// </summary>
    /// <param name="target">The target location to move towards.</param>
    public void ChooseMove(Vector2 target)
    {
        Direction direction = 0;
        
        // Determine direction
        if (target.Y < Position.Y) direction |= Direction.North;
        if (target.Y > Position.Y) direction |= Direction.South;
        if (target.X > Position.X) direction |= Direction.East;
        if (target.X < Position.X) direction |= Direction.West;

        // Invalid direction
        if (direction == 0) return;
        
        bool random = new Random().Next(0, 2) == 1;
        
        // movement
        var newPosition = direction switch
        {
            // Linear directions
            Direction.North => Position with { Y = Position.Y - 1 },
            Direction.South => Position with { Y = Position.Y + 1 },
            Direction.East  => Position with { X = Position.X + 1 },
            Direction.West  => Position with { X = Position.X - 1 },

            // diagonal directions
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

        Position = newPosition;
    }
}