using RogueConsoleGame.DataTypes;
using RogueConsoleGame.Enums;
using RogueConsoleGame.World.GameObjects.Items;

namespace RogueConsoleGame.World.GameObjects.Characters;

public class Enemy(Map map, Vector2 position, char symbol, int maxHealth, double strength)
    : Character(map, position, symbol, maxHealth, strength)
{
    
    private Queue<Vector2>? CurrentPath { get; set; }
    private Vector2? CurrentTarget { get; set; }
    
    /// <summary>
    /// Node for A* pathfinding.
    /// </summary>
    private class Node(Vector2 position)
    {
        public Node? PreviousNode { get; set; }
        public Vector2 Position { get; } = position;
        public int G { get; set; }
        public int H { get; set; }
        public int F => G + H;
    }
    
    /// <summary>
    /// Make the enemy move towards a target location.
    /// </summary>
    /// <param name="target">The target location to move towards.</param>
    public void ChooseMove(Vector2 target)
    {
        // Get Path
        if (CurrentTarget != target || CurrentPath is null || CurrentPath.Count == 0)
        {
            CurrentPath = AStarPathfind(target);
            CurrentTarget = target;
        }
        
        // No path
        if (CurrentPath is null) return;
        
        // Get next
        var newPosition = CurrentPath.Dequeue();
        
        // Get next again if needed
        if (newPosition == Position)
        {
            newPosition = CurrentPath.Dequeue();
        }
        
        // Next location too far
        if (newPosition.GetGeometricDistance(Position) > 1)
        {
            CurrentPath = null;
            return;
        }

        Position = newPosition;
    }

    /// <summary>
    /// Use A* algorithm (treating the map grid like a graph) to find a path to a target.
    /// </summary>
    private Queue<Vector2>? AStarPathfind(Vector2 target)
    {
        PriorityQueue<Node, int> possibleNodes = new PriorityQueue<Node, int>();
        Dictionary<Vector2, int> gCosts = new Dictionary<Vector2, int>();
        HashSet<Vector2> evaluated = [];
        
        // Define start node
        Node startNode = new Node(Position)
        {
            G = 0,
            H = Position.GetGeometricDistance(target)
        };
        
        gCosts.Add(Position, 0);
        possibleNodes.Enqueue(startNode, startNode.F);
        
        // Loop to find path
        while (possibleNodes.Count > 0)
        {
            Node current = possibleNodes.Dequeue();
            
            // Already evaluated
            if (evaluated.Contains(current.Position)) continue;
            
            // Path found
            if (current.Position == target)
            {
                return RetracePath(current);
            }
            
            evaluated.Add(current.Position);
            
            // Get all neighbours
            if (Map.GetAdjacentCount(current.Position, Map.Grid, out List<GameObject> adjacent) == 0) continue;

            // Add each neighbour to queue
            foreach (var space in adjacent)
            {
                // Check if already evaluated or isn't valid
                if (evaluated.Contains(space.Position)) continue;
                if (space is Wall or Hallway) continue;
                
                // new G
                int newG = current.G + 1;
                
                // Check if previously visited and faster
                if (gCosts.TryGetValue(space.Position, out int value) && newG >= value) continue;
                
                // This way is faster
                gCosts[space.Position] = newG;
                
                // Generate node at space with costs and enqueue
                Node newNode = new Node(space.Position)
                {
                    PreviousNode = current,
                    G = newG,
                    H = space.Position.GetGeometricDistance(target)
                };
                
                possibleNodes.Enqueue(newNode, newNode.F);
            }
        }

        // No path found
        return null;
    }

    /// <summary>
    /// Retrace and return the path.
    /// </summary>
    private Queue<Vector2> RetracePath(Node endNode)
    {
        List<Vector2> path = new List<Vector2>();
        Node? currentNode = endNode;

        while (currentNode != null)
        {
            path.Add(currentNode.Position);
            currentNode = currentNode.PreviousNode;
        }

        path.Reverse();
        return new Queue<Vector2>(path);
    }
}