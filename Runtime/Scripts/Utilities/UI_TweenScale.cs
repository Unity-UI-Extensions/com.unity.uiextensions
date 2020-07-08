/// Credit Tomasz Schelenz 
/// Sourced from - https://bitbucket.org/SimonDarksideJ/unity-ui-extensions/issues/83/ui_tweenscale
/// Demo - https://youtu.be/uVTV7Udx78k?t=1m33s Dynamic scaling of text or image (including button) based on curves. works on scrollrect scale so you can pretty much use it for any ui type. 
/// Notes In some cases it can create spikes due to redrawing on change, it is recommended to use it on simple objects in separated canvases to avoid redrawing full canvas.


using System.Collections;

namespace UnityEngine.UI.Extensions
{
    /// <summary>
    /// Dynamic scaling of text or image (including button) based on curves
    /// 
    /// Fields
    /// - animCurve - animation curve for scale (if isUniform set to false, will apply only to X scale)
    /// - speed - animation speed
    /// - isLoop - animation will play infinitely (in order to make it work set your animation curve to loop)
    /// - playAtAwake - starts automatically with script becoming active. Otherwise you need to call Play() method.
    /// - isUniform - if false animCurve will modify object X scale and animCurveY - Y scale.
    /// 
    /// 
    /// Notes
    /// - If you want to stop the animation call the ResetTween() method. 
    /// - In some cases it can create spikes due to redrawing on change, it is recommended  to use it on simple objects in separated canvases to 
    /// avoid redrawing full canvas. 
    /// - If you want to scale object only in 1 axis select non unifor and use linear curve from 1 to 1 to lock the scale. 
    /// 
    /// </summary>
    [AddComponentMenu("UI/Extensions/UI Tween Scale")]
    public class UI_TweenScale : MonoBehaviour
    {
        //ANIMATION FOR X AND Y, OR X IF isUniform set to false 
        public AnimationCurve animCurve;
        [Tooltip("Animation speed multiplier")]
        public float speed = 1;

        [Tooltip("If true animation will loop, for best effect set animation curve to loop on start and end point")]

        public bool isLoop = false;
        //IF TRUE ANIMATION STARTS AUTOMATICALLY
        [Tooltip("If true animation will start automatically, otherwise you need to call Play() method to start the animation")]

        public bool playAtAwake = false;

        [Space(10)]
        //if true both x and y axis will be using animCurve;
        [Header("Non uniform scale")]
        [Tooltip("If true component will scale by the same amount in X and Y axis, otherwise use animCurve for X scale and animCurveY for Y scale")]
        public bool isUniform = true;
        //IF isUniform set to false use this for Y axis
        public AnimationCurve animCurveY;
        private Vector3 initScale;
        private Transform myTransform;

        void Awake()
        {
            myTransform = GetComponent<Transform>();
            initScale = myTransform.localScale;
            if (playAtAwake)
            {
                Play();
            }
        }

        public void Play()
        {
            StartCoroutine("Tween");
        }

        Vector3 newScale = Vector3.one;

        IEnumerator Tween()
        {
            myTransform.localScale = initScale;
            float t = 0;
            float maxT = animCurve.keys[animCurve.length - 1].time;

            while (t < maxT || isLoop)
            {
                t += speed * Time.deltaTime;

                if (!isUniform)
                {
                    newScale.x = 1 * animCurve.Evaluate(t);
                    newScale.y = 1 * animCurveY.Evaluate(t);

                    myTransform.localScale = newScale;
                }
                else
                {
                    myTransform.localScale = Vector3.one * animCurve.Evaluate(t);
                }

                yield return null;
            }
        }

        public void ResetTween()
        {
            StopCoroutine("Tween");
            myTransform.localScale = initScale;
        }
    }
}
