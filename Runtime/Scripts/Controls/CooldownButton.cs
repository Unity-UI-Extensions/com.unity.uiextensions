/// Credit SimonDarksideJ
/// Sourced from my head

using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/Cooldown Button")]
    public class CooldownButton : MonoBehaviour, IPointerDownHandler
    {
        #region Sub-Classes
        [System.Serializable]
        public class CooldownButtonEvent : UnityEvent<PointerEventData.InputButton> { }
        #endregion

        #region Private variables

        [SerializeField]
        private float cooldownTimeout;
        [SerializeField]
        private float cooldownSpeed = 1;
        [SerializeField][ReadOnly]
        private bool cooldownActive;
        [SerializeField][ReadOnly]
        private bool cooldownInEffect;
        [SerializeField][ReadOnly]
        private float cooldownTimeElapsed;
        [SerializeField][ReadOnly]
        private float cooldownTimeRemaining;
        [SerializeField][ReadOnly]
        private int cooldownPercentRemaining;
        [SerializeField][ReadOnly]
        private int cooldownPercentComplete;

        PointerEventData buttonSource;
        #endregion

        #region Public Properties

        public float CooldownTimeout
        {
            get { return cooldownTimeout; }
            set { cooldownTimeout = value; }
        }

        public float CooldownSpeed
        {
            get { return cooldownSpeed; }
            set { cooldownSpeed = value; }
        }

        public bool CooldownInEffect
        {
            get { return cooldownInEffect; }
        }

        public bool CooldownActive
        {
            get { return cooldownActive; }
            set { cooldownActive = value; }
        }

        public float CooldownTimeElapsed
        {
            get { return cooldownTimeElapsed; }
            set { cooldownTimeElapsed = value; }
        }

        public float CooldownTimeRemaining
        {
            get { return cooldownTimeRemaining; }
        }

        public int CooldownPercentRemaining
        {
            get { return cooldownPercentRemaining; }
        }

        public int CooldownPercentComplete
        {
            get { return cooldownPercentComplete; }
        }

        #endregion

        #region Events
        [Tooltip("Event that fires when a button is initially pressed down")]
        public CooldownButtonEvent OnCooldownStart;
        [Tooltip("Event that fires when a button is released")]
        public CooldownButtonEvent OnButtonClickDuringCooldown;
        [Tooltip("Event that continually fires while a button is held down")]
        public CooldownButtonEvent OnCoolDownFinish;
        #endregion

        #region Update

        // Update is called once per frame
        void Update()
        {
            if (CooldownActive)
            {
                cooldownTimeRemaining -= Time.deltaTime * cooldownSpeed;
                cooldownTimeElapsed = CooldownTimeout - CooldownTimeRemaining;
                if (cooldownTimeRemaining < 0)
                {
                    StopCooldown();
                }
                else
                {
                    cooldownPercentRemaining = (int)(100 * cooldownTimeRemaining * CooldownTimeout / 100);
                    cooldownPercentComplete = (int)((CooldownTimeout - cooldownTimeRemaining) / CooldownTimeout * 100);
                }
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Pause Cooldown without resetting values, allows Restarting of cooldown
        /// </summary>
        public void PauseCooldown()
        {
            if (CooldownInEffect)
            {
                CooldownActive = false;
            }
        }

        /// <summary>
        /// Restart a paused cooldown
        /// </summary>
        public void RestartCooldown()
        {
            if (CooldownInEffect)
            {
                CooldownActive = true;
            }
        }

        /// <summary>
        /// Start a cooldown from outside
        /// </summary>
        public void StartCooldown()
        {
            PointerEventData emptySource = new PointerEventData(EventSystem.current);
            buttonSource = emptySource;
            OnCooldownStart.Invoke(emptySource.button);
            cooldownTimeRemaining = cooldownTimeout;
            CooldownActive = cooldownInEffect = true;
        }

        /// <summary>
        /// Stop a running Cooldown and reset all values
        /// </summary>
        public void StopCooldown()
        {
            cooldownTimeElapsed = CooldownTimeout;
            cooldownTimeRemaining = 0;
            cooldownPercentRemaining = 0;
            cooldownPercentComplete = 100;
            cooldownActive = cooldownInEffect = false;
            if (OnCoolDownFinish != null) OnCoolDownFinish.Invoke(buttonSource.button);
        }

        /// <summary>
        /// Stop a running Cooldown and retain current values
        /// </summary>
        public void CancelCooldown()
        {
            cooldownActive = cooldownInEffect = false;
        }

        #endregion

        #region IPointerDownHandler

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            buttonSource = eventData;
            if (CooldownInEffect)
            {
                if (OnButtonClickDuringCooldown != null) OnButtonClickDuringCooldown.Invoke(eventData.button);
            }
            if (!CooldownInEffect)
            {
                if(OnCooldownStart != null) OnCooldownStart.Invoke(eventData.button);
                cooldownTimeRemaining = cooldownTimeout;
                cooldownActive = cooldownInEffect = true;
            }
        }

        #endregion

    }
}