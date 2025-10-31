using RogueConsoleGame.Characters;
using RogueConsoleGame.Interfaces;
using RogueConsoleGame.Records;
using Enemy = RogueConsoleGame.Characters.Enemy;

namespace RogueConsoleGame.Screens;

public class GameScreen(GameManager gameManager) : IScreen
{
    public readonly GameManager GameManager = gameManager;
    
    public Dictionary<Vector2, Enemy> Enemies = new();
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
        // Player initialization
        GameManager.ColorConsoleWrite(ConsoleColor.Cyan, "Please enter your name: ");
        string? name = Console.ReadLine();
        name = string.IsNullOrEmpty(name) ? "Player" : name;
        Player = new Player(this, name, GameManager.PlayerChar, 10, 1);
        PlayerPosition = Player.Position;
        
        // Enemy initialization
        int enemyCount = GameManager.EnemyCount;
        Random random = new Random();
        
        while (enemyCount > 0)
        {
            int r = random.Next(0, GameManager.Enemies.Length);
            Enemy newEnemy = new Enemy(this, GameManager.Enemies[r].Symbol, GameManager.Enemies[r].MaxHealth, GameManager.Enemies[r].Strength);
            Enemies.Add(newEnemy.Position, newEnemy);
            enemyCount--;
        }
        
        // Define map
        _map = new char[GameManager.GameHeight, GameManager.GameWidth];
        
        for (int i = 0; i < GameManager.GameHeight; i++)
        {
            for (int j = 0; j < GameManager.GameWidth; j++)
            {
                _map[i, j] = GameManager.EmptyChar;
            }
        }
        
        _map[PlayerPosition.Y, PlayerPosition.X] = Player.Symbol;
        foreach (var enemyPair in Enemies)
        {
            _map[enemyPair.Key.Y, enemyPair.Key.X] = enemyPair.Value.Symbol;
        }
    }

    /// <summary>
    /// Output the game as it is right now.
    /// </summary>
    private void OutputGame()
    {
        ConsoleColor borderColor;
        
        if (Player == null) throw new NullReferenceException("Player doesn't exist!");
        
        // Border color based on player health and healing status
        if (Player.Health == Player.MaxHealth) borderColor = ConsoleColor.Blue;
        else if (Player.IsHealing) borderColor = ConsoleColor.Yellow;
        else if (Player.Health < 5) borderColor = ConsoleColor.Red;
        else borderColor = ConsoleColor.Green;
        
        // Output map
        string borderHorizontal = "";
        for (int i = 0; i < GameManager.GameWidth; i++)
        {
            borderHorizontal += "═";
        }
        
        // Top border
        GameManager.ColorConsoleWrite(borderColor, "╔");
        GameManager.ColorConsoleWrite(borderColor, borderHorizontal);
        GameManager.ColorConsoleWriteLine(borderColor, "╗");
        
        // Middle
        for (int i = 0; i < GameManager.GameHeight; i++)
        {
            GameManager.ColorConsoleWrite(borderColor, "║");
            
            for (int j = 0; j < GameManager.GameWidth; j++)
            {
                Console.Write(_map[i, j]);
            }
            
            GameManager.ColorConsoleWriteLine(borderColor, "║");
        }
        
        // Bottom border
        GameManager.ColorConsoleWrite(borderColor, "╚");
        GameManager.ColorConsoleWrite(borderColor, borderHorizontal);
        GameManager.ColorConsoleWriteLine(borderColor, "╝");
        
        // Output details
        Console.WriteLine();
        Console.WriteLine($"{Player.Name}:");
        Console.WriteLine($"Health        - {Player.Health}/{Player.MaxHealth}");
        Console.WriteLine($"Attack Damage - {Player.Strength}");
    }
}