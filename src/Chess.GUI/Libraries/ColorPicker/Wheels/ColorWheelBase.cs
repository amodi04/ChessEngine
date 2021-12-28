using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Chess.GUI.Libraries.ColorPicker.Structures;
using HA = Avalonia.Layout.HorizontalAlignment;
using VA = Avalonia.Layout.VerticalAlignment;


namespace Chess.GUI.Libraries.ColorPicker.Wheels;

public abstract class ColorWheelBase : Panel
{
    public static StyledProperty<double> InnerRadiusProperty =
        AvaloniaProperty.Register<HSVWheel, double>(nameof(InnerRadius));

    private Ellipse border;


    public double InnerRadius
    {
        get => GetValue(InnerRadiusProperty);
        set => SetValue(InnerRadiusProperty, value);
    }


    public double ActualOuterRadius { get; private set; }
    public double ActualInnerRadius => ActualOuterRadius * InnerRadius;

    public static void OnPropertyChanged(AvaloniaObject obj, AvaloniaPropertyChangedEventArgs args)
    {
        var ctl = obj as HSVWheel;
        ctl.InvalidateVisual();
    }


    public override void Render(DrawingContext dc)
    {
        base.Render(dc);
        DrawHsvDial(dc);
    }

    /// <summary>
    ///     The function used to draw the pixels in the color wheel.
    /// </summary>
    protected RGBStruct ColorFunction(double r, double theta)
    {
        var rgb = ColorMapping(r, theta, 1.0);
        return new RGBStruct(rgb.Rb, rgb.Gb, rgb.Bb);
    }

    /// <summary>
    ///     The color mapping between Rad/Theta and RGB
    /// </summary>
    /// <param name="r">Radius/Saturation, between 0 and 1</param>
    /// <param name="theta">Angle/Hue, between 0 and 360</param>
    /// <returns>The RGB colour</returns>
    public virtual RGBColor ColorMapping(double radius, double theta, double value)
    {
        return new RGBColor(1.0f, 1.0f, 1.0f);
    }

    public virtual Point InverseColorMapping(RGBColor rgb)
    {
        return new Point(0, 0);
    }

    protected void DrawHsvDial(DrawingContext drawingContext)
    {
        var cx = (float)Bounds.Width / 2.0f;
        var cy = (float)Bounds.Height / 2.0f;

        var outer_radius = Math.Min(cx, cy);
        ActualOuterRadius = outer_radius;

        var bmp_width = (int)Bounds.Width;
        var bmp_height = (int)Bounds.Height;

        if (bmp_width <= 0 || bmp_height <= 0)
            return;


        var stopwatch = new Stopwatch();
        stopwatch.Start();

        //This probably wants to move somewhere else....
        if (border == null)
        {
            border = new Ellipse();
            border.Fill = new SolidColorBrush(Colors.Transparent);
            border.Stroke = new SolidColorBrush(Colors.Black);
            border.StrokeThickness = 3;
            border.IsHitTestVisible = false;
            border.Opacity = 50;
            Children.Add(border);
            border.HorizontalAlignment = HA.Center;
            border.VerticalAlignment = VA.Center;
        }

        border.Width = Math.Min(bmp_width, bmp_height) + border.StrokeThickness / 2;
        border.Height = Math.Min(bmp_width, bmp_height) + border.StrokeThickness / 2;

        var writeableBitmap = new WriteableBitmap(new PixelSize(bmp_width, bmp_height), new Vector(96, 96),
            PixelFormat.Bgra8888);

        using (var lockedFrameBuffer = writeableBitmap.Lock())
        {
            unsafe
            {
                var bufferPtr = new IntPtr(lockedFrameBuffer.Address.ToInt64());

                for (var y = 0; y < bmp_height; y++)
                for (var x = 0; x < bmp_width; x++)
                {
                    var color_data = 0;

                    // Convert xy to normalized polar co-ordinates
                    double dx = x - cx;
                    double dy = y - cy;
                    var pr = Math.Sqrt(dx * dx + dy * dy);

                    // Only draw stuff within the circle
                    if (pr <= outer_radius)
                    {
                        // Compute the color for the given pixel using polar co-ordinates
                        var pa = Math.Atan2(dx, dy);
                        var c = ColorFunction(pr / outer_radius, (pa + Math.PI) * 180.0 / Math.PI);

                        // Anti-aliasing
                        // This works by adjusting the alpha to the alias error between the outer radius (which is integer) 
                        // and the computed radius, pr (which is float).
                        var aadelta = pr - (outer_radius - 1.0);
                        if (aadelta >= 0.0)
                            c.a = (byte)(255 - aadelta * 255);

                        color_data = c.ToARGB32();
                    }

                    *(int*)bufferPtr = color_data;
                    bufferPtr += 4;
                }
            }
        }

        drawingContext.DrawImage(writeableBitmap, Bounds);

        stopwatch.Stop();
        Debug.WriteLine($"YO! This puppy took {stopwatch.ElapsedMilliseconds} MS to complete");
    }
}