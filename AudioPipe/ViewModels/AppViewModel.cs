using AudioPipe.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioPipe.Extensions;

namespace AudioPipe.ViewModels
{
    public class AppViewModel : BindableBase
    {
        private DeviceViewModel _selectedDevice;

        public ObservableCollection<DeviceViewModel> Devices { get; private set; }
        public DeviceViewModel SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                _selectedDevice = value;
                RaisePropertyChanged(nameof(SelectedDevice));
            }
        }

        public AppViewModel()
        {
            Devices = new ObservableCollection<DeviceViewModel>();
        }

        public void Refresh()
        {
            var deviceModels = DeviceService.GetOutputDevices()
                .Select(d => new DeviceViewModel(d));
                // .OrderBy(d => d, DeviceViewModelComparer.Instance);

            foreach (var removedItem in Devices.Except(deviceModels, DeviceViewModelEqualityComparer.Instance).ToList())
            {
                Devices.Remove(removedItem);
            }

            foreach (var addedItem in deviceModels.Except(Devices, DeviceViewModelEqualityComparer.Instance).ToList())
            {
                Devices.InsertSorted(addedItem, DeviceViewModelComparer.Instance);
            }

            // TODO: replacing the whole list each time causes the selected item to change
            RaisePropertyChanged(nameof(Devices));
        }
    }
}
