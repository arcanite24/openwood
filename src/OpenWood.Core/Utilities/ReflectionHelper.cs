using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace OpenWood.Core.Utilities
{
    /// <summary>
    /// Reflection utilities for accessing private game members.
    /// </summary>
    public static class ReflectionHelper
    {
        private static readonly Dictionary<string, FieldInfo> _fieldCache = new Dictionary<string, FieldInfo>();
        private static readonly Dictionary<string, PropertyInfo> _propertyCache = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> _methodCache = new Dictionary<string, MethodInfo>();

        /// <summary>
        /// Get a private field value from an object.
        /// </summary>
        public static T GetField<T>(object obj, string fieldName)
        {
            var type = obj.GetType();
            var key = $"{type.FullName}.{fieldName}";

            if (!_fieldCache.TryGetValue(key, out var field))
            {
                field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                _fieldCache[key] = field;
            }

            return field != null ? (T)field.GetValue(obj) : default;
        }

        /// <summary>
        /// Set a private field value on an object.
        /// </summary>
        public static void SetField(object obj, string fieldName, object value)
        {
            var type = obj.GetType();
            var key = $"{type.FullName}.{fieldName}";

            if (!_fieldCache.TryGetValue(key, out var field))
            {
                field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                _fieldCache[key] = field;
            }

            field?.SetValue(obj, value);
        }

        /// <summary>
        /// Get a private property value from an object.
        /// </summary>
        public static T GetProperty<T>(object obj, string propertyName)
        {
            var type = obj.GetType();
            var key = $"{type.FullName}.{propertyName}";

            if (!_propertyCache.TryGetValue(key, out var prop))
            {
                prop = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                _propertyCache[key] = prop;
            }

            return prop != null ? (T)prop.GetValue(obj) : default;
        }

        /// <summary>
        /// Invoke a private method on an object.
        /// </summary>
        public static object InvokeMethod(object obj, string methodName, params object[] args)
        {
            var type = obj.GetType();
            var key = $"{type.FullName}.{methodName}";

            if (!_methodCache.TryGetValue(key, out var method))
            {
                method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                _methodCache[key] = method;
            }

            return method?.Invoke(obj, args);
        }

        /// <summary>
        /// Get a static field value from a type.
        /// </summary>
        public static T GetStaticField<T>(Type type, string fieldName)
        {
            var field = type.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            return field != null ? (T)field.GetValue(null) : default;
        }

        /// <summary>
        /// Find a component in the scene by type name.
        /// </summary>
        public static Component FindComponentByTypeName(string typeName)
        {
            foreach (var go in UnityEngine.Object.FindObjectsOfType<GameObject>())
            {
                foreach (var comp in go.GetComponents<Component>())
                {
                    if (comp.GetType().Name == typeName)
                    {
                        return comp;
                    }
                }
            }
            return null;
        }
    }
}
