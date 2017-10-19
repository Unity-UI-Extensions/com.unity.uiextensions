using UnityEngine;
using UnityEngine.UI.Extensions;

public class UICircleChangeColor : MonoBehaviour
{
    public GameObject TargetUICircle;
    private Color baseColor;
    private Color progressColor;

    private float r, g, b = 0;
    private float factor = 1536.145f;
    private void Awake()
    {
        baseColor = TargetUICircle.GetComponent<UICircle>().color;
        progressColor = TargetUICircle.GetComponent<UICircle>().ProgressColor;
    }

    public void UpdateBaseColor(float value)
    {
        baseColor = SetFixedColor(value, baseColor.a);
        TargetUICircle.GetComponent<UICircle>().color = baseColor;
    }

    public void UpdateProgressColor(float value)
    {

        progressColor = SetFixedColor(value, progressColor.a);
        TargetUICircle.GetComponent<UICircle>().SetProgressColor(progressColor);
    }

    private Color SetFixedColor(float value,float alpha)
    {
        if (value <= 0.166f)
        {
            g = 0;
            r = 255f;
            b = 255f - (255f - (value * factor));
        }else if(value <= 0.332f)
        {
            g = 0;
            r = 255f - (255f - ((0.332f - value)*factor));
            b = 255f;
        }else if(value <= 0.498f)
        {
            g = 255f - (255f - ((0.498f - value) * factor));
            r = 0f;
            b = 255f;
        }else if(value <= 0.664f)
        {
            g = 255f;
            r = 0f;
            b = 255f - (255f - ((0.664f - value) * factor));
        }else if(value <= 0.83f)
        {
            g = 255f;
            r = 255f - (255f - ((0.83f - value) * factor));
            b = 0;
        }else
        {
            g = 255f - (255f - ((1 - value) * factor));
            r = 255f;
            b = 0;
        }

        return new Color(r, g, b, alpha);
    }
}
