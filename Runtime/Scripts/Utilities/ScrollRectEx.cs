/// Credit CaptainSchnittchen
/// Credit TouchPad (OnScroll function) update - GamesRUs
/// sourced from: http://forum.unity3d.com/threads/scripts-useful-4-6-scripts-collection.264161/page-2#post-2011648

/*USAGE:
Simply place the script on the ScrollRect that contains the selectable children we'll be scrolling to
and drag'n'drop the RectTransform of the options "container" that we'll be scrolling.*/

using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/ScrollRectEx")]
    public class ScrollRectEx : ScrollRect
    {
        private bool routeToParent = false;

        /// <summary>
        /// Do action for all parents
        /// </summary>
        private void DoForParents<T>(Action<T> action) where T : IEventSystemHandler
        {
            Transform parent = transform.parent;
            while (parent != null)
            {
                foreach (var component in parent.GetComponents<Component>())
                {
                    if (component is T)
                        action((T)(IEventSystemHandler)component);
                }
                parent = parent.parent;
            }
        }

        /// <summary>
        /// Always route initialize potential drag event to parents
        /// </summary>
        public override void OnInitializePotentialDrag(PointerEventData eventData)
        {
            DoForParents<IInitializePotentialDragHandler>((parent) => { parent.OnInitializePotentialDrag(eventData); });
            base.OnInitializePotentialDrag(eventData);
        }

        /// <summary>
        /// Drag event
        /// </summary>
        public override void OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (routeToParent)
                DoForParents<IDragHandler>((parent) => { parent.OnDrag(eventData); });
            else
                base.OnDrag(eventData);
        }

        /// <summary>
        /// Begin drag event
        /// </summary>
        public override void OnBeginDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (!horizontal && Math.Abs(eventData.delta.x) > Math.Abs(eventData.delta.y))
                routeToParent = true;
            else if (!vertical && Math.Abs(eventData.delta.x) < Math.Abs(eventData.delta.y))
                routeToParent = true;
            else
                routeToParent = false;

            if (routeToParent)
                DoForParents<IBeginDragHandler>((parent) => { parent.OnBeginDrag(eventData); });
            else
                base.OnBeginDrag(eventData);
        }

        /// <summary>
        /// End drag event
        /// </summary>
        public override void OnEndDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (routeToParent)
                DoForParents<IEndDragHandler>((parent) => { parent.OnEndDrag(eventData); });
            else
                base.OnEndDrag(eventData);
            routeToParent = false;
        }

        public override void OnScroll(PointerEventData eventData)
        {
            if (!horizontal && Math.Abs(eventData.scrollDelta.x) > Math.Abs(eventData.scrollDelta.y))
            {
                routeToParent = true;
            }
            else if (!vertical && Math.Abs(eventData.scrollDelta.x) < Math.Abs(eventData.scrollDelta.y))
            {
                routeToParent = true;
            }
            else
            {
                routeToParent = false;
            }

            if (routeToParent)
                DoForParents<IScrollHandler>((parent) => {
                    parent.OnScroll(eventData);
                });
            else
                base.OnScroll(eventData);
        }
    }
}