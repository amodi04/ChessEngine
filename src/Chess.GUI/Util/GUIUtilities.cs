using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Engine.Pieces;
using Engine.Player;

namespace Chess.GUI.Util
{
    /// <summary>
    ///     GUI utility class
    /// </summary>
    public static class GUIUtilities
    {
        // Static reference to the asset loader for loading images
        private static readonly IAssetLoader ImageLoader = AvaloniaLocator.Current.GetService<IAssetLoader>();

        public static Image GenerateImage(Piece piece)
        {
            return new()
            {
                // Image format: "{Coalition}{Piece}.png"
                // Example: WB.png => White Bishop
                // Example: BK => Black King
                Source = new Bitmap(ImageLoader.Open(new Uri(
                    $"avares://Chess.GUI/Assets/{piece.PieceCoalition.ToAbbreviation()}{piece.Type.ToAbbreviation(Coalition.White)}.png")))
            };
        }
    }
}