using System;
using UnityEngine;


namespace Peg
{
    public static class GameObjectUtility
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="layer"></param>
        public static void SetLayerRecursively(this GameObject root, int layer)
        {
            SetLayerRecursively(root.transform, layer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="layer"></param>
        public static void SetLayerRecursively(Transform root, int layer)
        {
            root.gameObject.layer = layer;
            for (int i = 0; i < root.childCount; i++)
                SetLayerRecursively(root.GetChild(i), layer);
        }

        /// <summary>
        /// Searches up the transform hierarchy for a specific component.
        /// </summary>
        /// <returns></returns>
        public static T FindComponentInAncestors<T>(this Transform root) where T : Component
        {
            if(root.TryGetComponent<T>(out T comp))
                return comp;

            if (root.parent != null)
                return root.parent.FindComponentInAncestors<T>();

            return null;            
        }

        /// <summary>
        /// Searches up the transform hierarchy for a specific component.
        /// </summary>
        /// <returns></returns>
        public static Component FindComponentInAncestors(this Transform root, Type type)
        {
            if (root.TryGetComponent(type, out Component comp))
                return comp;

            if (root.parent != null)
                return root.parent.FindComponentInAncestors(type);

            return null;
        }
    }
}
