/**********************************************
* Pantagruel
* Copyright 2015-2017 James Clark
**********************************************/
using UnityEngine;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pantagruel.Serializer
{
    /// <summary>
    /// Public shared constants of Pantagruel.
    /// </summary>
    public static class Constants
    {
#if UNITY_EDITOR
#pragma warning disable CS0169 // The field 'Constants.CachedRootPath' is never used
        /// <summary>
        /// This is the root path used by all edit-time asset build tools - specifically the manifest generator.
        /// </summary>
        private static string CachedRootPath;
#pragma warning restore CS0169 // The field 'Constants.CachedRootPath' is never used

        public static string RootPath
        {
            get
            {
                //need to do a little detective work to figure out
                //where the root of Pantagruel is actually located.
                var root = AssetDatabase.FindAssets("\"Pantagruel Serializer Root\"");
                if (root == null || root.Length < 1) throw new UnityException("Cannot locate the root folder for Panatgruel's serializer!");

                string path = AssetDatabase.GUIDToAssetPath(root[0]);
                path = path.Remove(0, "Assets/".Length);
                int i = path.LastIndexOf("/");
                if(i < path.Length-1) path = path.Remove(i + 1);
                //Debug.Log("<color=green>" + path + "</color>");
                return path;
            }
        }

        /// <summary>
        /// Path to store the resource manifests when the library is built at edit time.
        /// </summary>
        public static readonly string ManifestPath = RootPath + "Resources/Singletons/";

        
#endif
        
        
    }
}
