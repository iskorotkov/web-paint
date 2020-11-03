using System.Drawing;

namespace Plugins.Base
{
    public interface IPlugin
    {
        string Name { get; }
        string Author { get; }
        void Transform(Bitmap image);
    }
}
