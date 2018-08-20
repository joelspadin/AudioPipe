using CSCore.CoreAudioAPI;
using System.ComponentModel;

namespace AudioPipe.ViewModels
{
    /// <summary>
    /// View model for an audio device.
    /// </summary>
    public interface IDeviceViewModel
    {
        /// <summary>
        /// Gets or sets the audio device this <see cref="DeviceViewModel"/> represents.
        /// </summary>
        MMDevice Device { get; set; }

        /// <summary>
        /// Gets the name of the device.
        /// </summary>
        string DeviceName { get; }

        /// <summary>
        /// Gets a value indicating whether this device is the default audio playback device.
        /// </summary>
        bool IsDefault { get; }

        /// <summary>
        /// Raises <see cref="INotifyPropertyChanged.PropertyChanged"/> for the <see cref="DeviceName"/>
        /// property. This should be called after any event that may change the default playback device.
        /// </summary>
        void RefreshName();
    }
}
