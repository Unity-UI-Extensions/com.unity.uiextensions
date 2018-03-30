namespace UnityEngine.UI.Extensions.ColorPicker
{
	/// <summary>
	/// Samples colors from a screen capture. 
	/// Warning! In the editor if you're not in Free aspect mode then 
	/// the captured area includes the grey areas to the left and right of the game view window.
	/// In a build this will not be an issue.
	/// 
	/// This does not work well with a world space UI as positioning is working with screen space.
	/// </summary>
	public class ColorSampler : MonoBehaviour
	{
		[SerializeField]
		protected Button sampler;

		[SerializeField]
		protected Outline samplerOutline;

		protected Texture2D screenCapture;

		public ColorChangedEvent oncolorSelected = new ColorChangedEvent();

		protected Color color;

		protected virtual void OnEnable()
		{
			screenCapture = ScreenCapture.CaptureScreenshotAsTexture();
			sampler.gameObject.SetActive(true);
			sampler.onClick.AddListener(SelectColor);
		}

		protected virtual void OnDisable()
		{
			Destroy(screenCapture);
			sampler.gameObject.SetActive(false);
			sampler.onClick.RemoveListener(SelectColor);
		}

		protected virtual void Update()
		{
			if (screenCapture == null)
				return;

			sampler.transform.position = Input.mousePosition;
			color = screenCapture.GetPixel((int)Input.mousePosition.x, (int)Input.mousePosition.y);
		
			HandleSamplerColoring();
		}

		protected virtual void HandleSamplerColoring()
		{
			sampler.image.color = color;

			if (samplerOutline)
			{
				var c = Color.Lerp(Color.white, Color.black, color.grayscale > 0.5f ? 1 : 0);
				c.a = samplerOutline.effectColor.a;
				samplerOutline.effectColor = c;
			}
		}

		protected virtual void SelectColor()
		{
			if (oncolorSelected != null)
				oncolorSelected.Invoke(color);

			enabled = false;
		}
	}
}
