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
        #region Private Fields

        private static object _lock = new object();

        #endregion Private Fields

        #region Public Constructors

        public DefaultMenuManager(DeviceEntity device)
        {
            ConnectedDevice = device;
            RegisterModules();
            LoadPicsFromDevice();
        }

        #endregion Public Constructors

        #region Protected Methods

        protected override void DeleteImages()
        {
            // TODO: BUG: Ohne GC gibt's Zugriffsfehler, mit GC reicht die Zeit beim Schließen nicht aus
            //            um alle Dateien zu löschen (nur 1 wird gelöscht).
            //            Eventuell wird was nach der Bildauswahl nicht richtig Disposed oder nach dem Kopieren geschlossen,
            //            denn die Bildauswahl ist das einzige Form, das die Bilder anrührt
            GC.Collect();
            GC.WaitForPendingFinalizers();

            for (int i = 0, max = Images.Count(); i < max; i++)
            {
                var img = Images.ElementAt(i);
                File.Delete($"{img.Path}\\{img.Name}");
            }
        }

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

        #endregion Protected Methods

        #region Private Methods

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
                    select t).First();
        }

        #endregion Private Methods
    }
}