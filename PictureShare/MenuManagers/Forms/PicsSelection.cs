using PictureShare.Core.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PictureShare.MenuManagers.Forms
{
    public partial class PicsSelection : Form
    {
        #region Private Fields

        private IEnumerable<ImageEntity> _images;

        #endregion Private Fields

        #region Public Properties

        public IEnumerable<ImageEntity> SelectedImages { get; set; }

        #endregion Public Properties

        #region Public Constructors

        public PicsSelection()
        {
            InitializeComponent();
        }

        public PicsSelection(IEnumerable<ImageEntity> images) : this()
        {
            _images = images;
            SelectedImages = new List<ImageEntity>();
        }

        #endregion Public Constructors

        #region Private Methods

        public bool ThumbnailCallback()
        {
            return true;
        }

        private void PicsSelection_Load(object sender, EventArgs e)
        {
            var imgList = new ImageList();
            imgList.ImageSize = new Size(120, 120);
            imgList.ColorDepth = ColorDepth.Depth32Bit;

            for (int i = 0, max = _images.Count(); i < max; i++)
            {
                var imgEntity = _images.ElementAt(i);
                var img = Image.FromFile($"{imgEntity.Path}\\{imgEntity.Name}");
                imgList.Images.Add(GetThumbnail(imgList.ImageSize.Width, img));
            }

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
            Image tmpImg = null;

            if (img.Width < width && img.Height < width)
            {
                using (var g = Graphics.FromImage(thumb))
                {
                    int xOffset = (int)((width - img.Width) / 2);
                    int yOffset = (int)((width - img.Height) / 2);

                    g.DrawImage(img, xOffset, yOffset, img.Width, img.Height);
                }
            }
            else
            {
                var callback = new Image.GetThumbnailImageAbort(ThumbnailCallback);

                if (img.Width == img.Height)
                {
                    thumb = img.GetThumbnailImage(width, width, callback, IntPtr.Zero);
                }
                else
                {
                    int thumbWidth = 0, xOffset = 0, yOffset = 0;

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

                    using (var g = Graphics.FromImage(thumb))
                    {
                        g.DrawImage(tmpImg, xOffset, yOffset, tmpImg.Width, tmpImg.Height);
                    }
                }
            }

            using (var g = Graphics.FromImage(thumb))
            {
                g.DrawRectangle(Pens.Green, 0, 0, thumb.Width - 1, thumb.Height - 1);
            }

            return thumb;
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

        #endregion Private Methods
    }
}