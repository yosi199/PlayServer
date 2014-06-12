using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayServer.Files
{
    /// <summary>
    /// A class to become a json object to pass to client by the server.
    /// contains song information
    /// </summary>
    class Song
    {

        public string artistName { get; set; }
        public string albumName { get; set; }
        public string titleName { get; set; }
        public string pathInfo { get; set; }
        public int indexPos { get; set; }
        public string messageType { get; set; }


        public Song(string artist, string album, string title, string path, int index)
        {
            messageType = "song";
            artistName = artist;
            albumName = album;
            titleName = title;
            pathInfo = path;
            indexPos = index;
        }
    }
}
