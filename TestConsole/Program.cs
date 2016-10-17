using PictureShare.Core.Data;
using PictureShare.MenuManagers;
using System;
using System.IO;
using System.Reflection;

namespace TestConsole
{
    internal class Program
    {
        #region Private Methods

        private static void Main(string[] args)
        {
            DeviceEntity device = SimulateDeviceConnection();

            var menu = new ConsoleMenuManager(device);

            menu.ShowMenu();

            Console.WriteLine();
            Console.WriteLine("Taste zum Beenden druecken");
            Console.ReadKey();
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

        #endregion Private Methods
    }
}