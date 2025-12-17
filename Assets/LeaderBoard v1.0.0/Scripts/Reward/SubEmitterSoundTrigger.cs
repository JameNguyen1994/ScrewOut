using EasyButtons;
using UnityEditor;
using UnityEngine;

namespace ps.modules.leaderboard.reward
{
    public class SubEmitterSoundTrigger : MonoBehaviour
    {
        [SerializeField] private ParticleSystem subEmitter; // ✅ Gắn SubEmitter vào đây
        [SerializeField] private AudioSource audioFirework;
        private bool hasTriggered = false;

        private void Update()
        {
            if (subEmitter == null || audioFirework == null)
                return;

            // Khi subEmitter bắt đầu phát hạt => particleCount > 0 lần đầu tiên
            if (!hasTriggered && subEmitter.particleCount > 0)
            {
                hasTriggered = true;
                Debug.Log("🔥 SubEmitter started spawning particles -> Play sound");
                audioFirework.Play();
            }

            // Reset lại trigger nếu muốn lặp lại lần sau (tuỳ bạn)
            if (!subEmitter.IsAlive() && hasTriggered)
            {
                // Nếu muốn chỉ phát 1 lần thì bỏ dòng này
                hasTriggered = false;
            }
        }

        [Button]
        public void SetUpReference()
        {
            audioFirework = gameObject.GetComponentInParent<AudioSource>();
            subEmitter = gameObject.GetComponent<ParticleSystem>();

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
    }
}
