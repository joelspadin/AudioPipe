using CSCore.CoreAudioAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioPipe.ViewModels
{
    public interface IDeviceViewModel
    {
        MMDevice Device { get; set; }
        string DeviceName { get; }
        bool IsDefault { get; }

        void RefreshName();
    }
}
