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
			
			if (this.group == null)
			{
				ToggleGroup tg = this.GetComponentInParent<ToggleGroup>();
				
				if (tg != null)
				{
					this.group = tg;
				}
			}
			
			LayoutElement le = this.gameObject.GetComponent<LayoutElement>();
			
			if (le != null)
			{
				if (this.isOn)
				{
					le.preferredHeight = -1f;
				}
				else
				{
					le.preferredHeight = this.m_MinHeight;
				}
			}
		}
#endif

		public void OnValueChanged(bool state)
		{
			if (this.m_LayoutElement == null)
				return;
			
			Accordion.Transition transition = (this.m_Accordion != null) ? this.m_Accordion.transition : Accordion.Transition.Instant;
			
			if (transition == Accordion.Transition.Instant)
			{
				if (state)
				{
					this.m_LayoutElement.preferredHeight = -1f;
				}
				else
				{
					this.m_LayoutElement.preferredHeight = this.m_MinHeight;
				}
			}
			else if (transition == Accordion.Transition.Tween)
			{
				if (state)
				{
					this.StartTween(this.m_MinHeight, this.GetExpandedHeight());
				}
				else
				{
					this.StartTween(this.m_RectTransform.rect.height, this.m_MinHeight);
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
		
		protected void StartTween(float startFloat, float targetFloat)
		{
			float duration = (this.m_Accordion != null) ? this.m_Accordion.transitionDuration : 0.3f;
			
			FloatTween info = new FloatTween
			{
				duration = duration,
				startFloat = startFloat,
				targetFloat = targetFloat
			};
			info.AddOnChangedCallback(SetHeight);
			info.ignoreTimeScale = true;
			this.m_FloatTweenRunner.StartTween(info);
		}
		
		protected void SetHeight(float height)
		{
			if (this.m_LayoutElement == null)
				return;
				
			this.m_LayoutElement.preferredHeight = height;
		}
	}
}