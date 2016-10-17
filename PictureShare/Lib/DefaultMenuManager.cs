using Core.Data;
using Core.Lib.Structure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PictureShare.Lib
{
    public abstract class DefaultMenuManager : BaseMenuManager
    {
        #region Public Constructors

        public DefaultMenuManager(DeviceEntity device)
        {
            ConnectedDevice = device;
            RegisterModules();
            LoadPicsFromDevice();
        }

        #endregion Public Constructors

        #region Protected Methods

        protected override void LoadPicsFromDevice()
        {
            var imgs = Directory.GetFiles(ConnectedDevice.ImageFolder, "*.jp*g", SearchOption.AllDirectories);
            var imgList = new List<ImageEntity>();

            for (int i = 0, max = imgs.Count(); i < max; i++)
            {
                var fullPath = imgs.ElementAt(i);
                var imgName = fullPath.Split('\\').Last();
                var imgPath = fullPath.Replace($"\\{imgName}", "");

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
            var allTypes = Assembly.GetExecutingAssembly().GetTypes();
            var modules = from t in allTypes
                          where t.GetInterfaces().Contains(typeof(IMenuModule))
                          select t;

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

            AvailableModules = availMods;
        }

        protected override void SaveImages()
        {
            var module = (from t in Assembly.GetExecutingAssembly().GetTypes()
                          where t.FullName == SelectedModule.FullName
                          select t).First();

            if (module != null)
            {
                var modInstance = (IMenuModule)Activator.CreateInstance(module);
                modInstance.SaveImages(Images);
            }
        }

        protected override void DeleteImages()
        {
            for (int i = 0, max = Images.Count(); i < max; i++)
                File.Delete(Images.ElementAt(i).Path);
        }

        #endregion Protected Methods
    }
}