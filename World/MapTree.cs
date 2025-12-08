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

    public const int MinDifficulty = 1;
    public const int MaxDifficulty = 10;
    public const int WallSegmentFactor = 50;

    // Need to add Update()

    /// <summary>
    /// Construct a new map tree.
    /// </summary>
    /// <param name="gameManager">The associated game manager.</param>
    /// <param name="difficulty">The difficulty level of this map tree (this.MinDifficulty to this.MaxDifficulty). Min is easiest, Max is hardest.</param>
    public MapTree(GameManager gameManager, int difficulty)
    {
        Console.CursorVisible = true;
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
            < MinDifficulty => MinDifficulty,
            > MaxDifficulty => MaxDifficulty,
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
            Map newMap = new Map(this, GameManager.MaxGameHeight, GameManager.MaxGameWidth, depth + 1, (depth + 1) * WallSegmentFactor);
            
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
        
        // Generate locks
        List<Map> validMaps = new List<Map>();
        GenerateLocks(Root, validMaps);
        
        // Clear the console of any input
        Console.Clear();
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
        if (ActiveMap == Root) Console.Clear(); // clear console because root map is small
        return ActiveMap.MovePlayerToThisMap(Player, hallway.DestinationHallway);
    }

    /// <summary>
    /// Traverse the tree to find and unlock all hallways that can be unlocked with this key.
    /// </summary>
    /// <param name="keyID">The ID of the key.</param>
    public void UnlockHallways(string keyID)
    {
        UnlockHallways(Root, keyID);
    }

    /// <summary>
    /// Use tree traversal to recursively find and unlock all hallways that can be unlocked with this key.
    /// </summary>
    private void UnlockHallways(Map map, string keyID)
    {
        foreach (var child in map.Children)
        {
            if (child.Locked) child.UnlockLock(keyID);
            UnlockHallways(child.DestinationMap, keyID);
        }
    }

    /// <summary>
    /// Use tree traversal to generate locks on hallways recursively.
    /// </summary>
    private void GenerateLocks(Map currentMap, List<Map> validMaps)
    {
        foreach (var child in currentMap.Children)
        {
            // Decide whether to lock the hallway with a key or not
            if (validMaps.Count > 0 && GameManager.Seed.Next(0, MaxDifficulty * 2) < (MaxDifficulty - 1) + Difficulty)
            {
                // Make a new ID
                string id = child.GetHashCode().ToString();
                
                // Iterate locking until successful or out of available maps
                List<Map> availableMaps = new List<Map>(validMaps);
                Map selectedMap;
                do
                {
                    // Select a map from validMaps to place the key in
                    selectedMap = availableMaps[GameManager.Seed.Next(0, availableMaps.Count)];
                    availableMaps.Remove(selectedMap);
                    
                } while (!selectedMap.AddKey(id, true, child) && availableMaps.Count > 0);
            }
            
            // Continue the recursive call
            validMaps.Add(child.DestinationMap);
            GenerateLocks(child.DestinationMap, validMaps);
        }
    }
}