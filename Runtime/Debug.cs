using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolbox
{
    /// <summary>
    /// Utility class with several additional debug drawing methods not found in unity.
    /// </summary>
    public static partial class TbDebug
    {
        public static void DrawBounds(Bounds b)
        {
            DrawBounds(b.center, b, Color.white, 0, false);
        }

        public static void DrawBounds(Bounds b, Color color)
        {
            DrawBounds(b.center, b, color, 0, false);
        }

        public static void DrawBounds(Bounds b, Color color, float duration)
        {
            DrawBounds(b.center, b, color, duration, false);
        }

        public static void DrawBounds(Bounds b, Color color, float duration, bool depthTest)
        {
            DrawBounds(b.center, b, color, duration, depthTest);
        }

        public static void DrawBounds(Vector3 position, Bounds b)
        {
            DrawBounds(position, b, Color.white, 0, false);
        }
        
        public static void DrawBounds(Vector3 position, Bounds b, Color color)
        {
            DrawBounds(position, b, color, 0, false);
        }
        
        public static void DrawBounds(Vector3 position, Bounds b, Color color, float duration)
        {
            DrawBounds(position, b, color, duration, false);
        }
        
        public static void DrawBounds(Vector3 position, Bounds b, Color color, float duration, bool depthTest)
        {
            float hw = b.extents.x;
            float hl = b.extents.z;
            float h = b.size.y;
            //define all bottom corners of the bounds
            Vector3 cen = position - new Vector3(0, b.extents.y, 0);
            Vector3 c1 = cen - new Vector3(-hw, 0, -hl);
            Vector3 c2 = cen - new Vector3(hw, 0, -hl);
            Vector3 c3 = cen - new Vector3(hw, 0, hl);
            Vector3 c4 = cen - new Vector3(-hw, 0, hl);

            //draw verticals
            Vector3 up = Vector3.up * h;
            Debug.DrawRay(c1, up, color, duration, depthTest);
            Debug.DrawRay(c2, up, color, duration, depthTest);
            Debug.DrawRay(c3, up, color, duration, depthTest);
            Debug.DrawRay(c4, up, color, duration, depthTest);

            //draw bottom horizontals
            Debug.DrawLine(c1, c2, color, duration, depthTest);
            Debug.DrawLine(c2, c3, color, duration, depthTest);
            Debug.DrawLine(c3, c4, color, duration, depthTest);
            Debug.DrawLine(c4, c1, color, duration, depthTest);

            //draw top horizontals
            Debug.DrawLine(c1 + up, c2 + up, color, duration, depthTest);
            Debug.DrawLine(c2 + up, c3 + up, color, duration, depthTest);
            Debug.DrawLine(c3 + up, c4 + up, color, duration, depthTest);
            Debug.DrawLine(c4 + up, c1 + up, color, duration, depthTest);
        }
    }
}
