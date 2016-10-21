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

        public RegistryKey GetRegKey()
        {
            var hkcu = Registry.CurrentUser;
            var swKey = hkcu.OpenSubKey("Software", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.CreateSubKey);
            var toolKey = swKey.OpenSubKey("PictureShare", true);

            if (toolKey == null)
                toolKey = swKey.CreateSubKey("PictureShare", true);

            return toolKey;
        }

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

        private ImageEntity CheckAlbum(ImageEntity img, int createYear)
        {
            if (string.IsNullOrWhiteSpace(img.Album))
                img.Album = createYear.ToString();

            return img;
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

        private string GetAlbumPath(string localPath, ref ImageEntity img)
        {
            var albumPath = $"{localPath}\\{img.Album}";

            if (!Directory.Exists(albumPath))
                Directory.CreateDirectory(albumPath);

            return albumPath;
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

        #endregion Private Methods
    }
}