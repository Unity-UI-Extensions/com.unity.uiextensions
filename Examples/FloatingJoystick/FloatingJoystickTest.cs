/// Credit Tima Zhum

using UnityEngine;

#if CROSS_PLATFORM_INPUT
using UnityStandardAssets.CrossPlatformInput;
#endif

namespace UnityEngine.UI.Extensions
{
	public class FloatingJoystickTest: MonoBehaviour
	{
		#if CROSS_PLATFORM_INPUT

		/// <summary>
		/// Update this instance
		/// </summary>
		void Update()
		{
			float _horizontalAxis = CrossPlatformInputManager.GetAxis("Horizontal");
			float _verticalAxis = CrossPlatformInputManager.GetAxis("Vertical");

			transform.position += 5f * (Vector3.right * _horizontalAxis + Vector3.up * _verticalAxis);
		}

		#endif
	}
}
