/// Credit Ralph Barbagallo (www.flarb.com /www.ralphbarbagallo.com / @flarb)
/// Sourced from - http://forum.unity3d.com/threads/vr-cursor-possible-unity-4-6-gui-bug-or-is-it-me

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/VR Cursor")]
    public class VRCursor : MonoBehaviour
    {
        public float xSens;
        public float ySens;

        private Collider currentCollider;

        // Update is called once per frame
        void Update()
        {
            Vector3 thisPosition;

            thisPosition.x = Input.mousePosition.x * xSens;
            thisPosition.y = Input.mousePosition.y * ySens - 1;
            thisPosition.z = transform.position.z;

            transform.position = thisPosition;

            VRInputModule.cursorPosition = transform.position;

            if (Input.GetMouseButtonDown(0) && currentCollider)
            {
                VRInputModule.PointerSubmit(currentCollider.gameObject);
            }

        }

        void OnTriggerEnter(Collider other)
        {
            //print("OnTriggerEnter other " + other.gameObject);
            VRInputModule.PointerEnter(other.gameObject);
            currentCollider = other;
        }

        void OnTriggerExit(Collider other)
        {
            //print("OnTriggerExit other " + other.gameObject);
            VRInputModule.PointerExit(other.gameObject);
            currentCollider = null;
        }
    }
}