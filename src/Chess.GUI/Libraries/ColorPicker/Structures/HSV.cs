using System;

namespace Chess.GUI.Libraries.ColorPicker.Structures;

public struct HSVColor
{
    /// <summary>
    ///     Hue (0.0 to 360.0)
    /// </summary>
    public float hue;

    /// <summary>
    ///     Saturation (0.0 to 1.0)
    /// </summary>
    public float sat;

    /// <summary>
    ///     Value (0.0 to 1.0)
    /// </summary>
    public float value;

    public HSVColor(float hue, float saturation, float value)
    {
        this.hue = hue;
        sat = saturation;
        this.value = value;
    }

    public void Clamp()
    {
        if (hue > 360.0f) hue = 360.0f;
        if (sat > 1.0f) sat = 1.0f;
        if (value > 1.0f) value = 1.0f;
        if (hue < 0.0f) hue = 0.0f;
        if (sat < 0.0f) sat = 0.0f;
        if (value < 0.0f) value = 0.0f;
    }

    /// <summary>
    ///     Convert this HSV color to RGB colorspace
    /// </summary>
    public RGBColor ToRGB()
    {
        var hue = this.hue;
        var sat = this.sat;
        var value = this.value;
        float r = 0;
        float g = 0;
        float b = 0;

        var hi = (int)Math.Floor(hue / 60) % 6;
        var f = hue / 60 - (float)Math.Floor(hue / 60);

        value = value * 255;
        var v = (int)value;
        var p = (int)(value * (1.0 - sat));
        var q = (int)(value * (1.0 - f * sat));
        var t = (int)(value * (1.0 - (1.0 - f) * sat));

        switch (hi)
        {
            case 0:
                r = v;
                g = t;
                b = p;
                break;
            case 1:
                r = q;
                g = v;
                b = p;
                break;
            case 2:
                r = p;
                g = v;
                b = t;
                break;
            case 3:
                r = p;
                g = q;
                b = v;
                break;
            case 4:
                r = t;
                g = p;
                b = v;
                break;
            case 5:
                r = v;
                g = p;
                b = q;
                break;
        }

        r /= 255.0f;
        g /= 255.0f;
        b /= 255.0f;

        return new RGBColor(r, g, b);
    }

    public static HSVColor FromRGB(RGBColor rgb)
    {
        var hsv = new HSVColor();

        var max = Math.Max(rgb.r, Math.Max(rgb.g, rgb.b));
        var min = Math.Min(rgb.r, Math.Min(rgb.g, rgb.b));

        hsv.value = max;

        var delta = max - min;

        if (max > float.Epsilon)
        {
            hsv.sat = delta / max;
        }
        else
        {
            // r = g = b = 0
            hsv.sat = 0;
            hsv.hue = float.NaN; // Undefined
            return hsv;
        }

        if (rgb.r == max)
            hsv.hue = (rgb.g - rgb.b) / delta; // Between yellow and magenta
        else if (rgb.g == max)
            hsv.hue = 2 + (rgb.b - rgb.r) / delta; // Between cyan and yellow
        else
            hsv.hue = 4 + (rgb.r - rgb.g) / delta; // Between magenta and cyan

        hsv.hue *= 60.0f; // degrees
        if (hsv.hue < 0)
            hsv.hue += 360;

        return hsv;
    }

    public static implicit operator RGBColor(HSVColor hsv)
    {
        return hsv.ToRGB();
    }

    public static implicit operator HSVColor(RGBColor rgb)
    {
        return FromRGB(rgb);
    }
}