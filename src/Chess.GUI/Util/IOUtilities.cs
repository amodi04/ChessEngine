using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Engine.MoveGeneration;
using Engine.MoveGeneration.Types;
using Engine.Pieces;
using Engine.Player;

namespace Chess.GUI.Util;

/// <summary>
///     GUI utility class
/// </summary>
public static class IOUtilities
{
    // Static reference to the asset loader for loading assets
    private static readonly IAssetLoader AssetLoader = AvaloniaLocator.Current.GetService<IAssetLoader>();

    public static Image GenerateImage(Piece? piece)
    {
        return new Image
        {
            // Image format: "{Coalition}{Piece}.png"
            // Example: WB.png => White Bishop
            // Example: BK => Black King
            Source = new Bitmap(AssetLoader.Open(new Uri(
                $"avares://Chess.GUI/Assets/{piece?.PieceCoalition.ToAbbreviation()}" +
                $"{piece?.Type.ToAbbreviation(Coalition.White)}.png")))
        };
    }

    /// <summary>
    ///     Gets the audio stream of the sound to play.
    /// </summary>
    /// <param name="move">The move to play the sound for.</param>
    /// <returns>A .NET audio stream.</returns>
    public static Stream GetSoundStream(IMove move)
    {
        var sound = move switch
        {
            CaptureMove => "capture.mp3",
            CastlingMove => "castle.mp3",
            _ => "move-self.mp3"
        };

        return AssetLoader.Open(new Uri($"avares://Chess.GUI/Assets/{sound}"));
    }

    /// <summary>
    ///     Gets the opening book of moves to use for the AIs first moves.
    /// </summary>
    /// <returns>A file stream containing opening move data.</returns>
    public static Stream GetOpeningBook()
    {
        return AssetLoader.Open(new Uri("avares://Chess.GUI/Assets/Games.txt"));
    }
}