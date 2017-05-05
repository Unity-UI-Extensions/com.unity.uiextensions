///Credit ChoMPHi
///Sourced from - http://forum.unity3d.com/threads/accordion-type-layout.271818/

using System.Collections;

namespace UnityEngine.UI.Extensions.Tweens
{
    // Tween runner, executes the given tween.
    // The coroutine will live within the given 
    // behaviour container.
    internal class TweenRunner<T> where T : struct, ITweenValue
	{
		protected MonoBehaviour m_CoroutineContainer;
		protected IEnumerator m_Tween;
		
		// utility function for starting the tween
		private static IEnumerator Start(T tweenInfo)
		{
			if (!tweenInfo.ValidTarget())
				yield break;
			
			float elapsedTime = 0.0f;
			while (elapsedTime < tweenInfo.duration)
			{
				elapsedTime += tweenInfo.ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
				var percentage = Mathf.Clamp01 (elapsedTime / tweenInfo.duration);
				tweenInfo.TweenValue (percentage);
				yield return null;
			}
			tweenInfo.TweenValue (1.0f);
			tweenInfo.Finished();
		}
		
		public void Init(MonoBehaviour coroutineContainer)
		{
			m_CoroutineContainer = coroutineContainer;
		}
		
		public void StartTween(T info)
		{
			if (m_CoroutineContainer == null)
			{
				Debug.LogWarning ("Coroutine container not configured... did you forget to call Init?");
				return;
			}
			
			if (m_Tween != null)
			{
				m_CoroutineContainer.StopCoroutine (m_Tween);
				m_Tween = null;
			}
			
			if (!m_CoroutineContainer.gameObject.activeInHierarchy)
			{
				info.TweenValue(1.0f);
				return;
			}
			
			m_Tween = Start (info);
			m_CoroutineContainer.StartCoroutine (m_Tween);
		}
	}
}