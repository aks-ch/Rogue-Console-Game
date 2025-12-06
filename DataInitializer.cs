using System.Text.Json;
using RogueConsoleGame.DataTypes;
using RogueConsoleGame.World.GameObjects.Characters;

namespace RogueConsoleGame;

/// <summary>
/// Initializes all data from files that needs initializing.
/// </summary>
public class DataInitializer
{
    public PlayerData PlayerData { get; private set; }
    public EnemyData[] EnemyData { get; private set; }
    
    public string PlayerJson { get; } = File.ReadAllText("./Data/Player.json");
    public string EnemyJson { get; } = File.ReadAllText("./Data/Enemies.json");

    private PlayerData _defaultPlayerData { get; } = new PlayerData('P', 10, 1, 5, 0.2);
    private EnemyData[] _defaultEnemyData { get; } =
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
            PlayerData = JsonSerializer.Deserialize<PlayerData>(PlayerJson) ?? throw new NullReferenceException();
            PlayerData = PlayerData with
            {
                Strength = Math.Round(PlayerData.Strength, 1),
                HealFactor = Math.Round(PlayerData.HealFactor, 1)
            };
        }
        catch
        {
            Console.WriteLine("Invalid Player JSON! Reverting to default.");
            PlayerData = _defaultPlayerData;
        }
        
        // Enemies checked
        try
        {
            EnemyData = JsonSerializer.Deserialize<EnemyData[]>(EnemyJson) ?? throw new NullReferenceException();
            for (int i = 0; i < EnemyData.Length; i++)
            {
                EnemyData[i] = EnemyData[i] with { Strength = Math.Round(EnemyData[i].Strength, 1) };
            }
        }
        catch
        {
            Console.WriteLine("Invalid Enemies JSON! Reverting to default.");
            EnemyData = _defaultEnemyData;
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
                PlayerData = JsonSerializer.Deserialize<PlayerData>(PlayerJson) ?? throw new NullReferenceException();
                PlayerData = PlayerData with
                {
                    Strength = Math.Round(PlayerData.Strength, 1),
                    HealFactor = Math.Round(PlayerData.HealFactor, 1)
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
                EnemyData = JsonSerializer.Deserialize<EnemyData[]>(EnemyJson) ?? throw new NullReferenceException();
                for (int i = 0; i < EnemyData.Length; i++)
                {
                    EnemyData[i] = EnemyData[i] with { Strength = Math.Round(EnemyData[i].Strength, 1) };
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
        if (playerData) PlayerData = _defaultPlayerData;
        if (enemyData) EnemyData = _defaultEnemyData;
    }
}