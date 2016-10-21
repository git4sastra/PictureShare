//  Copyright (c) 2016 Thomas Ohms
//
//  This file is part of PictureShare.
//
//  Get the original code from https://thomas4u.github.io/PictureShare/
//
//  PictureShare is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  PictureShare is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with PictureShare.  If not, see <http://www.gnu.org/licenses/>.

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

            return result.FirstOrDefault();
        }

        public override DeviceEntity GetDevice(int id)
        {
            var result = from d in Devices
                         where d.DeviceEntityId == id
                         select d;

            return result.FirstOrDefault();
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