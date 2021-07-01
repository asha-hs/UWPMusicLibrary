using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace UWPMusicLibrary.Model
{
    public class MusicCollection
    {
        public string Name { get; set; }
        public ImageSource CoverArtThumbnail { get; set; }

        public string CoverPhotoFilePath { get; set; }
    }
}
