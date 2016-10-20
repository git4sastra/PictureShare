using System;
using System.Windows.Forms;

namespace PictureShare.DeviceManagers.Forms
{
    public partial class AskForFolder : Form
    {
        #region Constructors

        public AskForFolder()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Properties

        public string SelectedPath { get; private set; }

        #endregion Properties

        #region Methods

        private void button1_Click(object sender, EventArgs e)
        {
            var result = folderBrowser.ShowDialog();

            while (result != DialogResult.OK)
                result = folderBrowser.ShowDialog();

            SelectedPath = folderBrowser.SelectedPath;

            Close();
        }

        #endregion Methods
    }
}