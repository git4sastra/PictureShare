using PictureShare.Core.Data;
using PictureShare.Lib;
using System;
using System.IO;
using System.Reflection;

namespace TestConsole
{
    internal class Program
    {
        #region Main Program

        private static void Main(string[] args)
        {
            //DeviceEntity device = SimulateDeviceConnection();

            //var menu = new ConsoleMenuManager(device);
            //var menu = new FormsMenuManager(device);

            //menu.ShowMenu();

            //ShowMenu(typeof(ConsoleMenuManager), device);
            //ShowMenu(typeof(FormsMenuManager), device);

            var svc = new PictureShareService();
            svc.Start();

            Console.WriteLine("Taste druecken zum Beenden ...");
            Console.ReadKey();

            svc.Stop();
        }

        #endregion Main Program

        #region Private Methods

        private static void CopySampleFiles(string folder, string samplesFolder, string[] sampleFiles)
        {
            for (int i = 0, max = sampleFiles.Length; i < max; i++)
            {
                if (Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var newPath = sampleFiles[i].Replace(samplesFolder, folder);

                File.Copy(sampleFiles[i], newPath);
            }
        }

        private static void ShowMenu(Type menuManager, DeviceEntity device)
        {
            var menu = (DefaultMenuManager)Activator.CreateInstance(menuManager, device);
            menu.ShowMenu();
        }

        private static DeviceEntity SimulateDeviceConnection()
        {
            var folder = Assembly.GetExecutingAssembly().Location.Replace("TestConsole.exe", "") + "images";
            var samplesFolder = folder.Replace("images", "imageSamples");

            if (Directory.Exists(folder))
                Directory.Delete(folder, true);

            Directory.CreateDirectory(folder);

            if (!Directory.Exists(samplesFolder))
                Directory.CreateDirectory(samplesFolder);

            var sampleFiles = Directory.GetFiles(samplesFolder);

            CopySampleFiles(folder, samplesFolder, sampleFiles);

            var device = new DeviceEntity() { DeviceEntityId = 1, DeviceId = "", ImageFolder = folder };

            return device;
        }

        #endregion Private Methods
    }
}