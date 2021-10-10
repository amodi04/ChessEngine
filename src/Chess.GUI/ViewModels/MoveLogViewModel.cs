using System.Collections.ObjectModel;
using System.Linq;
using Chess.GUI.Models;
using Engine.BoardRepresentation;
using Engine.MoveGeneration;
using Engine.Player;
using Engine.Util;

namespace Chess.GUI.ViewModels
{
    /// <summary>
    /// ViewModel for the MoveLog. 
    /// </summary>
    public class MoveLogViewModel : ViewModelBase
    {
        // Properties
        public ObservableCollection<MoveModel> Moves { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        public MoveLogViewModel()
        {
            // Create a new Observable collection of moves
            // This will notify and update GUI automatically when the collection is modified
            Moves = new ObservableCollection<MoveModel>();
        }
        
        /// <summary>
        /// Updates the ObservableCollection of MoveModels.
        /// </summary>
        /// <param name="move">The move to add.</param>
        /// <param name="boardTransition">The board transition that the move applies to.</param>
        public void UpdateMoveLog(IMove move, BoardTransition boardTransition)
        {
            // If there are no moves in the collection
            if (!Moves.Any())
            {
                // If the current player is white
                if (boardTransition.FromBoard.CurrentPlayer.Coalition.IsWhite())
                {
                    // Add a new move
                    Moves.Add(new MoveModel()
                    {
                        MoveNumber = "1",
                        // Add a white move because it is the first move
                        WhiteMove = MoveUtilities.ToSan(move, boardTransition),
                    
                        // Set the black move to null
                        BlackMove = null,
                    });
                }
                // Else if the current player is black (happens when game is started from fen sometimes)
                else
                {
                    // Add a new move
                    Moves.Add(new MoveModel()
                    {
                        MoveNumber = "1",
                        
                        // Add an empty white move
                        WhiteMove = "-",
                    
                        // Set the black move
                        BlackMove = MoveUtilities.ToSan(move, boardTransition),
                    });
                }
            }
            // If the last move in the collection does not have a null black move
            else if (Moves[^1].BlackMove is not null)
            {
                // Add a new move because the last move is complete
                Moves.Add(new MoveModel()
                {
                    MoveNumber = $"{Moves.Count + 1}",
                    // Add a white move
                    WhiteMove = MoveUtilities.ToSan(move, boardTransition),
                    
                    // Set the black move to null
                    BlackMove = null,
                });
            }
            // Last option is that there is a white move in the last move model but no black move
            // (hence both ply's are not complete)
            else
            {
                // Get the white move temporarily
                var whiteMove = Moves[^1].WhiteMove;
                
                // Replace the object at that index with a new move model
                Moves[^1] = new MoveModel()
                {
                    MoveNumber = $"{Moves.Count}",
                    // Store the white move
                    WhiteMove = whiteMove,
                    
                    // Set the black move
                    BlackMove = MoveUtilities.ToSan(move, boardTransition)
                };
            }
        }
    }
}