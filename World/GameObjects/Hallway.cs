using RogueConsoleGame.DataTypes;

namespace RogueConsoleGame.World.GameObjects;

/// <summary>
/// Represents a pathway to another map.
/// </summary>
public class Hallway : GameObject
{
    /// <summary>
    /// The map this hallway belongs to.
    /// </summary>
    public Map Map { get; }
    
    /// <summary>
    /// The map this hallway leads to.
    /// </summary>
    public Map DestinationMap { get; }
    
    /// <summary>
    /// The map this hallway belongs to.
    /// </summary>
    public Hallway? DestinationHallway { get; set; }
    
    /// <summary>
    /// Whether this hallway's map is parent to the destination map.
    /// </summary>
    public bool IsParent { get; }
    
    /// <summary>
    /// Where you will spawn if you emerge through this hallway.
    /// </summary>
    public Vector2 Spawn { get; }
    
    /// <summary>
    /// Creates a new hallway connecting two maps.
    /// </summary>
    /// <param name="map">The map this hallway is on.</param>
    /// <param name="position">The position of the hallway on its own map.</param>
    /// <param name="destination">The map this hallway points to.</param>
    /// <param name="isParent">Whether this map is parent to the destination map.</param>
    public Hallway(Map map, Vector2 position, Map destination, bool isParent) : base(position)
    {
        Map = map;
        DestinationMap = destination;
        IsParent = isParent;
        Spawn = IsParent ? Position + new Vector2(-1, 0) : Position + new Vector2(1, 0);

        Symbol = ' ';
    }
}