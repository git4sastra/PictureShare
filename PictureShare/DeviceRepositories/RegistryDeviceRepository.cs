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