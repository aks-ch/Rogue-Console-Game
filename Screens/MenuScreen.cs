using RogueConsoleGame.Enums;
using RogueConsoleGame.Interfaces;

namespace RogueConsoleGame.Screens;

public class MenuScreen(GameManager gameManager) : IScreen
{
    private bool _viewGuide;
    private bool _invalidInput;
    
    private MenuOption _currentOption = MenuOption.None;

    public void Draw()
    {
        GameManager.ColorConsoleWriteLine(ConsoleColor.Magenta, "Welcome to RogueConsoleGame!");

        string menuLine1 = "Please choose one of the listed options by entering the corresponding number:";
        string menuLine2 = "\t1. Start a new game with random settings.";
        string menuLine3 = "\t2. Start a new game with custom settings.";
        string menuLine4 = $"\t3. {(_viewGuide ? "Hide" : "View")} the guide.";
        string menuLine5 = "\t4. Exit.";

        if (_currentOption == MenuOption.None)
        {
            Console.WriteLine(menuLine1 + "\n" + menuLine2 + "\n" + menuLine3 + "\n" + menuLine4 + "\n" + menuLine5);
        }
        else
        {
            GameManager.ColorConsoleWriteLine(ConsoleColor.DarkGray, menuLine1);
            if (_currentOption == MenuOption.Random)
            {
                Console.WriteLine(menuLine2);
            }
            else
            {
                GameManager.ColorConsoleWriteLine(ConsoleColor.DarkGray, menuLine2);
            }

            if (_currentOption == MenuOption.Custom)
            {
                Console.WriteLine(menuLine3);
            }
            else
            {
                GameManager.ColorConsoleWriteLine(ConsoleColor.DarkGray, menuLine3);
            }

            GameManager.ColorConsoleWriteLine(ConsoleColor.DarkGray, menuLine4);
            if (_currentOption == MenuOption.Exit)
            {
                Console.WriteLine(menuLine5);
            }
            else
            {
                GameManager.ColorConsoleWriteLine(ConsoleColor.DarkGray, menuLine5);
            }
        }

        if (_viewGuide) GameManager.DrawGuide();
        Console.WriteLine();

        if (_invalidInput)
        {
            GameManager.ColorConsoleWriteLine(ConsoleColor.Red, "Invalid input. Please try again.");
            _invalidInput = false;
        }

        if (_currentOption == MenuOption.None)
        {
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.D1:
                    _currentOption = MenuOption.Random;
                    break;
                case ConsoleKey.D2:
                    _currentOption = MenuOption.Custom;
                    break;
                case ConsoleKey.D3:
                    _viewGuide = !_viewGuide;
                    break;
                case ConsoleKey.D4:
                    _currentOption = MenuOption.Exit;
                    break;
                default:
                    _invalidInput = true;
                    break;
            }
        }
        else if (_currentOption == MenuOption.Random)
        {
            RandomGameOptions();
        }
        else if (_currentOption == MenuOption.Custom)
        {
            CustomGameOptions();
        }
        else if (_currentOption == MenuOption.Exit)
        {
            gameManager.IsRunning = false;
        }
    }

    /// <summary>
    /// Randomize game options.
    /// </summary>
    private void RandomGameOptions()
    {
        Console.Write("Please enter a seed (Leave empty for random seed): ");
        var input = Console.ReadLine();

        if (input == null)
        {
            gameManager.Seed = new Random();
        }
        else if (int.TryParse(input, out var convertedInput))
        {
            gameManager.Seed = new Random(convertedInput);
        }
        else
        {
            gameManager.Seed = new Random(input.GetHashCode());
        }

        gameManager.GameWidth = gameManager.Seed.Next(gameManager.MinGameWidth, gameManager.MaxGameWidth + 1);
        gameManager.GameHeight = gameManager.Seed.Next(gameManager.MinGameHeight, gameManager.MaxGameHeight + 1);
        gameManager.EnemyCount = gameManager.Seed.Next(gameManager.MinEnemyCount, gameManager.MaxEnemyCount + 1);

        gameManager.CurrentGameState = GameState.Game;
    }

    /// <summary>
    /// Select custom game options.
    /// </summary>
    private void CustomGameOptions()
    {
        // game width
        string prompt =
            $"Please enter the width of the game [{gameManager.MinGameWidth}, {gameManager.MaxGameWidth}] (Default: {gameManager.MaxGameWidth}):";
        gameManager.GameWidth = GetIntInput(prompt, gameManager.MinGameWidth, gameManager.MaxGameWidth);

        // game height
        prompt =
            $"Please enter the height of the game [{gameManager.MinGameHeight}, {gameManager.MaxGameHeight}] (Default: {gameManager.MaxGameHeight}):";
        gameManager.GameHeight = GetIntInput(prompt, gameManager.MinGameHeight, gameManager.MaxGameHeight);

        // enemy count
        prompt =
            $"Please enter the number of enemies [{gameManager.MinEnemyCount}, {gameManager.MaxEnemyCount}] (Default: {gameManager.MaxEnemyCount}):";
        gameManager.GameHeight = GetIntInput(prompt, gameManager.MinEnemyCount, gameManager.MaxEnemyCount);
        
        gameManager.CurrentGameState = GameState.Game;
    }

    /// <summary>
    /// Get an integer input from the user that is within the bounds identified or the maximum value if input is empty.
    /// </summary>
    /// <param name="prompt">The prompt to present to the user.</param>
    /// <param name="minValue">Minimum integer required.</param>
    /// <param name="maxValue">Maximum integer required.</param>
    /// <returns>The integer within the bounds.</returns>
    private int GetIntInput(string prompt, int minValue, int maxValue)
    {
        string? input;
        int convertedInput;

        while (true)
        {
            Console.Write($"{prompt} ");
            input = Console.ReadLine();

            if (string.IsNullOrEmpty(input))
            {
                convertedInput = maxValue;
            }
            else if (!int.TryParse(input, out convertedInput) || convertedInput < minValue || convertedInput > maxValue)
            {
                GameManager.ColorConsoleWriteLine(ConsoleColor.Red, "Invalid input. Please try again.");
                continue;
            }

            return convertedInput;
        }
    }
}