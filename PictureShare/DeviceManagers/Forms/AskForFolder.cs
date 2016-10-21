//  Copyright (c) 2016 Thomas Ohms
//
//  This file is part of PictureShare.
//
//  Get the original code from https://github.com/thomas4U/PictureShare
//
//  PictureShare is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  Foobar is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with Foobar.  If not, see <http://www.gnu.org/licenses/>.

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