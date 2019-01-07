/// <summary>
/// Credit - ryanslikesocool 
/// Sourced from - https://github.com/ryanslikesocool/Unity-Card-UI
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI.Extensions
{
///The formula for a basic superellipse is
///Mathf.Pow(Mathf.Abs(x / a), n) + Mathf.Pow(Mathf.Abs(y / b), n) = 1
[ExecuteInEditMode]
public class SuperellipsePoints : MonoBehaviour
{
    public float xLimits = 1f;
    public float yLimits = 1f;
    [Range(1f, 96f)]
    public float superness = 4f;

    private float lastXLim;
    private float lastYLim;
    private float lastSuper;

    [Space]
    [Range(1, 32)]
    public int levelOfDetail = 4;

    private int lastLoD;

    [Space]
    public Material material;

    private List<Vector2> pointList = new List<Vector2>();

    void Start()
    {
        RecalculateSuperellipse();

        GetComponent<MeshRenderer>().material = material;

        lastXLim = xLimits;
        lastYLim = yLimits;
        lastSuper = superness;

        lastLoD = levelOfDetail;
    }

    void Update()
    {
        if (lastXLim != xLimits || lastYLim != yLimits || lastSuper != superness || lastLoD != levelOfDetail)
        {
            RecalculateSuperellipse();
        }

        lastXLim = xLimits;
        lastYLim = yLimits;
        lastSuper = superness;

        lastLoD = levelOfDetail;
    }

    void RecalculateSuperellipse()
    {
        pointList.Clear();

        float realLoD = levelOfDetail * 4;

        for (float i = 0; i < xLimits; i += 1 / realLoD)
        {
            float y = Superellipse(xLimits, yLimits, i, superness);
            Vector2 tempVecTwo = new Vector2(i, y);
            pointList.Add(tempVecTwo);
        }
        pointList.Add(new Vector2(xLimits, 0));
        pointList.Add(Vector2.zero);

        GetComponent<MeshCreator>().CreateMesh(pointList);
    }

    float Superellipse(float a, float b, float x, float n)
    {
        float alpha = Mathf.Pow((x / a), n);
        float beta = 1 - alpha;
        float y = Mathf.Pow(beta, 1 / n) * b;

        return y;
    }
}
}