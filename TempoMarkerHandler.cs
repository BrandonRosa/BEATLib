using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEATLib
{
    /// <summary>
    /// Handles all TempoMarkers. Keeps track of the song's tempo and time signature changes.
    /// </summary>
    public class TempoMarkerHandler
    {
        private List<TempoMarker> TempoMarkers = new List<TempoMarker>();
        private int TempoMarkerIndex = 0;
        private int nextTempoMarkerTime = int.MaxValue;

        public int NextTempoMarkerTime { get { return nextTempoMarkerTime; } }
        public TempoMarker CurrentTempoMarker { get { return TempoMarkers[TempoMarkerIndex]; } }

        public TempoMarkerHandler() { }
        public TempoMarkerHandler(TempoMarker tempoMarker) { TempoMarkers.Add(tempoMarker); }
        public TempoMarkerHandler(double BPM, int StartTime = 0, int TimeSignatureNumerator = 4, int TimeSignatureDenominator = 4) { AddTempoMarker(BPM, StartTime, TimeSignatureNumerator, TimeSignatureDenominator); }
        public void AddTempoMarker(double BPM, int StartTime = 0, int TimeSignatureNumerator = 4, int TimeSignatureDenominator = 4)
        {
            TempoMarkers.Add(new TempoMarker(BPM, StartTime, TimeSignatureNumerator, TimeSignatureDenominator));
        }


        /// <summary>
        /// Updates TempoMarkerHandler to the current TempoMarker if needed.
        /// </summary>
        /// <param name="currentTime">The current song time in milliseconds.</param>
        /// <returns>True if updated, false if nothing was changed.</returns>
        public bool UpdateMarkerIndex(int currentTime)
        {
            if (currentTime > nextTempoMarkerTime)
            {
                TempoMarkerIndex++;
                if (TempoMarkerIndex >= TempoMarkers.Count)
                    nextTempoMarkerTime = int.MaxValue;
                else
                    nextTempoMarkerTime = TempoMarkers[TempoMarkerIndex].GetStartTime();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Keeps track of tempo and time signature changes through a song.
        /// </summary>
        public class TempoMarker : IStartTime
        {
            private int StartTime;

            public readonly double BPM;
            public readonly int TimeSignatureNumerator;
            public readonly int TimeSignatureDenominator;

            /// <summary>
            /// Creates a TempoMarker Object with the inputed variables. (Start time 0 is the start of the file)
            /// </summary>
            /// <param name="BPM"></param>
            /// <param name="StartTime"></param>
            /// <param name="TimeSignatureNumerator"></param>
            /// <param name="TimeSignatureDenominator"></param>
            public TempoMarker(double BPM, int StartTime = 0, int TimeSignatureNumerator = 4, int TimeSignatureDenominator = 4)
            {
                this.BPM = BPM;
                this.StartTime = StartTime;
                this.TimeSignatureNumerator = TimeSignatureNumerator;
                this.TimeSignatureDenominator = TimeSignatureDenominator;
            }
            public int GetStartTime() { return StartTime; }
        }
    }
}
