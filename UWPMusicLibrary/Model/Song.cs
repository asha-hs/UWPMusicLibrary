using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWPMusicLibrary.Model
{
    public class Song
    {
        public string FileName { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        
        public string FilePath { get; set; }
    }
}
