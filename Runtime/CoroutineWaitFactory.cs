using UnityEngine;
using System.Collections.Generic;


namespace Peg
{
    /// <summary>
    /// Creates and caches a pool of <see cref="UnityEngine.WaitForSeconds" /> objects to avoid unecessary garbage when using corroutines.
    /// </summary>
    public static class CoroutineWaitFactory
    {
        static public readonly WaitForEndOfFrame EndOfFrame = new WaitForEndOfFrame();
        static Dictionary<float, WaitForSeconds> Map = new Dictionary<float, WaitForSeconds>(5);

        /// <summary>
        /// Returns a <see cref="WaitForSeconds"/> object with the given wait time.
        /// If the interval of time does not exist within this factory's internal
        /// cache, one is created and pooled for future use.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static WaitForSeconds RequestWait(float time)
        {
            WaitForSeconds wait;
            if(!Map.TryGetValue(time, out wait))
            {
                wait = new WaitForSeconds(time);
                Map[time] = wait;
            }

            return wait;
        }

    }
}
