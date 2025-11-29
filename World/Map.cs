using RogueConsoleGame.Interfaces;
using RogueConsoleGame.World.GameObjects;

namespace RogueConsoleGame.World;

/// <summary>
/// Used to create maps for the game world.
/// </summary>
public class Map
{
    public int MapWidth { get; }
    public int MapHeight { get; }
    
    public int Noise { get; }
    
    public IVisible[,] Grid { get; private set; }
    
    private GameManager _gameManager;
    
    /// <summary>
    /// Used to create maps for the game world.
    /// </summary>
    /// <param name="gameManager">Associated game manager.</param>
    /// <param name="height">Height of the map.</param>
    /// <param name="width">Width of the map.</param>
    /// <param name="noise">Noise percentage (walls) of the map. Must be between 0 and 100 inclusive. Any number below 0 will be rounded to 0, and above 100 will be rounded to 100. (Too high values may result in an unusable map. Recommended range is 40-60) Default: 0</param>
    public Map(GameManager gameManager, int height, int width, int? noise)
    {
        _gameManager = gameManager;
        MapHeight = height + 2;
        MapWidth = width + 2;

        if (noise is { } value)
        {
            if (value < 0) Noise = 0;
            else if (value > 100) Noise = 100;
            else Noise = value;
        }
        else
        {
            Noise = 0;
        }
        
        Grid = new IVisible[MapHeight, MapWidth];

        GenerateMap();
        if (Noise > 0)
        {
            for (int i = 0; i < 5; i++)
            {
                SmoothWalls();
            }
        }
        UpdateWalls();
        
    }

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
    /// Generate the map with noise if it isn't 0. This map only contains empty spaces and walls.
    /// </summary>
    private void GenerateMap()
    {
        for (int i = 0; i < MapHeight; i++)
        {
            for (int j = 0; j < MapWidth; j++)
            {
                if (i == 0 || j == 0 || i == MapHeight - 1 || j == MapWidth - 1)
                {
                    Grid[i, j] = new Wall(_gameManager);
                }
                else
                {
                    // noise walls
                    Grid[i, j] = _gameManager.Seed.Next(100) < Noise ? new Wall(_gameManager) : new EmptySpace(_gameManager.EmptyChar);
                }
            }
        }
    }

    /// <summary>
    /// Smoothen out random walls.
    /// </summary>
    private void SmoothWalls()
    {
        IVisible[,] newGrid = new IVisible[MapHeight, MapWidth];
        
        // Copy border to favor edge walls
        for (int i = 0; i < MapHeight; i++)
        {
            for (int j = 0; j < MapWidth; j++)
            {
                if (i == 0 || j == 0 || i == MapHeight - 1 || j == MapWidth - 1) newGrid[i, j] = Grid[i, j];
            }
        }

        // Make modifications
        for (int i = 1; i < MapHeight - 1; i++)
        {
            for (int j = 1; j < MapWidth - 1; j++)
            {
                int adjCount = GetSurroundingCount(typeof(Wall), i, j);

                if (adjCount > 4) newGrid[i, j] = new Wall(_gameManager); // Replace with wall
                else if (adjCount < 4) newGrid[i, j] = new EmptySpace(_gameManager.EmptyChar); // Replace with empty space
                else  newGrid[i, j] = Grid[i, j]; // Keep what is currently there
            }
        }
        
        Grid = newGrid;
    }
    
    

    /// <summary>
    /// Search for spaces in all directions (including diagonals) containing the specified type around the search location.
    /// </summary>
    /// <param name="type">The type to search for. This type must be of interface IVisible.</param>
    /// <param name="y">The y-coordinate of the search location.</param>
    /// <param name="x">The x-coordinate of the search location.</param>
    /// <returns>The count of the spaces found containing the type. -1 if type is not of interface IVisible.</returns>
    private int GetSurroundingCount(Type type, int y, int x)
    {
        if (!typeof(IVisible).IsAssignableFrom(type)) return -1;
        
        int count = 0;

        for (int adjY = y - 1; adjY <= y + 1; adjY++)
        {
            for (int adjX = x - 1; adjX <= x + 1; adjX++)
            {
                if (
                    (adjY != y || adjX != x) && // Not the search node
                    adjY > 0 && adjY < MapHeight && adjX > 0 && adjX < MapWidth && // Is within map bounds
                    type.IsInstanceOfType(Grid[adjY, adjX])) // Is the required type
                {
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
                    wall.CheckSymbol(this, i, j);
                }
            }
        }
    }
}