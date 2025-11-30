using RogueConsoleGame.Interfaces;
using RogueConsoleGame.Records;
using RogueConsoleGame.World.GameObjects;

namespace RogueConsoleGame.World;

/// <summary>
/// Used to create maps for the game world.
/// </summary>
public class Map
{
    public int MapWidth { get; }
    public int MapHeight { get; }

    public IVisible[,] Grid { get; private set; }
    
    private readonly GameManager _gameManager;
    
    /// <summary>
    /// Used to create maps for the game world.
    /// </summary>
    /// <param name="gameManager">Associated game manager.</param>
    /// <param name="height">Height of the map.</param>
    /// <param name="width">Width of the map.</param>
    /// <param name="wallSegments">Number of wall segments to create for this map. Values less than 0 will be rounded to 0. Default: 0</param>
    public Map(GameManager gameManager, int height, int width, int? wallSegments)
    {
        _gameManager = gameManager;
        MapHeight = height + 2;
        MapWidth = width + 2;
        
        Grid = new IVisible[MapHeight, MapWidth];
        
        int count = wallSegments ?? 0;
        GenerateMap(count);
        UpdateWalls();
        
    }

    /// <summary>
    /// Output this map to the console.
    /// </summary>
    public void OutputMap()
    {
        for (int i = 0; i < MapHeight; i++)
        {
            for (int j = 0; j < MapWidth; j++)
            {
                Console.Write(Grid[i, j].Symbol);
            }
            Console.WriteLine();
        }
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
        
        // Do wall segments
        throw new NotImplementedException();
    }

    /// <summary>
    /// Search for spaces in the 4 cardinal directions containing the specified type around the search location.
    /// </summary>
    /// <param name="position">The position of the search location.</param>
    /// <param name="results">Returns the classes associated with the spaces containing the required type. (This list does not include out of bounds spaces)</param>
    /// <param name="grid">The grid to check in.</param>
    /// <typeparam name="T">The type to search for. Must inherit IVisible.</typeparam>
    /// <returns>The count of the spaces found containing the type or outside map bounds.</returns>
    private int GetAdjacentCount<T>(Vector2 position, IVisible[,] grid, out List<T> results) where T : IVisible
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
    /// <typeparam name="T">The type to search for. Must inherit IVisible.</typeparam>
    /// <returns>The count of the spaces found containing the type or outside map bounds.</returns>
    private int GetSurroundingCount<T>(Vector2 position, IVisible[,] grid, out List<T> results) where T : IVisible
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
    /// Checks all walls on the map and updates their symbols according to wall adjacency.
    /// </summary>
    private void UpdateWalls()
    {
        for (int i = 0; i < MapHeight; i++)
        {
            for (int j = 0; j < MapWidth; j++)
            {
                if (Grid[i, j] is Wall wall)
                {
                    wall.CheckSymbol(this);
                }
            }
        }
    }
}