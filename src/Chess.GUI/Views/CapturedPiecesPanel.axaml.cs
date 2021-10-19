using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Chess.GUI.Util;
using Engine.MoveGeneration;
using Engine.MoveGeneration.Types;
using Engine.Pieces;
using Engine.Player;

namespace Chess.GUI.Views
{
    /// <summary>
    ///     Captured Pieces Panel class.
    /// </summary>
    public class CapturedPiecesPanel : UserControl
    {
        private readonly UniformGrid _bottomGrid;
        private readonly UniformGrid _topGrid;
        
        public CapturedPiecesPanel()
        {
            InitializeComponent();
            _topGrid = this.Find<UniformGrid>("TopGrid");
            _bottomGrid = this.Find<UniformGrid>("BottomGrid");
        }
        
        /// <summary>
        ///     Initialises GUI components
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        ///     Draws the panels with an updated move stack.
        /// </summary>
        /// <param name="moveStack">The current move stack (move history).</param>
        public void DrawPanels(Stack<IMove> moveStack)
        {
            _topGrid.Children.Clear();
            _bottomGrid.Children.Clear();

            // Calculate all captured pieces so far for white and black pieces
            ComputeCapturedPieces(true, moveStack);
            ComputeCapturedPieces(false, moveStack);
        }

        /// <summary>
        ///     Generates the captured pieces so far.
        /// </summary>
        /// <param name="isWhite">Used to decide which pieces to calculate for.</param>
        /// <param name="moveStack">The current move history.</param>
        private void ComputeCapturedPieces(bool isWhite, IEnumerable<IMove> moveStack)
        {
            var capturedPieces = PopulateCapturedPiecesList(moveStack, isWhite);

            // Sort the list based on piece value. This allows pieces to be placed in ascending order
            capturedPieces.Sort((piece, piece1) => piece.Type.CompareTo(piece1.Type));

            // Assign the piece images in the grid panel
            AssignImages(capturedPieces, isWhite ? _topGrid : _bottomGrid);
        }

        /// <summary>
        ///     Populates a list of pieces.
        /// </summary>
        /// <param name="moveStack">The current move history.</param>
        /// <param name="isWhite">Which coalition to generate for.</param>
        /// <returns>A list of inactive(captured) pieces.</returns>
        private static List<Piece> PopulateCapturedPiecesList(IEnumerable<IMove> moveStack, bool isWhite)
        {
            var capturedPieces = new List<Piece>();
            
            foreach (IMove move in moveStack)
                if (move is CaptureMove captureMove)
                {
                    Piece capturedPiece = captureMove.CapturedPiece;

                    switch (isWhite)
                    {
                        case true when capturedPiece.PieceCoalition.IsWhite():
                            capturedPieces.Add(capturedPiece);
                            break;
                        case false when !capturedPiece.PieceCoalition.IsWhite():
                            capturedPieces.Add(capturedPiece);
                            break;
                    }
                }
            
            return capturedPieces;
        }

        /// <summary>
        ///     Assigns the images in the grid
        /// </summary>
        /// <param name="capturedPieces">The list of captured pieces</param>
        /// <param name="grid">The grid to populate</param>
        private static void AssignImages(IEnumerable<Piece> capturedPieces, IPanel grid)
        {
            foreach (var capturedPiece in capturedPieces)
            {
                Image image = IOUtilities.GenerateImage(capturedPiece);
                grid.Children.Add(image);
            }
        }
    }
}