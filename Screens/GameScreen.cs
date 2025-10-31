using RogueConsoleGame.Characters;
using RogueConsoleGame.Interfaces;
using RogueConsoleGame.Records;

namespace RogueConsoleGame.Screens;

public class GameScreen(GameManager gameManager) : IScreen
{
    public readonly GameManager GameManager = gameManager;
    
    public Dictionary<Vector2, Character> Enemies = new();
    public Player? Player { get; private set; }
    public Vector2? PlayerPosition { get; private set; }
    
    private bool _initialized = false;
    private char[,] _map = new char[0, 0];
    
    // Execute the screen
    public void Draw()
    {
        if (!_initialized) Initialize();
    }

    /// <summary>
    /// Initialize the game.
    /// </summary>
    private void Initialize()
    {
        _map = new char[GameManager.GameHeight, GameManager.GameWidth];
        
        // Player Initialization
        GameManager.ColorConsoleWrite(ConsoleColor.Cyan, "Please enter your name: ");
        string? name = Console.ReadLine();
        name = string.IsNullOrEmpty(name) ? "Player" : name;
        Player = new Player(this, name, GameManager.PlayerChar, 10, 1);
        PlayerPosition = Player.Position;
    }
}