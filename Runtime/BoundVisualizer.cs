/*
#if UNITY_EDITOR
using UnityEngine;
using Sirenix.OdinInspector;

namespace Toolbox
{
    /// <summary>
    /// Stupidly simple component used to visiualize an object's bounds int he editor.
    /// </summary>
    public class BoundVisualizer : MonoBehaviour
    {
        public enum ShapeType
        {
            Cube,
            Sphere,
        }
        [Tooltip("Should the bounding shape be centered on the transform position or rest on top of it?")]
        public bool CenterBounds = true;
        public ShapeType Shape;
        [Indent]
        [ShowIf("IsSphere")]
        public float Radius = 4;
        [Indent]
        [ShowIf("IsBox")]
        public Vector3 Size = Vector3.one * 4;
        public Vector3 Offset;
        public Color color = new Color(1, 0.2f, 0, 1);

        bool IsBox => Shape == ShapeType.Cube;
        bool IsSphere => Shape == ShapeType.Sphere;

        void OnDrawGizmos()
        {
            Gizmos.color = new Color(color.r, color.g, color.b, 0.25f);
            float size = Radius;
            if (Shape == ShapeType.Cube)
            {
                Gizmos.DrawCube(Offset + transform.position + (CenterBounds ? Vector3.zero : (Vector3.up * (size / 2))), Size);
                Gizmos.color = color;
                Gizmos.DrawWireCube(Offset + transform.position + (CenterBounds ? Vector3.zero : (Vector3.up * (size / 2))), Size);
            }
            else if(Shape == ShapeType.Sphere)
            {
                Gizmos.DrawSphere(Offset + transform.position + (CenterBounds ? Vector3.zero : (Vector3.up * (size / 2))), size/2);
                Gizmos.color = color;
                Gizmos.DrawWireSphere(Offset + transform.position + (CenterBounds ? Vector3.zero : (Vector3.up * (size / 2))), size/2);
            }
        }
    }
}
#endif
*/