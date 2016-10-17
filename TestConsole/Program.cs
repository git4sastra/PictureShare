using Core.Data;
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
            var folder = Assembly.GetExecutingAssembly().Location.Replace("TestConsole.exe", "") + "images";
            var samplesFolder = folder.Replace("images", "imageSamples");

            if (Directory.Exists(folder))
                Directory.Delete(folder, true);

            Directory.CreateDirectory(folder);

            if (!Directory.Exists(samplesFolder))
                Directory.CreateDirectory(samplesFolder);

            var sampleFiles = Directory.GetFiles(samplesFolder);

            for (int i = 0, max = sampleFiles.Length; i < max; i++)
            {
                if (Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var newPath = sampleFiles[i].Replace(samplesFolder, folder);
                File.Copy(sampleFiles[i], newPath);
            }

            var device = new DeviceEntity() { DeviceEntityId = 1, DeviceId = "", ImageFolder = folder };
            var menu = new ConsoleMenuManager(device);

            menu.ShowMenu();

            Console.WriteLine();
            Console.WriteLine("Taste zum Beenden druecken");
            Console.ReadKey();
        }

        #endregion Private Methods
    }
}