using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AudioPipe.ViewModels
{
    public interface IAppViewModel
    {
        ICollectionView DevicesView { get; }
        ObservableCollection<IDeviceViewModel> Devices { get; }

        IDeviceViewModel SelectedDevice { get; set; }

        void Refresh();
    }
}
