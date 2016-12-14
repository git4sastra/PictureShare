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
using PictureShare.Core.Lib.Structure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PictureShare.Lib
{
    public abstract class DefaultMenuManager : BaseMenuManager
    {
        #region Fields

        private static object _lock = new object();

        #endregion Fields

        #region Constructors

        public DefaultMenuManager(DeviceEntity device)
        {
            ConnectedDevice = device;
            RegisterModules();
            LoadPicsFromDevice();
        }

        #endregion Constructors

        #region Methods

        protected override void DeleteImages()
        {
            for (int i = 0, max = Images.Count(); i < max; i++)
            {
                var img = Images.ElementAt(i);
                File.Delete($"{img.Path}\\{img.Name}");
            }

            Images = new List<ImageEntity>();
        }

        protected override void LoadPicsFromDevice()
        {
            var imgs = Directory.GetFiles(ConnectedDevice.ImageFolder, "*.jp*g", SearchOption.AllDirectories);
            var imgList = new List<ImageEntity>();

            for (int i = 0, max = imgs.Count(); i < max; i++)
            {
                var fullPath = imgs.ElementAt(i);

                // Eine mögliche Variante
                //var imgName = fullPath.Split('\\').Last();
                //var imgPath = fullPath.Replace($"\\{imgName}", "");

                // So geht's einfacher
                var imgName = Path.GetFileName(fullPath);
                var imgPath = Path.GetDirectoryName(fullPath);

                var image = new ImageEntity()
                {
                    Name = imgName,
                    Path = imgPath
                };

                imgList.Add(image);
            }

            Images = imgList;
        }

        protected override void RegisterModules()
        {
            IEnumerable<Type> modules = GetModulesFromAssembly();
            IEnumerable<ModuleEntity> availMods = GetAvailableModules(modules);

            AvailableModules = availMods;
        }

        protected override void SaveImages()
        {
            Type module = GetSelectedModule();

            if (module != null)
            {
                lock (_lock)
                {
                    var modInstance = (IMenuModule)Activator.CreateInstance(module);
                    modInstance.SaveImages(Images);
                }
            }
        }

        private static IEnumerable<Type> GetModulesFromAssembly()
        {
            var allTypes = Assembly.GetExecutingAssembly().GetTypes();
            var modules = from t in allTypes
                          where t.GetInterfaces().Contains(typeof(IMenuModule))
                          select t;
            return modules;
        }

        private List<ModuleEntity> GetAvailableModules(IEnumerable<Type> modules)
        {
            var availMods = new List<ModuleEntity>();

            for (int i = 0, max = modules.Count(); i < max; i++)
            {
                var modType = modules.ElementAt(i);
                var mod = (IMenuModule)Activator.CreateInstance(modType);
                var modData = new ModuleEntity()
                {
                    FullName = modType.FullName,
                    Name = modType.Name.Replace("MenuModule", "")
                };

                mod.Register(this);
                availMods.Add(modData);
            }

            return availMods;
        }

        private Type GetSelectedModule()
        {
            return (from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.FullName == SelectedModule.FullName
                    select t)?.First();
        }

        #endregion Methods
    }
}