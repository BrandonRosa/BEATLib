using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEATLib
{
    interface IDuration
    {
        int GetEndTime();
        int GetTotalDuration();
        float GetDurationPercent(int currentTime);
        int GetElaspedTime(int currentTime);
        int GetRemainingTime(int currentTime);
    }
}
