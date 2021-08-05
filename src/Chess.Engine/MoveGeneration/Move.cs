using Engine.BoardRepresentation;
using Engine.Enums;
using Engine.Pieces;

namespace Engine.MoveGeneration
{
    /// <summary>
    ///     This struct stores Move Data.
    /// </summary>
    public struct Move
    {
        /// <summary>
        ///     Move Data
        /// </summary>
        public readonly MoveType Type;

        public readonly Board Board;
        public readonly int FromCoordinate;
        public readonly int ToCoordinate;
        public readonly Piece MovedPiece;
        public readonly Piece CapturedPiece;

        private int _hashCode;

        /// <summary>
        ///     Constructor creates a non capture move.
        /// </summary>
        /// <param name="type">This will be confined to non-capturing states.</param>
        /// <param name="board">The current board that the move is to be executed on.</param>
        /// <param name="fromCoordinate">The initial board position that the piece is moving from.</param>
        /// <param name="toCoordinate">The destination board position that the piece is moving to.</param>
        /// <param name="movedPiece">The piece to be moved.</param>
        public Move(MoveType type, Board board, int fromCoordinate, int toCoordinate, Piece movedPiece)
        {
            Type = type;
            Board = board;
            FromCoordinate = fromCoordinate;
            ToCoordinate = toCoordinate;
            MovedPiece = movedPiece;
            CapturedPiece = null;
            _hashCode = 0;
        }

        /// <summary>
        ///     Constructor creates a capture move.
        /// </summary>
        /// <param name="type">This will be confined to capturing states.</param>
        /// <param name="board">The current board that the move is to be executed on.</param>
        /// <param name="fromCoordinate">The initial board position that the piece is moving from.</param>
        /// <param name="toCoordinate">The destination board position that the piece is moving to.</param>
        /// <param name="movedPiece">The piece to be moved.</param>
        /// <param name="capturedPiece">The piece at the destination coordinate being captured.</param>
        public Move(MoveType type, Board board, int fromCoordinate, int toCoordinate, Piece movedPiece,
            Piece capturedPiece)
        {
            Type = type;
            Board = board;
            FromCoordinate = fromCoordinate;
            ToCoordinate = toCoordinate;
            MovedPiece = movedPiece;
            CapturedPiece = capturedPiece;
            _hashCode = 0;
        }

        /// <summary>
        /// Checks if the current object is equal to the object passed in.
        /// </summary>
        /// <param name="obj">The object to compare against.</param>
        /// <returns>True if equal, false if not.</returns>
        public override bool Equals(object? obj)
        {
            // If the other move is not of type Move, return false
            if (obj is not Move move)
            {
                return false; 
            }

            // Return true if member field hash codes are equal
            // Captured piece not included because it could be null
            return Type == move.Type && Board == move.Board && FromCoordinate == move.FromCoordinate &&
                   ToCoordinate == move.ToCoordinate && MovedPiece == move.MovedPiece;
        }
        
        /// <summary>
        /// Gets the hashcode of this object.
        /// </summary>
        /// <returns>An integer hashcode of the object.</returns>
        public override int GetHashCode()
        {
            // Overflow is fine, just wrap around
            unchecked
            {
                var hash = _hashCode;
                
                // If hash == 0, the hash code hasn't been computed yet, so compute it
                if (hash != 0) return _hashCode;
                
                // Initialise hash to prime number. Here it is 17
                hash = 17;
                
                // Multiply each previous has by 31 and add the member fields.
                // 31 is used because it is odd and prime thus reducing collisions
                // Multiplying by 31 is very fast as it is just a shift and then a subtraction of 1
                // This algorithm is an implementation of Joshua Bloch's Effective Java hash code algorithm
                hash = 31 * hash + Type.GetHashCode();
                hash = 31 * hash + Board.GetHashCode();
                hash = 31 * hash + FromCoordinate.GetHashCode();
                hash = 31 * hash + ToCoordinate.GetHashCode();
                hash = 31 * hash + MovedPiece.GetHashCode();
                // Captured piece not included because it could be null
                
                // Store the computed value in the field
                _hashCode = hash;

                // Return the hash code field.
                return _hashCode;
            }
        }

        /// <summary>
        ///     Creates a new board with the moved piece.
        /// </summary>
        /// <returns>A new board with the piece moved.</returns>
        public Board ExecuteMove()
        {
            var boardBuilder = new BoardBuilder();

            // Set all pieces except the moved piece for the current player
            foreach (var piece in Board.CurrentPlayer.GetActiveAlliedPieces())
                if (!MovedPiece.Equals(piece))
                    boardBuilder.SetPieceAtTile(piece);

            // Set all the pieces for the opponent player
            foreach (var piece in Board.CurrentPlayer.GetOpponent().GetActiveAlliedPieces())
                boardBuilder.SetPieceAtTile(piece);

            // Move the moved piece
            boardBuilder.SetPieceAtTile(MovedPiece.MovePiece(this));

            // Set next player to move
            boardBuilder.SetCoalitionToMove(Board.CurrentPlayer.GetOpponent().Coalition);

            // Build the board
            return boardBuilder.BuildBoard();
        }
    }
}