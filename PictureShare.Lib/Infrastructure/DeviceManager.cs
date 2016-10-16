//  Copyright (c) 2016 Thomas Ohms
//
//  This file is part of PictureShare.
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

using PictureShare.Lib.Data;

namespace PictureShare.Lib.Infrastructure
{
    public class DeviceManager
    {
        #region Private Fields

        private DeviceEntity _currentDevice;

        #endregion Private Fields

        #region Public Properties

        public DeviceEntity DeviceInfo
        {
            get { return _currentDevice; }
            private set { _currentDevice = value; }
        }

        #endregion Public Properties

        #region Public Methods

        public DeviceManager Load()
        {
            return this;
        }

        #endregion Public Methods

        #region Private Methods

        private string GetFolder()
        {
            var result = string.Empty;

            return result;
        }

        #endregion Private Methods
    }
}