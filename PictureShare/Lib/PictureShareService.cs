using PictureShare.Core.Lib.Structure;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace PictureShare.Lib
{
    public class PictureShareService : IPictureShareService
    {
        #region Fields

        private string _addedDevice = string.Empty;
        private List<string> _connectedDevices = new List<string>();

        private ManagementEventWatcher _watcher;
        private ManagementEventWatcher _watcher2;

        #endregion Fields

        #region Methods

        public void Start()
        {
            UpdateConnectedDevices();
            WatchForVolumeChangedEvent();
        }

        public void Stop()
        {
            _watcher2.Stop();
            _watcher2.Dispose();
            _watcher2 = null;

            _watcher.Stop();
            _watcher.Dispose();
            _watcher = null;
        }

        private void OnDeviceChange(object sender, EventArrivedEventArgs e)
        {
            UpdateConnectedDevices();
        }

        private void OnVolumeChange(object sender, EventArrivedEventArgs e)
        {
            var driveLetter = e.NewEvent.Properties["DriveName"].Value.ToString();
            var deviceId = _addedDevice;

            if (string.IsNullOrWhiteSpace(driveLetter) || string.IsNullOrWhiteSpace(deviceId))
                return;

            var debug = "dummy";
        }

        private void UpdateConnectedDevices()
        {
            ManagementObjectCollection results;
            var query = new WqlObjectQuery("SELECT * FROM Win32_USBControllerDevice");

            using (var search = new ManagementObjectSearcher(query))
                results = search.Get();

            var resultsEnum = results.GetEnumerator();
            var isFirstRun = _connectedDevices.Count == 0;

            while (resultsEnum.MoveNext())
            {
                var props = resultsEnum.Current.Properties;
                var fullVal = props["Dependent"].Value.ToString();
                var pos = fullVal.IndexOf("DeviceID=") + 10;
                var max = fullVal.Length - pos - 1;
                var deviceId = fullVal.Substring(pos, max);

                if (isFirstRun)
                {
                    _connectedDevices.Add(deviceId);
                    continue;
                }

                if (_addedDevice == deviceId)
                {
                    _connectedDevices.Remove(deviceId);
                    _addedDevice = string.Empty;
                    continue;
                }

                var count = _connectedDevices.Count(cd => cd == deviceId);
                if (count == 0)
                {
                    _connectedDevices.Add(deviceId);
                    _addedDevice = deviceId;
                }
            }
        }

        private void WatchForVolumeChangedEvent()
        {
            _watcher = new ManagementEventWatcher();
            _watcher2 = new ManagementEventWatcher();
            var query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent");
            var query2 = new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent");

            _watcher.EventArrived += OnDeviceChange;
            _watcher.Query = query;
            _watcher.Start();

            _watcher2.EventArrived += OnVolumeChange;
            _watcher2.Query = query2;
            _watcher2.Start();
        }

        #endregion Methods
    }
}