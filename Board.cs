using System;
using Raylib_cs;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;

namespace Othello
{
    class Board
    {
        const int ROWS = 8;
        const int COLS = 8;
        const int BOARD_MARGINS = 100;

        TileSpace[] boardSpaces = new TileSpace[ROWS * COLS];
        int[] availableMovesIndex;

        public Color activeColor { get; set; }

        private bool _gameIsAlive = true;
        public bool GameIsAlive { get { return _gameIsAlive; } }

        bool cantPlace = false;

        bool _waitTurning = false;
        public bool WaitTurning { get { return _waitTurning; } }
        List<int> waitTurningIndexes = new List<int>();

        int team1Total;
        int team2Total;

        public Board()
        {
            int size = Program.HEIGHT / ROWS - BOARD_MARGINS / ROWS * 2;
            int startX = Program.WIDTH / 2 - (COLS / 2 * size);

            for (int row = 0; row < ROWS; row++)
            {
                for (int col = 0; col < COLS; col++)
                {
                    int posX = size * col + startX;
                    int posY = size * row + BOARD_MARGINS - 50;
                    boardSpaces[getIndexFrom2DIndex(row, col)] = new TileSpace(posX, posY, size);
                }
            }

            activeColor = Game.TEAM_1_COLOR;

            boardSpaces[getIndexFrom2DIndex(ROWS / 2 - 1, COLS / 2 - 1)].PieceColor = Game.TEAM_1_COLOR;
            boardSpaces[getIndexFrom2DIndex(ROWS / 2 - 1, COLS / 2)].PieceColor = Game.TEAM_2_COLOR;
            boardSpaces[getIndexFrom2DIndex(ROWS / 2, COLS / 2 - 1)].PieceColor = Game.TEAM_2_COLOR;
            boardSpaces[getIndexFrom2DIndex(ROWS / 2, COLS / 2)].PieceColor = Game.TEAM_1_COLOR;

            availableMovesIndex = validMoves(activeColor);
            calculateTotals();
        }

        public void Display()
        {
            foreach (TileSpace tile in boardSpaces)
            {
                tile.Display();
            }
            if (cantPlace && _gameIsAlive)
            {
                Raylib.DrawText("You have no legal move, click to skip turn", 150, Program.HEIGHT-50, 20, Color.BLACK);
            }

            displayTeamOverlays();
        }

        private void displayTeamOverlays()
        {
            int sideMargin = Program.WIDTH / 2 - (COLS / 2 * (Program.HEIGHT / ROWS - BOARD_MARGINS / ROWS * 2));

            int size = 25;
            int team1X = sideMargin / 2;
            int team2X = sideMargin / 2 + ((Program.HEIGHT / ROWS - BOARD_MARGINS / ROWS * 2) * COLS + sideMargin);
            Raylib.DrawCircle(team1X, Program.HEIGHT / 2, size, Game.TEAM_1_COLOR);
            Raylib.DrawCircleLines(team1X, Program.HEIGHT / 2, size, Color.BLACK);
            Raylib.DrawCircle(team2X, Program.HEIGHT / 2, size, Game.TEAM_2_COLOR);
            Raylib.DrawCircleLines(team2X, Program.HEIGHT / 2, size, Color.BLACK);

            Raylib.DrawText(Convert.ToString(team1Total), team1X - 8, Program.HEIGHT / 2 + size + 5, 25, Color.BLACK);
            Raylib.DrawText(Convert.ToString(team2Total), team2X - 8, Program.HEIGHT / 2 + size + 5, 25, Color.BLACK);

            int activeX = activeColor.Equals(Game.TEAM_1_COLOR) ? team1X : team2X;
            Raylib.DrawCircle(activeX, Program.HEIGHT / 2 - 100, 15, Color.BLUE);
            Raylib.DrawCircleLines(activeX, Program.HEIGHT / 2 - 100, 15, Color.BLACK);
        }

        private int getIndexFrom2DIndex(int row, int col)
        {
            int index = col + row * COLS;
            return index;
        }

        private bool validSpace(int tileIndex, Color teamColor)
        {
            if (!boardSpaces[tileIndex].PieceColor.Equals(Game.BOARD_COLOR)) return false;
            if (checkConversions(tileIndex, teamColor).Length <= 0) return false;

            return true;
        }

        private void placePiece(int tileIndex)
        {
            boardSpaces[tileIndex].PieceColor = activeColor;
            int[] conversionIndexes = checkConversions(tileIndex, activeColor);

            performConversions(conversionIndexes);

            activeColor = activeColor.Equals(Game.TEAM_1_COLOR) ? Game.TEAM_2_COLOR : Game.TEAM_1_COLOR;
            availableMovesIndex = validMoves(activeColor);
            calculateTotals();

            if (availableMovesIndex.Length <= 0) cantPlace = true; else cantPlace = false;
            bool emptySpot = false;
            foreach (TileSpace spot in boardSpaces)
            {
                if (spot.PieceColor.Equals(Game.BOARD_COLOR))
                {
                    emptySpot = true;
                    break;
                }
            }
            if (!emptySpot) _gameIsAlive = false;
        }

