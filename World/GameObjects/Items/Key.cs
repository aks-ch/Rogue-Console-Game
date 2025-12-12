using RogueConsoleGame.DataTypes;

namespace RogueConsoleGame.World.GameObjects.Items;

public class Key(Map map, Vector2 position, bool hidden, string keyID) : Item(map, position, hidden)
{
    public override char Symbol { get => Hidden ? Map.GameManager.EmptyChar : KeySymbol; protected set => KeySymbol = value; }
    public override ConsoleColor Color { get => Hidden ? ConsoleColor.Gray : KeyColor; set => KeyColor = value; }
    
    private char KeySymbol { get; set; } = '!';
    private ConsoleColor KeyColor { get; set; } = ConsoleColor.Yellow;

    public string KeyID { get; } = keyID;
}