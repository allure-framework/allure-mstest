using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace MSTestAllureAdapter
{
    public static class Serializer
    {
        /// <summary>
        /// Class TypeCache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class TypeCache<T>
        {
            private readonly ReaderWriterLockSlim _locker = new ReaderWriterLockSlim();

            private readonly Dictionary<Type, T> _cache = new Dictionary<Type, T>();

            /// <summary>
            /// Gets the state.
            /// </summary>
            /// <param name="type">The type.</param>
            /// <param name="createInstance">The create instance.</param>
            /// <returns>`0.</returns>
            public T GetState(Type type, Func<T> createInstance)
            {
                T match;
                _locker.EnterUpgradeableReadLock();
                try
                {
                    if (_cache.TryGetValue(type, out match) == false)
                    {
                        _locker.EnterWriteLock();
                        try
                        {
                            if (_cache.TryGetValue(type, out match) == false)
                            {
                                match = createInstance();
                                _cache[type] = match;
                            }
                        }
                        finally
                        {
                            _locker.ExitWriteLock();
                        }
                    }
                }
                finally
                {
                    _locker.ExitUpgradeableReadLock();
                }
                return match;
            }
        }

        /// <summary>
        /// Class XML.
        /// </summary>
        public static class XML
        {
            private static readonly TypeCache<System.Xml.Serialization.XmlSerializer> Cache = new TypeCache<System.Xml.Serialization.XmlSerializer>();

            /// <summary>
            /// Serializes the specified object.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="obj">The object.</param>
            /// <returns>System.String.</returns>
            public static string Serialize<T>(T obj) where T : class
            {
                if (obj == null) return string.Empty;
                var serializer = Cache.GetState(typeof(T), () => new System.Xml.Serialization.XmlSerializer(typeof(T)));
                using (var writer = new StringWriter())
                {
                    serializer.Serialize(writer, obj);
                    return writer.ToString();
                }
            }

            /// <summary>
            /// Deserializes the specified string.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="str">The string.</param>
            /// <returns>``0.</returns>
            public static T Deserialize<T>(string str)
            {
                if (string.IsNullOrEmpty(str)) return default(T);
                var serializer = Cache.GetState(typeof(T), () => new System.Xml.Serialization.XmlSerializer(typeof(T)));
                return (T)serializer.Deserialize(new StringReader(str));

            }
        }
    }
}