using PictureShare.Core.Data.Structure;
using System.Threading;
using System.Windows.Forms;

namespace PictureShare.DeviceManagers
{
    public sealed class FormsDeviceManager : DefaultDeviceManager
    {
        #region Delegates

        public delegate string AskForFolderHandler();

        #endregion Delegates

        #region Events

        public event AskForFolderHandler OnAskForFolder;

        #endregion Events

        #region Fields

        private const string _TITLE = "Automatische Bildordner Erkennung";

        private string _path = string.Empty;

        #endregion Fields

        #region Constructors

        public FormsDeviceManager(BaseDeviceRepository repository, string driveLetter) : base(repository, driveLetter)
        {
        }

        #endregion Constructors

        #region Methods

        protected override bool AskAutoPathCorrect(string path)
        {
            var msg = $"Der Ordner mit den meisten Bildern ist \n\n{path}\n\n" +
                      "Ist das korrekt?";
            var btns = MessageBoxButtons.YesNo;
            var symbol = MessageBoxIcon.Question;

            return MessageBox.Show(msg, _TITLE, btns, symbol) == DialogResult.Yes;
        }

        protected override string AskForFolder()
        {
            var path = string.Empty;

            var thread = new Thread(new ThreadStart(() =>
            {
                using (var form = new PictureShare.DeviceManagers.Forms.AskForFolder())
                {
                    Application.Run(form);
                    _path = form.SelectedPath;
                }
            }));

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            while (thread.ThreadState == ThreadState.Running) { Thread.Sleep(1000); }

            return _path;
        }
    }

    #endregion Methods
}