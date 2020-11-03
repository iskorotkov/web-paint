using System;

namespace Plugins.Base
{
    public class VersionAttribute : Attribute
    {
        public VersionAttribute(int major, int minor)
        {
            Major = major;
            Minor = minor;
        }

        public int Major { get; }
        public int Minor { get; }
    }
}
