using System;
using Raylib_cs;
using System.Numerics;

namespace Othello
{
    class Button
    {
        Vector2 pos = new Vector2();
        int width;
        int height;
        string text;

        bool _isPressed = false;
        public bool IsPressed
        {
            get
            {
                if (_isPressed)
                {
                    _isPressed = false;
                    return true;
                }
                return false;
            }
        }

        public Button(int posX, int posY, int aWidth, int aHeight, string aText)
        {
            pos.X = posX;
            pos.Y = posY;
            width = aWidth;
            height = aHeight;
            text = aText;
        }

        public void checkPressed(Vector2 mouse)
        {
            if (mouse.X > pos.X && mouse.Y > pos.Y && mouse.X < pos.X + width && mouse.Y < pos.Y + height) _isPressed = true;
        }

        public bool checkPressedReturn(Vector2 mouse)
        {
            checkPressed(mouse);
            return IsPressed;
        }

        public void Display()
        {
            Color color = _isPressed ? Color.GREEN : Color.RAYWHITE;
            Raylib.DrawRectangle((int)pos.X, (int)pos.Y, width, height, color);
            Raylib.DrawRectangleLines((int)pos.X, (int)pos.Y, width, height, Color.BLACK);
            Raylib.DrawText(text, (int)pos.X + 10, (int)pos.Y + height / 2 - 15, 30, Color.BLACK);
        }
    }
}
