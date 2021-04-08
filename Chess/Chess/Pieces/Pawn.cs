namespace Chess.Pieces
{
    public class Pawn : Piece
    {
        public Pawn( Player player) : base( ChessPiece.Pawn, player) { }
        protected override void CheckMoves()
        {
            //Move the pawn ahead.
            CheckMeleeMove(GetMovement(0, 1), MeleeMoveType.MoveIfVoid);

            //Move the pawn ahead twice.
            //if()

            //Attack to right.
            CheckMeleeMove(GetMovement(1, 1), MeleeMoveType.MoveOnEnemies);

            //Attack to left.
            CheckMeleeMove(GetMovement(-1, 1), MeleeMoveType.MoveOnEnemies);
        }
    }
}