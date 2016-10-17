using Core.Data;
using Core.Lib.Structure;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.AccessControl;

namespace PictureShare.MenuModules.LocalFolder
{
    public class LocalFolderMenuModule : IMenuModule
    {
        #region Public Methods

        public void Register(BaseMenuManager menuManager)
        {
        }

        public void SaveImages(IEnumerable<ImageEntity> images)
        {
            try
            {
                var localPath = GetLocalPath();
                var imgEnum = images.GetEnumerator();

                while (imgEnum.MoveNext())
                {
                    var img = imgEnum.Current;
                    var imgFullPath = $"{img.Path}\\{img.Name}";
                    var createYear = File.GetCreationTime(imgFullPath).Year;
                    var ticks = File.GetCreationTime(imgFullPath).Ticks;

                    if (string.IsNullOrWhiteSpace(img.Album))
                        img.Album = createYear.ToString();

                    var albumPath = $"{localPath}\\{img.Album}";

                    if (!Directory.Exists(albumPath))
                        Directory.CreateDirectory(albumPath);

                    var localFullPath = $"{albumPath}\\{img.Name}";

                    if (File.Exists(localFullPath))
                    {
                        img.Name = $"{ticks}__{img.Name}";
                        localFullPath = $"{albumPath}\\{img.Name}";
                    }

                    File.Copy(imgFullPath, localFullPath);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"#### Exception: {e.Message}");

                if (e.InnerException != null)
                    Debug.WriteLine($"#### Inner Exception: {e.InnerException.Message}");
            }
        }

        public RegistryKey GetRegKey()
        {
            var hkcu = Registry.CurrentUser;
            var swKey = hkcu.OpenSubKey("Software", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.CreateSubKey);
            var toolKey = swKey.OpenSubKey("PictureShare", true);

            if (toolKey == null)
                toolKey = swKey.CreateSubKey("PictureShare", true);

            return toolKey;
        }

        #endregion Public Methods

        #region Private Methods

        private string GetLocalPath()
        {
            var toolKey = GetRegKey();
            var asm = Assembly.GetExecutingAssembly();
            var location = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            var defaultPath = $"{location}\\PictureShare";

            if (toolKey.GetValue("LocalPath") == null)
                toolKey.SetValue("LocalPath", defaultPath, RegistryValueKind.String);

            var fullPath = toolKey.GetValue("LocalPath").ToString();

            return fullPath;
        }

        #endregion Private Methods
    }
}