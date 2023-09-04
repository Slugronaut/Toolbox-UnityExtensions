using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Peg.ToolboxEditor
{
    /// <summary>
    /// Utility for revealing hidden GameObjects in the scene.
    /// </summary>
    public class HiddenGameObjectsEditor : EditorWindow
    {
        [MenuItem("Window/Toolbox/Hidden GameObjects")]
        static void InitWindow()
        {
            var window = GetWindow<HiddenGameObjectsEditor>();
            window.Show();
        }



        List<GameObject> Inspector;
        List<GameObject> Hierarchy;
        List<GameObject> Both;

        bool MustRestore;

        private void OnEnable()
        {
            Inspector = new List<GameObject>();
            Hierarchy = new List<GameObject>();
            Both = new List<GameObject>();
        }

        public void OnGUI()
        {
            if(GUILayout.Button("Reveal Hidden GameObjects") && !MustRestore)
            {
                MustRestore = true;
                Inspector.Clear();
                Hierarchy.Clear();
                Both.Clear();

                float per = 0;
                EditorUtility.DisplayProgressBar("Getting all GameObjects", "Please wait..", per);

                var list = (GameObject.FindObjectsOfType<GameObject>());

                for(int i = 0; i < list.Length; i++)
                {
                    per = (float)i / (float)list.Length;
                    EditorUtility.DisplayProgressBar("Getting all GameObjects", "Please wait..", per);
                    GameObject go = list[i];

                    int hideFlags = (int)go.hideFlags;
                    int h1 = (int)HideFlags.HideInHierarchy;
                    int h2 = (int)HideFlags.HideInInspector;
                    if ((hideFlags & h1) != 0 && (hideFlags & h2) != 0)
                    {
                        Both.Add(go);
                        go.hideFlags ^= (HideFlags.HideInHierarchy | HideFlags.HideInInspector);
                    }
                    else if ((hideFlags & h1) != 0)
                    {
                        Hierarchy.Add(go);
                        go.hideFlags ^= HideFlags.HideInHierarchy;
                    }
                    else if ((hideFlags & h2) != 0)
                    {
                        Inspector.Add(go);
                        go.hideFlags ^= HideFlags.HideInInspector;
                    }
                }

                EditorUtility.ClearProgressBar();
            }
            
            if(GUILayout.Button("Restore Revealed GameObjects") && MustRestore)
            {
                EditorUtility.DisplayProgressBar("Getting all GameObjects", "Please wait..", 0.5f);
                MustRestore = false;

                foreach (var go in Both)
                    go.hideFlags ^= (HideFlags.HideInHierarchy | HideFlags.HideInInspector);
                foreach (var go in Hierarchy)
                    go.hideFlags ^= HideFlags.HideInHierarchy;
                foreach (var go in Inspector)
                    go.hideFlags ^= HideFlags.HideInInspector;

                EditorUtility.ClearProgressBar();
            }
        }
    }
}
