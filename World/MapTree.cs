using RogueConsoleGame.World.GameObjects;
using RogueConsoleGame.World.GameObjects.Characters;

namespace RogueConsoleGame.World;

/// <summary>
/// Create a tree of maps. Used for map traversal in the game world.
/// </summary>
public class MapTree
{
    public Map Root { get; }
    public Map ActiveMap { get; private set; }
    public int Difficulty { get; }
    
    public Player Player { get; }
    
    public readonly GameManager GameManager;
    
    private readonly int _wallSegmentFactor = 50;
    
    // Need to add Update()

    /// <summary>
    /// Construct a new map tree.
    /// </summary>
    /// <param name="gameManager">The associated game manager.</param>
    /// <param name="difficulty">The difficulty level of this map tree (1 to 10). 1 is easiest, 10 is hardest.</param>
    public MapTree(GameManager gameManager, int difficulty)
    {
        GameManager = gameManager;
        
        Root = new Map(this, GameManager.MinGameHeight, GameManager.MinGameWidth, 0, 0);
        ActiveMap = Root;
        
        // Initialize Player
        GameManager.ColorConsoleWrite(ConsoleColor.Cyan, "Please enter your name: ");
        DataInitializer data = GameManager.DataInitializer;
        string? name = Console.ReadLine();
        name = string.IsNullOrEmpty(name) ? "Player" : name;
        Player = new Player(ActiveMap, name, data.Player.Symbol, data.Player.MaxHealth, data.Player.Strength);
        ActiveMap.SpawnPlayerHere(Player);

        Difficulty = difficulty switch
        {
            < 1 => 1,
            > 10 => 10,
            _ => difficulty
        };

        int fails = 0;
        int mapCount = 5 + (Difficulty * 2);
        const int maxRandomDepth = 4;
        const int maxActualDepth = 7;
        const int maxChildren = 4;
        const int maxFails = 20;

        // Generate all maps
        while (mapCount > 0 && fails < maxFails)
        {
            var parent = Root;
            var children = parent.Children;
            var depth = 0;
            
            // Select a node which will get a new child map
            while (depth < maxRandomDepth)
            {
                // Closer to root = more children
                if (children.Count == 0 ||
                    (children.Count < maxChildren - depth &&
                     GameManager.Seed.Next(0, maxRandomDepth * (maxChildren - depth)) > depth * children.Count))
                {
                    break;
                }
                
                // Go deeper
                depth++;
                parent = children[GameManager.Seed.Next(0, children.Count)].DestinationMap;
                children = parent.Children;
            }

            // At maxDepth and deeper, only 1 child allowed
            if (depth == maxRandomDepth)
            {
                while (children.Count > 0 && depth < maxActualDepth)
                {
                    depth++;
                    parent = children[GameManager.Seed.Next(0, children.Count)].DestinationMap;
                    children = parent.Children;
                }

                if (depth == maxActualDepth && children.Count > 0)
                {
                    fails++;
                    continue;
                }
            }
            
            // Create the new map
            Map newMap = new Map(this, GameManager.MaxGameHeight, GameManager.MaxGameWidth, depth + 1, (depth + 1) * _wallSegmentFactor);
            
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
                fails++;
            }
        }
    }

    /// <summary>
    /// Switch to a different map.
    /// </summary>
    /// <param name="hallway"></param>
    /// <returns>Whether the switch was successful.</returns>
    public bool SwitchActiveMap(Hallway hallway)
    {
        if (hallway.DestinationHallway == null || hallway.DestinationHallway.Map != hallway.DestinationMap) throw new Exception("Impossible Error!");
        if (hallway.Map != ActiveMap || hallway.Locked) return false;

        // Update map
        ActiveMap.Player = null;
        ActiveMap = hallway.DestinationMap;
        return ActiveMap.MovePlayerToThisMap(Player, hallway.DestinationHallway);
    }
}