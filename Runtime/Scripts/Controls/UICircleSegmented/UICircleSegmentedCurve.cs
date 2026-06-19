///Credit Dmitry (mitay-walle)
///Sourced from - https://github.com/mitay-walle/com.mitay-walle.ui-circle-segmented

using UnityEditor;

namespace UnityEngine.UI.Extensions
{
	public class UICircleSegmentedCurve : MonoBehaviour
	{
		[SerializeField] AnimationCurve _curve;
		[SerializeField] UICircleSegmented _circle;
		void Update()
		{
			if (_curve.length > 0 && _circle)
			{
				
				_circle.FillAmount = _curve.Evaluate(Time.realtimeSinceStartup+10000);
				SetDirty();
			}
		}

		void SetDirty()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				EditorUtility.SetDirty(_circle);
			}
#endif
		}
	}
}