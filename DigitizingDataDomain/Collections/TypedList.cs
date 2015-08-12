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
    public class TypedList<T> : TypedIList<T>
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
    }
}
