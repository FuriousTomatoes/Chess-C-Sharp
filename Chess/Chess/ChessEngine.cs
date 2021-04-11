using System;
using DoubleZ.BoardGames;
using Chess.Pieces;
using System.Drawing;
using DoubleZ.Extensions;
using System.Collections.Generic;

namespace Chess
{
    public enum ChessPiece : byte { Pawn, Queen, King, Knight, Bishop, Rook }
    public enum Player : byte { White, Black }
    [Flags]
    public enum Castling { LongWhite = 1, LongBlack = 2, ShortWhite = 4, ShortBlack = 8 }

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
        public List<Piece> whitePieces = new List<Piece>();
        public List<Piece> blackPieces = new List<Piece>();
        private readonly King whiteKing;
        private readonly King blackKing;

        public Player Turn { get; private set; } = Player.White;
        public Castling castling = Castling.LongBlack | Castling.LongWhite | Castling.ShortBlack | Castling.ShortWhite;
        public GameBoard<Piece> GameBoard = new GameBoard<Piece>(8, 8);
        private Piece OnEnPassant { get; set; }

        public Chess()
        {
            for (int x = 0; x < 8; x++)
            {
                GameBoard.Add(new Pawn(Player.White), new Point(x, 1));
                GameBoard.Add(new Pawn(Player.Black), new Point(x, 6));
            }

            whiteKing = new King(this, Player.White);
            blackKing = new King(this, Player.Black);

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

            for (int x = 0; x < 8; x++)
            {
                whitePieces.Add(GameBoard.Board[x, 0].Piece);
                whitePieces.Add(GameBoard.Board[x, 1].Piece);
                blackPieces.Add(GameBoard.Board[x, 6].Piece);
                blackPieces.Add(GameBoard.Board[x, 7].Piece);
            }
        }

        public void Move(Point start, Point finish)
        {
            if (CanMove(start, finish))
            {
                MakeMovement(start, finish);

                Piece mover = GameBoard.Board.FromPoint(start).Piece;

                //Manages castling.
                if (mover != null)
                    if (mover.ChessPiece == ChessPiece.Rook)
                    {
                        Castling longCastling = mover.Player == Player.White ? Castling.LongWhite : Castling.LongBlack;
                        Castling shortCastling = mover.Player == Player.White ? Castling.ShortWhite : Castling.ShortBlack;

                        if (start.X == 0) castling &= ~longCastling;
                        if (start.X == 7) castling &= ~shortCastling;
                    }
                    else if (mover.ChessPiece == ChessPiece.King)
                        castling &= mover.Player == Player.Black ? Castling.LongWhite | Castling.ShortWhite : Castling.LongBlack | Castling.ShortBlack;
            }
            else if (!Castle(Turn, start, finish))
                throw new InvalidChessMoveException();

            ChangeTurn();
        }

        private bool CanMove(Point start, Point finish)
        {
            Piece pieceToMove = GameBoard.Board.FromPoint(start).Piece;

            //If the player can command the piece, if the piece can move there and if it doesn't cause a check.
            return pieceToMove != null && pieceToMove.Player == Turn && pieceToMove.PossibleMoves.Contains(finish) && !SimulateMoveIsOnCheck(Turn, start, finish);
        }

        private bool SimulateMoveIsOnCheck(Player player, Point start, Point finish)
        {
            //Makes a backup of start piece
            Piece piece = GameBoard.Board.FromPoint(finish).Piece;
            //Makes the move
            GameBoard.Move(start, finish);

            bool simulationReturn = IsOnCheck(player);

            //Resets the move
            GameBoard.Move(finish, start);
            //Reverts changes
            GameBoard.Board.FromPoint(finish).Piece = piece;

            return simulationReturn;
        }

        public bool IsOnCheck(Player player)
        {
            List<Piece> attackers = player == Player.White ? blackPieces : whitePieces;
            Piece king = player == Player.White ? whiteKing : blackKing;

            foreach (Piece piece in attackers)
                if (piece.PossibleMoves.Contains(king.Cell.Position))
                    return true;

            return false;
        }

        public bool IsOnCheckmate(Player player)
        {
            if (!IsOnCheck(player)) return false;

            List<Piece> list = player == Player.White ? whitePieces : blackPieces;
            Piece king = player == Player.White ? whiteKing : blackKing;

            //Checks only king moves
            foreach (Point p in king.PossibleMoves)
                if (SimulateMoveIsOnCheck(player, king.Cell.Position, p))
                    return false;

            //Checks every possible move
            foreach (Piece piece in list)
                foreach (Point p in piece.PossibleMoves)
                    if (SimulateMoveIsOnCheck(player, piece.Cell.Position, p))
                        return false;

            return true;
        }

        private void MakeMovement(Point start, Point finish)
        {
            Piece ToMove = GameBoard.Board.FromPoint(start).Piece;
            Piece ToGoTo = GameBoard.Board.FromPoint(finish).Piece;

            //Checks en passant
            if (ToMove.ChessPiece == ChessPiece.Pawn && finish == ToMove.GetMovement(0, 2))
                OnEnPassant = ToMove;
            else OnEnPassant = null;

            //In case of en passant, remove the eaten piece.
            if (OnEnPassant != null && ToMove.ChessPiece == ChessPiece.Pawn && finish == OnEnPassant.GetMovement(0, -1))
                GameBoard.RemoveAt(OnEnPassant.Cell.Position);

            //Removes the eaten piece from the list.
            else if (ToGoTo != null)
            {
                List<Piece> list = ToGoTo.Player == Player.White ? whitePieces : blackPieces;
                list.Remove(ToGoTo);
            }

            GameBoard.Move(start, finish);
        }

        private bool Castle(Player turn, Point start, Point finish)
        {
            Piece ToStartFrom = GameBoard.Board.FromPoint(start).Piece;

            if (ToStartFrom != null && ToStartFrom.ChessPiece == ChessPiece.King)
            {
                King king = (King)ToStartFrom;

                Castling longCastling = turn == Player.White ? Castling.LongWhite : Castling.LongBlack;
                Castling shortCastling = turn == Player.White ? Castling.ShortWhite : Castling.ShortBlack;

                Castling possibleCastlings = king.PossibleCastlings();

                if (finish.X <= 2 && (possibleCastlings | shortCastling) == possibleCastlings)
                {
                    GameBoard.Move(start, new Point(1, start.Y));
                    GameBoard.Move(new Point(0, start.Y), new Point(2, start.Y));
                    return true;
                }
                else if (finish.X >= 6 && (possibleCastlings | longCastling) == possibleCastlings)
                {
                    GameBoard.Move(start, new Point(6, start.Y));
                    GameBoard.Move(new Point(7, start.Y), new Point(5, start.Y));
                    return true;
                }
            }
            return false;
        }
        private void ChangeTurn() => Turn = Turn == Player.White ? Player.Black : Player.White;
    }
}