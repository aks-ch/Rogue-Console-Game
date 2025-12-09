using RogueConsoleGame.DataTypes;
using RogueConsoleGame.Enums;

namespace RogueConsoleGame.World.GameObjects.Items;

public class WinItem(Map map, Vector2 position, bool hidden) : Item(map, position, hidden)
{
    public override char Symbol { get => Hidden ? Map.GameManager.EmptyChar : WinSymbol; protected set => WinSymbol = value; }
    public override ConsoleColor Color { get => Hidden ? ConsoleColor.DarkGray : WinColor; set => WinColor = value; }
    
    private char WinSymbol { get; set; } = '%';
    private ConsoleColor WinColor { get; set; } = ConsoleColor.Magenta;

    public void Update()
    {
        Map.MapTree.Victory = true;
    }
}