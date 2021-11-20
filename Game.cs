using System;
using Raylib_cs;

namespace Othello
{
    class Game
    {
        Board gameBoard;

        public static Color TEAM_1_COLOR = Color.RAYWHITE;
        public static Color TEAM_2_COLOR = Color.BLACK;
        public static Color BOARD_COLOR = Color.DARKGREEN;

        Button gameOverQuitButton = new Button(Program.WIDTH/2 - 150, Program.HEIGHT - 100, 200, 75, "Game Over");

        public Game()
        {
            gameBoard = new Board();
        }

        public void Update()
        {
            if (!gameBoard.WaitTurning)
            {
                if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
                {
                    if (gameBoard.GameIsAlive)
                    {
                        gameBoard.AttemptPlacement(Raylib.GetMousePosition(), true);
                    }
                    else
                    {
                        gameOverQuitButton.checkPressed(Raylib.GetMousePosition());
                    }
                }
                if (Raylib.IsMouseButtonReleased(MouseButton.MOUSE_LEFT_BUTTON) && !gameBoard.GameIsAlive)
                {
                    if (gameOverQuitButton.IsPressed) Program.QuitGame();
                }
                if (Raylib.IsKeyPressed(KeyboardKey.KEY_BACKSPACE)) Program.QuitGame();
            }
            else
            {
                System.Threading.Thread.Sleep(300);
                gameBoard.turnOverUpdate();
            }
        }

        public void Display()
        {
            gameBoard.Display();
            if (!gameBoard.GameIsAlive)
            {
                gameOverQuitButton.Display();
            }
        }
    }
}
