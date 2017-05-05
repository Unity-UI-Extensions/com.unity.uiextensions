using System.Collections.Generic;
using UnityEngine;

public class TestAddingPoints : MonoBehaviour {

    public UnityEngine.UI.Extensions.UILineRenderer LineRenderer;
    public UnityEngine.UI.Text XValue;
    public UnityEngine.UI.Text YValue;

    // Use this for initialization
    public void AddNewPoint () {
        var point = new Vector2() { x = float.Parse(XValue.text), y = float.Parse(YValue.text) };
        var pointlist = new List<Vector2>(LineRenderer.Points);
        pointlist.Add(point);
        LineRenderer.Points = pointlist.ToArray();
    }
}
