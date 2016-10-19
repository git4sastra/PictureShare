using PictureShare.Core.Data;
using PictureShare.Core.Data.Structure;
using System.Collections.Generic;
using System.Linq;

namespace PictureShare.Lib
{
    public abstract class DefaultDeviceRepository : BaseDeviceRepository
    {
        #region Constructors

        public DefaultDeviceRepository()
        {
            Devices = new List<DeviceEntity>();
        }

        #endregion Constructors

        #region Methods

        public override void AddDevice(DeviceEntity device)
        {
            var list = Devices.ToList();
            list.Add(device);
            Devices = list;
        }

        public override void DeleteDevice(DeviceEntity device)
        {
            var list = Devices.ToList();
            list.Remove(device);
            Devices = list;
        }

        public override IEnumerable<DeviceEntity> GetAll()
        {
            return Devices;
        }

        public override DeviceEntity GetDevice(string deviceId)
        {
            var result = from d in Devices
                         where d.DeviceId == deviceId
                         select d;

            return result.First();
        }

        public override DeviceEntity GetDevice(int id)
        {
            var result = from d in Devices
                         where d.DeviceEntityId == id
                         select d;

            return result.First();
        }

        public override string GetImagePath(string deviceId)
        {
            return GetDevice(deviceId).ImageFolder;
        }

        public override string GetImagePath(int id)
        {
            return GetDevice(id).ImageFolder;
        }

        public override void UpdateDevice(DeviceEntity device)
        {
            var list = Devices.ToList();
            var item = list.Find(d => d.DeviceId == device.DeviceId);

            if (item.DeviceEntityId == 0)
            {
                AddDevice(device);
                return;
            }

            list.Remove(device);
            list.Add(device);

            Devices = list;
        }

        #endregion Methods
    }
}