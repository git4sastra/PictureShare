using PictureShare.Core.Data.Structure;
using System.Windows.Forms;
using static System.Environment;

namespace PictureShare.DeviceManagers
{
    public sealed class FormsDeviceManager : DefaultDeviceManager
    {
        private const string _TITLE = "Automatische Bildordner Erkennung";

        public FormsDeviceManager(BaseDeviceRepository repository, string driveLetter) : base(repository, driveLetter)
        {
        }

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
            var result = string.Empty;
            var msg = "Es konnte kein Ordner ermittelt werden.\n\n" +
                      "Bitte klicken Sie auf 'OK' und wählen Sie anschließend\n" +
                      "den Ordner auf dem Gerät aus, der die Bilder enthält.";
            var btns = MessageBoxButtons.OK;
            var symbol = MessageBoxIcon.Information;

            MessageBox.Show(msg, _TITLE, btns, symbol);

            result = GetPathFromDialog();

            return result;
        }

        private string GetPathFromDialog()
        {
            var path = string.Empty;

            var dialog = new FolderBrowserDialog();
            InitDialog(ref dialog);

            path = dialog.SelectedPath;

            return path;
        }

        private void InitDialog(ref FolderBrowserDialog dialog)
        {
            dialog.Description = "Bildordner des externen Gerätes angeben";
            dialog.ShowNewFolderButton = false;
            dialog.RootFolder = SpecialFolder.MyComputer;
        }
    }
}