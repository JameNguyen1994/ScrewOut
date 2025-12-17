using DG.Tweening;
using UnityEngine;

namespace ps.modules.leaderboard.reward
{
    public class ParticleCallBack : MonoBehaviour
    {
        [SerializeField] private AudioSource audioFirework;

/*
        private void OnParticleSystemStopped()
        {
            Debug.Log("Particle system stopped");
            if (audioFirework != null)
            {
                Debug.Log("Play firework sound");
                audioFirework.Play();
            }
        }*/
     /*   private void OnEnable()
        {
            // ✅ Phát âm thanh ngay khi particle bắt đầu chạy
            if (audioFirework != null)
            {
                DOVirtual.DelayedCall(3, () =>
                {
                    Debug.Log("🚀 Firework started -> Play sound immediately");
                    audioFirework.Play();
                });
            }
        }*/
    }
}
