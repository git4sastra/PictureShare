using Microsoft.Win32;
using PictureShare.Core.Data;
using PictureShare.Lib;
using System.Collections.Generic;
using System.Linq;

namespace PictureShare.DeviceRepositories
{
    public class RegistryDeviceRepository : DefaultDeviceRepository
    {
        #region Fields

        private const string _path = @"PictureShare\Devices";
        private RegistryKey _key;

        #endregion Fields

        #region Constructors

        public RegistryDeviceRepository()
        {
            InitKey();

            LoadDevices();
        }

        #endregion Constructors

        #region Methods

        public override bool SaveChanges()
        {
            for (int i = 0, max = Devices.Count(); i < max; i++)
            {
                var dev = Devices.ElementAt(i);

                if (dev.ImageFolder == null)
                {
                    var list = Devices.ToList();
                    list.Remove(dev);
                    Devices = list;
                }
                else
                {
                    _key.SetValue(dev.DeviceId, dev.ImageFolder);
                }
            }

            return Devices.Count() == _key.GetValueNames().Length;
        }

        private void InitKey()
        {
            var root = Registry.CurrentUser;
            var software = root.OpenSubKey("Software", true);

            _key = software.CreateSubKey(_path, true);
        }

        private void LoadDevices()
        {
            var regVals = _key.GetValueNames();
            var count = regVals.Length;
            var list = new List<DeviceEntity>();

            for (int i = 0; i < count; i++)
            {
                var deviceId = regVals[i];

                if (string.IsNullOrWhiteSpace(deviceId))
                    continue;

                var imgPath = (string)_key.GetValue(deviceId);
                var id = i + 1;
                var entity = new DeviceEntity()
                {
                    DeviceEntityId = id,
                    DeviceId = deviceId,
                    ImageFolder = imgPath
                };

                list.Add(entity);
            }

            Devices = list;
        }

        #endregion Methods
    }
}