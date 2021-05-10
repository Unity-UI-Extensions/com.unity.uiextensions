///Credit ChoMPHi
///Sourced from - http://forum.unity3d.com/threads/accordion-type-layout.271818/

using System;
using UnityEngine.UI.Extensions.Tweens;

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(RectTransform), typeof(LayoutElement))]
	[AddComponentMenu("UI/Extensions/Accordion/Accordion Element")]
	public class AccordionElement : Toggle
	{

		[SerializeField] private float m_MinHeight = 18f;

		public float MinHeight => m_MinHeight;

		[SerializeField] private float m_MinWidth = 40f;

		public float MinWidth => m_MinWidth;

		private Accordion m_Accordion;
		private RectTransform m_RectTransform;
		private LayoutElement m_LayoutElement;
		
		[NonSerialized]
		private readonly TweenRunner<FloatTween> m_FloatTweenRunner;
		
		protected AccordionElement()
		{
			if (this.m_FloatTweenRunner == null)
				this.m_FloatTweenRunner = new TweenRunner<FloatTween>();
			
			this.m_FloatTweenRunner.Init(this);
		}
		
		protected override void Awake()
		{
			base.Awake();
			base.transition = Transition.None;
			base.toggleTransition = ToggleTransition.None;
			this.m_Accordion = this.gameObject.GetComponentInParent<Accordion>();
			this.m_RectTransform = this.transform as RectTransform;
			this.m_LayoutElement = this.gameObject.GetComponent<LayoutElement>();
			this.onValueChanged.AddListener(OnValueChanged);
		}

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			base.OnValidate();
			this.m_Accordion = this.gameObject.GetComponentInParent<Accordion>();

			if (this.group == null)
			{
				ToggleGroup tg = this.GetComponentInParent<ToggleGroup>();
				
				if (tg != null)
				{
					this.group = tg;
				}
			}
			
			LayoutElement le = this.gameObject.GetComponent<LayoutElement>();

			if (le != null && m_Accordion != null)
			{
				if (this.isOn)
				{
                    if (m_Accordion.ExpandVerticval)
                    {
						le.preferredHeight = -1f;
					}
                    else
                    {
						le.preferredWidth = -1f;
                    }
				}
				else
				{
					if (m_Accordion.ExpandVerticval)
					{
						le.preferredHeight = this.m_MinHeight;
					}
                    else
                    {
						le.preferredWidth = this.m_MinWidth;

					}
				}
			}
		}
#endif

		public void OnValueChanged(bool state)
		{
			if (this.m_LayoutElement == null)
				return;
			
			Accordion.Transition transition = (this.m_Accordion != null) ? this.m_Accordion.transition : Accordion.Transition.Instant;

			if (transition == Accordion.Transition.Instant && m_Accordion != null)
			{
				if (state)
				{
					if (m_Accordion.ExpandVerticval)
					{
						this.m_LayoutElement.preferredHeight = -1f;
					}
                    else
                    {
						this.m_LayoutElement.preferredWidth = -1f;
					}
				}
				else
				{
					if (m_Accordion.ExpandVerticval)
					{
						this.m_LayoutElement.preferredHeight = this.m_MinHeight;
					}
                    else
                    {
						this.m_LayoutElement.preferredWidth = this.m_MinWidth;
					}
				}
			}
			else if (transition == Accordion.Transition.Tween)
			{
				if (state)
				{
					if (m_Accordion.ExpandVerticval)
					{
						this.StartTween(this.m_MinHeight, this.GetExpandedHeight());
					}
                    else
                    {
						this.StartTween(this.m_MinWidth, this.GetExpandedWidth());
					}
				}
				else
				{
					if (m_Accordion.ExpandVerticval)
					{
						this.StartTween(this.m_RectTransform.rect.height, this.m_MinHeight);
					}
                    else
                    {
						this.StartTween(this.m_RectTransform.rect.width, this.m_MinWidth);
					}
				}
			}
		}
		
		protected float GetExpandedHeight()
		{
			if (this.m_LayoutElement == null)
				return this.m_MinHeight;
			
			float originalPrefH = this.m_LayoutElement.preferredHeight;
			this.m_LayoutElement.preferredHeight = -1f;
			float h = LayoutUtility.GetPreferredHeight(this.m_RectTransform);
			this.m_LayoutElement.preferredHeight = originalPrefH;
			
			return h;
		}

		protected float GetExpandedWidth()
		{
			if (this.m_LayoutElement == null)
				return this.m_MinWidth;

			float originalPrefW = this.m_LayoutElement.preferredWidth;
			this.m_LayoutElement.preferredWidth = -1f;
			float w = LayoutUtility.GetPreferredWidth(this.m_RectTransform);
			this.m_LayoutElement.preferredWidth = originalPrefW;

			return w;
		}

		protected void StartTween(float startFloat, float targetFloat)
		{
			float duration = (this.m_Accordion != null) ? this.m_Accordion.transitionDuration : 0.3f;
			
			FloatTween info = new FloatTween
			{
				duration = duration,
				startFloat = startFloat,
				targetFloat = targetFloat
			};
			if (m_Accordion.ExpandVerticval)
			{
				info.AddOnChangedCallback(SetHeight);
			}
            else
            {
				info.AddOnChangedCallback(SetWidth);
			}
			info.ignoreTimeScale = true;
			this.m_FloatTweenRunner.StartTween(info);
		}
		
		protected void SetHeight(float height)
		{
			if (this.m_LayoutElement == null)
				return;
				
			this.m_LayoutElement.preferredHeight = height;
		}

		protected void SetWidth(float width)
		{
			if (this.m_LayoutElement == null)
				return;

			this.m_LayoutElement.preferredWidth = width;
		}
	}
}