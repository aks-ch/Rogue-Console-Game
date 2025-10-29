using RogueConsoleGame.Interfaces;

namespace RogueConsoleGame.Screens;

public class GameScreen(GameManager gameManager) : IScreen
{
    private bool _initialized;
    private char[] _map;
    
    public void Draw()
    {
        if (!_initialized) Initialize();
    }

    private void Initialize()
    {
        
    }
}