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
    
    public IVisible[,] Grid { get; private set; }
    
    private GameManager _gameManager;
    
    public Map(GameManager gameManager, int width, int height)
    {
        _gameManager = gameManager;
        MapWidth = width;
        MapHeight = height;
        
        Grid = new IVisible[MapHeight, MapWidth];

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
                    Grid[i, j] = new EmptySpace(_gameManager.EmptyChar);
                }
            }
        }
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
}