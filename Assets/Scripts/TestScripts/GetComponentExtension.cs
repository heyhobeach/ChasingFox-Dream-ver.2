using System.Collections.Generic;
using UnityEngine;

namespace MyUtiles
{
    public static class GetComponentExtension
    {
        public static T GetInterface<T>(this GameObject go) where T : class
        {
            if(!typeof(T).IsInterface) return default(T);
            return go.GetComponent(typeof(T)) as T;
        }
        public static IEnumerable<T> GetInterfaces<T>(this GameObject go) where T : class
        {
            if(!typeof(T).IsInterface) return null;
            return go.GetComponents(typeof(T)) as IEnumerable<T>;
        }
    }
}
