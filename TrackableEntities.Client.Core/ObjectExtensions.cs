using System.Reflection;
using System.ArrayExtensions;
using TrackableEntities.Common.Core;
using System.Diagnostics.CodeAnalysis;

namespace System
{
    /// <summary>
    /// Common extension methods for object cloning Trackable Entities.
    /// </summary>
    public static class ObjectExtensions
    {
        [NotNull]
        private static readonly MethodInfo? CloneMethod = typeof(object).GetMethod(nameof(MemberwiseClone), BindingFlags.NonPublic | BindingFlags.Instance);

        /// <summary>
        /// Determines if a type is a primitive type or string.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>true when type is primitive or string</returns>
        public static bool IsPrimitive(this Type type)
        {
            if (type == typeof(string)) return true;
            return (type.IsValueType & type.IsPrimitive);
        }

        /// <summary>
        /// creates a deep copy of an object
        /// </summary>
        /// <param name="originalObject"></param>
        /// <returns></returns>
        public static object? Copy(this object? originalObject)
        {
            return InternalCopy(originalObject, new Dictionary<object, object>(new ReferenceEqualityComparer()));
        }
        /// <summary>
        /// returns a copy of all changes to an object
        /// </summary>
        /// <param name="originalObject"></param>
        /// <returns></returns>
        public static object? CopyChanges(this object? originalObject)
        {
            return InternalCopy(originalObject, new Dictionary<object, object>(new ReferenceEqualityComparer()), true);
        }

        private static object? InternalCopy(object? originalObject, IDictionary<object, object> visited, bool changesOnly = false)
        {
            if (originalObject == null) return null;
            var typeToReflect = originalObject.GetType();
            if (IsPrimitive(typeToReflect)) return originalObject;
            if (visited.TryGetValue(originalObject, out var result)) return result;
            if (typeof(Delegate).IsAssignableFrom(typeToReflect)) return null;            
            var cloneObject = CloneMethod.Invoke(originalObject, null) ?? new object();
            if (typeToReflect.IsArray)
            {
                var arrayType = typeToReflect.GetElementType();
                if (arrayType != null && IsPrimitive(arrayType) == false)
                {
                    if (changesOnly && (cloneObject is ITrackable[] trackedArray))
                    {
                        var trimmedObject = (object)trackedArray.Where(t => t != null && t.TrackingState != TrackingState.Unchanged).ToArray();
                        Array clonedArray = (Array)trimmedObject;
                        clonedArray.ForEach((array, indices) => array.SetValue(InternalCopy(clonedArray.GetValue(indices), visited, changesOnly), indices));
                    }
                    else
                    {
                        Array clonedArray = (Array)cloneObject;
                        clonedArray.ForEach((array, indices) => array.SetValue(InternalCopy(clonedArray.GetValue(indices), visited, changesOnly), indices));
                    }
                }

            }
            visited.Add(originalObject, cloneObject);
            CopyFields(originalObject, visited, cloneObject, typeToReflect, changesOnly: changesOnly);
            RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect, changesOnly);
            return cloneObject;
        }

        private static void RecursiveCopyBaseTypePrivateFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect, bool changesOnly = false)
        {
            if (typeToReflect.BaseType != null)
            {
                RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect.BaseType, changesOnly);
                CopyFields(originalObject, visited, cloneObject, typeToReflect.BaseType, BindingFlags.Instance | BindingFlags.NonPublic, info => info.IsPrivate, changesOnly);
            }
        }

        private static void CopyFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect, 
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy, 
            Func<FieldInfo, bool>? filter = null, bool changesOnly = false)
        {
            foreach (FieldInfo fieldInfo in typeToReflect.GetFields(bindingFlags))
            {
                if (filter != null && filter(fieldInfo) == false) continue;
                if (IsPrimitive(fieldInfo.FieldType)) continue;
                var originalFieldValue = fieldInfo.GetValue(originalObject);
                var clonedFieldValue = InternalCopy(originalFieldValue, visited, changesOnly);
                fieldInfo.SetValue(cloneObject, clonedFieldValue);
            }
        }

        /// <summary>
        /// Creates a deep copy of an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original"></param>
        /// <returns></returns>
        public static T? Copy<T>(this T original) => (T?)Copy((object?)original);
        /// <summary>
        /// Creates a copy of all changes to an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original"></param>
        /// <returns></returns>
        public static T? CopyChanges<T>(this T original) => (T?)CopyChanges((object?)original);        
    }

    /// <summary>
    /// Compares objects by their hash code or reference.
    /// </summary>
    public class ReferenceEqualityComparer : EqualityComparer<Object>
    {
        /// <summary>
        /// Compares by reference
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override bool Equals(object? x, object? y)
        {
            return ReferenceEquals(x, y);
        }

        /// <summary>
        /// compares by hashcode
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override int GetHashCode(object? obj)
        {
            if (obj == null) return 0;
            return obj.GetHashCode();
        }
    }

    namespace ArrayExtensions
    {
        /// <summary>
        /// Extension methods for arrays
        /// </summary>
        public static class ArrayExtensions
        {
            /// <summary>
            /// apply action to all elements of an array
            /// </summary>
            /// <param name="array"></param>
            /// <param name="action"></param>
            public static void ForEach(this Array array, Action<Array, int[]> action)
            {
                if (array.LongLength == 0) return;
                ArrayTraverse walker = new(array);
                do action(array, walker.Position);
                while (walker.Step());
            }
        }

        internal class ArrayTraverse
        {
            public int[] Position;
            private readonly int[] maxLengths;

            public ArrayTraverse(Array array)
            {
                maxLengths = new int[array.Rank];
                for (int i = 0; i < array.Rank; ++i)
                {
                    maxLengths[i] = array.GetLength(i) - 1;
                }
                Position = new int[array.Rank];
            }

            public bool Step()
            {
                for (int i = 0; i < Position.Length; ++i)
                {
                    if (Position[i] < maxLengths[i])
                    {
                        Position[i]++;
                        for (int j = 0; j < i; j++)
                        {
                            Position[j] = 0;
                        }
                        return true;
                    }
                }
                return false;
            }
        }
    }

}