//Thanks to Benblo
//https://gist.github.com/benblo/10732554

using System;
using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GoMap
{
	public class GORoutine
	{

		public bool finished = false;

		public static GORoutine start( IEnumerator _routine, MonoBehaviour owner)
		{
			if (Application.isEditor) {
			
				GORoutine coroutine = new GORoutine(_routine);
				coroutine.start();
				return coroutine;
			} else {

				owner.StartCoroutine (_routine);
				return null;
			}
		}

		readonly IEnumerator routine;
		GORoutine( IEnumerator _routine )
		{
			routine = _routine;
		}

		void start()
		{
			//Debug.Log("start");
			#if UNITY_EDITOR
			EditorApplication.update += update;
			#endif
		}
		public void stop()
		{
			#if UNITY_EDITOR
			EditorApplication.update -= update;
			#endif
		}

		void update()
		{
			/* NOTE: no need to try/catch MoveNext,
			 * if an IEnumerator throws its next iteration returns false.
			 * Also, Unity probably catches when calling EditorApplication.update.
			 */

			if (!routine.MoveNext())
			{
				finished = true;
				stop();
			}
		}


		public IEnumerator WaitFor()
		{
			while(!finished)
			{
				yield return null;
			}
		}
	}
}
