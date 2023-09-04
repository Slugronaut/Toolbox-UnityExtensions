using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Peg
{
    /// <summary>
    /// Can be invoked to force the Editor to pause.
    /// </summary>
    public class BreakHandler : MonoBehaviour
    {
        public void PauseEditor()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPaused = true;
#endif
        }
    }
}