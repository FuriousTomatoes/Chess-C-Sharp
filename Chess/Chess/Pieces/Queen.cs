namespace Chess.Pieces
{
    class Queen : Piece
    {
        public Queen(Player player) : base(ChessPiece.Queen, player) { }
        protected override void CheckMoves()
        => CheckRangedMove(RangedMoveType.Diagonally | RangedMoveType.HorizontallyAndVertically);
    }
}