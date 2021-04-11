using System;
using System.Drawing;
using System.Collections.Generic;
using DoubleZ.BoardGames;
using DoubleZ.Extensions;

namespace Chess
{
    public abstract class Piece : IPlaceable<Piece>
    {
        [Flags]
        public enum MeleeMoveType { MoveOnEnemies = 1, MoveIfVoid = 2 }
        [Flags]
        public enum RangedMoveType { HorizontallyAndVertically = 1, Diagonally = 2 }

        protected List<Point> possibleMoves = new List<Point>();
        public List<Point> PossibleMoves
        {
            get
            {
                possibleMoves = new List<Point>();
                CheckMoves();
                return possibleMoves;
            }
        }
        public Cell<Piece> Cell { get; set; }
        public Player Player { get; }
        public ChessPiece ChessPiece { get; set; }

        public Piece(ChessPiece chessPiece, Player player)
        {
            ChessPiece = chessPiece;
            Player = player;
        }

        /// <summary>
        /// Checks every possible move of the piece and adds it to the possible moves.
        /// </summary>
        protected abstract void CheckMoves();

        /// <summary>
        /// Gets a relative movement of a piece basing on its team.
        /// </summary>
        public Point GetMovement(int xOffset, int yOffset)
        {
            if (Player == Player.Black)
            {
                xOffset = -xOffset;
                yOffset = -yOffset;
            }
            return new Point(Cell.Position.X + xOffset, Cell.Position.Y + yOffset);
        }

        /// <summary>
        /// Checks every possible melee move.
        /// </summary>
        protected bool CheckMeleeMove(Point move, MeleeMoveType meleeMoveType)
        {
            bool isValid = Cell.GameBoard.IsOnBoard(move) && meleeMoveType switch
            {
                MeleeMoveType.MoveIfVoid => Cell.GameBoard.Board.FromPoint(move).Piece == null,
                MeleeMoveType.MoveOnEnemies => Cell.GameBoard.Board.FromPoint(move).Piece != null && Cell.GameBoard.Board.FromPoint(move).Piece.Player != Player,
                MeleeMoveType.MoveIfVoid | MeleeMoveType.MoveOnEnemies => Cell.GameBoard.Board.FromPoint(move).Piece == null || Cell.GameBoard.Board.FromPoint(move).Piece.Player != Player,
                _ => throw new Exception(),
            };
            if (isValid)
            {
                possibleMoves.Add(move);
                return true;
            }
            return false;
        }

        protected void CheckRangedMove(RangedMoveType rangedMoveType)
        {
            switch (rangedMoveType)
            {
                case RangedMoveType.HorizontallyAndVertically:
                    CheckHorizontallyAndVertically();
                    break;

                case RangedMoveType.Diagonally:
                    CheckDiagonally();
                    break;

                case RangedMoveType.HorizontallyAndVertically | RangedMoveType.Diagonally:
                    CheckHorizontallyAndVertically();
                    CheckDiagonally();
                    break;

                default: throw new Exception();
            };
        }

        private void CheckDiagonally()
        {
            CheckRangedMove(1, 1);
            CheckRangedMove(-1, -1);
            CheckRangedMove(-1, 1);
            CheckRangedMove(1, -1);
        }
        private void CheckHorizontallyAndVertically()
        {
            CheckRangedMove(1, 0);
            CheckRangedMove(0, 1);
            CheckRangedMove(-1, 0);
            CheckRangedMove(0, -1);
        }

        /// <summary>
        /// Checks if a move is valid basing of X and Y offset.
        /// </summary>
        private void CheckRangedMove(int deltaX, int deltaY)
        {
            int x = Cell.Position.X + deltaX;
            int y = Cell.Position.Y + deltaY;

            for (; Cell.GameBoard.IsOnBoard(new Point(x, y)); x += deltaX, y += deltaY)
            {
                Point p = new Point(x, y);
                if (Cell.GameBoard.Board.FromPoint(p).Piece == null)
                    possibleMoves.Add(p);
                else if (Cell.GameBoard.Board.FromPoint(p).Piece.Player != Player)
                {
                    possibleMoves.Add(p);
                    return;
                }
                else return;
            }
        }
    }
}