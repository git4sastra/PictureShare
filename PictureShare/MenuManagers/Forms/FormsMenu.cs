using PictureShare.Core.Data;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PictureShare.MenuManagers.Forms
{
    public partial class FormsMenu : Form
    {
        #region Private Fields

        private IEnumerable<Button> _buttons;
        private IEnumerable<ModuleEntity> _modules;

        #endregion Private Fields

        #region Public Constructors

        public FormsMenu()
        {
            InitializeComponent();
        }

        public FormsMenu(IEnumerable<ModuleEntity> modules) : this()
        {
            _modules = modules;
            btnLoad.Enabled = false;
        }

        #endregion Public Constructors

        #region Public Properties

        public bool DontShowAgain { get; set; }

        #endregion Public Properties

        #region Private Methods

        private Button CreateButton(ModuleEntity module)
        {
            var button = new Button();

            button.Text = module.Name;
            button.Height = 50;
            button.Width = mainPanel.Width - 1;
            button.BackColor = Color.LightGray;
            button.Cursor = Cursors.Hand;
            button.Tag = module.FullName;
            button.FlatStyle = FlatStyle.Popup;

            button.Click += ModuleButton_Click;
            button.MouseHover += ModuleButton_MouseHover;
            button.MouseLeave += ModuleButton_MouseLeave;

            var oldFont = button.Font;
            var newFont = new Font(oldFont.FontFamily, 14, FontStyle.Regular);
            button.Font = newFont;

            return button;
        }

        private void FormsMenu_Load(object sender, System.EventArgs e)
        {
            var buttons = new List<Button>();

            for (int i = 0, max = _modules.Count(); i < max; i++)
            {
                var module = _modules.ElementAt(i);
                var button = CreateButton(module);

                buttons.Add(button);
            }

            _buttons = buttons;

            mainPanel.Controls.AddRange(_buttons.ToArray());
        }

        private void ModuleButton_Click(object sender, System.EventArgs e)
        {
            var btn = sender as Button;

            btn.BackColor = Color.Coral;
            btnLoad.Enabled = true;

            Tag = btn.Tag;

            foreach (var b in _buttons)
            {
                if (b.Tag != Tag)
                    b.BackColor = Color.CornflowerBlue;
            }
        }

        private void ModuleButton_MouseHover(object sender, System.EventArgs e)
        {
            var btn = sender as Button;
            btn.BackColor = Color.Coral;
        }

        private void ModuleButton_MouseLeave(object sender, System.EventArgs e)
        {
            var btn = sender as Button;

            if (btn.Tag != Tag)
                btn.BackColor = Color.LightGray;
        }

        #endregion Private Methods

        #region Methods

        private void btnLoad_Click(object sender, System.EventArgs e)
        {
            DontShowAgain = cbDontShow.Checked;
        }

        #endregion Methods
    }
}