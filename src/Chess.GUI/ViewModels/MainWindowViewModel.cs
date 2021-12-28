using System;
using System.IO;
using LibVLCSharp.Shared;

namespace Chess.GUI.ViewModels;

/// <summary>
///     Class for handling ui and data logic binding.
/// </summary>
public class MainWindowViewModel : ViewModelBase, IDisposable
{
    private readonly LibVLC _libVlc = new();

    public MainWindowViewModel()
    {
        MediaPlayer = new MediaPlayer(_libVlc);
    }

    public MediaPlayer MediaPlayer { get; }

    /// <summary>
    ///     IDisposable dispose implementation.
    /// </summary>
    public void Dispose()
    {
        MediaPlayer.Dispose();
        _libVlc.Dispose();
    }

    /// <summary>
    ///     Plays an audio stream.
    /// </summary>
    /// <param name="stream">The audio stream to play.</param>
    public void Play(Stream stream)
    {
        using var media = new Media(_libVlc, new StreamMediaInput(stream));
        MediaPlayer.Play(media);
    }
}