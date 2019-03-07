///Credit judah4
///Sourced from - http://forum.unity3d.com/threads/color-picker.267043/
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions.ColorPicker
{
	[ExecuteInEditMode]
	public class ColorPickerControl : MonoBehaviour
	{
		private float _hue = 0;
		private float _saturation = 0;
		private float _brightness = 0;

		private float _red = 0;
		private float _green = 0;
		private float _blue = 0;

		private float _alpha = 1;

		public ColorChangedEvent onValueChanged = new ColorChangedEvent();
		public HSVChangedEvent onHSVChanged = new HSVChangedEvent();

		[SerializeField]
		bool hsvSlidersOn = true;

		[SerializeField]
		List<GameObject> hsvSliders = new List<GameObject>();

		[SerializeField]
		bool rgbSlidersOn = true;

		[SerializeField]
		List<GameObject> rgbSliders = new List<GameObject>();

		[SerializeField]
		GameObject alphaSlider = null;

		public void SetHSVSlidersOn(bool value)
		{
			hsvSlidersOn = value;

			foreach (var item in hsvSliders)
				item.SetActive(value);

			if (alphaSlider)
				alphaSlider.SetActive(hsvSlidersOn || rgbSlidersOn);
		}

		public void SetRGBSlidersOn(bool value)
		{
			rgbSlidersOn = value;
			foreach (var item in rgbSliders)
				item.SetActive(value);

			if (alphaSlider)
				alphaSlider.SetActive(hsvSlidersOn || rgbSlidersOn);
		}


		void Update()
		{
#if UNITY_EDITOR
			SetHSVSlidersOn(hsvSlidersOn);
			SetRGBSlidersOn(rgbSlidersOn);
#endif
		}

		public Color CurrentColor
		{
			get
			{
				return new Color(_red, _green, _blue, _alpha);
			}
			set
			{
				if (CurrentColor == value)
					return;

				_red = value.r;
				_green = value.g;
				_blue = value.b;
				_alpha = value.a;

				RGBChanged();

				SendChangedEvent();
			}
		}

		private void Start()
		{
			SendChangedEvent();
		}

		public float H
		{
			get
			{
				return _hue;
			}
			set
			{
				if (_hue == value)
					return;

				_hue = value;

				HSVChanged();

				SendChangedEvent();
			}
		}

		public float S
		{
			get
			{
				return _saturation;
			}
			set
			{
				if (_saturation == value)
					return;

				_saturation = value;

				HSVChanged();

				SendChangedEvent();
			}
		}

		public float V
		{
			get
			{
				return _brightness;
			}
			set
			{
				if (_brightness == value)
					return;

				_brightness = value;

				HSVChanged();

				SendChangedEvent();
			}
		}

		public float R
		{
			get
			{
				return _red;
			}
			set
			{
				if (_red == value)
					return;

				_red = value;

				RGBChanged();

				SendChangedEvent();
			}
		}

		public float G
		{
			get
			{
				return _green;
			}
			set
			{
				if (_green == value)
					return;

				_green = value;

				RGBChanged();

				SendChangedEvent();
			}
		}

		public float B
		{
			get
			{
				return _blue;
			}
			set
			{
				if (_blue == value)
					return;

				_blue = value;

				RGBChanged();

				SendChangedEvent();
			}
		}

		private float A
		{
			get
			{
				return _alpha;
			}
			set
			{
				if (_alpha == value)
					return;

				_alpha = value;

				SendChangedEvent();
			}
		}

		private void RGBChanged()
		{
			HsvColor color = HSVUtil.ConvertRgbToHsv(CurrentColor);

			_hue = color.NormalizedH;
			_saturation = color.NormalizedS;
			_brightness = color.NormalizedV;
		}

		private void HSVChanged()
		{
			Color color = HSVUtil.ConvertHsvToRgb(_hue * 360, _saturation, _brightness, _alpha);

			_red = color.r;
			_green = color.g;
			_blue = color.b;
		}

		private void SendChangedEvent()
		{
			onValueChanged.Invoke(CurrentColor);
			onHSVChanged.Invoke(_hue, _saturation, _brightness);
		}

		public void AssignColor(ColorValues type, float value)
		{
			switch (type)
			{
				case ColorValues.R:
					R = value;
					break;
				case ColorValues.G:
					G = value;
					break;
				case ColorValues.B:
					B = value;
					break;
				case ColorValues.A:
					A = value;
					break;
				case ColorValues.Hue:
					H = value;
					break;
				case ColorValues.Saturation:
					S = value;
					break;
				case ColorValues.Value:
					V = value;
					break;
				default:
					break;
			}
		}

		public float GetValue(ColorValues type)
		{
			switch (type)
			{
				case ColorValues.R:
					return R;
				case ColorValues.G:
					return G;
				case ColorValues.B:
					return B;
				case ColorValues.A:
					return A;
				case ColorValues.Hue:
					return H;
				case ColorValues.Saturation:
					return S;
				case ColorValues.Value:
					return V;
				default:
					throw new System.NotImplementedException("");
			}
		}
	}
}