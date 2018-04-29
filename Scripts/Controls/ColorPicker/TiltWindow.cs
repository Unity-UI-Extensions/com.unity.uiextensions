///Credit judah4
///Sourced from - http://forum.unity3d.com/threads/color-picker.267043/

using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    public class TiltWindow : MonoBehaviour, IDragHandler
    {
        public Vector2 range = new Vector2(5f, 3f);

        private Transform mTrans;
        private Quaternion mStart;
        private Vector2 mRot = Vector2.zero;
        private Vector2 m_screenPos;


        void Start()
        {
            mTrans = transform;
            mStart = mTrans.localRotation;
        }

        void Update()
        {
            Vector3 pos = m_screenPos;

            float halfWidth = Screen.width * 0.5f;
            float halfHeight = Screen.height * 0.5f;
            float x = Mathf.Clamp((pos.x - halfWidth) / halfWidth, -1f, 1f);
            float y = Mathf.Clamp((pos.y - halfHeight) / halfHeight, -1f, 1f);
            mRot = Vector2.Lerp(mRot, new Vector2(x, y), Time.deltaTime * 5f);

            mTrans.localRotation = mStart * Quaternion.Euler(-mRot.y * range.y, mRot.x * range.x, 0f);
        }


        public void OnDrag(PointerEventData eventData)
        {
            m_screenPos = eventData.position;
        }
    }
}