        private void placePieceAnimated(int tileIndex)
        {
            boardSpaces[tileIndex].PieceColor = activeColor;
            int[] conversionIndexes = checkConversions(tileIndex, activeColor);

            _waitTurning = true;
            waitTurningIndexes = conversionIndexes.ToList();

            calculateTotals();

            if (availableMovesIndex.Length <= 0) cantPlace = true; else cantPlace = false;
            bool emptySpot = false;
            foreach (TileSpace spot in boardSpaces)
            {
                if (spot.PieceColor.Equals(Game.BOARD_COLOR))
                {
                    emptySpot = true;
                    break;
                }
            }
            if (!emptySpot) _gameIsAlive = false;
        }

        private int getIndexFromMouse(Vector2 mouse)
        {
            for (int i = 0; i < boardSpaces.Length; i++)
            {
                if (boardSpaces[i].IsClicked(mouse))
                {
                    return i;
                }
            }
            return -1;
        }

        public void AttemptPlacement(Vector2 mouse, bool animated=false)
        {
            int index = getIndexFromMouse(mouse);
            if (index == -1) return;

            if (cantPlace)
            {
                activeColor = activeColor.Equals(Game.TEAM_1_COLOR) ? Game.TEAM_2_COLOR : Game.TEAM_1_COLOR;
                availableMovesIndex = validMoves(activeColor);
                if (availableMovesIndex.Length <= 0) _gameIsAlive = false;
                else cantPlace = false;
                return;
            }

            if (availableMovesIndex.Contains(index))
            {
                if (!animated) placePiece(index); else placePieceAnimated(index);
            }
        }

        private int[] checkConversions(int tileIndex, Color teamColor)
        {
            bool something = Raylib.IsKeyDown(KeyboardKey.KEY_T);
            List<int> totalConversions = new List<int>();
            Color oppositeTeamColor = teamColor.Equals(Game.TEAM_1_COLOR) ? Game.TEAM_2_COLOR : Game.TEAM_1_COLOR;

            Vector2[] directions =
            {
                new Vector2(-1, -1),
                new Vector2(-1, 0),
                new Vector2(-1, 1),
                new Vector2(0, -1),
                new Vector2(0, 1),
                new Vector2(1, -1),
                new Vector2(1, 0),
                new Vector2(1, 1)
            };

            foreach (Vector2 direction in directions)
            {
                List<int> potentialConversions = new List<int>();
                int[] lineIndexes = getAllIndexesFromLine(tileIndex, direction);
                bool beginningConversion = false;
                bool confirmedConversion = false;

                for (int i = 0; i < lineIndexes.Length; i++)
                {
                    if (boardSpaces[lineIndexes[i]].PieceColor.Equals(oppositeTeamColor))
                    {
                        beginningConversion = true;
                        potentialConversions.Add(lineIndexes[i]);
                    }
                    else if (boardSpaces[lineIndexes[i]].PieceColor.Equals(teamColor))
                    {
                        if (beginningConversion)
                        {
                            confirmedConversion = true;
                        }
                        break;
                    }
                    else break;
                }

                if (confirmedConversion)
                {
                    potentialConversions.Reverse();
                    totalConversions.AddRange(potentialConversions);
                }
            }

            return totalConversions.ToArray();
        }

        private int[] getAllIndexesFromLine(int start, Vector2 direction)
        {
            Vector2 tilePos = new Vector2();
            tilePos.X = start % ROWS;
            tilePos.Y = start / ROWS;

            List<int> lineIndexes = new List<int>();

            while (true)
            {
                tilePos += direction;
                if (tilePos.X >= 0 && tilePos.X < COLS && tilePos.Y >= 0 && tilePos.Y < ROWS)
                {
                    lineIndexes.Add(getIndexFrom2DIndex((int)tilePos.Y, (int)tilePos.X));
                }
                else break;
            }

            return lineIndexes.ToArray();
        }

        private void performConversions(int[] conversionsIndexes)
        {
            foreach(int index in conversionsIndexes)
            {
                Color turningColor = boardSpaces[index].PieceColor.Equals(Game.TEAM_1_COLOR) ? Game.TEAM_2_COLOR : Game.TEAM_1_COLOR;
                boardSpaces[index].PieceColor = turningColor;
            }
        }

        private int[] validMoves(Color teamColor)
        {
            List<int> validList = new List<int>();
            for (int i = 0; i < boardSpaces.Length; i++)
            {
                if (validSpace(i, teamColor))
                {
                    validList.Add(i);
                }
            }
            return validList.ToArray();
        }

        private void calculateTotals()
        {
            team1Total = 0;
            team2Total = 0;
            foreach (TileSpace tile in boardSpaces)
            {
                if (tile.PieceColor.Equals(Game.TEAM_1_COLOR)) team1Total++;
                if (tile.PieceColor.Equals(Game.TEAM_2_COLOR)) team2Total++;
            }
        }

        public void turnOverUpdate()
        {
            int index = waitTurningIndexes.Last();
            Color turningColor = boardSpaces[index].PieceColor.Equals(Game.TEAM_1_COLOR) ? Game.TEAM_2_COLOR : Game.TEAM_1_COLOR;
            boardSpaces[index].PieceColor = turningColor;
            waitTurningIndexes.RemoveAt(waitTurningIndexes.Count - 1);
            if (waitTurningIndexes.Count <= 0)
            {
                _waitTurning = false;
                activeColor = activeColor.Equals(Game.TEAM_1_COLOR) ? Game.TEAM_2_COLOR : Game.TEAM_1_COLOR;
                availableMovesIndex = validMoves(activeColor);
            }
            calculateTotals();
        }

        


    }
}
