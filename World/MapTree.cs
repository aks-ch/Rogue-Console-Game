using RogueConsoleGame.World.GameObjects;

namespace RogueConsoleGame.World;

/// <summary>
/// Create a tree of maps. Used for map traversal in the game world.
/// </summary>
public class MapTree
{
    public Map Root { get; }
    public Map ActiveMap { get; private set; }
    
    private readonly int _wallSegmentFactor = 50;
    private readonly GameManager _gameManager;

    /// <summary>
    /// Construct a new map tree.
    /// </summary>
    /// <param name="gameManager">The associated game manager.</param>
    /// <param name="difficulty">The difficulty level of this map tree (1 to 10). 1 is easiest, 10 is hardest.</param>
    public MapTree(GameManager gameManager, int difficulty)
    {
        _gameManager = gameManager;
        
        Root = new Map(_gameManager, _gameManager.MinGameHeight, _gameManager.MinGameWidth, 1, 0);
        ActiveMap = Root;

        difficulty = difficulty switch
        {
            < 1 => 1,
            > 10 => 10,
            _ => difficulty
        };

        Map parent;
        int depth;
        List<Hallway> children;
        int mapCount = 5 + (difficulty * 2);
        const int maxDepth = 4;
        const int maxChildren = 5;

        while (mapCount > 0)
        {
            parent = Root;
            children = parent.Children;
            depth = 1;
            
            // Select a node which will get a new child map
            while (depth < maxDepth)
            {
                // Closer to root = more children
                if (children.Count == 0 ||
                    (children.Count < maxChildren - depth &&
                     _gameManager.Seed.Next(0, maxDepth * (maxChildren - depth)) > depth * children.Count))
                {
                    break;
                }
                
                // Go deeper
                depth++;
                parent = children[_gameManager.Seed.Next(0, children.Count)].DestinationMap;
                children = parent.Children;
            }

            // At depth 4 and deeper, only 1 child allowed
            if (depth == maxDepth)
            {
                while (children.Count > 0)
                {
                    depth++;
                    parent = children[_gameManager.Seed.Next(0, children.Count)].DestinationMap;
                    children = parent.Children;
                }
            }
            
            // Create the new map
            Map newMap = new Map(_gameManager, _gameManager.MaxGameHeight, _gameManager.MaxGameWidth, depth + 1, depth * _wallSegmentFactor);
            
            // Create and connect hallways

            try
            {
                Hallway childHallway = newMap.CreateHallway(parent, false) ?? throw new InvalidOperationException();
                Hallway parentHallway = parent.CreateHallway(newMap, true) ?? throw new InvalidOperationException();

                if (!newMap.ConnectHallway(childHallway, parentHallway) ||
                    !parent.ConnectHallway(parentHallway, childHallway))
                    throw new Exception("Hallway connection failed!");
                
                // Continue on success
                mapCount--;
            }
            catch (InvalidOperationException)
            {
                
            }
        }
    }
}