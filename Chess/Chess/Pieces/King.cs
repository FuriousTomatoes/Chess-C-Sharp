namespace Chess.Pieces
{
    class King : Piece
    {
        public King(Player player) : base(ChessPiece.King, player) { }
        protected override void CheckMoves()
        {
            for (int x = -1; x <= 1; x++)
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;
                    CheckMeleeMove(GetMovement(x, y), MeleeMoveType.MoveIfVoid | MeleeMoveType.MoveOnEnemies);
                }  
        }
    }
}