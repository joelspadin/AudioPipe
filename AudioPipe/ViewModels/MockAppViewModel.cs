using AudioPipe.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System;

namespace AudioPipe.ViewModels
{
    /// <summary>
    /// IAppViewModel that shows a list of fake devices.
    /// </summary>
    public class MockAppViewModel : BindableBase, IAppViewModel
    {
        public ICollectionView DevicesView { get; }
        public ObservableCollection<IDeviceViewModel> Devices { get; }

        private IDeviceViewModel _selectedDevice;

        public IDeviceViewModel SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                _selectedDevice = value;
                RaisePropertyChanged(nameof(SelectedDevice));
            }
        }

        public MockAppViewModel()
        {
            Devices = new ObservableCollection<IDeviceViewModel>();
            DevicesView = CollectionViewSource.GetDefaultView(Devices);

            Sort();
        }

        public void Refresh()
        {
            var deviceModels = new List<IDeviceViewModel>
            {
                new MockDeviceViewModel("Device 1", true),
                new MockDeviceViewModel("Device 2", false),
                new MockDeviceViewModel("Device 3", false),
                new MockDeviceViewModel("Device 4", false),
            };

            foreach (var removedItem in Devices.Except(deviceModels, new DeviceComparer()).ToList())
            {
                Devices.Remove(removedItem);
            }

            foreach (var addedItem in deviceModels.Except(Devices, new DeviceComparer()).ToList())
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
                var deviceX = x as IDeviceViewModel;
                var deviceY = y as IDeviceViewModel;

                return string.CompareOrdinal(deviceX.DeviceName, deviceY.DeviceName);
            }
        }

        private class DeviceComparer : IEqualityComparer<IDeviceViewModel>
        {
            public bool Equals(IDeviceViewModel x, IDeviceViewModel y)
            {
                return x.DeviceName == y.DeviceName;
            }

            public int GetHashCode(IDeviceViewModel obj)
            {
                return obj.DeviceName.GetHashCode();
            }
        }
    }
}
