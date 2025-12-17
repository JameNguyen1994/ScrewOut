using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace VideoEditorHelper
{
    public class PreviewInScene : MonoBehaviour
    {
        public static PreviewInScene Instance;

        [Header("UI Components")]
        public GameObject previewPanel;
        public Image imageUI;                // ⬅️ HIỂN THỊ HÌNH
        public RawImage videoRawImage;       // ⬅️ HIỂN THỊ VIDEO
        public VideoPlayer videoPlayer;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            Hide();
        }

        // ============================================================
        // SHOW IMAGE (using UI.Image)
        // ============================================================
        public void ShowImage(Texture2D tex)
        {
            if (tex == null) return;

            previewPanel.SetActive(true);

            // video off
            videoPlayer.Stop();
            videoPlayer.gameObject.SetActive(false);
            videoRawImage.gameObject.SetActive(false);

            // image on
            imageUI.gameObject.SetActive(true);

            // Convert Texture2D → Sprite
            Sprite sprite = Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f)
            );

            imageUI.sprite = sprite;
            imageUI.preserveAspect = true;
        }

        // ============================================================
        // SHOW VIDEO
        // ============================================================
        public void ShowVideo(VideoClip clip)
        {
            if (clip == null) return;

            previewPanel.SetActive(true);

            // image off
            imageUI.gameObject.SetActive(false);

            // video on
            videoRawImage.gameObject.SetActive(true);
            videoPlayer.gameObject.SetActive(true);

            videoPlayer.clip = clip;
            videoPlayer.Play();
        }

        // ============================================================
        // HIDE PREVIEW
        // ============================================================
        public void Hide()
        {
            previewPanel.SetActive(false);

            if (imageUI != null)
            {
                imageUI.sprite = null;
                imageUI.gameObject.SetActive(false);
            }

            if (videoPlayer != null)
            {
                videoPlayer.Stop();
                videoPlayer.gameObject.SetActive(false);
            }

            if (videoRawImage != null)
            {
                videoRawImage.texture = null;
                videoRawImage.gameObject.SetActive(false);
            }
        }
    }
}
