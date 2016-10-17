using Microsoft.Win32;
using PictureShare.Core.Data;
using PictureShare.Core.Lib.Structure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.AccessControl;

namespace PictureShare.MenuModules.LocalFolder
{
    public sealed class LocalFolderMenuModule : IMenuModule
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
                    string imgFullPath, localFullPath;

                    GeneratePaths(localPath, img, out imgFullPath, out localFullPath);

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

        private static string GetImageLocalPath(ImageEntity img, long ticks, string albumPath)
        {
            string localFullPath = $"{albumPath}\\{img.Name}";
            if (File.Exists(localFullPath))
            {
                img.Name = $"{ticks}__{img.Name}";
                localFullPath = $"{albumPath}\\{img.Name}";
            }

            return localFullPath;
        }

        private string GetAlbumPath(string localPath, ref ImageEntity img)
        {
            var albumPath = $"{localPath}\\{img.Album}";

            if (!Directory.Exists(albumPath))
                Directory.CreateDirectory(albumPath);

            return albumPath;
        }

        private ImageEntity CheckAlbum(ImageEntity img, int createYear)
        {
            if (string.IsNullOrWhiteSpace(img.Album))
                img.Album = createYear.ToString();

            return img;
        }

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

        private void GeneratePaths(string localPath, ImageEntity img, out string imgFullPath, out string localFullPath)
        {
            imgFullPath = $"{img.Path}\\{img.Name}";
            var createYear = File.GetCreationTime(imgFullPath).Year;
            var ticks = File.GetCreationTime(imgFullPath).Ticks;
            img = CheckAlbum(img, createYear);
            var albumPath = GetAlbumPath(localPath, ref img);
            localFullPath = GetImageLocalPath(img, ticks, albumPath);
        }

        #endregion Private Methods
    }
}