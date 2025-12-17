using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using PS.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using Storage;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Life
{

    public class DBLifeController : Singleton<DBLifeController>
    {
        private LifeInfo _lifeInfo;

        public LifeInfo LIFE_INFO
        {
            get => _lifeInfo;
            set
            {
                _lifeInfo = value;
                Save(DBKey.LIFE_INFO, value);
            }
        }
        protected override void CustomAwake()
        {
            base.CustomAwake();
            Initializing();
        }
        void Initializing()
        {
            CheckDependency(DBKey.LIFE_INFO, key =>

            {
                // LifeInfo lifeInfo = new LifeInfo(5, LifeConfig.TIME_REGENT, 1000 * 60 * 30); /// 15 ph�t infinity
                LifeInfo lifeInfo = new LifeInfo(5, LifeConfig.TIME_REGENT, 0); /// 15 ph�t infinity
                LIFE_INFO = lifeInfo;
            });
            Load();
        }
        void Load()
        {
            _lifeInfo = LoadDataByKey<LifeInfo>(DBKey.LIFE_INFO);
        }
        void CheckDependency(string key, UnityAction<string> onComplete)
        {
            if (!ObscuredPrefs.HasKey(key))
            {
                onComplete?.Invoke(key);
            }
        }
        public void Save<T>(string key, T values)
        {

            if (typeof(T) == typeof(int) ||
                typeof(T) == typeof(bool) ||
                typeof(T) == typeof(string) ||
                typeof(T) == typeof(float) ||
                typeof(T) == typeof(long) ||
                typeof(T) == typeof(Quaternion) ||
                typeof(T) == typeof(Vector2) ||
                typeof(T) == typeof(Vector3) ||
                typeof(T) == typeof(Vector2Int) ||
                typeof(T) == typeof(Vector3Int))
            {
                ObscuredPrefs.Set(key, values);
            }
            else
            {
                try
                {
                    ObscuredString json = JsonUtility.ToJson(values);
                    ObscuredPrefs.Set(key, json);
                }
                catch (UnityException e)
                {
                    throw new UnityException(e.Message);
                }
            }
        }
        public T LoadDataByKey<T>(string key)
        {
            if (typeof(T) == typeof(int) ||
                 typeof(T) == typeof(bool) ||
                 typeof(T) == typeof(string) ||
                 typeof(T) == typeof(float) ||
                 typeof(T) == typeof(long) ||
                 typeof(T) == typeof(Quaternion) ||
                 typeof(T) == typeof(Vector2) ||
                 typeof(T) == typeof(Vector3) ||
                 typeof(T) == typeof(Vector2Int) ||
                 typeof(T) == typeof(Vector3Int))
            {
                var value = ObscuredPrefs.Get<T>(key);
                return value;
            }
            else
            {
                string json = ObscuredPrefs.Get<string>(key);
                return JsonUtility.FromJson<T>(json);
            }
        }
    }
    [System.Serializable]
    public class LifeInfo
    {
        public ObscuredInt lifeAmount;
        public ObscuredLong timeRegen;
        public ObscuredLong timeInfinity;
        public ObscuredLong markTick;
        public ObscuredBool isQuitGameWhenLose;

        public LifeInfo(ObscuredInt lifeAmount, long timeRegen, long timeInfinity)
        {
            this.lifeAmount = lifeAmount;
            this.timeRegen = timeRegen;
            this.timeInfinity = timeInfinity;

        }

        public LifeInfo() { }

        public LifeInfo DeepClone()
        {
            return new LifeInfo()
            {
                lifeAmount = lifeAmount,
                timeRegen = timeRegen,
                timeInfinity = timeInfinity,
                markTick = markTick,
                isQuitGameWhenLose = isQuitGameWhenLose
            };
        }

        public void SetQuitGameWhenLose(bool state)
        {
            this.isQuitGameWhenLose = state;
            SaveToDB();
        }
        public void ResetTimeRegen()
        {
            this.timeRegen = LifeConfig.TIME_REGENT;
            SaveToDB();
        }
        public void AddLifeAmount(int amount)
        {
            EditorLogger.Log("[AddLifeAmount] amount " + amount);

            this.lifeAmount += amount;
            this.lifeAmount = Mathf.Min(this.lifeAmount, LifeConfig.MAX_LIFE);
            SaveToDB();
        }
        public void AddTimeRegen(long amount)
        {
            this.timeRegen = Math.Max(0, this.timeRegen + amount);
            SaveToDB();
        }
        public void AddTimeInfinity(long amount)
        {
            //Debug.Log($"AddTimeInfinity {this.timeInfinity} + {amount} = {this.timeInfinity + amount}");
            markTick = TimeGetter.Instance.CurrentTime;
            timeInfinity = Math.Max(0, timeInfinity + amount);
            SaveToDB();
        }
        private void SaveToDB()
        {
            DBLifeController.Instance.LIFE_INFO = this;
        }
        public void MarkTime(long currentTick)
        {
            markTick = currentTick;
            SaveToDB();
        }
    }
    public class DBKey
    {
        public readonly static string LIFE_INFO = "LIFE_INFO";
    }
}