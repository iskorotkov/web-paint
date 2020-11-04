using System;
using System.Drawing;
using Plugins.Base;

namespace Plugins.Randomize
{
    [Version(1, 0)]
    public class Randomize : IPlugin
    {
        public string Name => "Add noise";
        public string Author => "Korotkov Ivan";

        private const int From = -64;
        private const int To = 64;

        public void Transform(Bitmap image)
        {
            var random = new Random(42);
            for (var x = 0; x < image.Width; x++)
            {
                for (var y = 0; y < image.Height; y++)
                {
                    var value = random.Next(From, To + 1);
                    var c = image.GetPixel(x, y);
                    c = Color.FromArgb(
                        c.A,
                        Math.Clamp(c.R + value, 0, 255),
                        Math.Clamp(c.G + value, 0, 255),
                        Math.Clamp(c.B + value, 0, 255));
                    image.SetPixel(x, y, c);
                }
            }
        }
    }
}
