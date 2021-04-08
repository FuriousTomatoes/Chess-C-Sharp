namespace Chess.Pieces
{
    class Bishop : Piece
    {
        public Bishop(Player player) : base(ChessPiece.Bishop, player) { }
        protected override void CheckMoves()
        => CheckRangedMove(RangedMoveType.Diagonally);
        
    }
}