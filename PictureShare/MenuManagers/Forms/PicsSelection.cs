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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PictureShare.MenuManagers.Forms
{
    public partial class PicsSelection : Form
    {
        #region Fields

        private IEnumerable<ImageEntity> _images;

        #endregion Fields

        #region Constructors

        public PicsSelection()
        {
            InitializeComponent();
        }

        public PicsSelection(IEnumerable<ImageEntity> images) : this()
        {
            _images = images;
            SelectedImages = new List<ImageEntity>();
        }

        #endregion Constructors

        #region Properties

        public IEnumerable<ImageEntity> SelectedImages { get; set; }

        #endregion Properties

        #region Methods

        public bool ThumbnailCallback()
        {
            return true;
        }

        private static void CreateThumbnailImage(int width, Image img, Image.GetThumbnailImageAbort callback, ref Image tmpImg, ref int thumbWidth, ref int xOffset, ref int yOffset)
        {
            if (img.Width < img.Height)
            {
                thumbWidth = width * img.Width / img.Height;
                tmpImg = img.GetThumbnailImage(thumbWidth, width, callback, IntPtr.Zero);
                xOffset = (width - thumbWidth) / 2;
            }

            if (img.Width > img.Height)
            {
                thumbWidth = width * img.Height / img.Width;
                tmpImg = img.GetThumbnailImage(width, thumbWidth, callback, IntPtr.Zero);
                yOffset = (width - thumbWidth) / 2;
            }
        }

        private static void DrawThumbnailForSmallImage(int width, Image img, Image thumb)
        {
            using (var g = Graphics.FromImage(thumb))
            {
                int xOffset = (int)((width - img.Width) / 2);
                int yOffset = (int)((width - img.Height) / 2);

                g.DrawImage(img, xOffset, yOffset, img.Width, img.Height);
            }
        }

        private void AddImagesToList(ImageList imgList)
        {
            for (int i = 0, max = _images.Count(); i < max; i++)
            {
                var imgEntity = _images.ElementAt(i);
                Image img;

                using (var stream = new FileStream($"{imgEntity.Path}\\{imgEntity.Name}", FileMode.Open, FileAccess.Read))
                {
                    img = Image.FromStream(stream);
                }

                imgList.Images.Add(GetThumbnail(imgList.ImageSize.Width, img));

                img.Dispose();
                img = null;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var items = imgListView.Items;
            var imgList = new List<ImageEntity>();

            for (int i = 0, max = items.Count; i < max; i++)
            {
                var item = items[i];

                if (!item.Checked)
                    continue;

                var imgEntity = (ImageEntity)item.Tag;
                imgEntity.Album = txtAlbumName.Text;
                imgList.Add(imgEntity);
            }

            SelectedImages = imgList;
        }

        private Image DrawThumbnailForBigImage(int width, Image img, Image thumb)
        {
            var callback = new Image.GetThumbnailImageAbort(ThumbnailCallback);
            Image tmpImg = null;

            if (img.Width == img.Height)
            {
                thumb = img.GetThumbnailImage(width, width, callback, IntPtr.Zero);
            }
            else
            {
                int thumbWidth = 0, xOffset = 0, yOffset = 0;

                CreateThumbnailImage(width, img, callback, ref tmpImg, ref thumbWidth, ref xOffset, ref yOffset);

                using (var g = Graphics.FromImage(thumb))
                {
                    g.DrawImage(tmpImg, xOffset, yOffset, tmpImg.Width, tmpImg.Height);
                }
            }

            return thumb;
        }

        private void FillListViewWithImages(ImageList imgList)
        {
            for (int i = 0, max = _images.Count(); i < max; i++)
            {
                var img = _images.ElementAt(i);
                imgListView.Items.Add($"{img.Name}");
                imgListView.Items[i].ImageIndex = i;
                imgListView.Items[i].Tag = img;
            }

            imgListView.LargeImageList = imgList;
        }

        private Image GetThumbnail(int width, Image img)
        {
            Image thumb = new Bitmap(width, width);

            if (img.Width < width && img.Height < width)
            {
                DrawThumbnailForSmallImage(width, img, thumb);
            }
            else
            {
                thumb = DrawThumbnailForBigImage(width, img, thumb);
            }

            using (var g = Graphics.FromImage(thumb))
            {
                g.DrawRectangle(Pens.Green, 0, 0, thumb.Width - 1, thumb.Height - 1);
            }

            return thumb;
        }

        private void PicsSelection_Load(object sender, EventArgs e)
        {
            var imgList = new ImageList();
            imgList.ImageSize = new Size(120, 120);
            imgList.ColorDepth = ColorDepth.Depth32Bit;

            AddImagesToList(imgList);

            FillListViewWithImages(imgList);
        }

        #endregion Methods
    }
}