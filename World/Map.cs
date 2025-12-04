using RogueConsoleGame.DataTypes;
using RogueConsoleGame.World.GameObjects;

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

    public Hallway? Parent { get; private set; }
    public List<Hallway> Children { get; } = [];
    
    private readonly GameManager _gameManager;
    
    /// <summary>
    /// Used to create maps for the game world.
    /// </summary>
    /// <param name="gameManager">Associated game manager.</param>
    /// <param name="height">Height of the map.</param>
    /// <param name="width">Width of the map.</param>
    /// <param name="depth">Depth of the map in the tree it belongs to.</param>
    /// <param name="wallSegments">Number of wall segments to create for this map. Values less than 0 will be rounded to 0. Default: 0</param>
    public Map(GameManager gameManager, int height, int width, int depth, int? wallSegments)
    {
        _gameManager = gameManager;
        MapHeight = height + 2;
        MapWidth = width + 2;
        Depth = depth;
        
        Grid = new GameObject[MapHeight, MapWidth];
        
        int count = wallSegments ?? 0;
        GenerateMap(count);
    }

    /// <summary>
    /// Output this map to the console.
    /// </summary>
    public void OutputMap()
    {
        Console.WriteLine(); // margin
        for (int y = 0; y < MapHeight; y++)
        {
            Console.Write(" "); // margin
            for (int x = 0; x < MapWidth; x++)
            {
                if (Grid[y, x].IsVisible) Console.Write(Grid[y, x].Symbol);
                else Console.Write(" ");
            }
            Console.WriteLine();
        }
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
            initialPosition = new Vector2(MapWidth - 1, _gameManager.Seed.Next(1, MapHeight - 1));
        }
        else
        {
            if (Parent != null) return null;
            shift = new Vector2(1, 0);
            initialPosition = new Vector2(0, _gameManager.Seed.Next(1, MapHeight - 1));
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
        GetAdjacentCount(position, Grid, out List<Wall> walls);
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
    /// Finds all connected types of from the starting space of the specified type.
    /// </summary>
    /// <param name="start">The starting space.</param>
    /// <typeparam name="T">The type to search for.</typeparam>
    /// <returns>A list of all connected spaces of the same type.</returns>
    /// <exception cref="ArgumentException">Start node is not in the grid.</exception>
    public List<T> GetConnected<T>(T start) where T : GameObject
    {
        // Start node is not on the grid
        if (Grid[start.Position.Y, start.Position.X] != start) throw new ArgumentException("Start node is not in the grid!");
        
        // Search definition
        List<T> connected = new List<T>();
        Queue<T> toSearch = new Queue<T>();
        toSearch.Enqueue(start);
        
        // Begin Search
        while (toSearch.Count > 0)
        {
            T next = toSearch.Dequeue();
            
            connected.Add(next);
            GetAdjacentCount<T>(next.Position, Grid, out List<T> spaces);

            foreach (T space in spaces)
            {
                if (!connected.Contains(space) && !toSearch.Contains(space)) toSearch.Enqueue(space);
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
                    Grid[y, x] = new Wall(_gameManager, new Vector2(x, y));
                }
                else
                {
                    Grid[y, x] = new EmptySpace(_gameManager.EmptyChar, new Vector2(x, y));
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
            y = _gameManager.Seed.Next(0, MapHeight);
            x = _gameManager.Seed.Next(0, MapWidth);

            if (Grid[y, x] is Wall && GetAdjacentCount<Wall>(new Vector2(x, y), newGrid, out _) < 4)
            {
                flag = true;
                break;
            }
        }

        // Find any random starting point
        for (int j = 0; j < maxPasses; j++)
        {
            y = _gameManager.Seed.Next(0, MapHeight);
            x = _gameManager.Seed.Next(0, MapWidth);

            if (GetAdjacentCount<Wall>(new Vector2(x, y), newGrid, out _) < 4)
            {
                newGrid[y, x] = new Wall(_gameManager, new Vector2(x, y));
                count++;
                flag = true;
                break;
            }
        }

        // Rare case, no valid starting point found
        if (!flag) return;
            
        // Select a direction
        GetAdjacentCount<EmptySpace>(new Vector2(x, y), newGrid, out List<EmptySpace> emptySpaces);
        
        // Ensure empty space availability
        if (emptySpaces.Count == 0) return;
        
        GameObject nextWall = emptySpaces[_gameManager.Seed.Next(0, emptySpaces.Count)];
        
        if (GetAdjacentCount<Wall>(nextWall.Position, newGrid, out _) > 1) return;
        
        // Assign the next wall
        nextWall = new Wall(_gameManager, nextWall.Position);
        newGrid[nextWall.Position.Y, nextWall.Position.X] = nextWall;
        count++;
        
        // Get the direction
        y = nextWall.Position.Y - y;
        x = nextWall.Position.X - x;
        
        // Complete the segment
        while (count < maxWalls)
        {
            // Higher the segment length, lesser the chance of increasing length
            if (count >= minWalls && _gameManager.Seed.Next(0, maxWalls) < count) break;
                
            Vector2 nextPosition = new Vector2(nextWall.Position.X + x, nextWall.Position.Y + y);

            // Ensure empty space
            if (!(newGrid[nextPosition.Y, nextPosition.X] is EmptySpace)) break;
            
            GetAdjacentCount<Wall>(nextPosition, newGrid, out List<Wall> walls);

            if (walls.Count is 1 or 2)
            {
                nextWall = new Wall(_gameManager, nextPosition);
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
                    if (_gameManager.Seed.Next(0, 2) == 0) Grid[y, x + 1] = new Wall(_gameManager, new Vector2(x + 1, y));
                    else Grid[y + 1, x] = new Wall(_gameManager, new Vector2(x, y + 1));
                }
                
                // EW
                // WE
                if (block[0] is EmptySpace && block[1] is Wall && block[2] is Wall && block[3] is EmptySpace)
                {
                    if (_gameManager.Seed.Next(0, 2) == 0) Grid[y, x] = new Wall(_gameManager, new Vector2(x, y));
                    else Grid[y + 1, x + 1] = new Wall(_gameManager, new Vector2(x + 1, y + 1));
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
        List<EmptySpace> remainingEmptySpaces = new List<EmptySpace>();
        List<List<EmptySpace>> regions = new List<List<EmptySpace>>();
        List<EmptySpace> selectedRegion = new List<EmptySpace>();
        bool flag = false;
        
        // Collect all empty spaces
        for (int y = 0; y < MapHeight; y++)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                if (Grid[y, x] is EmptySpace space) remainingEmptySpaces.Add(space);
            }
        }
        
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
                    Grid[y, x] = new Wall(_gameManager, new Vector2(x, y));
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