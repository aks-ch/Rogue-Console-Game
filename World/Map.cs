using RogueConsoleGame.DataTypes;
using RogueConsoleGame.World.GameObjects;
using RogueConsoleGame.World.GameObjects.Characters;
using RogueConsoleGame.World.GameObjects.Items;

namespace RogueConsoleGame.World;

/// <summary>
/// Used to create maps for the game world.
/// </summary>
public class Map
{
    public int Depth { get; }
    public int MapWidth { get; }
    public int MapHeight { get; }

    public GameObject[,] Grid { get; private set; }
    private (char Symbol, ConsoleColor Color)[,] PreviousOutput { get; set; }
    private bool Outputted { get; set; }

    public Hallway? Parent { get; private set; }
    public List<Hallway> Children { get; } = [];
    
    public Player? Player { get; set; }
    public Vector2 PlayerPosition { get; set; } = new Vector2(0, 0);
    
    public List<Enemy> Enemies { get; } = [];
    
    public readonly MapTree MapTree;
    public readonly GameManager GameManager;
    
    /// <summary>
    /// Used to create maps for the game world.
    /// </summary>
    /// <param name="mapTree">Associated map tree.</param>
    /// <param name="height">Height of the map.</param>
    /// <param name="width">Width of the map.</param>
    /// <param name="depth">Depth of the map in the tree it belongs to.</param>
    /// <param name="wallSegments">Number of wall segments to create for this map. Values less than 0 will be rounded to 0. Default: 0</param>
    public Map(MapTree mapTree, int height, int width, int depth, int? wallSegments)
    {
        MapTree = mapTree;
        GameManager = mapTree.GameManager;
        MapHeight = height + 2;
        MapWidth = width + 2;
        Depth = depth;
        
        Grid = new GameObject[MapHeight, MapWidth];
        
        int count = wallSegments ?? 0;
        GenerateMap(count);
        
        // Generate Previous Output (but leave empty)
        PreviousOutput = new (char Symbol, ConsoleColor Color)[MapHeight, MapWidth];
        
        // Need to spawn enemies
    }

    /// <summary>
    /// Update this map based on new parameters.
    /// </summary>
    public void Update()
    {
        if (MapTree.ActiveMap != this) return; // Map isn't active
        if (Player == null) throw new NullReferenceException("Player is null");
        
        // Player movement
        Player.CheckKey();
        switch (Grid[Player.Position.Y, Player.Position.X])
        {
            case EmptySpace empty:
                // Move Empty
                Grid[PlayerPosition.Y, PlayerPosition.X] = empty;
                empty.Position = PlayerPosition;
            
                // Move Player
                PlayerPosition = Player.Position;
                Grid[PlayerPosition.Y, PlayerPosition.X] = Player;
                break;
            case Key key:
                // Attempt unlock
                MapTree.UnlockHallways(key.KeyID);
                
                // Add empty space behind player
                Grid[PlayerPosition.Y, PlayerPosition.X] = new EmptySpace(this, PlayerPosition);
                
                // Move Player (removing the key)
                PlayerPosition = Player.Position;
                Grid[PlayerPosition.Y, PlayerPosition.X] = Player;
                break;
            case Enemy enemy:
                Player.Attack(enemy);
                
                // Check for dead enemy
                if (enemy.Health <= 0)
                {
                    Enemies.Remove(enemy);
                    Grid[enemy.Position.Y, enemy.Position.X] = new EmptySpace(this, enemy.Position);
                }
                
                Player.Position = PlayerPosition;
                break;
            case Hallway { Locked: false } hallway:
                if (MapTree.SwitchActiveMap(hallway))
                {
                    Outputted = false;
                    return;
                }
                Player.Position = PlayerPosition;
                break;
            default:
                Player.Position = PlayerPosition;
                break;
        }
        
        // Enemy movement
        foreach (var enemy in Enemies)
        {
            var position = enemy.Position;
            enemy.ChooseMove(PlayerPosition);
            switch (Grid[enemy.Position.Y, enemy.Position.X])
            {
                case EmptySpace empty:
                    // Move Empty
                    Grid[position.Y, position.X] = empty;
                    empty.Position = position;
            
                    // Move enemy
                    Grid[enemy.Position.Y, enemy.Position.X] = enemy;
                    break;
                case Player player:
                    enemy.Attack(player);
                    enemy.Position = position;
                    break;
                default:
                    enemy.Position = position;
                    break;
            }
        }
        
        // Check Player healing
        Player.Heal();
        
        // Unlock hallways if all enemies gone
        if (Enemies.Count == 0)
        {
            foreach (var hallway in Children.Where(hallway => hallway.Locked))
            {
                hallway.UnlockLock("enemy");
            }
        }
    }

