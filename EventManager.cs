using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEATLib
{
    public class EventManager
    {

        //Static Variables
        public static string version = "0.0.1";

        //Private Variables
        private readonly int songLength;
        private readonly int offset;
        private TempoMarkerHandler tempoMarkerHandler;
        private BEATEventHandlerBase BEATEventHandler;

        //Public Variables

        ///<summary>
        ///The total length of the song (in milliseconds).
        ///</summary>
        public int SongLength
        {
            get { return songLength; }
        }

        ///<summary>
        ///The time in milliseconds from the start of the song to the start of the BEAT file.
        ///</summary>
        public int Offset
        {
            get { return offset; }
        }


        public EventManager()
        {

        }
    }

}
