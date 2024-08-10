using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;

namespace BEATLib
{
    public abstract class BEATEventHandlerBase: ICollection<BEATEvent>
    {
        /// <summary>
        /// Is the collection that holds all of the BEATEvents.
        /// </summary>
        /// <remarks>
        /// It is reccomended that an effecient collection is used for performance.
        /// </remarks>
        protected abstract ICollection<BEATEvent> FullList { get; set; }

        /// <summary>
        /// A BEATEvent making the start of a song (not included in file)
        /// </summary>
        

        public int Count => ((ICollection<BEATEvent>)FullList).Count;

        public bool IsReadOnly => ((ICollection<BEATEvent>)FullList).IsReadOnly;

        public BEATEventHandlerBase()
        {

        }
        public BEATEventHandlerBase(string fileName)
        {
            LoadBEATFile(fileName);
        }

        public BEATEventHandlerBase(FileInfo file)
        {
            LoadBEATFile(file);
        }

        public abstract void LoadBEATFile(string fileName);

        public abstract void SaveBeatFile(string fileName);

        protected virtual void LoadBEATFile(FileInfo file)
        {
            LoadBEATFile(file.FullName);
        }

        public virtual void AddBEATEventLast(BEATEvent BEvent)
        {
            FullList.Add(BEvent);
        }

        public virtual void AddBEATEvent(int StartTime, int Duration, int TrackID, int ObjectID, int ActionID, string ActionDetails = "", int GroupID = -1)
        {
            AddBEATEventLast(new BEATEvent(StartTime, Duration, TrackID, ObjectID, ActionID, ActionDetails, GroupID));
        }


        public abstract bool UpdateTime(int time);

        public void Add(BEATEvent item)
        {
            ((ICollection<BEATEvent>)FullList).Add(item);
        }

        public void Clear()
        {
            ((ICollection<BEATEvent>)FullList).Clear();
        }

        public bool Contains(BEATEvent item)
        {
            return ((ICollection<BEATEvent>)FullList).Contains(item);
        }

        public void CopyTo(BEATEvent[] array, int arrayIndex)
        {
            ((ICollection<BEATEvent>)FullList).CopyTo(array, arrayIndex);
        }

        public bool Remove(BEATEvent item)
        {
            return ((ICollection<BEATEvent>)FullList).Remove(item);
        }

        public IEnumerator<BEATEvent> GetEnumerator()
        {
            return ((IEnumerable<BEATEvent>)FullList).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)FullList).GetEnumerator();
        }

       
    }

    public class BEATEvent : IStartTime, IDuration, IComparable<BEATEvent>, IFormattable, IEquatable<BEATEvent>
    {
        public static BEATEvent GetStartTemplate(string version, string gameName, string songName="", int offset=0, int songLength=0) { return new BEATEvent(0, 0, -1, 0, 0, version + "-" + gameName+"-"+songName+"-"+offset+"-"+songLength); }
        public static BEATEvent GetEndTemplate(int endTime) { return new BEATEvent(endTime, 0, -1, 1, 1); }
        public static BEATEvent GetTempoTemplate(double BPM = 120, string TimeSignature = "4/4") { return new BEATEvent(0, 0, -2, 0, 0, BPM + "-" + TimeSignature); }

        private readonly int StartTime;
        private readonly int Duration;

        public readonly int TrackID;
        public readonly int ObjectID;
        public readonly int ActionID;
        public readonly string ActionDetails;
        public readonly int GroupID;

        public event Action<BEATEvent> StartEvent;
        public event Action<BEATEvent> EndEvent;
        public event Action<BEATEvent> DuringEvent;

        /// <summary>
        /// Invokes StartEvent.
        /// </summary>
        public void RaiseStartEvent()
        {
            StartEvent?.Invoke(this);
        }

        /// <summary>
        /// Invokes EndEvent.
        /// </summary>
        public void RaiseEndEvent()
        {
            EndEvent?.Invoke(this);
        }

        /// <summary>
        /// Invokes DuringEvent.
        /// </summary>
        public void RaiseDuringEvent()
        {
            DuringEvent?.Invoke(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="StartTime">the start time of the Event in milliseconds.</param>
        /// <param name="Duration">the duration of the Event in milliseconds</param>
        /// <param name="TrackID">the track the Event is played on. Pureley visual.</param>
        /// <param name="ObjectID">the unique identifier of the Event on the track.</param>
        /// <param name="ActionID">the action this event will invoke.</param>
        /// <param name="ActionDetails">details of the action.</param>
        /// <param name="GroupID">the ID that visually groups objects with the same time and duration.</param>
        public BEATEvent(int StartTime, int Duration, int TrackID, int ObjectID, int ActionID, string ActionDetails = "", int GroupID = -1)
        {
            this.StartTime = StartTime;
            this.Duration = Duration;
            this.TrackID = TrackID;
            this.ObjectID = ObjectID;
            this.ActionID = ActionID;
            this.ActionDetails = ActionDetails;
            this.GroupID = GroupID;
        }

        public int GetStartTime()
        {
            return StartTime;
        }

        public int GetTotalDuration()
        {
            return Duration;
        }

        public int GetEndTime()
        {
            return StartTime + Duration;
        }

        public float GetDurationPercent(int currentTime)
        {
            return (float)(currentTime - StartTime) / ((float)Duration);
        }

        public int GetElaspedTime(int currentTime)
        {
            return (currentTime - StartTime);
        }

        public int GetRemainingTime(int currentTime)
        {
            return Duration - (currentTime - StartTime);
        }

        public int CompareTo(BEATEvent other)
        {
            return StartTime.CompareTo(other.StartTime);
        }

        public bool Equals(BEATEvent other)
        {
            return this.StartTime.Equals(other.StartTime) && this.Duration.Equals(other.Duration) && this.TrackID.Equals(other.TrackID) && this.ObjectID.Equals(other.ObjectID) 
                && this.ActionID.Equals(other.ActionID) && this.ActionDetails.Equals(other.ActionDetails) && this.GroupID.Equals(other.GroupID);
        }

        public override string ToString()
        {
            return ToString("PrintAll",null);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (String.IsNullOrEmpty(format)) format = "PrintAll";

            switch (format)
            {
                case "PrintAll":
                    return $"[Start:{StartTime} Duration:{Duration} Track:{TrackID} ID:{ObjectID} Action:{ActionID}{(ActionDetails!=""?" ActionDetails:"+ActionDetails:"")}{(GroupID != -1 ? " GroupID:" + GroupID : "")}]";
                default:
                    throw new FormatException(String.Format("The {0} format string is not supported.", format));
            }
        }
    }
}