using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ps.modules.leaderboard
{
    public class AllYearRuntimeManager : MonoBehaviour
    {
        private const string FILE_NAME = "AllYearDataRuntime.json";

        [Header("References")]
        [SerializeField] private AllYearData allYearDataAsset; // bản gốc trong Assets
        [SerializeField] private AllYearData allYearDataRuntime; // bản runtime clone

        public static AllYearRuntimeManager Instance { get; private set; }

        private string SavePath => Path.Combine(Application.persistentDataPath, FILE_NAME);

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Initialize()
        {
            // 1️⃣ Clone từ asset gốc
            allYearDataRuntime = ScriptableObject.Instantiate(allYearDataAsset);

            // 2️⃣ Nếu có file JSON cũ → load đè dữ liệu
            if (File.Exists(SavePath))
            {
                string json = File.ReadAllText(SavePath);
                JsonUtility.FromJsonOverwrite(json, allYearDataRuntime);
                Debug.Log($"🔄 Loaded AllYearDataRuntime from {SavePath}");
            }
            else
            {
                Debug.Log("🆕 No runtime data found — using default asset data.");
                Save(); // tạo file ban đầu
            }
        }

        public AllYearData GetData() => allYearDataRuntime;

        // 3️⃣ Lưu runtime về JSON
        public void Save()
        {
            string json = JsonUtility.ToJson(allYearDataRuntime, true);
            File.WriteAllText(SavePath, json);
            Debug.Log($"✅ Saved AllYearDataRuntime → {SavePath}");
        }
    }
}
