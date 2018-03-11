///Credit judah4
///Sourced from - http://forum.unity3d.com/threads/color-picker.267043/
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions.ColorPicker
{
	public class ColorPickerPresets : MonoBehaviour
	{
		public ColorPickerControl picker;

		[SerializeField]
		protected GameObject presetPrefab;

		[SerializeField]
		protected int maxPresets = 16;

		[SerializeField]
		protected Color[] predefinedPresets;

		protected List<Color> presets = new List<Color>();
		public Image createPresetImage;
		public Transform createButton;

		public enum SaveType { None, PlayerPrefs, JsonFile }
		[SerializeField]
		public SaveType saveMode = SaveType.None;

		[SerializeField]
		protected string playerPrefsKey;

		public virtual string JsonFilePath
		{
			get { return Application.persistentDataPath + "/" + playerPrefsKey + ".json"; }
		}

		protected virtual void Reset()
		{
			playerPrefsKey = "colorpicker_" + GetInstanceID().ToString();
		}

		protected virtual void Awake()
		{
			picker.onHSVChanged.AddListener(HSVChanged);
			picker.onValueChanged.AddListener(ColorChanged);
			picker.CurrentColor = Color.white;
			presetPrefab.SetActive(false);

			presets.AddRange(predefinedPresets);
			LoadPresets(saveMode);
		}

		public virtual void CreatePresetButton()
		{
			CreatePreset(picker.CurrentColor);
		}

		public virtual void LoadPresets(SaveType saveType)
		{
			string jsonData = "";
			switch (saveType)
			{
				case SaveType.None:
					break;
				case SaveType.PlayerPrefs:
					if (PlayerPrefs.HasKey(playerPrefsKey))
					{
						jsonData = PlayerPrefs.GetString(playerPrefsKey);
					}
					break;
				case SaveType.JsonFile:
					if (System.IO.File.Exists(JsonFilePath))
					{
						jsonData = System.IO.File.ReadAllText(JsonFilePath);
					}
					break;
				default:
					throw new System.NotImplementedException(saveType.ToString());
			}	

			if (!string.IsNullOrEmpty(jsonData))
			{
				try
				{
					var jsonColors = JsonUtility.FromJson<JsonColor>(jsonData);
					presets.AddRange(jsonColors.GetColors());
				}
				catch (System.Exception e)
				{
					Debug.LogException(e);
				}
			}

			foreach (var item in presets)
			{
				CreatePreset(item, true);
			}
		}

		public virtual void SavePresets(SaveType saveType)
		{
			if (presets == null || presets.Count <= 0)
			{
				Debug.LogError(
					"presets cannot be null or empty: " + (presets == null ? "NULL" : "EMPTY"));
				return;
			}

			var jsonColor = new JsonColor();
			jsonColor.SetColors(presets.ToArray());


			string jsonData = JsonUtility.ToJson(jsonColor);

			switch (saveType)
			{
				case SaveType.None:
					Debug.LogWarning("Called SavePresets with SaveType = None...");
					break;
				case SaveType.PlayerPrefs:
					PlayerPrefs.SetString(playerPrefsKey, jsonData);
					break;
				case SaveType.JsonFile:
					System.IO.File.WriteAllText(JsonFilePath, jsonData);
					//Application.OpenURL(JsonFilePath);
					break;
				default:
					throw new System.NotImplementedException(saveType.ToString());
			}
		}

		protected class JsonColor
		{
			public Color32[] colors;
			public void SetColors(Color[] colorsIn)
			{
				this.colors = new Color32[colorsIn.Length];
				for (int i = 0; i < colorsIn.Length; i++)
				{
					this.colors[i] = colorsIn[i];
				}
			}

			public Color[] GetColors()
			{
				Color[] colorsOut = new Color[colors.Length];
				for (int i = 0; i < colors.Length; i++)
				{
					colorsOut[i] = colors[i];
				}
				return colorsOut;
			}
		}

		public virtual void CreatePreset(Color color, bool loading)
		{
			createButton.gameObject.SetActive(presets.Count < maxPresets);

			var newPresetButton = Instantiate(presetPrefab, presetPrefab.transform.parent);
			newPresetButton.transform.SetAsLastSibling();
			newPresetButton.SetActive(true);
			newPresetButton.GetComponent<Image>().color = color;
			
			createPresetImage.color = Color.white;

			if (!loading)
			{
				presets.Add(color);
				SavePresets(saveMode);
			}
		}

		public virtual void CreatePreset(Color color)
		{
			CreatePreset(color, false);
		}

		public virtual void PresetSelect(Image sender)
		{
			picker.CurrentColor = sender.color;
		}

		protected virtual void HSVChanged(float h, float s, float v)
		{
			createPresetImage.color = HSVUtil.ConvertHsvToRgb(h * 360, s, v, 1);
			//Debug.Log("hsv util color: " + createPresetImage.color);
		}

		protected virtual void ColorChanged(Color color)
		{
			createPresetImage.color = color;
			//Debug.Log("color changed: " + color);
		}
	}
}