    /// <summary>
    /// Output this map to the console.
    /// </summary>
    public void OutputMap()
    {
        if (!Outputted) Console.WriteLine(); // margin
        for (int y = 0; y < MapHeight; y++)
        {
            if (!Outputted) Console.Write(' '); // margin
            for (int x = 0; x < MapWidth; x++)
            {
                (char Symbol, ConsoleColor Color) output = (Grid[y, x].Symbol, Grid[y, x].Color);
                
                // If map was already outputted and previous output is as current
                if (Outputted && PreviousOutput[y, x] == output) continue;
                
                if (Grid[y, x].IsVisible)
                {
                    Console.SetCursorPosition(x + 1, y + 1);
                    if (output.Color != Console.ForegroundColor) Console.ForegroundColor = output.Color; // change color if needed
                    Console.Write(output.Symbol);
                }
                else Console.Write(' ');
                    
                PreviousOutput[y, x] = output;
            }
            Console.WriteLine();
        }
        
        // Reset console
        Console.SetCursorPosition(0, MapHeight + 2);
        Console.ResetColor();
        
        Outputted = true;
    }

    /// <summary>
    /// Spawn the player on this map. Only to use for root spawn.
    /// </summary>
    /// <param name="player">The player.</param>
    public void SpawnPlayerHere(Player player)
    {
        if (MapTree.Root != this) return;
        Player = player;
        Player.Position = new Vector2(1, MapHeight / 2);
        PlayerPosition = Player.Position;
        Grid[Player.Position.Y, Player.Position.X] = Player;
    }

    /// <summary>
    /// Move the player to this map.
    /// </summary>
    /// <param name="player">The player emerging.</param>
    /// <param name="hallway">The hallway the player emerges through.</param>
    /// <returns>True if successful. Else false.</returns>
    public bool MovePlayerToThisMap(Player player, Hallway hallway)
    {
        if (hallway.Map != this) return false;
        
        PlayerPosition = hallway.Spawn;
        Player = player;
        Player.Map = this;
        Player.Position = PlayerPosition;
        Grid[PlayerPosition.Y, PlayerPosition.X] = Player;
        return true;
    }

    /// <summary>
    /// Hide a key in this map.
    /// </summary>
    /// <param name="keyID">The ID of the key.</param>
    /// <param name="hidden">Whether the key should be hidden at first.</param>
    /// <param name="hallway">The hallway this key unlocks.</param>
    /// <returns>True if key was placed successfully. Else false.</returns>
    public bool AddKey(string keyID, bool hidden, Hallway hallway)
    {
        // Going back to a parent map should not require a key
        if (!hallway.IsParent) return false;
        
        // Check if lock can even be applied to hallway
        if (!hallway.AddLock(keyID)) return false;

        List<EmptySpace> allEmptySpaces = GetAll<EmptySpace>();
        List<EmptySpace> validEmptySpaces = new List<EmptySpace>();
        List<Hallway> allHallways = new List<Hallway>(Children);

        for (int i = 15; i > 5; i--)
        {
            validEmptySpaces.AddRange(allEmptySpaces);
            List<EmptySpace> banned = new List<EmptySpace>();

            // Get empty spaces near hallways
            foreach (var aHallway in allHallways)
            {
                if (Grid[aHallway.Spawn.Y, aHallway.Spawn.X] is EmptySpace space) banned.AddRange(GetConnected(space, i));
            }
            
            validEmptySpaces.RemoveAll(space => banned.Contains(space));
            
            // Select a valid empty space if any
            if (validEmptySpaces.Count > 0)
            {
                EmptySpace selected = validEmptySpaces[GameManager.Seed.Next(0, validEmptySpaces.Count)];
                Grid[selected.Position.Y, selected.Position.X] = new Key(this, selected.Position, hidden, keyID);
                return true;
            }
        }
        
        // Remove lock if failed and return
        hallway.UnlockLock(keyID);
        return false;
    }

