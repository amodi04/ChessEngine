namespace Engine
{
    public abstract class Piece
    {
        protected int PiecePosition { get; }
        protected Coalition PieceCoalition { get; }

        private Piece(int piecePosition, Coalition pieceCoalition)
        {
            PiecePosition = piecePosition;
            PieceCoalition = pieceCoalition;
        }
        
        // TODO: Add move generation
    }
}