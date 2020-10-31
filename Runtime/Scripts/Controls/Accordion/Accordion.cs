///Credit ChoMPHi
///Sourced from - http://forum.unity3d.com/threads/accordion-type-layout.271818/


namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(HorizontalOrVerticalLayoutGroup), typeof(ContentSizeFitter), typeof(ToggleGroup))]
	[AddComponentMenu("UI/Extensions/Accordion/Accordion Group")]
	public class Accordion : MonoBehaviour
	{
		private bool m_expandVertical = true;
		[HideInInspector]
		public bool ExpandVerticval => m_expandVertical;

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

        private void Awake()
        {
			m_expandVertical = GetComponent<HorizontalLayoutGroup>() ? false : true;
			var group = GetComponent<ToggleGroup>();
		}

#if UNITY_EDITOR

        private void OnValidate()
        {
            if (!GetComponent<HorizontalLayoutGroup>() && !GetComponent<VerticalLayoutGroup>())
            {
				Debug.LogError("Accordion requires either a Horizontal or Vertical Layout group to place children");
            }
        }
#endif
	}
}