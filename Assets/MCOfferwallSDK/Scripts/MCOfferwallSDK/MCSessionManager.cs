using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.MCOfferwallSDK.Scripts.MCOfferwallSDK
{

    class MCSessionManager : MonoBehaviour
    {
        const string SessionTimeKey = "mc_currentSessionTime";
        const string LastSessionTimeKey = "mc_lastSessionTime";
        const string SessionCountKey = "mc_sessionCount";


        private const float UpdateInterval = 3f;
        private const float MaxInactiveInterval = 600f; // 10 minutes in seconds


        private float lastInactiveTimestamp = -1f;
        private float totalInactiveTime = 0f;
        private float sessionStartTimestamp;

        public UnityEvent<int> OnNewSession;

        private Coroutine updateCoroutine;
        public static MCSessionManager Instance { get; private set; }
        private void Awake()
        {
            // Ensure only one instance of SessionManager exists.
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            StartNewSession();

            updateCoroutine = StartCoroutine(UpdateTime());

        }
        void OnAppStartOrResume()
        {
            if (lastInactiveTimestamp > 0)
            {
                float inactiveTime = Time.realtimeSinceStartup - lastInactiveTimestamp;
                totalInactiveTime += inactiveTime;

                if (inactiveTime > MaxInactiveInterval)
                {
                    Debug.Log("More than 10 minutes passed. Starting a new session.");
                    StartNewSession();
                }
            }

            lastInactiveTimestamp = -1f;
        }

        // Start is called before the first frame update
        void Start()
        {
            OnAppStartOrResume();

        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus)
            {
                OnAppStartOrResume();
                // Code here runs when the app resumes from pause.
            }
            else
            {
                lastInactiveTimestamp = Time.realtimeSinceStartup;
            }

        }


        private IEnumerator<WaitForSeconds> UpdateTime()
        {
            while (true)
            {
                //if app paused never update time
                if (lastInactiveTimestamp <= 0)
                {
                    // Update current session time.
                    int sessionTime = (int)(Time.realtimeSinceStartup - sessionStartTimestamp - totalInactiveTime);
                    PlayerPrefs.SetInt(SessionTimeKey, sessionTime);
                    PlayerPrefs.Save();
                }

                yield return new WaitForSeconds(UpdateInterval);
            }
        }

        private void OnApplicationQuit()
        {
            PlayerPrefs.SetInt(LastSessionTimeKey, GetCurrentSessionTime());
            PlayerPrefs.SetInt(SessionTimeKey, 0);
            PlayerPrefs.Save();

            if (updateCoroutine != null)
            {
                StopCoroutine(updateCoroutine);
                updateCoroutine = null;
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void StartNewSession()
        {
            int sessionTime = PlayerPrefs.GetInt(SessionTimeKey, 0);
            if (sessionTime > 0)
            {
                PlayerPrefs.SetInt(LastSessionTimeKey, sessionTime);
                PlayerPrefs.SetInt(SessionTimeKey, 0);
            }

            int sessionCount = PlayerPrefs.GetInt(SessionCountKey, 0);
            // Save the session count.
            PlayerPrefs.SetInt(SessionCountKey, sessionCount + 1);
            PlayerPrefs.Save();

            sessionStartTimestamp = Time.realtimeSinceStartup;

            OnNewSession?.Invoke(GetPreviousSessionTime());
        }


        #region public method

        public int GetCurrentSessionTime()
        {
            return PlayerPrefs.GetInt(SessionTimeKey, 0);
        }

        public int GetPreviousSessionTime()
        {
            return PlayerPrefs.GetInt(LastSessionTimeKey, 0);
        }

        /// <summary>
        /// Returns the total number of sessions played.
        /// </summary>
        public int GetSessionCount()
        {
            return PlayerPrefs.GetInt(SessionCountKey, 0);
        }


        #endregion
    }
}
