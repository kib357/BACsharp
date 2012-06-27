using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace BACSharp.Types
{
    public class BacNetWeeklySchedule
    {
        public ArrayList ValueList
        {
            get { return new ArrayList() {Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday}; }
            set
            {
                if (value.Count == 7)
                {
                    if (value.Cast<object>().Any(day => !(day is Dictionary<BacNetTime, bool?>)))
                        return;
                    Monday = value[0] as Dictionary<BacNetTime, bool?>;
                    Tuesday = value[1] as Dictionary<BacNetTime, bool?>;
                    Wednesday = value[2] as Dictionary<BacNetTime, bool?>;
                    Thursday = value[3] as Dictionary<BacNetTime, bool?>;
                    Friday = value[4] as Dictionary<BacNetTime, bool?>;
                    Saturday = value[5] as Dictionary<BacNetTime, bool?>;
                    Sunday = value[6] as Dictionary<BacNetTime, bool?>;
                }
            }
        }

        public Dictionary<BacNetTime, bool?> Monday { get; set; }
        public Dictionary<BacNetTime, bool?> Tuesday { get; set; }
        public Dictionary<BacNetTime, bool?> Wednesday { get; set; }
        public Dictionary<BacNetTime, bool?> Thursday { get; set; }
        public Dictionary<BacNetTime, bool?> Friday { get; set; }
        public Dictionary<BacNetTime, bool?> Saturday { get; set; }
        public Dictionary<BacNetTime, bool?> Sunday { get; set; }

        public BacNetWeeklySchedule()
        {
            Monday = new Dictionary<BacNetTime, bool?>();
            Tuesday = new Dictionary<BacNetTime, bool?>();
            Wednesday = new Dictionary<BacNetTime, bool?>();
            Thursday = new Dictionary<BacNetTime, bool?>();
            Friday = new Dictionary<BacNetTime, bool?>();
            Saturday = new Dictionary<BacNetTime, bool?>();
            Sunday = new Dictionary<BacNetTime, bool?>();
        }       
    }
}
