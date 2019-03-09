using System.Collections.Generic;

namespace UnityEngine.UI.Extensions.Examples
{
    [RequireComponent(typeof(UILineRendererList))]
    public class LineRendererOrbitList : MonoBehaviour
    {
        UILineRendererList lr;
        Circle circle;
        public GameObject OrbitGO;
        RectTransform orbitGOrt;
        float orbitTime;

        [SerializeField]
        private float _xAxis = 3;

        public float xAxis
        {
            get { return _xAxis; }
            set { _xAxis = value; GenerateOrbit(); }
        }

        [SerializeField]
        private float _yAxis = 3;

        public float yAxis
        {
            get { return _yAxis; }
            set { _yAxis = value; GenerateOrbit(); }
        }

        [SerializeField]
        private int _steps = 10;

        public int Steps
        {
            get { return _steps; }
            set { _steps = value; GenerateOrbit(); }
        }



        // Use this for initialization
        void Awake()
        {
            lr = GetComponent<UILineRendererList>();
            orbitGOrt = OrbitGO.GetComponent<RectTransform>();
            GenerateOrbit();
        }

        // Update is called once per frame
        void Update()
        {
            orbitTime = orbitTime > _steps ? orbitTime = 0 : orbitTime + Time.deltaTime;
            orbitGOrt.localPosition = circle.Evaluate(orbitTime);
        }

        void GenerateOrbit()
        {
            circle = new Circle(xAxis: _xAxis, yAxis: _yAxis, steps: _steps);
            for (int i = 0; i < _steps; i++)
            {
                lr.AddPoint(circle.Evaluate(i));
            }
            lr.AddPoint(circle.Evaluate(0));
        }

        private void OnValidate()
        {
            if (lr != null)
            {
                GenerateOrbit();
            }
        }
    }
}