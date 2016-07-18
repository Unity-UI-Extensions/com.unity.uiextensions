///Credit judah4
///Sourced from - http://forum.unity3d.com/threads/color-picker.267043/

namespace UnityEngine.UI.Extensions
{
	public class HSVPicker : MonoBehaviour
	{
		public HexRGB hexrgb;

		public Color currentColor;
		public Image colorImage;
		public Image pointer;
		public Image cursor;
		public RawImage hsvSlider;
		public RawImage hsvImage;

		//public InputField inputR;
		//public InputField inputG;
		//public InputField inputB;

		public Slider sliderR;
		public Slider sliderG;
		public Slider sliderB;
		public Text sliderRText;
		public Text sliderGText;
		public Text sliderBText;

		public float pointerPos = 0;

		public float cursorX = 0;
		public float cursorY = 0;


		public HSVSliderEvent onValueChanged = new HSVSliderEvent();

		private bool dontAssignUpdate = false;

		void Awake()
		{
			hsvSlider.texture = HSVUtil.GenerateHSVTexture((int)hsvSlider.rectTransform.rect.width, (int)hsvSlider.rectTransform.rect.height);

			sliderR.onValueChanged.AddListener(newValue =>
			{
				currentColor.r = newValue;
				if (dontAssignUpdate == false)
				{
					AssignColor(currentColor);
				}
				sliderRText.text = "R:" + (int)(currentColor.r * 255f);
				hexrgb.ManipulateViaRGB2Hex();
			});
			sliderG.onValueChanged.AddListener(newValue =>
			{
				currentColor.g = newValue;
				if (dontAssignUpdate == false)
				{
					AssignColor(currentColor);
				}
				sliderGText.text = "G:" + (int)(currentColor.g * 255f);
				hexrgb.ManipulateViaRGB2Hex();
			});
			sliderB.onValueChanged.AddListener(newValue =>
			{
				currentColor.b = newValue;
				if (dontAssignUpdate == false)
				{
					AssignColor(currentColor);
				}
				sliderBText.text = "B:" + (int)(currentColor.b * 255f);
				hexrgb.ManipulateViaRGB2Hex();
			});


			hsvImage.texture = HSVUtil.GenerateColorTexture((int)hsvImage.rectTransform.rect.width, (int)hsvImage.rectTransform.rect.height, ((Texture2D)hsvSlider.texture).GetPixelBilinear(0, 0));
			MoveCursor(cursorX, cursorY);
		}

		// Update is called once per frame
		void Update()
		{
			//if (Input.GetKeyDown(KeyCode.R))
			//{
			//    var color = new Color(45f / 255, 200f / 255, 255f / 255);
			//    Debug.Log(color);
			//    AssignColor(color);

			// }


		}


		public void AssignColor(Color color)
		{

			var hsv = HSVUtil.ConvertRgbToHsv(color);

			// Debug.Log(hsv.ToString());

			float hOffset = (float)(hsv.H / 360);

			//if (hsv.S > 1)
			//{
			//    hsv.S %= 1f;
			//}
			//if (hsv.V > 1)
			//{
			//    hsv.V %= 1f;
			//}

			MovePointer(hOffset, false);
			MoveCursor((float)hsv.S, (float)hsv.V, false);

			currentColor = color;
			colorImage.color = currentColor;

			onValueChanged.Invoke(currentColor);

		}


		public Color MoveCursor(float posX, float posY, bool updateInputs = true)
		{
			dontAssignUpdate = updateInputs;
			if (posX > 1)
			{
				posX %= 1;
			}
			if (posY > 1)
			{
				posY %= 1;
			}

			posY = Mathf.Clamp(posY, 0, .9999f);
			posX = Mathf.Clamp(posX, 0, .9999f);


			cursorX = posX;
			cursorY = posY;
			cursor.rectTransform.anchoredPosition = new Vector2(posX * hsvImage.rectTransform.rect.width, posY * hsvImage.rectTransform.rect.height - hsvImage.rectTransform.rect.height);

			currentColor = GetColor(cursorX, cursorY);
			colorImage.color = currentColor;

			if (updateInputs)
			{
				UpdateInputs();
				onValueChanged.Invoke(currentColor);
			}
			dontAssignUpdate = false;
			return currentColor;
		}

		public Color GetColor(float posX, float posY)
		{
			//Debug.Log(posX + " " + posY);
			return ((Texture2D)hsvImage.texture).GetPixel((int)(cursorX * hsvImage.texture.width), (int)(cursorY * hsvImage.texture.height));
		}

		public Color MovePointer(float newPos, bool updateInputs = true)
		{
			dontAssignUpdate = updateInputs;
			if (newPos > 1)
			{
				newPos %= 1f;//hsv
			}
			pointerPos = newPos;

			var mainColor = ((Texture2D)hsvSlider.texture).GetPixelBilinear(0, pointerPos);
			if (hsvImage.texture != null)
			{
				if ((int)hsvImage.rectTransform.rect.width != hsvImage.texture.width || (int)hsvImage.rectTransform.rect.height != hsvImage.texture.height)
				{
					Destroy(hsvImage.texture);
					hsvImage.texture = null;

					hsvImage.texture = HSVUtil.GenerateColorTexture((int)hsvImage.rectTransform.rect.width, (int)hsvImage.rectTransform.rect.height, mainColor);
				}
				else
				{
					HSVUtil.GenerateColorTexture(mainColor, (Texture2D)hsvImage.texture);
				}
			}
			else
			{

				hsvImage.texture = HSVUtil.GenerateColorTexture((int)hsvImage.rectTransform.rect.width, (int)hsvImage.rectTransform.rect.height, mainColor);
			}
			pointer.rectTransform.anchoredPosition = new Vector2(0, -pointerPos * hsvSlider.rectTransform.rect.height);

			currentColor = GetColor(cursorX, cursorY);
			colorImage.color = currentColor;

			if (updateInputs)
			{
				UpdateInputs();
				onValueChanged.Invoke(currentColor);
			}
			dontAssignUpdate = false;
			return currentColor;
		}

		public void UpdateInputs()
		{

			sliderR.value = currentColor.r;
			sliderG.value = currentColor.g;
			sliderB.value = currentColor.b;

			sliderRText.text = "R:" + (currentColor.r * 255f);
			sliderGText.text = "G:" + (currentColor.g * 255f);
			sliderBText.text = "B:" + (currentColor.b * 255f);
		}

		void OnDestroy()
		{
			if (hsvSlider.texture != null)
			{
				Destroy(hsvSlider.texture);
			}

			if (hsvImage.texture != null)
			{
				Destroy(hsvImage.texture);
			}
		}
	}
}