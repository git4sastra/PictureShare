using PictureShare.Core.Data;
using PictureShare.Core.Data.Structure;
using PictureShare.Core.Lib.Structure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PictureShare.DeviceManagers
{
    public abstract class DefaultDeviceManager : BaseDeviceManager
    {
        #region Fields

        private BaseDeviceRepository _repository;

        #endregion Fields

        #region Constructors

        public DefaultDeviceManager(BaseDeviceRepository repository, string driveLetter) : base(driveLetter)
        {
            if (driveLetter.EndsWith(":"))
                DriveLetter = driveLetter;
            else
                DriveLetter = $"{driveLetter}:";

            _repository = repository;
        }

        #endregion Constructors

        #region Methods

        public override DeviceEntity GetDevice(string deviceId)
        {
            var result = _repository.GetDevice(deviceId);

            if (result.DeviceEntityId == 0)
            {
                result.ImageFolder = FindImageFolder();
                AddDevice(result);
                result = _repository.GetDevice(deviceId);
            }

            result = ReplaceDriveLetter(result);

            return result;
        }

        protected override void AddDevice(DeviceEntity device)
        {
            _repository.AddDevice(device);

            var success = _repository.SaveChanges();

            if (!success)
                throw new OperationCanceledException("Beim Hinzufügen des Geräts kam es zu Schwierigkeiten.");
        }

        protected override string FindImageFolder()
        {
            string imgDir = GuessImageFolder(DriveLetter);

            if (string.IsNullOrWhiteSpace(imgDir) || AskAutoPathCorrect(imgDir) == false)
                imgDir = AskForFolder();

            return imgDir;
        }

        private static string GuessImageFolder(string deviceRootPath)
        {
            var dirs = Directory.GetDirectories(deviceRootPath, "*", SearchOption.AllDirectories);
            var imgCount = new Dictionary<int, string>();
            var imgDir = string.Empty;

            for (int i = 0, max = dirs.Count(); i < max; i++)
            {
                var files = Directory.GetFiles(dirs[i], "*.jp*g", SearchOption.TopDirectoryOnly);
                var filesCount = files.Count();

                if (filesCount > 0)
                    imgCount.Add(filesCount, dirs[i]);
            }

            if (imgCount.Count > 0)
                imgDir = imgCount.OrderByDescending(i => i.Key).First().Value;

            return imgDir;
        }

        private DeviceEntity ReplaceDriveLetter(DeviceEntity result)
        {
            var pattern = @"^[A-Z]:";
            var regex = new Regex(pattern);

            result.ImageFolder = regex.Replace(result.ImageFolder, DriveLetter);
            return result;
        }

        #endregion Methods
    }
}