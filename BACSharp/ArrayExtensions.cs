using System;
using System.Collections;

namespace BACSharp
{
    public static class ArrayExtensions
    {
        public static Array Add(this Array array, object newItem)
        {
            if (array == null) return null;
            var list = new ArrayList();

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
            var list = new ArrayList();

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
