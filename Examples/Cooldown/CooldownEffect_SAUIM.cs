/// Credit SimonDarksideJ
/// Sourced from my head

namespace UnityEngine.UI.Extensions.Examples
{
    [RequireComponent(typeof(SoftMaskScript))]
    public class CooldownEffect_SAUIM : MonoBehaviour {

        public CooldownButton cooldown;
        private SoftMaskScript sauim;

        // Use this for initialization
        void Start() {
            if (cooldown == null)
            {
                Debug.LogError("Missing Cooldown Button assignment");
            }
            sauim = GetComponent<SoftMaskScript>();
        }

        // Update is called once per frame
        void Update() {
            sauim.CutOff = Mathf.Lerp(0,1, cooldown.CooldownTimeElapsed / cooldown.CooldownTimeout);
        }
    }
}