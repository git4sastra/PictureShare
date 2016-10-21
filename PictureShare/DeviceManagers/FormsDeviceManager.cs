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

using PictureShare.Core.Data.Structure;
using System.Threading;
using System.Windows.Forms;

namespace PictureShare.DeviceManagers
{
    public sealed class FormsDeviceManager : DefaultDeviceManager
    {
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