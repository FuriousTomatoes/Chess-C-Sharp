using System;
using DoubleZ.BoardGames;
using Chess.Pieces;
using System.Drawing;
using DoubleZ.Extensions;
using System.Collections.Generic;

namespace Chess
{
    public enum ChessPiece { Pawn, Queen, King, Knight, Bishop, Rook }
    public enum Player { White, Black }

    /// <summary>
    /// Represents an invalid move error.
    /// </summary>
    public class InvalidChessMoveException : Exception
    {
        public InvalidChessMoveException() : base() { }
        public InvalidChessMoveException(string message) : base(message) { }
    }

    public class Chess
    {
        public readonly List<Piece> WhitePieces = new List<Piece>();
        public readonly List<Piece> BlackPieces = new List<Piece>();
        private readonly King whiteKing;
        private readonly King blackKing;
        private Piece OnEnPassant { get; set;}

        public Player Turn { get; private set; } = Player.White;
        public GameBoard<Piece> GameBoard = new GameBoard<Piece>(8, 8);

        public Chess()
        {
            for (int x = 0; x < 8; x++)
            {
                GameBoard.Add(new Pawn(Player.White), new Point(x, 1));
                GameBoard.Add(new Pawn(Player.Black), new Point(x, 6));
            }

            whiteKing = new King(Player.White);
            blackKing = new King(Player.Black);

            GameBoard.Add(new Rook(Player.White), new Point(0, 0));
            GameBoard.Add(new Knight(Player.White), new Point(1, 0));
            GameBoard.Add(new Bishop(Player.White), new Point(2, 0));
            GameBoard.Add(new Queen(Player.White), new Point(3, 0));
            GameBoard.Add(whiteKing, new Point(4, 0));
            GameBoard.Add(new Bishop(Player.White), new Point(5, 0));
            GameBoard.Add(new Knight(Player.White), new Point(6, 0));
            GameBoard.Add(new Rook(Player.White), new Point(7, 0));

            GameBoard.Add(new Rook(Player.Black), new Point(0, 7));
            GameBoard.Add(new Knight(Player.Black), new Point(1, 7));
            GameBoard.Add(new Bishop(Player.Black), new Point(2, 7));
            GameBoard.Add(new Queen(Player.Black), new Point(3, 7));
            GameBoard.Add(blackKing, new Point(4, 7));
            GameBoard.Add(new Bishop(Player.Black), new Point(5, 7));
            GameBoard.Add(new Knight(Player.Black), new Point(6, 7));
            GameBoard.Add(new Rook(Player.Black), new Point(7, 7));

            for (int x = 0; x < 7; x++)
            {
                WhitePieces.Add(GameBoard.Board[x, 0].Piece);
                WhitePieces.Add(GameBoard.Board[x, 1].Piece);
                BlackPieces.Add(GameBoard.Board[x, 6].Piece);
                BlackPieces.Add(GameBoard.Board[x, 7].Piece);
            }
        }

        public void Move(Point start, Point finish)
        {
            if (CanMove(start, finish))
            {
                MakeMovement(start, finish);
                Turn = Turn == Player.White ? Player.Black : Player.White;
            }
        }

        private bool CanMove(Point start, Point finish)
        {
            //If the player can command the piece, if the piece can move there and if it doesn't cause a check.
            if (GameBoard.Board.FromPoint(start).Piece != null && GameBoard.Board.FromPoint(start).Piece.Player == Turn && GameBoard.Board.FromPoint(start).Piece.PossibleMoves.Contains(finish))
            {
                //Starts a "simulation" and controls if the player king is on check.
                return !SimulateMoveIsOnCheck(Turn, start, finish);
            }
            return false;
        }

        public bool IsOnCheck(Player player)
        {
            List<Piece> list = player == Player.White ? BlackPieces : WhitePieces;
            Piece king = player == Player.White ? whiteKing : blackKing;

            foreach (Piece piece in list)
                if (piece.PossibleMoves.Contains(king.Cell.Position))
                    return true;

            return false;
        }

        public bool IsOnCheckmate(Player player)
        {
            if (!IsOnCheck(player)) return false;

            List<Piece> list = player == Player.White ? WhitePieces : BlackPieces;
            Piece king = player == Player.White ? whiteKing : blackKing;

            //Checks only king moves.
            foreach (Point p in king.PossibleMoves)
            {
                if (SimulateMoveIsOnCheck(player, king.Cell.Position, p))
                    return false;
            }

            //Checks every possible move.
            foreach (Piece piece in list)
                foreach (Point p in piece.PossibleMoves)
                {
                    if (SimulateMoveIsOnCheck(player, piece.Cell.Position, p))
                        return false;
                }

            return true;
        }

        private bool SimulateMoveIsOnCheck(Player player, Point start, Point finish)
        => GameBoard.Simulate(start, finish, IsOnCheck, player);

        private void MakeMovement(Point start, Point finish)
        {
            //Controls en passant
            if (GameBoard.Board.FromPoint(start).Piece.ChessPiece == ChessPiece.Pawn && finish == GameBoard.Board.FromPoint(start).Piece.GetMovement(0, 2))
                OnEnPassant = GameBoard.Board.FromPoint(start).Piece;

            //In case of en passant, remove the eated piece.
            if (OnEnPassant != null && GameBoard.Board.FromPoint(start).Piece.ChessPiece == ChessPiece.Pawn && finish == OnEnPassant.GetMovement(0, -1))
                GameBoard.RemoveAt(OnEnPassant.Cell.Position);
            
            //Removes the eaten piece from the list.
            else if (GameBoard.Board.FromPoint(finish).Piece != null)
                (GameBoard.Board.FromPoint(finish).Piece.Player == Player.White ? WhitePieces : BlackPieces).Remove(GameBoard.Board.FromPoint(finish).Piece);

            GameBoard.Move(start, finish);
        }
    }
}