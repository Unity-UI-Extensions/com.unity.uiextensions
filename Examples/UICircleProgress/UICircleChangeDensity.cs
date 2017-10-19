using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class UICircleChangeDensity : MonoBehaviour
{
    public GameObject MultiColorObject;
    public GameObject TextOutputObject;

    private UICircle _uiCircleComponent;
    private Text _densityOutput;

    private void Awake()
    {
        _uiCircleComponent = MultiColorObject.GetComponent<UICircle>();
        _densityOutput = TextOutputObject.GetComponent<Text>();
    }
    private void OnEnable()
    {
        _densityOutput.text = _uiCircleComponent.ArcSteps.ToString();
    }

    public void UpdateDensity(float value)
    {
        _uiCircleComponent.SetArcSteps((int)value);
        _densityOutput.text = value.ToString();
    }
}
