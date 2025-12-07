using RogueConsoleGame.DataTypes;

namespace RogueConsoleGame.World.GameObjects.Items;

/// <summary>
/// Default constructor for a new item.
/// </summary>
/// <param name="map">The map this item is on.</param>
/// <param name="position">The position of the item on the board.</param>
/// <param name="hidden">Whether this item is hidden at first.</param>
public abstract class Item(Map map, Vector2 position, bool hidden) : GameObject(map, position)
{
    public bool Hidden { get; set; } = hidden;
}