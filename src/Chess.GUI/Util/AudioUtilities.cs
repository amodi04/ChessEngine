using System;
using System.IO;
using Avalonia;
using Avalonia.Platform;
using Engine.MoveGeneration;
using Engine.MoveGeneration.Types;

namespace Chess.GUI.Util
{
    /// <summary>
    /// Utility class for handling audio.
    /// </summary>
    public static class AudioUtilities
    {
        // Static reference to the asset loader for audio images
        private static readonly IAssetLoader AudioLoader = AvaloniaLocator.Current.GetService<IAssetLoader>();
        
        /// <summary>
        /// Gets the audio stream of the sound to play.
        /// </summary>
        /// <param name="move">The move to play the sound for.</param>
        /// <returns>A .NET audio stream.</returns>
        public static Stream GetSoundStream(IMove move)
        {
            string sound = move switch
            {
                CaptureMove => "capture.mp3",
                CastlingMove => "castle.mp3",
                _ => "move-self.mp3"
            };
            
            // Get the sound path and open as an audio stream.
            return AudioLoader.Open(new Uri($"avares://Chess.GUI/Assets/{sound}"));
        }
    }
}