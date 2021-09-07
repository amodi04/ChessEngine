using Avalonia;
using Avalonia.Platform;

namespace Chess.GUI.Util
{
    /// <summary>
    /// GUI utility class
    /// </summary>
    public static class GUIUtilities
    {
        // Static reference to the asset loader for loading images
        public static readonly IAssetLoader AssetLoader = AvaloniaLocator.Current.GetService<IAssetLoader>();
    }
}