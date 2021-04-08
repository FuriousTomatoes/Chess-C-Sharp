using System;
namespace Chess.Pieces
{
    class Knight : Piece
    {
        public Knight(Player player) : base(ChessPiece.Knight, player) { }
        protected override void CheckMoves()
        {
            for (int x = -2; x <= 2; x++)
                for (int y = -2; y <= 2; y++)
                {
                    if (x == 0 || y == 0 ||  Math.Abs(x) == Math.Abs(y)) continue;
                    CheckMeleeMove(GetMovement(x, y), MeleeMoveType.MoveIfVoid | MeleeMoveType.MoveOnEnemies);
                }
        }
    }
}