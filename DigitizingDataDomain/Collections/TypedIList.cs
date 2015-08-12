using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using System.Collections;

namespace DigitizingDataDomain.Collections
{
    public class TypedIList<T> : IList<T>, ITypedList
    {
        #region ITypedList Implementation
        //
        // ITypedList Implementation
        //

        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            if (listAccessors == null || listAccessors.Length <= 0)
            {
                // return the properties of items on
                // this list
                return GetTypeProperties(typeof(T));
            }
            else
            {
                // return the properties of the specified member
                // this is needed when the list is used in master/detail
                // structures to ensure the child grid can figure out
                // the types contained herein.
                string memberName = listAccessors[0].Name;
                PropertyInfo pinfo = typeof(T).GetProperty(memberName);
                if (pinfo != null)
                {
                    Type type = pinfo.PropertyType;
                    if (typeof(IList).IsAssignableFrom(type) && type.IsGenericType)
                    {
                        // if it is a generic list, find the first generic type and
                        // assume that's the type of the list contents
                        // a hack, but it could be worse :)
                        Type paramType = type.GetGenericArguments()[0];
                        return GetTypeProperties(paramType);
                    }
                    return GetTypeProperties(type);
                }
                return null;
            }
        }

        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            return string.Format("List of {0}", typeof(T).Name);
        }

        #endregion // ITypedList Implementation

        private PropertyDescriptorCollection GetTypeProperties(Type type)
        {
            TypeDescriptionProvider provider = TypeDescriptor.GetProvider(type);
            return provider.GetTypeDescriptor(type).GetProperties();
        }

        private IList<T> wrappedIListT;

        public IList<T> WrappedIListT
        {
            get
            {
                return wrappedIListT;
            }
            set
            {
                wrappedIListT = value;
            }
        }

        #region IList<T> Members

        int IList<T>.IndexOf(T item)
        {
            return wrappedIListT.IndexOf(item);
        }

        void IList<T>.Insert(int index, T item)
        {
            wrappedIListT.Insert(index, item);
        }

        void IList<T>.RemoveAt(int index)
        {
            wrappedIListT.RemoveAt(index);
        }

        T IList<T>.this[int index]
        {
            get
            {
                return wrappedIListT[index];
            }
            set
            {
                wrappedIListT[index] = value;
            }
        }

        #endregion

        #region ICollection<T> Members

        void ICollection<T>.Add(T item)
        {
            wrappedIListT.Add(item);
        }

        void ICollection<T>.Clear()
        {
            wrappedIListT.Clear();
        }

        bool ICollection<T>.Contains(T item)
        {
            return wrappedIListT.Contains(item);
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            wrappedIListT.CopyTo(array, arrayIndex);
        }

        int ICollection<T>.Count
        {
            get { return wrappedIListT.Count; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return wrappedIListT.IsReadOnly; }
        }

        bool ICollection<T>.Remove(T item)
        {
            return wrappedIListT.Remove(item);
        }

        #endregion

        #region IEnumerable<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return wrappedIListT.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return wrappedIListT.GetEnumerator();
        }

        #endregion
    }
}
