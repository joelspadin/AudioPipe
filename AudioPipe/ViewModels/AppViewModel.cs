using AudioPipe.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioPipe.Extensions;
using System.ComponentModel;
using System.Windows.Data;

namespace AudioPipe.ViewModels
{
    public class AppViewModel : BindableBase
    {
        public ICollectionView DevicesView { get; }
        public ObservableCollection<DeviceViewModel> Devices { get; }

        private DeviceViewModel _selectedDevice;

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
            DevicesView = CollectionViewSource.GetDefaultView(Devices);

            Sort();
        }

        public void Refresh()
        {
            var deviceModels = DeviceService.GetOutputDevices().Select(d => new DeviceViewModel(d));

            foreach (var removedItem in Devices.Except(deviceModels, DeviceViewModelEqualityComparer.Instance).ToList())
            {
                Devices.Remove(removedItem);
            }

            foreach (var addedItem in deviceModels.Except(Devices, DeviceViewModelEqualityComparer.Instance).ToList())
            {
                Devices.Add(addedItem);
            }

            foreach (var device in Devices)
            {
                device.RefreshName();
            }

            Sort();
        }

        private void Sort()
        {
            var _deviceView = DevicesView as ListCollectionView;
            _deviceView.CustomSort = new DeviceSorter();
        }

        private class DeviceSorter : System.Collections.IComparer
        {
            public int Compare(object x, object y)
            {
                var deviceX = x as DeviceViewModel;
                var deviceY = y as DeviceViewModel;

                return DeviceViewModelComparer.Instance.Compare(deviceX, deviceY);
            }
        }
    }
}