    /// <summary>
    /// Creates a new hallway that will serve as a path to get to another map once connected to a hallway on that map.
    /// </summary>
    /// <param name="map">The map that the hallway leads to.</param>
    /// <param name="isParent">Whether the map this hallway is on is parent to the destination map.</param>
    /// <returns>The created hallway. Null if hallway creation failed.</returns>
    public Hallway? CreateHallway(Map map, bool isParent)
    {
        Vector2 shift;
        Vector2 initialPosition;

        // Determine whether hallway must be on left border (leads to parent) or right border (leads to child)
        if (isParent)
        {
            shift = new Vector2(-1, 0);
            initialPosition = new Vector2(MapWidth - 1, GameManager.Seed.Next(1, MapHeight - 1));
        }
        else
        {
            if (Parent != null) return null;
            shift = new Vector2(1, 0);
            initialPosition = new Vector2(0, GameManager.Seed.Next(1, MapHeight - 1));
        }

        var position = new Vector2(initialPosition.X, initialPosition.Y);

        // Find a valid space for hallway
        while (true)
        {
            Vector2 mustBeEmpty = position + shift;
            if (Grid[mustBeEmpty.Y, mustBeEmpty.X] is EmptySpace &&
                Grid[position.Y, position.X] is Wall &&
                Grid[position.Y - 1, position.X] is Wall &&
                Grid[position.Y + 1, position.X] is Wall) break;
            
            // Shift
            position = mustBeEmpty;

            // Move down a row if encountered an empty space
            if (Grid[mustBeEmpty.Y, mustBeEmpty.X] is EmptySpace ||
                Grid[mustBeEmpty.Y - 1, mustBeEmpty.X] is EmptySpace ||
                Grid[mustBeEmpty.Y + 1, mustBeEmpty.X] is EmptySpace)
            {
                if (isParent) position = new Vector2(MapWidth - 1, position.Y + 1);
                else position = new Vector2(0, position.Y + 1);
            }
            // Move down a row if reached end
            else if (position.X + shift.X == 0) position = new Vector2(MapWidth - 1, position.Y + 1);
            else if (position.X + shift.X == MapWidth - 1) position = new Vector2(0, position.Y + 1);
            
            // Move to top of map
            if (position.Y == MapHeight - 1) position = position with { Y = 1 };

            if (position == initialPosition) return null;
        }

        // Create hallway and return it
        Hallway hallway = new Hallway(this, position, map, isParent);
        Grid[position.Y, position.X] = hallway;
        
        if (isParent) Children.Add(hallway);
        else Parent = hallway;
        
        // Update nearby walls
        GetAdjacentCount<Wall>(position, Grid, out List<Wall> walls);
        foreach (var wall in walls)
        {
            wall.CheckSymbol(this);
        }
            
        return hallway;
    }

    /// <summary>
    /// Connect a hallway on this map with a hallway on another map.
    /// </summary>
    /// <param name="thisHallway">The hallway on this map.</param>
    /// <param name="destinationHallway">The hallway on the other map.</param>
    /// <returns>Whether the connection was successful. (Failed connection means the hallways are improperly configured or not meant to be connected)</returns>
    public bool ConnectHallway(Hallway thisHallway, Hallway destinationHallway)
    {
        // Validate
        if (thisHallway.Map != this ||
            thisHallway.DestinationMap != destinationHallway.Map ||
            thisHallway.Map != destinationHallway.DestinationMap ||
            thisHallway.IsParent == destinationHallway.IsParent)
        {
            return false;
        }
        
        // Connect
        thisHallway.DestinationHallway = destinationHallway;
        destinationHallway.DestinationHallway = thisHallway;
        return true;
    }

