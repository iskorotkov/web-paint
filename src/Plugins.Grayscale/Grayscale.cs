using System.Drawing;
using Plugins.Base;

namespace Plugins.Grayscale
{
    [Version(1, 0)]
    public class Grayscale : IPlugin
    {
        public string Name => "To grayscale";
        public string Author => "Korotkov Ivan";

        public void Transform(Bitmap image)
        {
            for (var x = 0; x < image.Width; x++)
            {
                for (var y = 0; y < image.Height; y++)
                {
                    var color = image.GetPixel(x, y);
                    var sum = color.R + color.G + color.B;
                    var value = sum / 3;
                    var newColor = Color.FromArgb(color.A, value, value, value);
                    image.SetPixel(x, y, newColor);
                }
            }
        }
    }
}
