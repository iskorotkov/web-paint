using System.Drawing;
using Plugins.Base;

namespace Plugins.Inverse
{
    [Version(1, 0)]
    public class Inverse : IPlugin
    {
        public string Name => "Inverse image";
        public string Author => "Korotkov Ivan";

        public void Transform(Bitmap image)
        {
            for (var x = 0; x < image.Width; x++)
            {
                for (var y = 0; y < image.Height / 2; y++)
                {
                    var color = image.GetPixel(x, y);
                    image.SetPixel(x, y, image.GetPixel(x, image.Height - y - 1));
                    image.SetPixel(x, image.Height - y - 1, color);
                }
            }
        }
    }
}
