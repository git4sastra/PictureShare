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
using PictureShare.Lib;
using System;
using System.Linq;

namespace PictureShare.MenuManagers
{
    public sealed class ConsoleMenuManager : DefaultMenuManager
    {
        #region Public Constructors

        public ConsoleMenuManager(DeviceEntity device) : base(device)
        {
        }

        #endregion Public Constructors

        #region Public Methods

        public override void ShowMenu()
        {
            Console.WriteLine($"Available Modules: {AvailableModules.Count()}");
            Console.WriteLine();
            Console.WriteLine($"Device Image Path: {ConnectedDevice.ImageFolder}");
            Console.WriteLine();

            var firstMod = AvailableModules.FirstOrDefault();
            SelectedModule = new ModuleEntity() { FullName = firstMod.FullName, Name = firstMod.Name };

            ShowPicsSelectionMenu();

            Console.WriteLine();
            Console.WriteLine("Taste zum Beenden drücken ...");
            Console.ReadKey();
        }

        public override void ShowPicsSelectionMenu()
        {
            Console.WriteLine();
            Console.WriteLine("Pics Selection called");
            Console.WriteLine();
            Console.WriteLine($"Available Images: {Images.Count()}");
            Console.WriteLine();

            SaveImages();

            Console.WriteLine();
            Console.WriteLine("Images saved by module ...");
            Console.WriteLine();
        }

        #endregion Public Methods
    }
}