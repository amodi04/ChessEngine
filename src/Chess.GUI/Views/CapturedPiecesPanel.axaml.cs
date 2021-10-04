using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Chess.GUI.Util;
using Engine.MoveGeneration;
using Engine.Pieces;
using Engine.Player;

namespace Chess.GUI.Views
{
    /// <summary>
    /// Captured Pieces Panel class.
    /// </summary>
    public class CapturedPiecesPanel : UserControl
    {
        // Member fields
        private readonly UniformGrid _topGrid;
        private readonly UniformGrid _bottomGrid;
        
        /// <summary>
        /// User control constructor initialises components and values.
        /// </summary>
        public CapturedPiecesPanel()
        {
            InitializeComponent();
            _topGrid = this.Find<UniformGrid>("TopGrid");
            _bottomGrid = this.Find<UniformGrid>("BottomGrid");
        }

        /// <summary>
        /// Load the xaml file.
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        /// <summary>
        /// Draws the panels with an updated move stack.
        /// </summary>
        /// <param name="moveStack">The current move stack (move history).</param>
        public void DrawPanels(IEnumerable<IMove> moveStack)
        {
            // Clear elements
            _topGrid.Children.Clear();
            _bottomGrid.Children.Clear();

            // Calculate all captured pieces so far for white and black pieces
            ComputeCapturedPieces(true, moveStack);
            ComputeCapturedPieces(false, moveStack);
        }

        /// <summary>
        /// Generates the captured pieces so far.
        /// </summary>
        /// <param name="isWhite">Used to decide which pieces to calculate for.</param>
        /// <param name="moveStack">The current move history.</param>
        private void ComputeCapturedPieces(bool isWhite, IEnumerable<IMove> moveStack)
        {
            // Generate the list of captured pieces
            var capturedPieces = PopulateCapturedPiecesList(moveStack, isWhite);
            
            // Sort the list based on piece value. This allows same value pieces to be placed together in ascending order
            capturedPieces.Sort((piece, piece1) => piece.PieceType.CompareTo(piece1.PieceType));
            
            // Assign the piece images in the grid panel
            AssignImages(capturedPieces, isWhite ? _topGrid : _bottomGrid);
        }

        /// <summary>
        /// Populates a list of pieces.
        /// </summary>
        /// <param name="moveStack">The current move history.</param>
        /// <param name="isWhite">Which coalition to generate for.</param>
        /// <returns>A list of inactive(captured) pieces.</returns>
        private static List<Piece> PopulateCapturedPiecesList(IEnumerable<IMove> moveStack, bool isWhite)
        {
            var capturedPieces = new List<Piece>();
            
            // Loop through all moves played so far
            foreach (IMove move in moveStack)
            {
                // If the move is a capture move
                if (move is CaptureMove captureMove)
                {
                    Piece capturedPiece = captureMove.CapturedPiece;
                    
                    switch (isWhite)
                    {
                        // If we are generating for white and the piece is white, add it to the list
                        case true when capturedPiece.PieceCoalition.IsWhite():
                            capturedPieces.Add(capturedPiece);
                            break;
                        // If we are generating for black and the piece is black, add it to the list
                        case false when !capturedPiece.PieceCoalition.IsWhite():
                            capturedPieces.Add(capturedPiece);
                            break;
                    }
                }
            }

            // Return the populated list
            return capturedPieces;
        }

        /// <summary>
        /// Assigns the images in the grid
        /// </summary>
        /// <param name="capturedPieces">The list of captured pieces</param>
        /// <param name="grid">The grid to populate</param>
        private void AssignImages(IList<Piece> capturedPieces, UniformGrid grid)
        {
            // Loop through each captured piece in list
            foreach (var capturedPiece in capturedPieces)
            {
                // Get the corresponding image
                Image image = GUIUtilities.GenerateImage(capturedPiece);
                
                // Add the image to the panel
                grid.Children.Add(image);
            }
        }
    }
}