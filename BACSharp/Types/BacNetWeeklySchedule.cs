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
                    if (value.Cast<object>().Any(day => !(day is Dictionary<BacNetTime, object>)))
                        return;
                    Monday = value[0] as Dictionary<BacNetTime, object>;
                    Tuesday = value[1] as Dictionary<BacNetTime, object>;
                    Wednesday = value[2] as Dictionary<BacNetTime, object>;
                    Thursday = value[3] as Dictionary<BacNetTime, object>;
                    Friday = value[4] as Dictionary<BacNetTime, object>;
                    Saturday = value[5] as Dictionary<BacNetTime, object>;
                    Sunday = value[6] as Dictionary<BacNetTime, object>;
                }
            }
        }

        public Dictionary<BacNetTime, object> Monday { get; set; }
        public Dictionary<BacNetTime, object> Tuesday { get; set; }
        public Dictionary<BacNetTime, object> Wednesday { get; set; }
        public Dictionary<BacNetTime, object> Thursday { get; set; }
        public Dictionary<BacNetTime, object> Friday { get; set; }
        public Dictionary<BacNetTime, object> Saturday { get; set; }
        public Dictionary<BacNetTime, object> Sunday { get; set; }

        public BacNetWeeklySchedule()
        {
            Monday = new Dictionary<BacNetTime, object>();
            Tuesday = new Dictionary<BacNetTime, object>();
            Wednesday = new Dictionary<BacNetTime, object>();
            Thursday = new Dictionary<BacNetTime, object>();
            Friday = new Dictionary<BacNetTime, object>();
            Saturday = new Dictionary<BacNetTime, object>();
            Sunday = new Dictionary<BacNetTime, object>();
        }       
    }
}
