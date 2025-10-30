using RogueConsoleGame.Characters;
using RogueConsoleGame.Interfaces;
using RogueConsoleGame.Structures;

namespace RogueConsoleGame.Screens;

public class GameScreen(GameManager gameManager) : IScreen
{
    public GameManager GameManager = gameManager;
    
    public List<Vector2> OccupiedPositions { get; private set; }
    public List<Character> Characters { get; private set; }
    
    private bool _initialized;
    private char[,] _map;
    
    public void Draw()
    {
        if (!_initialized) Initialize();
    }

    private void Initialize()
    {
        _map = new char[GameManager.GameHeight, GameManager.GameWidth];
        
        Characters.Add(new Player(this, GameManager.PlayerChar, 10, 1));
    }
}