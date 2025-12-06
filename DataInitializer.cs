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
    public EnemyData[] Enemies { get; private set; }
    
    public string PlayerFilePath { get; } = "./Data/Player.json";
    public string EnemiesFilePath { get; } = "./Data/Enemies.json";

    private readonly PlayerData _defaultPlayerData = new PlayerData('P', 10, 1, 5, 0.2);
    private readonly EnemyData[] _defaultEnemyData =
    [
        new EnemyData('S', 3, 0.2, 1),
        new EnemyData('K', 5, 0.5, 2),
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
            Player = JsonSerializer.Deserialize<PlayerData>(File.ReadAllText(PlayerFilePath)) ?? throw new NullReferenceException();
            Player = Player with
            {
                Strength = Math.Round(Player.Strength, 1),
                HealFactor = Math.Round(Player.HealFactor, 1)
            };
        }
        catch
        {
            Console.WriteLine("Invalid Player JSON! Reverting to default.");
            Player = _defaultPlayerData;
        }
        
        // Enemies checked
        try
        {
            Enemies = JsonSerializer.Deserialize<EnemyData[]>(File.ReadAllText(EnemiesFilePath)) ?? throw new NullReferenceException();
            for (int i = 0; i < Enemies.Length; i++)
            {
                Enemies[i] = Enemies[i] with { Strength = Math.Round(Enemies[i].Strength, 1) };
            }
        }
        catch
        {
            Console.WriteLine("Invalid Enemies JSON! Reverting to default.");
            Enemies = _defaultEnemyData;
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
                Player = JsonSerializer.Deserialize<PlayerData>(File.ReadAllText(PlayerFilePath)) ?? throw new NullReferenceException();
                Player = Player with
                {
                    Strength = Math.Round(Player.Strength, 1),
                    HealFactor = Math.Round(Player.HealFactor, 1)
                };
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
                Enemies = JsonSerializer.Deserialize<EnemyData[]>(File.ReadAllText(EnemiesFilePath)) ?? throw new NullReferenceException();
                for (int i = 0; i < Enemies.Length; i++)
                {
                    Enemies[i] = Enemies[i] with { Strength = Math.Round(Enemies[i].Strength, 1) };
                }
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
        if (playerData) Player = _defaultPlayerData;
        if (enemyData) Enemies = _defaultEnemyData;
    }
}