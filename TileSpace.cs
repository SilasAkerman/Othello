using System;
using Raylib_cs;
using System.Numerics;

namespace Othello
{
    class TileSpace
    {
        public Color PieceColor { get; set; } = Game.BOARD_COLOR; // The default color for no color

        Vector2 pos = new Vector2();
        int size;

        const int PIECE_SIZE_MARGIN = 15;

        public TileSpace(int posX, int posY, int aSize)
        {
            pos.X = posX;
            pos.Y = posY;
            size = aSize;
        }

        public bool IsClicked(Vector2 mouse)
        {
            if (mouse.X > pos.X && mouse.Y > pos.Y && mouse.X < pos.X + size && mouse.Y < pos.Y + size)
            {
                return true;
            }
            return false;
        }

        public void Display()
        {
            Raylib.DrawRectangle((int)pos.X, (int)pos.Y, size, size, Game.BOARD_COLOR);
            Raylib.DrawRectangleLines((int)pos.X, (int)pos.Y, size, size, Color.BLACK);
            if (!PieceColor.Equals(Game.BOARD_COLOR))
            {
                Raylib.DrawCircle((int)pos.X + size / 2, (int)pos.Y + size / 2, size / 2 - PIECE_SIZE_MARGIN, PieceColor);
                Raylib.DrawCircleLines((int)pos.X + size / 2, (int)pos.Y + size / 2, size / 2 - PIECE_SIZE_MARGIN, PieceColor);
            }
        }

        public void PlaceColor(Color color)
        {
            PieceColor = color;
        }

        public TileSpace (TileSpace other)
        {
            pos.X = other.pos.X;
            pos.Y = other.pos.Y;
            size = other.size;

            PieceColor = new Color(other.PieceColor.r, other.PieceColor.g, other.PieceColor.b, other.PieceColor.a);
        }
    }
}
