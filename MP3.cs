using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareLib17
{

    public class MP3
    {

        TagLib.File mp3Tag;

        public TagLib.File Tag { get { return mp3Tag; } }


        public bool valid
        {
            get
            {
                return mp3Tag != null;
            }
        }

        public MP3(String FileName)
        {
            try
            {
                mp3Tag = TagLib.File.Create(FileName);
            }
            catch { }
        }

        public TimeSpan TimeDuration()
        {
            if (!valid)
                return new TimeSpan();
            return mp3Tag.Properties.Duration;
        }

        public string Album
        {
            get { return mp3Tag.Tag.Album; }
            set { mp3Tag.Tag.Album = value; }
        }

        public string Title
        {
            get { return mp3Tag.Tag.Title; }
            set { mp3Tag.Tag.Title = value; }
        }

        public void Save()
        {
            mp3Tag.Save();
        }

        public void SetAlbum(string album)
        {
            Album = album;
            Save();
        }

    }

}
