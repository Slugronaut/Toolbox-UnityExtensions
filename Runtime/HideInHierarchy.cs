using UnityEngine;


namespace Peg
{
    /// <summary>
    /// Utility component that can be used in the editor to
    /// hide objects that were accidentally revealed.
    /// </summary>
    [ExecuteInEditMode]
    public class HideInHierarchy : MonoBehaviour
    {
        //[Sirenix.OdinInspector.Button("HideNow", Sirenix.OdinInspector.ButtonSizes.Small)]
        public void HideNow()
        {
            if (!Application.isPlaying)
            {
                gameObject.hideFlags = gameObject.hideFlags | HideFlags.HideInHierarchy;
                //DestroyImmediate(this);
            }
            else Debug.Log("HideInHierarchy is meant to be used at edit-time only.");
        }

#if UNITY_EDITOR
        void Update()
        {
            if(!Application.isPlaying)
            {
                if (((int)gameObject.hideFlags & (int)HideFlags.HideInHierarchy) != 0)
                    DestroyImmediate(this);
            }
        }
#endif
    }
}
