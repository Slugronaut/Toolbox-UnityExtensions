using System.Collections;
using UnityEngine;

/*
namespace Toolbox
{
    /// <summary>
    /// Helper class for starting coroutines that are static methods.
    /// The coroutine handle cannot be obtained from this.
    /// </summary>
    public class StaticCoroutine : GlobalSingletonMonoBehaviour<StaticCoroutine>
    {
        public override void AutoSingletonInit()
        {
        }

        protected override void SingletonAwake()
        {
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        IEnumerator PerformCoroutine(IEnumerator coroutine)
        {
            yield return StartCoroutine(coroutine);
        }

        /// <summary>
        /// Starts a coroutine that is defined as a static method.
        /// </summary>
        /// <param name="coroutine"></param>
        public static void AwaitCoroutine(IEnumerator coroutine)
        {
            Instance.StartCoroutine(Instance.PerformCoroutine(coroutine));
        }

        /// <summary>
        /// Starts a coroutine that is defined as a static method.
        /// </summary>
        /// <param name="coroutine"></param>
        public static Coroutine DoCoroutine(IEnumerator coroutine)
        {
            return Instance.StartCoroutine(coroutine);
        }
    }
}
*/