using RogueConsoleGame.Characters;
using RogueConsoleGame.Interfaces;
using RogueConsoleGame.Structures;

namespace RogueConsoleGame.Screens;

public class GameScreen(GameManager gameManager) : IScreen
{
    public GameManager GameManager = gameManager;
    
    public Dictionary<Vector2, Character> CharactersByPosition = new Dictionary<Vector2, Character>();
    
    private bool _initialized = false;
    private char[,] _map = new char[0, 0];
    
    public void Draw()
    {
        if (!_initialized) Initialize();
    }

    private void Initialize()
    {
        _map = new char[GameManager.GameHeight, GameManager.GameWidth];
        
        Character newCharacter = new Player(this, GameManager.PlayerChar, 10, 1);
        CharactersByPosition.Add(newCharacter.Position, newCharacter);
    }
}