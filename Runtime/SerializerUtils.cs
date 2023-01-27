using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using System.IO;
using UnityEngine.Assertions;

namespace Toolbox
{
    /// <summary>
    /// Utilities for handling serialization of objects.
    /// </summary>
    public static class SerializerUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="comp"></param>
        /// <param name="type"></param>
        public static void SerializeComponentToDisk(Component comp, Type type, string savePath)
        {
            byte[] bytes = null;
            List<UnityEngine.Object> list = null;
            UnitySerializationUtility.SerializeUnityObject(comp, ref bytes, ref list, DataFormat.JSON, true);

            File.WriteAllBytes(savePath, bytes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comp"></param>
        /// <param name="type"></param>
        public static bool DeserializedComponentFromDisk(Component comp, Type type, string filePath)
        {
            var bytes = File.ReadAllBytes(filePath);
            if(comp == null)
            {
                Debug.LogError("Null component reference in Singleton Deserializer");
                return false;
            }
            if (bytes == null)
            {
                Debug.LogError("Null byte stream in Singleton Deserializer");
                return false;
            }
            List<UnityEngine.Object> list = null;
            UnitySerializationUtility.DeserializeUnityObject(comp, ref bytes, ref list, DataFormat.JSON);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comp"></param>
        /// <param name="type"></param>
        public static bool DeserializedComponent(Component comp, Type type, byte[] rawBytes)
        {
            Assert.IsNotNull(comp);
            Assert.IsNotNull(rawBytes);

            List<UnityEngine.Object> list = null;
            UnitySerializationUtility.DeserializeUnityObject(comp, ref rawBytes, ref list, DataFormat.JSON);
            return true;
        }

    }
}
