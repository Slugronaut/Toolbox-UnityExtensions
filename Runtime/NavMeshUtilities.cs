using Toolbox.Math;
using UnityEngine;
using UnityEngine.AI;

namespace Toolbox
{ 
    /// <summary>
    /// Simple utilities class for Unity's built-in NavMesh.
    /// </summary>
    public class NavMeshUtilities
    {
        /// <remarks>
        /// For the caller of this method, it should be assumed that a value of zero did not occur
        /// naturally and implies failure to produce a valid result. In such a case it is advisable
        /// to either retry or perform another action since it will definately *not* look random
        /// to be running to (0,0,0) with any kind of frequency.
        /// </remarks>
        /// <param name="center"></param>
        /// <param name="maxRange"></param>
        /// <param name="sampleRate"></param>
        /// <param name="sampleSize"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool RandomPoint(Vector3 center, float minRange, float maxRange, int sampleRate, float sampleSize, out Vector3 result, int areaMask = NavMesh.AllAreas)
        {
            for (int i = 0; i < sampleRate; i++)
            {
                Vector3 randomPoint = center + (UnityEngine.Random.insideUnitSphere.normalized * UnityEngine.Random.Range(minRange, maxRange));
                if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, sampleSize, areaMask))
                {
                    result = hit.position;
                    return true;
                }
            }

            //this should imply 'failure' to produce valid result and be ignored by caller
            result = center;
            return false;
        }

        /// <remarks>
        /// For the caller of this method, it should be assumed that a value of zero did not occur
        /// naturally and implies failure to produce a valid result. In such a case it is advisable
        /// to either retry or perform another action since it will definately *not* look random
        /// to be running to (0,0,0) with any kind of frequency.
        /// 
        /// This version provides a limiter for range on the y-axis. Useful for finding ranges around a target
        /// that still remain relatively close in height on the y-axis.
        /// </remarks>
        /// <param name="center"></param>
        /// <param name="maxRange"></param>
        /// <param name="heightRange"></param>
        /// <param name="sampleRate"></param>
        /// <param name="sampleSize"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool RandomPoint(Vector3 center, float minRange, float maxRange, float heightRange, int sampleRate, float sampleSize, out Vector3 result, int areaMask = NavMesh.AllAreas)
        {
            for (int i = 0; i < sampleRate; i++)
            {
                var sphereRange = Random.insideUnitSphere.normalized * Random.Range(minRange, maxRange);
                sphereRange.y = MathUtils.ConvertRange(sphereRange.y, -maxRange, maxRange, -heightRange, heightRange);
                Vector3 randomPoint = center + sphereRange;
                if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, sampleSize, areaMask))
                {
                    result = hit.position;
                    return true;
                }
            }

            //this should imply 'failure' to produce valid result and be ignored by caller
            result = center;
            return false;
        }

        /// <remarks>
        /// For the caller of this method, it should be assumed that a value of zero did not occur
        /// naturally and implies failure to produce a valid result. In such a case it is advisable
        /// to either retry or perform another action since it will definately *not* look random
        /// to be running to (0,0,0) with any kind of frequency.
        /// </remarks>
        /// <param name="center"></param>
        /// <param name="maxRange"></param>
        /// <param name="sampleRate"></param>
        /// <param name="sampleSize"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool RandomOnScreenPoint(Vector3 center, float minRange, float maxRange, int sampleRate, float sampleSize, Camera cam, float xSafe, float ySafe, out Vector3 result, int areaMask = NavMesh.AllAreas)
        {
            for (int i = 0; i < sampleRate; i++)
            {
                Vector3 randomPoint = center + (UnityEngine.Random.insideUnitSphere.normalized * UnityEngine.Random.Range(minRange, maxRange));
                Debug.DrawRay(randomPoint, Vector3.up, Color.green, 3.0f);
                if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, sampleSize, areaMask))
                {
                    if (Math.MathUtils.IsInViewport(cam, hit.position, xSafe, ySafe))
                    {
                        result = hit.position;
                        return true;
                    }
                }
            }

            //this should imply 'failure' to produce valid result and be ignored by caller
            result = center;
            return false;
        }

        /// <remarks>
        /// For the caller of this method, it should be assumed that a value of zero did not occur
        /// naturally and implies failure to produce a valid result. In such a case it is advisable
        /// to either retry or perform another action since it will definately *not* look random
        /// to be running to (0,0,0) with any kind of frequency.
        /// </remarks>
        /// <param name="center"></param>
        /// <param name="maxRange"></param>
        /// <param name="sampleRate"></param>
        /// <param name="sampleSize"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool RandomOnScreenPoint(Vector3 center, float minRange, float maxRange, float heightRange, int sampleRate, float sampleSize, Camera cam, float xSafe, float ySafe, out Vector3 result, int areaMask = NavMesh.AllAreas)
        {
            for (int i = 0; i < sampleRate; i++)
            {
                var sphereRange = Random.insideUnitSphere.normalized * Random.Range(minRange, maxRange);
                sphereRange.y = MathUtils.ConvertRange(sphereRange.y, -maxRange, maxRange, -heightRange, heightRange);
                Vector3 randomPoint = center + sphereRange;
                Debug.DrawRay(randomPoint, Vector3.up, Color.green, 3.0f);
                if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, sampleSize, areaMask))
                {
                    if (Math.MathUtils.IsInViewport(cam, hit.position, xSafe, ySafe))
                    {
                        result = hit.position;
                        return true;
                    }
                }
            }

            //this should imply 'failure' to produce valid result and be ignored by caller
            result = center;
            return false;
        }

        /// <summary>
        /// Checks to see if the given point is on a navmesh.
        /// </summary>
        /// <param name="center">The center of the point to check.</param>
        /// <param name="Radius">A radius around the point to check for navmeshes.</param>
        /// <returns></returns>
        public static bool IsPointOnNavMesh(Vector3 center, float radius, int areaMask = NavMesh.AllAreas)
        {
            NavMeshHit hit;
            return NavMesh.SamplePosition(center, out hit, radius, areaMask);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="dest"></param>
        /// <returns></returns>
        public static bool HasNavLos(Vector3 center, Vector3 dest)
        {
            UnityEngine.AI.NavMeshHit hit;
            return !UnityEngine.AI.NavMesh.Raycast(center, dest, out hit, UnityEngine.AI.NavMesh.AllAreas);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="dest"></param>
        /// <returns></returns>
        public static Vector3 GetClosest(Vector3 center, Vector3 dest)
        {
            return NavMesh.Raycast(center, dest, out UnityEngine.AI.NavMeshHit hit, UnityEngine.AI.NavMesh.AllAreas) ? hit.position : dest;
        }
    }
}