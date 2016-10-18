using PictureShare.Core.Data;
using PictureShare.Lib;
using PictureShare.MenuManagers.Forms;
using System;
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
                    return;

                var selModTag = form.Tag.ToString();

                SelectedModule = AvailableModules.FirstOrDefault(am => am.FullName == selModTag);

                if (form.DontShowAgain)
                    SetDontShowAgainConfig();
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

        private void SetDontShowAgainConfig()
        {
            // In Registrierung eintragen
            throw new NotImplementedException();
        }

        #endregion Private Methods
    }
}