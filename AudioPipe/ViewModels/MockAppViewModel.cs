using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace AudioPipe.ViewModels
{
    /// <summary>
    /// <see cref="IAppViewModel"/> that shows a list of fake devices.
    /// </summary>
    public class MockAppViewModel : BindableBase, IAppViewModel
    {
        private IDeviceViewModel selectedDevice;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockAppViewModel"/> class.
        /// </summary>
        public MockAppViewModel()
        {
            Devices = new ObservableCollection<IDeviceViewModel>();
            DevicesView = CollectionViewSource.GetDefaultView(Devices);

            Sort();
        }

        /// <inheritdoc/>
        public ObservableCollection<IDeviceViewModel> Devices { get; }

        /// <inheritdoc/>
        public ICollectionView DevicesView { get; }

        /// <inheritdoc/>
        public IDeviceViewModel SelectedDevice
        {
            get => selectedDevice;
            set
            {
                selectedDevice = value;
                NotifyPropertyChanged(nameof(SelectedDevice));
            }
        }

        /// <inheritdoc/>
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
            var deviceView = DevicesView as ListCollectionView;
            deviceView.CustomSort = new DeviceSorter();
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

        private class DeviceSorter : System.Collections.IComparer
        {
            public int Compare(object x, object y)
            {
                var deviceX = x as IDeviceViewModel;
                var deviceY = y as IDeviceViewModel;

                return string.CompareOrdinal(deviceX.DeviceName, deviceY.DeviceName);
            }
        }
    }
}
