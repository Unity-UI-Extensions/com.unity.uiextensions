/// Credit Tima Zhum
/// Based on Joystick.cs from Unity Standard Assets/Cross Platform Input

using UnityEngine.EventSystems;

#if CROSS_PLATFORM_INPUT
using UnityStandardAssets.CrossPlatformInput;
#endif

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(Image))]
	[AddComponentMenu("Scripts/UnityEngine.UI.Extensions/FloatingJoystick")]
	public class FloatingJoystick: MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
	{		
		/// <summary>
		/// The joystick base point image
		/// </summary>
		[Tooltip("Pivot of a joystick (not required)")]
		[SerializeField] private Image _joystickBasePoint = null;

		#if CROSS_PLATFORM_INPUT

		/// <summary>
		/// The joystick image
		/// </summary>
		private Image _joystickImage = null;

		/// <summary>
		/// The joystick enabled state
		/// </summary>
		private bool _joystickEnabled = false;

		/// <summary>
		/// The previous joystick enabled state
		/// </summary>
		private bool _joystickPreviouslyEnabled = false;

		/// <summary>
		/// The last point event data
		/// </summary>
		private PointerEventData _pedLast = null;

		/// <summary>
		/// Start this instance
		/// </summary>
		protected void Awake()
		{
			_joystickImage = GetComponent<Image>();
		}

		/// <summary>
		/// Update this instance
		/// </summary>
		protected void Update()
		{		
			if (_joystickImage) _joystickImage.enabled = _joystickEnabled;
			if (_joystickBasePoint) _joystickBasePoint.enabled = _joystickEnabled;

			if (_joystickEnabled)
			{
				if (!_joystickPreviouslyEnabled)
				{
					Vector3 _position = _pedLast.position;
					ReinitializeAt(_position);
					if (_joystickBasePoint) _joystickBasePoint.transform.position = _position;
				}
			}

			_joystickPreviouslyEnabled = _joystickEnabled;
		}

		/// <summary>
		/// Raises the pointer down event
		/// </summary>
		/// <param name="_ped">Ped</param>
		public void OnPointerDownHelper(PointerEventData _ped) 
		{
			OnPointerDown(_ped);
			_joystickEnabled = true;
			_pedLast = _ped;
		}

		/// <summary>
		/// Raises the pointer up event
		/// </summary>
		/// <param name="_ped">Ped</param>
		public void OnPointerUpHelper(PointerEventData _ped) 
		{
			OnPointerUp(_ped);
			_joystickEnabled = false;
			_pedLast = _ped;
		}

		/// <summary>
		/// Raises the drag event
		/// </summary>
		/// <param name="_ped">Ped</param>
		public void OnDragHelper(PointerEventData _ped)
		{
			OnDrag(_ped);
			_pedLast = _ped;
		}

		/// <summary>
		/// Reinitializes the joystick at specific position
		/// </summary>
		/// <param name="_position">Position</param>
		public void ReinitializeAt(Vector3 _position)
		{
			transform.position = _position;
			Start();
			OnDisable();
			OnEnable();
		}

		#region STANDARDASSETSCODE

		/*
		 * This is a copy of the Joystick.cs script from Unity Standard Assets/Cross Platform Input
		 * The reason of copying: original script is not designed for inheriting (required for extending the functionalities)
		 */

		public enum AxisOption
		{
			// Options for which axes to use
			Both, // Use both
			OnlyHorizontal, // Only horizontal
			OnlyVertical // Only vertical
		}

		private int InitialMovementRange;
		public int MovementRange = 100;
		public AxisOption axesToUse = AxisOption.Both; // The options for the axes that the still will use
		public string horizontalAxisName = "Horizontal"; // The name given to the horizontal axis for the cross platform input
		public string verticalAxisName = "Vertical"; // The name given to the vertical axis for the cross platform input

		Vector3 m_StartPos;
		bool m_UseX; // Toggle for using the x axis
		bool m_UseY; // Toggle for using the Y axis
		CrossPlatformInputManager.VirtualAxis m_HorizontalVirtualAxis; // Reference to the joystick in the cross platform input
		CrossPlatformInputManager.VirtualAxis m_VerticalVirtualAxis; // Reference to the joystick in the cross platform input


		void OnEnable()
		{
			CreateVirtualAxes();
		}

		void Start()
		{
			m_StartPos = transform.position;
		}

		void UpdateVirtualAxes(Vector3 value)
		{
			var delta = m_StartPos - value;
			delta.y = -delta.y;
			delta /= MovementRange;
			if (m_UseX)
			{
				m_HorizontalVirtualAxis.Update(-delta.x);
			}

			if (m_UseY)
			{
				m_VerticalVirtualAxis.Update(delta.y);
			}
		}

		void CreateVirtualAxes()
		{
			// set axes to use
			m_UseX = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyHorizontal);
			m_UseY = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyVertical);

			// create new axes based on axes to use
			if (m_UseX)
			{
				m_HorizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(horizontalAxisName);
				CrossPlatformInputManager.RegisterVirtualAxis(m_HorizontalVirtualAxis);
			}
			if (m_UseY)
			{
				m_VerticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(verticalAxisName);
				CrossPlatformInputManager.RegisterVirtualAxis(m_VerticalVirtualAxis);
			}
		}


		public void OnDrag(PointerEventData data)
		{
			Vector3 newPos = Vector3.zero;

			if (m_UseX)
			{
				int delta = (int)(data.position.x - m_StartPos.x);
				delta = Mathf.Clamp(delta, - MovementRange, MovementRange);
				newPos.x = delta;
			}

			if (m_UseY)
			{
				int delta = (int)(data.position.y - m_StartPos.y);
				delta = Mathf.Clamp(delta, -MovementRange, MovementRange);
				newPos.y = delta;
			}

			transform.position = new Vector3(m_StartPos.x + newPos.x, m_StartPos.y + newPos.y, m_StartPos.z + newPos.z);
			UpdateVirtualAxes(transform.position);
		}


		public void OnPointerUp(PointerEventData data)
		{
			transform.position = m_StartPos;
			UpdateVirtualAxes(m_StartPos);
		}


		public void OnPointerDown(PointerEventData data) { }

		void OnDisable()
		{
			// remove the joysticks from the cross platform input
			if (m_UseX)
			{
				m_HorizontalVirtualAxis.Remove();
			}
			if (m_UseY)
			{
				m_VerticalVirtualAxis.Remove();
			}
		}			

		#endregion

		#else

		public void OnDrag(PointerEventData data) {}
		public void OnPointerUp(PointerEventData data) {}
		public void OnPointerDown(PointerEventData data) {}

		#endif
	}		
}