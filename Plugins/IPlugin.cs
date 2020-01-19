using System;
using System.Collections.Generic;
using System.Text;

namespace MaeveFramework.Plugins
{
    public interface IPlugin
    {
        string Name { get; set; }
        string Author { get; set; }
        Version Version { get; set; }

        void Dispose();
    }
}
