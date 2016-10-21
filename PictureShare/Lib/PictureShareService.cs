//  Copyright (c) 2016 Thomas Ohms
//
//  This file is part of PictureShare.
//
//  Get the original code from https://github.com/thomas4U/PictureShare
//
//  PictureShare is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  Foobar is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with Foobar.  If not, see <http://www.gnu.org/licenses/>.

using PictureShare.Core.Lib.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace PictureShare.Lib
{
    public class PictureShareService : IPictureShareService, IDisposable
    {
        #region Delegates

        public delegate void OnVolumeChangedHandler(object sender, VolumeChangedEventArgs e);

        #endregion Delegates

        #region Events

        public event OnVolumeChangedHandler VolumeChanged;

        #endregion Events

        #region Fields

        private string _addedDevice = string.Empty;
        private List<string> _connectedDevices = new List<string>();
        private bool _disposed = false;
        private object _lock = new object();
        private ManagementEventWatcher _watcher;

        #endregion Fields

        #region Methods

        public void Start()
        {
            UpdateConnectedDevices();
            WatchForVolumeChangedEvent();
        }

        public void Stop()
        {
            if (_watcher == null)
                return;

            _watcher.Stop();
            _watcher.Dispose();
            _watcher = null;
        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
                Stop();

            _disposed = true;
        }

        #endregion IDisposable

        private static string RawStringToDeviceId(ManagementBaseObject usbDevice)
        {
            var props = usbDevice.Properties;
            var fullVal = props["Dependent"].Value.ToString();
            var pos = fullVal.IndexOf("DeviceID=") + 10;
            var max = fullVal.Length - pos - 1;
            var deviceId = fullVal.Substring(pos, max);

            return deviceId;
        }

        private void OnVolumeChange(object sender, EventArrivedEventArgs e)
        {
            UpdateConnectedDevices();

            var eventType = int.Parse(e.NewEvent.Properties["EventType"].Value.ToString());
            var driveLetter = e.NewEvent.Properties["DriveName"].Value.ToString();
            var deviceId = _addedDevice;

            if (string.IsNullOrWhiteSpace(driveLetter) || string.IsNullOrWhiteSpace(deviceId) || eventType != 2)
                return;

            var eventArgs = new VolumeChangedEventArgs()
            {
                DriveLetter = driveLetter,
                DeviceId = deviceId
            };

            lock (_lock)
                VolumeChanged?.Invoke(this, eventArgs);
        }

        private void UpdateConnectedDevices()
        {
            lock (_lock)
            {
                ManagementObjectCollection results;
                var query = new WqlObjectQuery("SELECT * FROM Win32_USBControllerDevice");

                using (var search = new ManagementObjectSearcher(query))
                    results = search.Get();

                var resultsEnum = results.GetEnumerator();
                var isFirstRun = _connectedDevices.Count == 0;
                var deviceFound = false;

                while (resultsEnum.MoveNext())
                {
                    string deviceId = RawStringToDeviceId(resultsEnum.Current);

                    if (isFirstRun)
                    {
                        _connectedDevices.Add(deviceId);
                        deviceFound = true;
                        continue;
                    }

                    if (_addedDevice == deviceId)
                        deviceFound = true;

                    var count = _connectedDevices.Count(cd => cd == deviceId);
                    if (count == 0)
                    {
                        _connectedDevices.Add(deviceId);
                        _addedDevice = deviceId;
                        deviceFound = true;
                    }
                    else
                    {
                        _addedDevice = deviceId;
                        deviceFound = true;
                    }
                }

                if (!deviceFound && !isFirstRun && !string.IsNullOrWhiteSpace(_addedDevice))
                {
                    _connectedDevices.Remove(_addedDevice);
                    _addedDevice = string.Empty;
                }
            }
        }

        private void WatchForVolumeChangedEvent()
        {
            _watcher = new ManagementEventWatcher();
            var query = new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent");

            _watcher.EventArrived += OnVolumeChange;
            _watcher.Query = query;
            _watcher.Start();
        }

        #endregion Methods
    }

    public sealed class VolumeChangedEventArgs : EventArgs
    {
        #region Properties

        public string DeviceId { get; set; }

        public string DriveLetter { get; set; }

        #endregion Properties
    }
}