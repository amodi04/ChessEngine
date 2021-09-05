﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia;
using Avalonia.Platform;
using Engine.Util;

namespace Chess.GUI
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