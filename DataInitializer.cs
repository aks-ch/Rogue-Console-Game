using System.Text.Json;
using RogueConsoleGame.DataTypes;
using RogueConsoleGame.World.GameObjects.Characters;

namespace RogueConsoleGame;

/// <summary>
/// Initializes all data from files that needs initializing.
/// </summary>
public class DataInitializer
{
    public PlayerData Player { get; private set; }
    public List<EnemyData> Enemies { get; private set; }
    
    public string PlayerFilePath => "./Data/Player.json";
    public string EnemiesFilePath => "./Data/Enemies.json";

    private readonly PlayerData _defaultPlayerData = new PlayerData('P', 10, 1, 5, 0.2);
    private readonly List<EnemyData> _defaultEnemyData =
    [
        new EnemyData('S', 3, 0.2, 2),
        new EnemyData('K', 5, 0.5, 3),
        new EnemyData('Q', 8, 1, 4),
    ];
    
    /// <summary>
    /// Creates a new Initializer.
    /// </summary>
    public DataInitializer()
    {
        // Player checked
        try
        {
            Player = FetchPlayerData() ?? throw new InvalidOperationException();
        }
        catch
        {
            Console.WriteLine("Invalid Player JSON! Reverting to default.");
            Player = _defaultPlayerData with {};
        }
        
        // Enemies checked
        try
        {
            Enemies = FetchEnemyData() ?? throw new InvalidOperationException();
        }
        catch
        {
            Console.WriteLine("Invalid Enemies JSON! Reverting to default.");
            Enemies = [.._defaultEnemyData];
        }
    }

    /// <summary>
    /// Reinitialize data from files.
    /// </summary>
    /// <param name="playerData">Reinitialize player data. Default: true</param>
    /// <param name="enemyData">Reinitialize enemy data. Default: true</param>
    public void Reinitialize(bool playerData = true, bool enemyData = true)
    {
        // Player checked
        if (playerData)
        {
            try
            {
                Player = FetchPlayerData() ?? throw new InvalidOperationException();
            }
            catch
            {
                Console.WriteLine("Invalid Player JSON! No changes performed.");
            }
        }
        
        // Enemies checked
        if (enemyData)
        {
            try
            {
                Enemies = FetchEnemyData() ?? throw new InvalidOperationException();
            }
            catch
            {
                Console.WriteLine("Invalid Enemies JSON! No changes performed.");
            }
        }
    }

    /// <summary>
    /// Reset data to defaults.
    /// </summary>
    /// <param name="playerData">Reset player data. Default: true</param>
    /// <param name="enemyData">Reset enemy data. Default: true</param>
    public void ResetToDefault(bool playerData = true, bool enemyData = true)
    {
        if (playerData) Player = _defaultPlayerData with {};
        if (enemyData) Enemies = _defaultEnemyData.Select(enemy => enemy with {}).ToList();
    }

    private PlayerData? FetchPlayerData()
    {
        try
        {
            var playerData = JsonSerializer.Deserialize<PlayerData>(File.ReadAllText(PlayerFilePath)) ?? throw new NullReferenceException();
            return playerData with
            {
                Strength = Math.Round(playerData.Strength, 1),
                HealFactor = Math.Round(playerData.HealFactor, 1)
            };
        }
        catch
        {
            return null;
        }
    }
    
    private List<EnemyData>? FetchEnemyData()
    {
        try
        {
            var enemiesData = JsonSerializer.Deserialize<List<EnemyData>>(File.ReadAllText(PlayerFilePath)) ?? throw new NullReferenceException();
            for (int i = 0; i < enemiesData.Count; i++)
            {
                enemiesData[i] = enemiesData[i] with { Strength = Math.Round(enemiesData[i].Strength, 1) };
            }
            
            // Sort data
            enemiesData = enemiesData.OrderBy(x => x.Points).ToList();
            
            return enemiesData;
        }
        catch
        {
            return null;
        }
    }
}