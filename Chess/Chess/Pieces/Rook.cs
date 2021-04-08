namespace Chess.Pieces
{
    class Rook : Piece
    {
        public Rook(Player player) : base(ChessPiece.Rook, player) { }
        protected override void CheckMoves()
        => CheckRangedMove(RangedMoveType.HorizontallyAndVertically);
    }
}