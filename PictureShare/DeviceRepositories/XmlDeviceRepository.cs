using PictureShare.Core.Data;
using PictureShare.Lib;
using System;
using System.Data;
using System.IO;
using System.Linq;

namespace PictureShare.DeviceRepositories
{
    public sealed class XmlDeviceRepository : DefaultDeviceRepository
    {
        #region Fields

        private const string COLUMN_NAME_DEVICEID = "DeviceId";
        private const string COLUMN_NAME_IMAGEPATH = "ImagePath";
        private const string TABLE_NAME = "Devices";
        private string _filePath;

        #endregion Fields

        #region Constructors

        public XmlDeviceRepository(string filePath) : base()
        {
            _filePath = filePath;
            LoadDevices();
        }

        #endregion Constructors

        #region Methods

        public override bool SaveChanges()
        {
            var ds = GetDataSet();
            var table = ds.Tables[TABLE_NAME];

            for (int i = 0, max = Devices.Count(); i < max; i++)
            {
                var dev = Devices.ElementAt(i);
                var row = table.NewRow();

                row[COLUMN_NAME_DEVICEID] = dev.DeviceId;
                row[COLUMN_NAME_IMAGEPATH] = dev.ImageFolder;

                table.Rows.Add(row);
            }

            ds.WriteXml(_filePath, XmlWriteMode.WriteSchema);

            return table.Rows.Count == Devices.Count();
        }

        private DataColumn[] GetColumns()
        {
            var colDeviceId = new DataColumn(COLUMN_NAME_DEVICEID, typeof(string));
            var colImagePath = new DataColumn(COLUMN_NAME_IMAGEPATH, typeof(string));
            return new DataColumn[] { colDeviceId, colImagePath };
        }

        private DataSet GetDataSet()
        {
            var ds = new DataSet("DevicesDataSet");
            ds.Tables.Add(GetDataTable());
            return ds;
        }

        private DataTable GetDataTable()
        {
            var table = new DataTable(TABLE_NAME);
            table.Columns.AddRange(GetColumns());
            return table;
        }

        private void LoadDevices()
        {
            if (!File.Exists(_filePath))
                return;

            try
            {
                var ds = GetDataSet();
                ds.ReadXml(_filePath);

                if (ds.Tables.Count == 0)
                    return;

                var rows = ds.Tables[TABLE_NAME].Rows;
                var rowsEnum = rows.GetEnumerator();
                var count = 1;
                var deviceList = Devices.ToList();

                while (rowsEnum.MoveNext())
                {
                    var row = (DataRow)rowsEnum.Current;
                    var deviceId = row.ItemArray[0] as string;
                    var path = row.ItemArray[1] as string;
                    var entity = new DeviceEntity()
                    {
                        DeviceEntityId = count,
                        DeviceId = deviceId,
                        ImageFolder = path
                    };

                    deviceList.Add(entity);
                }

                Devices = deviceList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Methods
    }
}