using System.Drawing;
using System.Collections.Generic;

namespace Chess.Pieces
{
    class King : Piece
    {
        private readonly Chess chess;
        public King(Chess chess, Player player) : base(ChessPiece.King, player) => this.chess = chess;

        protected override void CheckMoves()
        {
            for (int x = -1; x <= 1; x++)
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;
                    CheckMeleeMove(GetMovement(x, y), MeleeMoveType.MoveIfVoid | MeleeMoveType.MoveOnEnemies);
                }
        }

        public Castling PossibleCastlings()
        {
            Castling possibleCastlings = 0;

            Castling longCastling = Player == Player.White ? Castling.LongWhite : Castling.LongBlack;
            Castling shortCastling = Player == Player.White ? Castling.ShortWhite : Castling.ShortBlack;

            //If long castling is legal.
            if ((chess.castling | longCastling) == chess.castling)
                if (IsCastlingPossible(false)) possibleCastlings |= longCastling;

            //If short castling is possible
            if ((chess.castling | shortCastling) == chess.castling)
                if (IsCastlingPossible(true)) possibleCastlings |= shortCastling;

            return possibleCastlings;
        }

        private bool IsCastlingPossible(bool isShortCastling)
        {
            List<Piece> attackers = Player == Player.White ? chess.blackPieces : chess.whitePieces;

            for (int x = 4; x >= 1 && x <= 7; x += isShortCastling ? 1 : -1)
                if (Cell.GameBoard.Board[x, Cell.Position.Y].Piece == null) return false;

            foreach (Piece piece in attackers)
                foreach (Point p in piece.PossibleMoves)
                    if (p.Y == Cell.Position.Y && p.X >= 4)
                        return false;
            return true;
        }
    }
}