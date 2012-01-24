using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BACSharp
{
    public static class ArrayExtensions
    {
        public static Array Add(this Array array, object newItem)
        {
            if (array == null) return null;
            ArrayList list = new ArrayList();

            Type t = typeof(object);
            foreach (var item in array)
            {
                t = item.GetType();
                list.Add(item);
            }
            list.Add(newItem);
            return list.ToArray(t);
        }

        public static Array Add(this Array array, object newItem, Type type)
        {
            ArrayList list = new ArrayList();

            if (array != null)
                foreach (var item in array)
                {
                    list.Add(item);
                }
            list.Add(newItem);
            return list.ToArray(type);
        }
    }
}
