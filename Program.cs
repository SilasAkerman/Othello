using System;
using Raylib_cs;
using System.Numerics;

namespace Othello
{
    class Program
    {
        enum MainState
        {
            Intro,
            GameRun,
            Help
        }

        static Game Othello;
        
        static MainState currentState;

        public const int WIDTH = 1100;
        public const int HEIGHT = 1000;


        static Button introHelpButton = new Button(400, 800, 200, 80, "Help");
        static Button introGameButton = new Button(400, 600, 200, 80, "Play Game");
        static Button helpIntroButton = new Button(400, 800, 200, 80, "Back");


        static void Main(string[] args)
        {
            currentState = MainState.Intro;
            Othello = new Game();

            Raylib.InitWindow(WIDTH, HEIGHT, "Othello");
            Raylib.SetTargetFPS(30);

            while (!Raylib.WindowShouldClose())
            {
                Display();
                Update();
            }
        }

        static void Display()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.GRAY);
            switch (currentState)
            {
                case MainState.Intro:

                    Raylib.DrawText("Othello", WIDTH / 2 - 200, 200, 60, Color.BLACK);
                    Raylib.DrawText("By Silas Åkerman", WIDTH / 2 + 200, 250, 20, Color.BLACK);
                    introHelpButton.Display();
                    introGameButton.Display();
                    break;

                case MainState.Help:
                    Raylib.DrawText("Welcome to Othello!", 300, 50, 60, Color.BLACK);
                    string[] helpTexts =
                    {
                        "The goal of the game is to have as many pieces of your color as possible at the end of the game.",
                        "Each turn, one of you place a piece of your color and must convert at least on other piece to your color.",
                        "A piece is converted if it is surrounded by the placed piece and some other piece of the same colors at opposite ends.",
                        "This rule applies to all directions, orthogonal and diagonal! Keep in mind that chains of conversions can't happen.",
                        "If a player is unable to convert a single piece, their turn is skipped.",
                        "The game ends if noone kan place or if the board is filled. Whomever has the most pieces wins!"
                    };
                    for (int i = 0; i < helpTexts.Length; i++)
                    {
                        Raylib.DrawText(helpTexts[i], 5, 150 + i * 60, 19, Color.BLACK);
                    }
                    Raylib.DrawText("Made by: Silas Åkerman", 5, 500, 25, Color.BLACK);
                    helpIntroButton.Display();
                    break;

                case MainState.GameRun:
                    Othello.Display();
                    break;
            }
            Raylib.EndDrawing();
        }

        static void Update()
        {
            switch (currentState)
            {
                case MainState.Intro:
                    if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
                    {
                        Vector2 mouse = Raylib.GetMousePosition();
                        introHelpButton.checkPressed(mouse);
                        introGameButton.checkPressed(mouse);
                    }
                    if (Raylib.IsMouseButtonReleased(MouseButton.MOUSE_LEFT_BUTTON))
                    {
                        if (introHelpButton.IsPressed)
                        {
                            currentState = MainState.Help;
                        }
                        if (introGameButton.IsPressed)
                        {
                            Othello = new Game();
                            currentState = MainState.GameRun;
                        }
                    }
                    break;

                case MainState.Help:
                    if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
                    {
                        Vector2 mouse = Raylib.GetMousePosition();
                        helpIntroButton.checkPressed(mouse);
                    }
                    if (Raylib.IsMouseButtonReleased(MouseButton.MOUSE_LEFT_BUTTON))
                    {
                        if (helpIntroButton.IsPressed)
                        {
                            currentState = MainState.Intro;
                        }
                    }
                    break;

                case MainState.GameRun:
                    Othello.Update();
                    break;
            }
        }

        public static void QuitGame()
        {
            currentState = MainState.Intro;
        }
    }
}