    /// <summary>
    /// Get all spaces of this type on the map.
    /// </summary>
    /// <typeparam name="T">The type to collect. Must inherit GameObject.</typeparam>
    /// <returns>A list of all of this type in this map.</returns>
    public List<T> GetAll<T>() where T : GameObject
    {
        List<T> result = new List<T>();
        
        for (int y = 0; y < MapHeight; y++)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                if (Grid[y, x] is T space) result.Add(space);
            }
        }

        return result;
    }

    /// <summary>
    /// Finds connected types of from the starting space of the specified type as far as the defined maxDistance (if any).
    /// </summary>
    /// <param name="start">The starting space.</param>
    /// <param name="maxDistance">The maximum distance to search for. 0 or less means no limit. Default: No limit</param>
    /// <typeparam name="T">The type to search for. Must inherit GameObject.</typeparam>
    /// <returns>A list of connected spaces of the same type.</returns>
    /// <exception cref="ArgumentException">Start node is not in the grid.</exception>
    public List<T> GetConnected<T>(T start, int maxDistance = 0) where T : GameObject
    {
        // Start node is not on the grid
        if (Grid[start.Position.Y, start.Position.X] != start) throw new ArgumentException("Start node is not in the grid!");
        
        // Search definition
        List<T> connected = new List<T>();
        Queue<(int Depth, T Node)> toSearch = new Queue<(int Depth, T Node)>();
        toSearch.Enqueue((0, start));
        
        // Begin Search
        while (toSearch.Count > 0)
        {
            var next = toSearch.Dequeue();
            
            connected.Add(next.Node);
            GetAdjacentCount<T>(next.Node.Position, Grid, out List<T> spaces);

            foreach (T space in spaces)
            {
                // Check for depth and if the space has already been considered
                if ((maxDistance <= 0 || next.Depth < maxDistance) &&
                    !connected.Contains(space) &&
                    toSearch.All(item => item.Node != space))
                {
                    toSearch.Enqueue((next.Depth + 1, space));
                }
            }
        }
        
        // Return all the spaces in the region
        return connected;
    }

    /// <summary>
    /// Search for spaces in the 4 cardinal directions containing the specified type around the search location.
    /// </summary>
    /// <param name="position">The position of the search location.</param>
    /// <param name="results">Returns the classes associated with the spaces containing the required type. (This list does not include out of bounds spaces)</param>
    /// <param name="grid">The grid to check in.</param>
    /// <typeparam name="T">The type to search for. Must inherit GameObject.</typeparam>
    /// <returns>The count of the spaces found containing the type or outside map bounds.</returns>
    public int GetAdjacentCount<T>(Vector2 position, GameObject[,] grid, out List<T> results) where T : GameObject
    {
        int count = 0;
        results = [];

        int[][] directions =
        [
            [-1, 0], // north
            [1, 0], // south
            [0, 1], // east
            [0, -1] // west
        ];

        foreach (int[] direction in directions)
        {
            int adjY = position.Y + direction[0];
            int adjX = position.X + direction[1];

            if (adjY == position.Y && adjX == position.X) continue; // Is not the required type
            
            if (adjY < 0 || adjY >= MapHeight || adjX < 0 || adjX >= MapWidth) // Is outside map bounds
            {
                count++;
            }
            else if (grid[adjY, adjX] is T match) // Is the required type
            {
                results.Add(match);
                count++;
            }
        }
        
        return count;
    }

    /// <summary>
    /// Search for spaces in all directions (including diagonals) containing the specified type around the search location.
    /// </summary>
    /// <param name="position">The position of the search location.</param>
    /// <param name="results">Returns the classes associated with the spaces containing the required type. (This list does not include out of bounds spaces)</param>
    /// <param name="grid">The grid to check in.</param>
    /// <typeparam name="T">The type to search for. Must inherit GameObject.</typeparam>
    /// <returns>The count of the spaces found containing the type or outside map bounds.</returns>
    public int GetSurroundingCount<T>(Vector2 position, GameObject[,] grid, out List<T> results) where T : GameObject
    {
        int count = 0;
        results = new List<T>();

        for (int adjY = position.Y - 1; adjY <= position.Y + 1; adjY++)
        {
            for (int adjX = position.X - 1; adjX <= position.X + 1; adjX++)
            {
                if (adjY == position.Y && adjX == position.X) continue; // Is not the required type
                
                if (adjY < 0 || adjY >= MapHeight || adjX < 0 || adjX >= MapWidth) // Is outside map bounds
                {
                    count++;
                }
                else if (grid[adjY, adjX] is T match) // Is the required type
                {
                    results.Add(match);
                    count++;
                }
            }
        }
        
        return count;
    }
    
    /// <summary>
    /// Checks if a position is out of the map or not.
    /// </summary>
    /// <param name="position">The position to check.</param>
    /// <returns>True if outside map. False if inside.</returns>
    public bool PositionOutOfBounds(Vector2 position)
    {
        if (position.X < 0 ||
            position.X >= MapWidth ||
            position.Y < 0 ||
            position.Y >= MapHeight)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Generate an empty map and attempt to generate the defined number of wall segments.
    /// </summary>
    /// <param name="wallSegments">The number of wall segments to attempt to generate.</param>
    private void GenerateMap(int wallSegments = 0)
    {
        // Basic map with no wall segments
        for (int y = 0; y < MapHeight; y++)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                if (y == 0 || x == 0 || y == MapHeight - 1 || x == MapWidth - 1)
                {
                    Grid[y, x] = new Wall(this, new Vector2(x, y));
                }
                else
                {
                    Grid[y, x] = new EmptySpace(this, new Vector2(x, y));
                }
            }
        }
        
        int maxWallsPerSegment = Math.Min(MapHeight, MapWidth) - 3;
        
        // Carving wall segments if needed
        for (int i = 0; i < wallSegments; i++)
        {
            AttemptCreateWallSegment(4, maxWallsPerSegment);
        }
        
        // Clear diagonals
        FillDiagonalWalls();
        
        // Fill small regions
        FillSmallRegions();
        
        // Set walls' visibility & symbols
        UpdateWalls();
    }
    
    /// <summary>
    /// Attempt to create a wall segment in the map.
    /// </summary>
    /// <param name="minWalls">The minimum number of walls required to be placed in this attempt. Any value less than 1 will default to 1.</param>
    /// <param name="maxWalls">The maximum number of walls that can be placed in this attempt. Any value less than 3 may result in 1 or 2 walls in this segment.</param>
    /// <exception cref="InvalidOperationException">Grid is not initialized.</exception>
    private void AttemptCreateWallSegment(int minWalls, int maxWalls)
    {
        int count = 0;
        int y = -1;
        int x = -1;
        int maxPasses = MapHeight * MapWidth;
        bool flag = false;
        GameObject[,] newGrid = Grid.Clone() as GameObject[,] ?? throw new InvalidOperationException("Grid is not initialized!");

        // Find a wall for starting point
        for (int j = 0; j < maxPasses; j++)
        {
            y = GameManager.Seed.Next(0, MapHeight);
            x = GameManager.Seed.Next(0, MapWidth);

            if (Grid[y, x] is Wall && GetAdjacentCount<Wall>(new Vector2(x, y), newGrid, out _) < 4)
            {
                flag = true;
                break;
            }
        }

        if (!flag)
        {
            // Find any random starting point
            for (int j = 0; j < maxPasses; j++)
            {
                y = GameManager.Seed.Next(0, MapHeight);
                x = GameManager.Seed.Next(0, MapWidth);

                if (GetAdjacentCount<Wall>(new Vector2(x, y), newGrid, out _) < 4)
                {
                    newGrid[y, x] = new Wall(this, new Vector2(x, y));
                    count++;
                    flag = true;
                    break;
                }
            }
        }

        // Rare case, no valid starting point found
        if (!flag) return;
            
        // Select a direction
        GetAdjacentCount<EmptySpace>(new Vector2(x, y), newGrid, out List<EmptySpace> emptySpaces);
        
        // Ensure empty space availability
        if (emptySpaces.Count == 0) return;
        
        GameObject nextWall = emptySpaces[GameManager.Seed.Next(0, emptySpaces.Count)];
        
        if (GetAdjacentCount<Wall>(nextWall.Position, newGrid, out _) > 1) return;
        
        // Assign the next wall
        nextWall = new Wall(this, nextWall.Position);
        newGrid[nextWall.Position.Y, nextWall.Position.X] = nextWall;
        count++;
        
        // Get the direction
        y = nextWall.Position.Y - y;
        x = nextWall.Position.X - x;
        
        // Complete the segment
        while (count < maxWalls)
        {
            // Higher the segment length, lesser the chance of increasing length
            if (count >= minWalls && GameManager.Seed.Next(0, maxWalls) < count) break;
                
            Vector2 nextPosition = new Vector2(nextWall.Position.X + x, nextWall.Position.Y + y);

            // Ensure empty space
            if (!(newGrid[nextPosition.Y, nextPosition.X] is EmptySpace)) break;
            
            GetAdjacentCount<Wall>(nextPosition, newGrid, out List<Wall> walls);

            if (walls.Count is 1 or 2)
            {
                nextWall = new Wall(this, nextPosition);
                newGrid[nextWall.Position.Y, nextWall.Position.X] = nextWall;
                count++;
                if (walls.Count == 2) break;
            }
            else break;
        }
            
        // Confirm segment if long enough
        if (count >= minWalls) Grid = newGrid;
    }

    /// <summary>
    /// Fills walls that look awkward due to a forming diagonal.
    /// </summary>
    private void FillDiagonalWalls()
    {
        // Loop 1 short to enable 2x2 block search
        for (int y = 0; y < MapHeight - 1; y++)
        {
            for (int x = 0; x < MapWidth - 1; x++)
            {
                // Get the 2x2 block
                GameObject[] block = [Grid[y, x], Grid[y, x + 1], Grid[y + 1, x], Grid[y + 1, x + 1]];
                
                // Check:
                // WE
                // EW
                if (block[0] is Wall && block[1] is EmptySpace && block[2] is EmptySpace && block[3] is Wall)
                {
                    if (GameManager.Seed.Next(0, 2) == 0) Grid[y, x + 1] = new Wall(this, new Vector2(x + 1, y));
                    else Grid[y + 1, x] = new Wall(this, new Vector2(x, y + 1));
                }
                
                // EW
                // WE
                if (block[0] is EmptySpace && block[1] is Wall && block[2] is Wall && block[3] is EmptySpace)
                {
                    if (GameManager.Seed.Next(0, 2) == 0) Grid[y, x] = new Wall(this, new Vector2(x, y));
                    else Grid[y + 1, x + 1] = new Wall(this, new Vector2(x + 1, y + 1));
                }
            }
        }
    }
    
    /// <summary>
    /// Fills small regions in the map with walls.
    /// </summary>
    /// <exception cref="InvalidOperationException">Encountered a region that is not there. Error should not occur.</exception>
    private void FillSmallRegions()
    {
        List<EmptySpace> remainingEmptySpaces = GetAll<EmptySpace>();
        List<List<EmptySpace>> regions = new List<List<EmptySpace>>();
        List<EmptySpace> selectedRegion = new List<EmptySpace>();
        bool flag = false;
        
        // Collect all regions
        while (remainingEmptySpaces.Count > 0)
        {
            List<EmptySpace> region = GetConnected(remainingEmptySpaces[0]);
            
            // Select the region that is larger than half the map's total space, if there
            if (region.Count > ((MapHeight - 2) * (MapWidth - 2)) / 2) 
            {
                regions.Clear();
                selectedRegion = region;
                flag = true;
                break;
            }
            
            // Add region to list
            regions.Add(region);
            
            // Remove region from search
            foreach (EmptySpace space in region)
            {
                remainingEmptySpaces.Remove(space);
            }
        }

        // Select the largest region as play area
        if (!flag)
        {
            selectedRegion = regions.MaxBy(region => region.Count) ?? throw new InvalidOperationException("There is no region!");
        }
        
        // Fill all other regions
        for (int y = 0; y < MapHeight; y++)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                if (Grid[y, x] is EmptySpace space && !selectedRegion.Contains(space))
                {
                    Grid[y, x] = new Wall(this, new Vector2(x, y));
                }
            }
        }
    }

    /// <summary>
    /// Checks all walls on the map and updates their visibility and symbols according to wall adjacency.
    /// </summary>
    private void UpdateWalls()
    {
        List<Wall> walls = new List<Wall>();
        
        // Get all walls
        for (int y = 0; y < MapHeight; y++)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                if (Grid[y, x] is Wall wall)
                {
                    walls.Add(wall);
                }
            }
        }
        
        // Check visibility
        foreach (var wall in walls)
        {
            wall.CheckVisibility(this);
        }
        
        // Check symbol
        foreach (var wall in walls)
        {
            wall.CheckSymbol(this);
        }
    }
}