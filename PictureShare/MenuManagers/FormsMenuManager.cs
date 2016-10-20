using Microsoft.Win32;
using PictureShare.Core.Data;
using PictureShare.Lib;
using PictureShare.MenuManagers.Forms;
using System.Linq;
using System.Windows.Forms;

namespace PictureShare.MenuManagers
{
    public class FormsMenuManager : DefaultMenuManager
    {
        #region Public Constructors

        public FormsMenuManager(DeviceEntity device) : base(device)
        {
        }

        #endregion Public Constructors

        #region Public Methods

        public override void ShowMenu()
        {
            using (var form = new FormsMenu(AvailableModules))
            {
                var result = form.ShowDialog();

                if (result != DialogResult.OK)
                {
                    SetDontShowAgainConfig(form.DontShowAgain);
                    return;
                }

                var selModTag = form.Tag.ToString();

                SelectedModule = AvailableModules.FirstOrDefault(am => am.FullName == selModTag);

                SetDontShowAgainConfig(form.DontShowAgain);
            }

            ShowPicsSelectionMenu();
        }

        public override void ShowPicsSelectionMenu()
        {
            using (var form = new PicsSelection(Images))
            {
                var result = form.ShowDialog();

                if (result != DialogResult.OK)
                    return;

                Images = form.SelectedImages;
            }

            SaveImages();

            AskForDeletion();
        }

        #endregion Public Methods

        #region Private Methods

        private void AskForDeletion()
        {
            var result = MessageBox.Show("Sollen alle Bilder auf dem externen Geräte gelöscht werden?", "",
                                         MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            DeleteImages();
        }

        private void SetDontShowAgainConfig(bool isChecked)
        {
            var regKey = Registry.CurrentUser.OpenSubKey(@"Software\PictureShare", true);
            var value = isChecked ? "0" : "1";
            regKey.SetValue("AutoloadMenu", value, RegistryValueKind.String);
        }

        #endregion Private Methods
    }
}