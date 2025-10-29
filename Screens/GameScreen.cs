using RogueConsoleGame.Interfaces;

namespace RogueConsoleGame.Screens;

public class GameScreen(GameManager gameManager) : IScreen
{
    /// <summary>
    /// Draw & execute the game.
    /// </summary>
    public void Draw()
    {
        throw new NotImplementedException("Game Screen not implemented");
    }
}