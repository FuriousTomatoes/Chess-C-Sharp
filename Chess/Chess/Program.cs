using System;
using Chess.Pieces;
using System.Drawing;
using System.Collections.Generic;
using DoubleZ.Input;
using DoubleZ.Extensions;

namespace Chess
{
    class Program
    {
        public static Chess Chess = new Chess(OnPromotion);
        static void ChessboardDraw()
        {
            Console.WriteLine(" +---+---+---+---+---+---+---+---+");
            for (int y = 7; y >= 0; y--)
            {
                Console.Write($"{y + 1}|");
                for (int x = 0; x < 8; x++)
                    Console.Write(Chess.GameBoard.Board[x, y].Piece == null ? "   |" : Chess.GameBoard.Board[x, y].Piece.Player.ToString()[0] + Chess.GameBoard.Board[x, y].Piece.ChessPiece.ToString().Substring(0, 2) + "|");
                Console.WriteLine("\n +---+---+---+---+---+---+---+---+");
            }
            Console.WriteLine($"   a   b   c   d   e   f   g   h");
            if (Chess.IsOnCheck(Player.White)) Console.WriteLine("White on check!");
            if (Chess.IsOnCheck(Player.Black)) Console.WriteLine("Black on check!");
            if (Chess.IsOnCheckmate(Player.White)) Console.WriteLine("White on checkmate!");
            if (Chess.IsOnCheckmate(Player.White)) Console.WriteLine("Black on checkmate!");
        }

        static void ChessboardDraw(Point point)
        {
            List<Point> possibleMoves = Chess.PossibleMovesOnBoard(point);

            Console.WriteLine(" +---+---+---+---+---+---+---+---+");
            for (int y = 7; y >= 0; y--)
            {
                Console.Write($"{y + 1}|");
                for (int x = 0; x < 8; x++)
                    if (possibleMoves.Contains(new Point(x, y)))
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write(Chess.GameBoard.Board[x, y].Piece == null ? "   " : Chess.GameBoard.Board[x, y].Piece.Player.ToString()[0] + Chess.GameBoard.Board[x, y].Piece.ChessPiece.ToString().Substring(0, 2));
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("|");
                    }
                    else Console.Write(Chess.GameBoard.Board[x, y].Piece == null ? "   |" : Chess.GameBoard.Board[x, y].Piece.Player.ToString()[0] + Chess.GameBoard.Board[x, y].Piece.ChessPiece.ToString().Substring(0, 2) + "|");
                Console.WriteLine("\n +---+---+---+---+---+---+---+---+");
            }
            Console.WriteLine($"   a   b   c   d   e   f   g   h");
        }

        static void Main()
        {
            ChessboardDraw();
            while (Chess.Winner() == Player.None)
            {
                try
                {
                    Point start = Read.Coords(8, 8);
                    if (Chess.GameBoard.Board.FromPoint(start).Piece != null)
                    {
                        Console.Clear();
                        ChessboardDraw(start);
                    }
                    Chess.Move(start, Read.Coords(8, 8));
                    Console.Clear();
                    ChessboardDraw();
                }
                catch (Exception) { }
            }
        }
        private static ChessPiece OnPromotion()
        {
            return ChessPiece.Queen;
        }
    }
}