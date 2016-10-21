//  Copyright (c) 2016 Thomas Ohms
//
//  This file is part of PictureShare.
//
//  Get the original code from https://thomas4u.github.io/PictureShare/
//
//  PictureShare is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  PictureShare is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with PictureShare.  If not, see <http://www.gnu.org/licenses/>.

using PictureShare.Core.Data;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PictureShare.MenuManagers.Forms
{
    public partial class FormsMenu : Form
    {
        #region Fields

        private IEnumerable<Button> _buttons;
        private IEnumerable<ModuleEntity> _modules;

        #endregion Fields

        #region Constructors

        public FormsMenu()
        {
            InitializeComponent();
        }

        public FormsMenu(IEnumerable<ModuleEntity> modules) : this()
        {
            _modules = modules;
            btnLoad.Enabled = false;
        }

        #endregion Constructors

        #region Properties

        public bool DontShowAgain { get; set; }

        #endregion Properties

        #region Methods

        private void btnLoad_Click(object sender, System.EventArgs e)
        {
            DontShowAgain = cbDontShow.Checked;
        }

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

        #endregion Methods
    }
}