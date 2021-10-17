using System.Collections.ObjectModel;
using System.Linq;
using Chess.GUI.Models;
using Engine.BoardRepresentation;
using Engine.MoveGeneration;
using Engine.Player;
using static Engine.IO.PgnParser;

namespace Chess.GUI.ViewModels
{
    /// <summary>
    ///     ViewModel for the MoveLog.
    /// </summary>
    public class MoveLogViewModel : ViewModelBase
    {
        public MoveLogViewModel()
        {
            Moves = new ObservableCollection<MoveModel>();
        }
        
        public ObservableCollection<MoveModel> Moves { get; }

        /// <summary>
        ///     Updates the ObservableCollection of MoveModels.
        /// </summary>
        /// <param name="move">The move to add.</param>
        /// <param name="boardTransition">The board transition that the move applies to.</param>
        public void UpdateMoveLog(IMove move, BoardTransition boardTransition)
        {
            // No moves in collection
            if (!Moves.Any())
            {
                if (boardTransition.FromBoard.CurrentPlayer.Coalition.IsWhite())
                    Moves.Add(new MoveModel
                    {
                        WhiteMove = ToSan(move, boardTransition),
                        BlackMove = null
                    });
                // Else if the current player is black (happens when game is started from fen with black to move)
                else
                    Moves.Add(new MoveModel
                    {
                        WhiteMove = "-",
                        BlackMove = ToSan(move, boardTransition)
                    });
            }
            // Move complete (create new move)
            else if (Moves[^1].BlackMove is not null)
            {
                Moves.Add(new MoveModel
                {
                    WhiteMove = ToSan(move, boardTransition),
                    BlackMove = null
                });
            }
            // Last option is that there is a white move in the last move model but no black move
            // (hence both ply's are not complete)
            // Replace move at index with complete one
            else
            {
                var whiteMove = Moves[^1].WhiteMove;
                Moves[^1] = new MoveModel
                {
                    WhiteMove = whiteMove,
                    BlackMove = ToSan(move, boardTransition)
                };
            }
        }
    }
}