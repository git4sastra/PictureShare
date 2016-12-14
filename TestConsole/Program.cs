using PictureShare.Core.Data;
using PictureShare.DeviceManagers;
using PictureShare.DeviceRepositories;
using PictureShare.Lib;
using PictureShare.MenuManagers;
using System;
using System.IO;

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

            svc.VolumeChanged += Svc_VolumeChanged;

            svc.Start();

            Console.WriteLine("Taste zum Beenden drücken ...");
            Console.ReadKey();

            svc.Stop();
        }

        private static void Svc_VolumeChanged(object sender, VolumeChangedEventArgs e)
        {
            var repository = new XmlDeviceRepository(AppDomain.CurrentDomain.BaseDirectory); //RegistryDeviceRepository();
            var manager = new FormsDeviceManager(repository, e.DriveLetter);
            var device = manager.GetDevice(e.DeviceId);

            //var menu = new ConsoleMenuManager(device);
            var menu = new FormsMenuManager(device);

            menu.ShowMenu();
        }

        #endregion Main Program

        #region Private Methods

        private static void CopySampleFiles(string folder, string samplesFolder)
        {
            if (Directory.Exists(folder))
                Directory.Delete(folder, true);

            Directory.CreateDirectory(folder);

            if (!Directory.Exists(samplesFolder))
                Directory.CreateDirectory(samplesFolder);

            var sampleFiles = Directory.GetFiles(samplesFolder);

            for (int i = 0, max = sampleFiles.Length; i < max; i++)
            {
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
            var folder = AppDomain.CurrentDomain.BaseDirectory + "images";
            var samplesFolder = folder.Replace("images", "imageSamples");

            CopySampleFiles(folder, samplesFolder);

            var device = new DeviceEntity() { DeviceEntityId = 1, DeviceId = "", ImageFolder = folder };

            return device;
        }

        #endregion Private Methods
    }
}