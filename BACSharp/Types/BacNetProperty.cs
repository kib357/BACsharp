using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BACSharp.Types
{
    public class BacNetProperty
    {
        public BacNetUInt PropertyId { get; set; }
        public ArrayList Values { get; set; }

        public string Value
        {
            get 
            { 
                string res = string.Empty;
                foreach (var value in Values)
                {
                    if (res != string.Empty)
                        res += " ";
                    res += value.ToString();
                }
                return res;
            }
            set { Values = new ArrayList() {value}; }
        }

        public BacNetProperty()
        {
            InitializeFields();
        }

        public BacNetProperty(uint id)
        {
            InitializeFields();
            PropertyId.Value = id;
        }

        private void InitializeFields()
        {
            Values = new ArrayList();
            PropertyId = new BacNetUInt();
        }
    }
}
