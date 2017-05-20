using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioPipe.Services
{
    public interface IColorService
    {
        Color this[String colorName] { get; }
    }
}
