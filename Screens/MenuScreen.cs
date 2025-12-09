using RogueConsoleGame.Enums;
using RogueConsoleGame.Interfaces;

namespace RogueConsoleGame.Screens;

public class MenuScreen(GameManager gameManager) : IScreen
{
    private bool _viewGuide;
    private bool _invalidInput;
    
    private MenuOption _currentOption = MenuOption.None;
    private GameManager GameManager { get; } = gameManager;

    // Execute the screen.
    public void Draw()
    {
        // Basic information
        GameManager.ColorConsoleWriteLine(ConsoleColor.Magenta, "Welcome to RogueConsoleGame!");
        GameManager.ColorConsoleWriteLine(ConsoleColor.DarkGray, "Please increase the window size if you haven't already to avoid rendering issues.\n");
        string menuLine1 = "Please choose one of the listed options by entering the corresponding number:";
        string menuLine2 = "\t1. Start a new game.";
        string menuLine3 = $"\t2. {(_viewGuide ? "Hide" : "View")} the guide.";
        string menuLine4 = "\t3. Exit.";

        // Highlight options if selected
        if (_currentOption == MenuOption.None)
        {
            Console.WriteLine(menuLine1 + "\n" + menuLine2 + "\n" + menuLine3 + "\n" + menuLine4);
        }
        else
        {
            GameManager.ColorConsoleWriteLine(ConsoleColor.DarkGray, menuLine1);
            if (_currentOption == MenuOption.Start)
            {
                Console.WriteLine(menuLine2);
            }
            else
            {
                GameManager.ColorConsoleWriteLine(ConsoleColor.DarkGray, menuLine2);
            }

            GameManager.ColorConsoleWriteLine(ConsoleColor.DarkGray, menuLine3);
            if (_currentOption == MenuOption.Exit)
            {
                Console.WriteLine(menuLine4);
            }
            else
            {
                GameManager.ColorConsoleWriteLine(ConsoleColor.DarkGray, menuLine4);
            }
        }

        // The Guide
        if (_viewGuide) GameManager.DrawGuide(GameManager);
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
                    _currentOption = MenuOption.Start;
                    break;
                case ConsoleKey.D2:
                    _viewGuide = !_viewGuide;
                    break;
                case ConsoleKey.D3:
                    _currentOption = MenuOption.Exit;
                    break;
                default:
                    _invalidInput = true;
                    break;
            }
        }
        else if (_currentOption == MenuOption.Start)
        {
            SelectGameOptions();
            _currentOption = MenuOption.None;
        }
        else if (_currentOption == MenuOption.Exit)
        {
            GameManager.IsRunning = false;
        }
    }

    /// <summary>
    /// Select custom game options.
    /// </summary>
    private void SelectGameOptions()
    {
        Console.CursorVisible = true;
        
        // Custom seed
        Console.Write("Please enter a seed (Leave empty for random seed): ");
        var input = Console.ReadLine();
        if (int.TryParse(input, out var convertedInput))
        {
            GameManager.Seed = new Random(convertedInput);
        }
        else
        {
            int seed = new Random().Next(int.MinValue, int.MaxValue);
            Console.WriteLine($"Random seed is {seed}");
            GameManager.Seed = new Random(seed);
        }

        // Set difficulty
        while (true)
        {
            string inputRequest = "Please enter a difficulty level between 1 and 10 (inclusive): ";
            Console.Write(inputRequest);
            input = Console.ReadLine();

            if (int.TryParse(input, out var convertInput) && convertInput >= 1 && convertInput <= 10)
            {
                GameManager.Difficulty = convertInput;
                break;
            }
            
            GameManager.ColorConsoleWriteLine(ConsoleColor.Red, "Invalid input. Please try again.");
        }
        
        GameManager.CurrentGameState = GameState.Game;
    }
}