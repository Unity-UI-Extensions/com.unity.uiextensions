/// Credit Chris Trueman
/// Sourced from - http://forum.unity3d.com/threads/use-reticle-like-mouse-for-worldspace-uis.295271/

using System.Collections.Generic;

namespace UnityEngine.EventSystems.Extensions
{

    [RequireComponent(typeof(EventSystem))]
    [AddComponentMenu("UI/Extensions/Aimer Input Module")]
    public class AimerInputModule : BaseInputModule
    {
        public string ActivateAxis = "Interact";

        public static GameObject ObjectUnderAimer;

        public static Camera CurrentPlayerCamera;

        protected AimerInputModule() { }

        public void Awake()
        {
            var StandAloneSystem = GetComponent<StandaloneInputModule>();
            if (StandAloneSystem != null)
            {
                Debug.LogError("Aimer Input Module is incompatible with the StandAloneInputSystem, please remove it from the Event System in this scene");
            }
            if (!CurrentPlayerCamera)
            {
                CurrentPlayerCamera = Camera.main;
            }
        }

        public override void UpdateModule()
        {
            GetObjectUnderAimer();
        }

        public override void Process()
        {
            if (ObjectUnderAimer)
            {
                if (Input.GetButtonDown(ActivateAxis))
                {
                    BaseEventData eventData = GetBaseEventData();
                    eventData.selectedObject = ObjectUnderAimer;
                    ExecuteEvents.Execute(ObjectUnderAimer, eventData, ExecuteEvents.submitHandler);
                }
            }
        }

        List<RaycastResult> results = new List<RaycastResult>();

        private bool GetObjectUnderAimer()
        {
            PointerEventData pointerData = new PointerEventData(eventSystem);
            pointerData.worldPosition = CurrentPlayerCamera.transform.position;

            eventSystem.RaycastAll(pointerData, results);

            if (results.Count > 0)
            {
                RaycastResult rayResult = FindFirstRaycast(results);
                if (ObjectUnderAimer != rayResult.gameObject)
                {
                    Debug.Log(rayResult.gameObject.name);
                    ObjectUnderAimer = rayResult.gameObject;
                    BaseEventData eData = GetBaseEventData();
                    eData.selectedObject = ObjectUnderAimer;
                    ExecuteEvents.Execute(ObjectUnderAimer, eData, ExecuteEvents.pointerEnterHandler);
                }

                results.Clear();
                return true;
            }

            //We didn't hit anything

            if (ObjectUnderAimer)
            {
                BaseEventData eData = GetBaseEventData();
                eData.selectedObject = ObjectUnderAimer;

                ExecuteEvents.Execute(ObjectUnderAimer, eData, ExecuteEvents.pointerExitHandler);
            }

            results.Clear();
            ObjectUnderAimer = null;
            return false;
        }
    }
}