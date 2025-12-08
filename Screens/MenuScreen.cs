using RogueConsoleGame.Enums;
using RogueConsoleGame.Interfaces;

namespace RogueConsoleGame.Screens;

public class MenuScreen(GameManager gameManager) : IScreen
{
    private bool _viewGuide;
    private bool _invalidInput;
    
    private MenuOption _currentOption = MenuOption.None;

    // Execute the screen.
    public void Draw()
    {
        // Basic information
        GameManager.ColorConsoleWriteLine(ConsoleColor.Magenta, "Welcome to RogueConsoleGame!");
        GameManager.ColorConsoleWriteLine(ConsoleColor.DarkGray, "Please increase the window size if you haven't already to avoid rendering issues.\n");
        string menuLine1 = "Please choose one of the listed options by entering the corresponding number:";
        string menuLine2 = "\t1. Start a new game with random settings.";
        string menuLine3 = "\t2. Start a new game with custom settings.";
        string menuLine4 = $"\t3. {(_viewGuide ? "Hide" : "View")} the guide.";
        string menuLine5 = "\t4. Exit.";

        // Highlight options if selected
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

        // The Guide
        if (_viewGuide) GameManager.DrawGuide(gameManager);
        Console.WriteLine();

        // Invalid input
        if (_invalidInput)
        {
            GameManager.ColorConsoleWriteLine(ConsoleColor.Red, "Invalid input. Please try again.");
            _invalidInput = false;
        }

        // Process Option
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
            _currentOption = MenuOption.None;
        }
        else if (_currentOption == MenuOption.Custom)
        {
            CustomGameOptions();
            _currentOption = MenuOption.None;
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
        gameManager.Seed = new Random();

        gameManager.CurrentGameState = GameState.Game;
    }

    /// <summary>
    /// Select custom game options.
    /// </summary>
    private void CustomGameOptions()
    {
        // custom seed
        Console.Write("Please enter a seed (Leave empty for random seed): ");
        var input = Console.ReadLine();
        if (string.IsNullOrEmpty(input))
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
                convertedInput = gameManager.Seed.Next(minValue, maxValue + 1);
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