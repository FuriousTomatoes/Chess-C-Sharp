namespace Chess.Pieces
{
    public class Pawn : Piece
    {
        public Pawn( Player player) : base( ChessPiece.Pawn, player) { }
        protected override void CheckMoves()
        {
            //Move the pawn ahead.
            if (CheckMeleeMove(GetMovement(0, 1), MeleeMoveType.MoveIfVoid) && Cell.Position.Y == 1 || Cell.Position.Y == 6)
                //Move the pawn ahead twice.
                CheckMeleeMove(GetMovement(0, 2), MeleeMoveType.MoveIfVoid);

            //Attack to right.
            CheckMeleeMove(GetMovement(1, 1), MeleeMoveType.MoveOnEnemies);

            //Attack to left.
            CheckMeleeMove(GetMovement(-1, 1), MeleeMoveType.MoveOnEnemies);
        }
    }
}