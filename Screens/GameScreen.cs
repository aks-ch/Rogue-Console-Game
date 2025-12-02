using RogueConsoleGame.DataTypes;
using RogueConsoleGame.Enums;
using RogueConsoleGame.Interfaces;
using RogueConsoleGame.World.GameObjects.Characters;

namespace RogueConsoleGame.Screens;

public class GameScreen(GameManager gameManager) : IScreen
{
    public readonly GameManager GameManager = gameManager;
    
    public Dictionary<Vector2, Enemy> Enemies = new();
    public Player? Player { get; private set; }
    
    private bool _initialized;
    private char[,] _map = new char[0, 0];
    
    // Execute the screen
    public void Draw()
    {
        
        if (!_initialized)
        {
            Initialize();
            return;
        }
        
        if (Player == null) throw new NullReferenceException("Player doesn't exist!");
        
        OutputGame();
        
        // Processing if input valid
        if (!Player.CheckKey()) return;
        
        // Player position
        _map[Player.Position.Y, Player.Position.X] = GameManager.EmptyChar;
        Player.Position = Player.Position;
        _map[Player.Position.Y, Player.Position.X] = Player.Symbol;
            
        // Enemies
        List<Enemy> localEnemies = new();
        
        foreach (var enemyPair in Enemies)
        {
            localEnemies.Add(enemyPair.Value);
        }
        
        foreach (var enemy in localEnemies)
        {
            _map[enemy.Position.Y, enemy.Position.X] = GameManager.EmptyChar;
            // Remove if dead
            if (enemy.Health <= 0)
            {
                _map[enemy.Position.Y, enemy.Position.X] = GameManager.EmptyChar;
                Enemies.Remove(enemy.Position);
                continue;
            }
            
            Vector2 position = enemy.Position;
            if (enemy.ChooseMove(Player.Position))
            {
                Enemies.Remove(position);
                Enemies.Add(enemy.Position, enemy);
            }
            _map[enemy.Position.Y, enemy.Position.X] = enemy.Symbol;
        }
        
        // Checking Victory/Defeat
        if (Player.Health <= 0)
        {
            Clear();
            GameManager.CurrentGameState = GameState.Defeat;
            return;
        }
        if (Enemies.Count == 0)
        {
            Clear();
            GameManager.CurrentGameState = GameState.Victory;
            return;
        }
        
        Player.Heal();
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
        Player = new Player(this, name, GameManager.Player.Symbol, GameManager.Player.MaxHealth, Math.Round(GameManager.Player.Strength, 1));
        Player.Position = Player.Position;
        
        // Enemy initialization
        int enemyCount = GameManager.EnemyCount;
        
        while (enemyCount > 0)
        {
            int r = GameManager.Seed.Next(0, GameManager.Enemies.Length);
            Enemy newEnemy = new Enemy(this, GameManager.Enemies[r].Symbol, GameManager.Enemies[r].MaxHealth, Math.Round(GameManager.Enemies[r].Strength, 1));
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
        
        _map[Player.Position.Y, Player.Position.X] = Player.Symbol;
        foreach (var enemyPair in Enemies)
        {
            _map[enemyPair.Key.Y, enemyPair.Key.X] = enemyPair.Value.Symbol;
        }
        
        _initialized = true;
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
        else if (Player.Health < 0.5 * Player.MaxHealth) borderColor = ConsoleColor.Red;
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

    /// <summary>
    /// Clear everything.
    /// </summary>
    private void Clear()
    {
        Enemies.Clear();
        _map = new char[0, 0];
        Player = null;
        _initialized = false;
    }
}