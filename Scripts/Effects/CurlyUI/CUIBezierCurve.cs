/// Credit Titinious (https://github.com/Titinious)
/// Sourced from - https://github.com/Titinious/CurlyUI

namespace UnityEngine.UI.Extensions
{
    /// <summary>
    /// Assume to be a cubic bezier curve at the moment.
    /// </summary>
    public class CUIBezierCurve : MonoBehaviour
    {
        public readonly static int CubicBezierCurvePtNum = 4;

        #region Descriptions

        [SerializeField]
        protected Vector3[] controlPoints;

        public Vector3[] ControlPoints
        {
            get
            {
                return controlPoints;
            }

        }

#if UNITY_EDITOR
        /// <summary>
        /// Reserve for editor only
        /// </summary>
        public Vector3[] EDITOR_ControlPoints
        {
            set
            {
                controlPoints = value;
            }
        }
#endif

        #endregion

        #region Events

#if UNITY_EDITOR
        protected void OnValidate()
        {
            Refresh();
        }
#endif

        public void Refresh()
        {

            if (OnRefresh != null)
                OnRefresh();
        }

        #endregion
        #region Services

        /// <summary>
        /// call this to get a sample
        /// </summary>
        /// <param name="_time"></param>
        /// <returns>sample returned by said time</returns>
        public Vector3 GetPoint(float _time)
        {
            float oneMinusTime = 1 - _time;

            return oneMinusTime * oneMinusTime * oneMinusTime * controlPoints[0] +
                3 * oneMinusTime * oneMinusTime * _time * controlPoints[1] +
                3 * oneMinusTime * _time * _time * controlPoints[2] +
                _time * _time * _time * controlPoints[3];
        }

        public Vector3 GetTangent(float _time)
        {
            float oneMinusTime = 1 - _time;

            return 3 * oneMinusTime * oneMinusTime * (controlPoints[1] - controlPoints[0]) +
                6 * oneMinusTime * _time * (controlPoints[2] - controlPoints[1]) +
                3 * _time * _time * (controlPoints[3] - controlPoints[2]);
        }

        #endregion

        #region Configurations

        public void ReportSet()
        {
            if (controlPoints == null)
            {
                controlPoints = new Vector3[CUIBezierCurve.CubicBezierCurvePtNum];
                controlPoints[0] = new Vector3(0, 0, 0);
                controlPoints[1] = new Vector3(0, 1, 0);
                controlPoints[2] = new Vector3(1, 1, 0);
                controlPoints[3] = new Vector3(1, 0, 0);
            }

            bool isPointsReady = true;

            isPointsReady = isPointsReady & controlPoints.Length == CUIBezierCurve.CubicBezierCurvePtNum;
        }
        #endregion


        #region Services

        public System.Action OnRefresh;



        #endregion

    }
}