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