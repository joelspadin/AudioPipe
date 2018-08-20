using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AudioPipe.ViewModels
{
    /// <summary>
    /// View model for the main application window.
    /// </summary>
    public interface IAppViewModel
    {
        /// <summary>
        /// Gets an <see cref="ICollectionView"/> over <see cref="Devices"/>.
        /// </summary>
        ICollectionView DevicesView { get; }

        /// <summary>
        /// Gets the list of output audio devices.
        /// </summary>
        ObservableCollection<IDeviceViewModel> Devices { get; }

        /// <summary>
        /// Gets or sets the selected output audio device.
        /// </summary>
        IDeviceViewModel SelectedDevice { get; set; }

        /// <summary>
        /// Updates the view model's device list.
        /// </summary>
        void Refresh();
    }
}
