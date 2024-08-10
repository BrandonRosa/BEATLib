using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEATLib.BEATEventHandlers
{
    /// <summary>
    /// Handles Playback and Editing of BEAT Files.
    /// </summary>
    public class BEATEventPlayEditHandler : BEATEventHandlerBase
    {
        protected override ICollection<BEATEvent> FullList { get; set; } = new LinkedList<BEATEvent>();
        private LinkedListNode<BEATEvent> previousEvent;
        private LinkedList<BEATEvent> inProgressList= new LinkedList<BEATEvent>();

        public LinkedListNode<BEATEvent> Start;
        public LinkedListNode<BEATEvent> End;

        public BEATEventPlayEditHandler()
        {
            ((LinkedList<BEATEvent>)FullList).AddLast(BEATEvent.GetStartTemplate("1.0.0", "test"));
            Start = ((LinkedList<BEATEvent>)FullList).First;
            ((LinkedList<BEATEvent>)FullList).AddLast(BEATEvent.GetEndTemplate(0));
            End = ((LinkedList<BEATEvent>)FullList).Last;
        }

        public override void LoadBEATFile(string fileName)
        {
            FullList = new LinkedList<BEATEvent>(BEATFile.LoadBEATEvents(fileName));
            Start= ((LinkedList<BEATEvent>)FullList).First;
            End= ((LinkedList<BEATEvent>)FullList).Last;
            previousEvent = ((LinkedList<BEATEvent>)FullList).First;
        }

        public override void SaveBeatFile(string fileName)
        {
            BEATFile.SaveBEATEvents(fileName, ((LinkedList<BEATEvent>)FullList).ToList());
        }

        public override void AddBEATEventLast(BEATEvent BEvent)
        {
            ((LinkedList<BEATEvent>)FullList).RemoveLast();
            ((LinkedList<BEATEvent>)FullList).AddLast(BEvent);
            ((LinkedList<BEATEvent>)FullList).AddLast(BEATEvent.GetEndTemplate(BEvent.GetEndTime()));
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        /// <returns>true if time is within song range.</returns>
        public override bool UpdateTime(int time)
        {
            //Check if the song has ended yet [this catches cases where it somehow continues after it ends]
            if(previousEvent.Value==End.Value) return false;
            
            //If the time is ON or PAST a start event, raise the event and add to progress list if needed
            while (hasStartEvent(time))
            {
                if (previousEvent.Value == End.Value)
                    break;

                previousEvent.Value.RaiseStartEvent();
                if(previousEvent.Value.GetTotalDuration() >0)
                    inProgressList.AddLast(previousEvent.Value);
            }

            //for (LinkedListNode<BEATEvent> currentNode=inProgressList.First; currentNode!=null;currentNode=currentNode.Next)
            for (LinkedListNode<BEATEvent> currentNode = inProgressList.First; currentNode != null;)
            {
                LinkedListNode<BEATEvent> nextNode = currentNode.Next;

                //Checks if the event in progress is over
                if (currentNode.Value.GetEndTime() <= time)
                {
                     

                    currentNode.Value.RaiseEndEvent();

                    //Removes the current node then patches up the list
                    //LinkedListNode<BEATEvent> removeNode = currentNode;
                    //currentNode = currentNode.Previous;
                    //inProgressList.Remove(removeNode);

                    // Removes the current node from the list
                    inProgressList.Remove(currentNode);
                }
                else
                {
                    currentNode.Value.RaiseDuringEvent();
                }

                // Move to the next node
                currentNode = nextNode;
            }

            //Check if the song has ended yet [this is the fist place where itl return false, doing it like this will let the in progress events resolve]
            if (previousEvent.Value == End.Value) return false;

            return true;
        }

        public void Seek(int time)
        {
            LinkedListNode<BEATEvent> current = ((LinkedList<BEATEvent>)FullList).First;
            inProgressList.Clear();

            //Start at the beggining of the song, then traverse until you reach an event with an end time bigger on the same number as time
            while (current.Value.GetEndTime() < time)
            {
                current = current.Next;
            }

            while(current.Value.GetStartTime() < time)
            {
                inProgressList.AddLast((LinkedListNode<BEATEvent>)current);
            }

            previousEvent = current.Previous;

            UpdateTime(time);

        }

        public void Reset()
        {
            previousEvent = ((LinkedList<BEATEvent>)FullList).First;
        }

        private bool hasStartEvent(int time)
        {
            //If time is PAST or ON the next startTime return true and change the previousEvent
            if (time >= previousEvent.Next.Value.GetStartTime())
            {
                previousEvent = previousEvent.Next;
                return true;
            }
            return false;
        }
    }
}
