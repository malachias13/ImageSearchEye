using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ImageSearch.Models
{
    public class ImageItemSource
    {
        private List<ImageItem> _imageItem;
        public ImageItemSource()
        {
            _imageItem = new List<ImageItem>();
        }

        public List<ImageItem> GetAllImages()
        {
            var result = _imageItem.ToList<ImageItem>();
            return result;
        }
        public void FillImageList(List<ImageItem> imageData)
        {
            _imageItem = imageData;
        }
    }
}
