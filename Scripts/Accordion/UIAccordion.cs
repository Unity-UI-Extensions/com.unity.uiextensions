///Credit ChoMPHi
///Sourced from - http://forum.unity3d.com/threads/accordion-type-layout.271818/

using UnityEngine;
using System.Collections;

namespace UnityEngine.UI
{
	[RequireComponent(typeof(VerticalLayoutGroup)), RequireComponent(typeof(ContentSizeFitter)), RequireComponent(typeof(ToggleGroup))]
	public class UIAccordion : MonoBehaviour {
		
		public enum Transition
		{
			Instant,
			Tween
		}
		
		[SerializeField] private Transition m_Transition = Transition.Instant;
		[SerializeField] private float m_TransitionDuration = 0.3f;
		
		/// <summary>
		/// Gets or sets the transition.
		/// </summary>
		/// <value>The transition.</value>
		public Transition transition
		{
			get { return this.m_Transition; }
			set { this.m_Transition = value; }
		}
		
		/// <summary>
		/// Gets or sets the duration of the transition.
		/// </summary>
		/// <value>The duration of the transition.</value>
		public float transitionDuration
		{
			get { return this.m_TransitionDuration; }
			set { this.m_TransitionDuration = value; }
		}
	}
}