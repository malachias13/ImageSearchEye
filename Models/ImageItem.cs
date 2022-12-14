using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSearch.Models
{
    public class ImageItem
    {
        public string Name { get; set; }
        public string ContentUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public int Id { get; set; }
    }
}
