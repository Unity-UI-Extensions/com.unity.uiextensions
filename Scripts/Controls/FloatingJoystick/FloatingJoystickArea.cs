/// Credit Tima Zhum

using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(Image))]
	[AddComponentMenu("Scripts/UnityEngine.UI.Extensions/FloatingJoystickArea")]
	public class FloatingJoystickArea: MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
	{		
		/// <summary>
		/// The floating joystick controller
		/// </summary>
		[Tooltip("The floating joystick that should appear in the area on tap")]
		[SerializeField] private FloatingJoystick _floatingJoystick = null;

		#if CROSS_PLATFORM_INPUT

		/// <summary>
		/// Raises the pointer down event
		/// </summary>
		/// <param name="_ped">Ped</param>
		public void OnPointerDown(PointerEventData _ped) 
		{
			if (_floatingJoystick) _floatingJoystick.OnPointerDownHelper(_ped);
		}

		/// <summary>
		/// Raises the pointer up event
		/// </summary>
		/// <param name="_ped">Ped</param>
		public void OnPointerUp(PointerEventData _ped) 
		{
			if (_floatingJoystick) _floatingJoystick.OnPointerUpHelper(_ped);
		}

		/// <summary>
		/// Raises the drag event
		/// </summary>
		/// <param name="_ped">Ped</param>
		public void OnDrag(PointerEventData _ped)
		{
			if (_floatingJoystick) _floatingJoystick.OnDragHelper(_ped);
		}

		#else

		public void OnDrag(PointerEventData data) {}
		public void OnPointerUp(PointerEventData data) {}
		public void OnPointerDown(PointerEventData data) {}

		#endif
	}
}