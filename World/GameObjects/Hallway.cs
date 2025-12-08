using RogueConsoleGame.DataTypes;

namespace RogueConsoleGame.World.GameObjects;

/// <summary>
/// Represents a pathway to another map.
/// </summary>
public sealed class Hallway : GameObject
{
    public Map DestinationMap { get; }
    public Hallway? DestinationHallway { get; set; }
    public bool IsParent { get; }
    public bool Locked => LockIDs.Count > 0;
    
    /// <summary>
    /// Where you will spawn if you emerge through this hallway.
    /// </summary>
    public Vector2 Spawn { get; }
    
    /// <summary>
    /// Whether the player has gone through this hallway yet.
    /// </summary>
    public bool Used { get; set; }

    /// <summary>
    /// What IDs are required to unlock this door.
    /// </summary>
    private List<string> LockIDs { get; }
    
    public override char Symbol => Locked ? '|' : IsParent ? '>' : '<';
    public override ConsoleColor Color => Locked ? ConsoleColor.Red : Used ? ConsoleColor.Blue : ConsoleColor.Cyan;
    
    /// <summary>
    /// Creates a new hallway connecting two maps.
    /// </summary>
    /// <param name="map">The map this hallway is on.</param>
    /// <param name="position">The position of the hallway on its own map.</param>
    /// <param name="destination">The map this hallway points to.</param>
    /// <param name="isParent">Whether this map is parent to the destination map.</param>
    public Hallway(Map map, Vector2 position, Map destination, bool isParent) : base(map, position)
    {
        DestinationMap = destination;
        IsParent = isParent;
        Spawn = IsParent ? Position + new Vector2(-1, 0) : Position + new Vector2(1, 0);

        Symbol = ' ';

        LockIDs = new List<string>();
        if (isParent) LockIDs.Add("enemy");
    }

    /// <summary>
    /// Adds a new lockId to the lock.
    /// </summary>
    /// <param name="lockId">The lockId to add.</param>
    /// <returns>True if added. False if lockId is already on this hallway.</returns>
    public bool AddLock(string lockId)
    {
        if (LockIDs.Contains(lockId)) return false;
        
        LockIDs.Add(lockId);
        return true;
    }

    /// <summary>
    /// Unlock the defined lockId.
    /// </summary>
    /// <param name="lockId">The lockId to unlock.</param>
    /// <returns>True if unlock was successful. False if not.</returns>
    public bool UnlockLock(string lockId)
    {
        return LockIDs.Remove(lockId);
    }
}