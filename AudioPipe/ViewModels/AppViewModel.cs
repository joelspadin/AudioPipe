﻿using AudioPipe.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace AudioPipe.ViewModels
{
    public class AppViewModel : BindableBase, IAppViewModel
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

        public AppViewModel()
        {
            Devices = new ObservableCollection<IDeviceViewModel>();
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
                var deviceX = x as IDeviceViewModel;
                var deviceY = y as IDeviceViewModel;

                return DeviceViewModelComparer.Instance.Compare(deviceX, deviceY);
            }
        }
    }
}
