using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Storage.Model
{

    [Serializable]
    public class OfferwallData
    {
        public double TotalVirtualCurrency;
        public double TotalRevenue;
    }

    [Serializable]
    public class OfferwallUserInfo
    {
        public string uuid;
        public string token;
    }

    [Serializable]
    public class UserInfo
    {
        public ObscuredInt coin;
        public ObscuredInt score;
        public ObscuredInt star;
        public ObscuredInt level;
        public ObscuredInt totalSession;
        public ObscuredInt maxLevel;
        public ObscuredDouble total_ad_revenue;
        public ObscuredDouble total_iap_pack;
        public ObscuredDateTime beginSaleAdsValidUntil;
        public ObscuredDateTime beginSaleAdsNextAvailable;
        public ObscuredInt countInterAds;
        public ObscuredInt playTime;
        public ObscuredInt playTimeShowAds;
        public ObscuredInt spinCount;

        public UserInfo()
        {
            coin = 0;
            score = 0;
            star = 0;
            level = 1;
        }

        public void RandomizeCryptoKey()
        {
            coin.RandomizeCryptoKey();
            score.RandomizeCryptoKey();
            star.RandomizeCryptoKey();
            level.RandomizeCryptoKey();
            totalSession.RandomizeCryptoKey();
            maxLevel.RandomizeCryptoKey();
            total_ad_revenue.RandomizeCryptoKey();
            beginSaleAdsValidUntil.RandomizeCryptoKey();
            beginSaleAdsNextAvailable.RandomizeCryptoKey();
            countInterAds.RandomizeCryptoKey();
        }

        public override string ToString()
        {
            return $@"coin: {coin}
score: {score}
level: {level}";
        }

        [System.Reflection.Obfuscation(Exclude = false)]
        public string GetH()
        {
            var dict = new Dictionary<string, object>()
            {
                ["coin"] = this.coin.GetDecrypted(),
                ["score"] = this.score.GetDecrypted(),
                ["star"] = this.star.GetDecrypted(),
                ["totalSession"] = this.totalSession.GetDecrypted(),
                ["maxLevel"] = this.maxLevel.GetDecrypted()
            };

            var json = JsonConvert.SerializeObject(dict, Formatting.None);
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
    [Serializable]
    public class BoosterDatas
    {
        public List<BoosterData> lstBoosterData;
        public BoosterDatas()
        {
            /*            Debug.Log("Old BoosterDatas");
                        lstBoosterData = new List<BoosterData>
                        {
                            new BoosterData(BoosterType.AddHole, 2, 50),
                            new BoosterData(BoosterType.Hammer, 2, 100),
                            new BoosterData(BoosterType.Clears,2, 200),
                            new BoosterData(BoosterType.Magnet, 2, 300),
                            new BoosterData(BoosterType.UnlockBox, 0, 300)
                        };*/
        }
        public BoosterDatas(bool newBooster)
        {
            Debug.Log("New BoosterDatas");
            lstBoosterData = new List<BoosterData>();
            lstBoosterData.Add(new BoosterData(BoosterType.AddHole, 3, 500));
            lstBoosterData.Add(new BoosterData(BoosterType.Hammer, 3, 800));
            lstBoosterData.Add(new BoosterData(BoosterType.Clears, 3, 1500));
            // lstBoosterData.Add(new BoosterData(BoosterType.Magnet, 0, 150));
            lstBoosterData.Add(new BoosterData(BoosterType.UnlockBox, 3, 900));
        }

        [System.Reflection.Obfuscation(Exclude = false)]
        public string GetH()
        {
            var dict = new Dictionary<string, object>();

            for (int i = 0; i < this.lstBoosterData.Count; i++)
            {
                dict.Add($"item_{i}", this.lstBoosterData[i].GetElementH());
            }

            var json = JsonConvert.SerializeObject(dict, Formatting.None);
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        public void RandomizeCryptoKey()
        {
            for (int i = 0; i < lstBoosterData.Count; i++)
            {
                lstBoosterData[i].RandomizeCryptoKey();
            }
        }
        public void AddBooster(BoosterType boosterType, int value)
        {
            Debug.Log($"AddBooster type {boosterType} : {value}");
            if (this.GetH() != ObscuredPrefs.Get(DbKey.H_BOOSTER, ""))
            {
                return;
            }

            var boosterData = lstBoosterData.Find(x => x.boosterType == boosterType);
            if (boosterData == null)
            {
                if (boosterType == BoosterType.FreeRevive)
                {
                    lstBoosterData.Add(new BoosterData(BoosterType.FreeRevive, value, 0));
                    SaveToDB();
                    return;
                }

                Console.WriteLine($"Null booster type {boosterType}");
                return;
            }
            boosterData.value += value;
            SaveToDB();
        }
        public void UseBooster(BoosterType boosterType)
        {
            if (this.GetH() != ObscuredPrefs.Get(DbKey.H_BOOSTER, ""))
            {
                return;
            }

            var boosterData = lstBoosterData.Find(x => x.boosterType == boosterType);

            if (boosterData == null)
            {
                Console.WriteLine($"Null booster type {boosterType}");
                return;
            }
            boosterData.value--;

            switch (boosterType)
            {
                case BoosterType.AddHole:
                    IngameData.TRACKING_ADD_HOLE_COUNT++;
                    break;
                case BoosterType.Hammer:
                    IngameData.TRACKING_HAMMER_COUNT++;
                    break;
                case BoosterType.Clears:
                    IngameData.TRACKING_CLEAR_COUNT++;
                    break;
                case BoosterType.UnlockBox:
                    IngameData.TRACKING_UNLOCK_BOX_COUNT++;
                    break;
            }

            Debug.Log($"USE booster type {boosterType} : {boosterData.value}");

            SaveToDB();

            int level = 0;
            float percentage = 0;

            if (SceneManager.GetActiveScene().name == "GamePlayNewControl")
            {
                level = Db.storage.USER_INFO.level;
                percentage = IngameData.TRACKING_UN_SCREW_COUNT * 1.0f / LevelController.Instance.Level.LstScrew.Count;
            }

            TrackingController.Instance.TrackingInventory(level, percentage);
        }
        public int CountBooster(BoosterType boosterType)
        {
            if (this.GetH() != ObscuredPrefs.Get(DbKey.H_BOOSTER, ""))
            {
                return 0;
            }

            var boosterData = lstBoosterData.Find(x => x.boosterType == boosterType);
            if (boosterData == null)
            {
                Console.WriteLine($"Null booster type {boosterType}");
                return 0;
            }
            return boosterData.value;
        }
        public int PriceBooster(BoosterType boosterType)
        {
            if (this.GetH() != ObscuredPrefs.Get(DbKey.H_BOOSTER, ""))
            {
                return 10000;
            }

            var boosterData = lstBoosterData.Find(x => x.boosterType == boosterType);
            if (boosterData == null)
            {
                Console.WriteLine($"Null booster type {boosterType}");
                return 0;
            }
            return boosterData.price;
        }
        public void GetFreeBooster(BoosterType boosterType)
        {
            if (this.GetH() != ObscuredPrefs.Get(DbKey.H_BOOSTER, ""))
            {
                return;
            }

            var boosterData = lstBoosterData.Find(x => x.boosterType == boosterType);

            if (boosterData == null)
            {
                Console.WriteLine($"Null booster type {boosterType}");
                return;
            }
            boosterData.receiveFreeBooster = true;
            Debug.Log($"GetFreeBooster booster type {boosterType} : {boosterData.value}");

            SaveToDB();
        }
        public bool IsReceived(BoosterType boosterType)
        {
            if (this.GetH() != ObscuredPrefs.Get(DbKey.H_BOOSTER, ""))
            {
                return true;
            }

            var boosterData = lstBoosterData.Find(x => x.boosterType == boosterType);
            if (boosterData == null)
            {
                Console.WriteLine($"Null booster type {boosterType}");
                return true;
            }
            return boosterData.receiveFreeBooster;
        }
        public void SaveToDB()
        {
            Db.storage.BOOSTER_DATAS = this;
            ObscuredPrefs.Set(DbKey.H_BOOSTER, this.GetH());
        }

        public override string ToString()
        {
            string debugStr = "";
            for (int i = 0; i < lstBoosterData.Count; i++)
            {
                debugStr += $"BoosterType: {lstBoosterData[i].boosterType} - Value: {lstBoosterData[i].value}\n";
            }
            return debugStr;
        }
    }
    [Serializable]
    public class BoosterData
    {
        public ObscuredInt value;
        public ObscuredInt price;
        public BoosterType boosterType;
        public ObscuredBool receiveFreeBooster;

        public BoosterData(BoosterType boosterType, int defaultValue, int price)
        {
            this.boosterType = boosterType;
            this.value = defaultValue;
            this.price = price;
            receiveFreeBooster = false;
            RandomizeCryptoKey();
        }
        public void RandomizeCryptoKey()
        {
            value.RandomizeCryptoKey();
            price.RandomizeCryptoKey();
            receiveFreeBooster.RandomizeCryptoKey();
        }

        [System.Reflection.Obfuscation(Exclude = false)]
        public Dictionary<string, object> GetElementH()
        {
            return new Dictionary<string, object>
            {
                ["value"] = this.value.GetDecrypted(),
                ["price"] = this.price.GetDecrypted(),
                ["boosterType"] = this.boosterType,
                ["receiveFreeBooster"] = this.receiveFreeBooster.GetDecrypted()
            };
        }
    }

    [Serializable]
    public class SettingData
    {
        public ObscuredBool sound;
        public ObscuredBool music;
        public ObscuredBool vibra;

        public SettingData(bool no)
        {
            sound = true;
            music = true;
            vibra = true;
            RandomizeCryptoKey();
        }
        public void RandomizeCryptoKey()
        {
            sound.RandomizeCryptoKey();
            music.RandomizeCryptoKey();
            vibra.RandomizeCryptoKey();
        }
        public void ChangeValueSound(bool isActive)
        {
            sound = isActive;
            Db.storage.SETTING_DATAS = this;
        }
        public void ChangeValueVibra(bool isActive)
        {
            vibra = isActive;
            Db.storage.SETTING_DATAS = this;
        }
        public void ChangeValueMusic(bool isActive)
        {
            music = isActive;
            Db.storage.SETTING_DATAS = this;
        }
    }
    [System.Serializable]
    public class BoosterRewardValue
    {
        public BoosterType boosterType;
        public ObscuredInt value;
        public BoosterRewardValue(BoosterType boosterType, ObscuredInt value)
        {
            this.boosterType = boosterType;
            this.value = value;
        }

        [System.Reflection.Obfuscation(Exclude = false)]
        public string GetH()
        {
            var dict = new Dictionary<string, object>()
            {
                ["type_booster"] = (int)boosterType,
                ["amount"] = value.GetDecrypted()
            };

            var json = JsonConvert.SerializeObject(dict, Formatting.None);
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
    [System.Serializable]
    public class RewardData
    {
        public ObscuredInt coinAmount;
        public ObscuredInt heartTimeAmount; //Hour
        public List<BoosterRewardValue> lstBoosterValue;
        public ObscuredInt itemAmount; //Exp
        public ObscuredBool eventWinLevel;
        public RewardData()
        {
            coinAmount = 0;
            heartTimeAmount = 0;
            itemAmount = 0;
            lstBoosterValue = new List<BoosterRewardValue>();
            eventWinLevel = false;
            RandomizeCryptoKey();
        }

        public RewardData DeepClone()
        {
            List<BoosterRewardValue> lstBooster = new List<BoosterRewardValue>();
            for (int i = 0; i < lstBoosterValue.Count; i++)
            {
                lstBooster.Add(new BoosterRewardValue(lstBoosterValue[i].boosterType, lstBoosterValue[i].value.GetDecrypted()));
            }

            RewardData reward = new RewardData()
            {
                coinAmount = this.coinAmount,
                heartTimeAmount = this.heartTimeAmount,
                itemAmount = this.itemAmount,
                lstBoosterValue = lstBooster,
                eventWinLevel = this.eventWinLevel,
            };

            return reward;
        }

        public void RandomizeCryptoKey()
        {
            coinAmount.RandomizeCryptoKey();
            heartTimeAmount.RandomizeCryptoKey();
        }
        public void BoosterValue(BoosterType boosterType, ObscuredInt value)
        {
            bool hasBooster = lstBoosterValue.Exists(x => x.boosterType == boosterType);
            if (hasBooster)
            {
                var boosterData = lstBoosterValue.Find(x => x.boosterType == boosterType);
                boosterData.value += value;
            }
            else
            {
                var newBooster = new BoosterRewardValue(boosterType, value.GetDecrypted());
                lstBoosterValue.Add(newBooster);
            }

        }
        public void AddCoinValue(ObscuredInt value)
        {
            coinAmount += value;

        }
        public void AddHeartTimeValue(ObscuredInt value)
        {
            heartTimeAmount += value;
        }

        // public void SaveToDB()
        // {
        //     Db.storage.RewardData = this;
        // }

        [System.Reflection.Obfuscation(Exclude = false)]
        public string GetH()
        {
            var dict = new Dictionary<string, object>()
            {
                ["coin"] = coinAmount.GetDecrypted(),
                ["heart_time"] = heartTimeAmount.GetDecrypted(),
                ["item_amount"] = itemAmount.GetDecrypted(),
                ["booster_data"] = lstBoosterValue
            };

            var json = JsonConvert.SerializeObject(dict, Formatting.None);
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }


    [Serializable]
    public class UserExp
    {
        public ObscuredInt level;
        public ObscuredLong exp;
        public ObscuredInt totalExp;

        public void RandomizeCryptoKey()
        {
            level.RandomizeCryptoKey();
            exp.RandomizeCryptoKey();
            totalExp.RandomizeCryptoKey();
        }

        public UserExp DeepClone()
        {
            return new UserExp()
            {
                level = this.level,
                exp = this.exp,
                totalExp = this.totalExp
            };
        }

        [System.Reflection.Obfuscation(Exclude = false)]
        public string GetH()
        {
            var dict = new Dictionary<string, object>()
            {
                ["level"] = this.level.GetDecrypted(),
                ["exp"] = this.exp.GetDecrypted(),
                ["totalExp"] = this.totalExp.GetDecrypted()
            };

            var json = JsonConvert.SerializeObject(dict, Formatting.None);
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }

    [Serializable]
    public struct TrackingAdData
    {
        public double bannerRev;
        public int bannerCount;
        public double interRev;
        public int interCount;
        public double rewardRev;
        public int rewardCount;
        public double mrecRev;
        public int mrecCount;
        public double openRev;
        public int openCount;

        public void Reset()
        {
            this.bannerRev = 0;
            this.interRev = 0;
            this.rewardRev = 0;
            this.mrecRev = 0;
            this.openRev = 0;
            bannerCount = 0;
            interCount = 0;
            rewardCount = 0;
            mrecCount = 0;
            openCount = 0;
        }

        [System.Reflection.Obfuscation(Exclude = false)]
        public string GetH()
        {
            var dict = new Dictionary<string, object>()
            {
                ["bannerCount"] = this.bannerCount,
                ["interCount"] = this.interCount,
                ["rewardCount"] = this.rewardCount,
                ["mrecCount"] = this.mrecCount,
                ["openCount"] = this.openCount
            };
            var json = JsonConvert.SerializeObject(dict, Formatting.None);
            using SHA256 sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }

    [Serializable]
    public struct TrackingData
    {
        public int numOfBlock;
        public int numX1OfLine;
        public int numX2OfLine;
        public int numX3OfLine;
        public int numX4OfLine;
        public int numX5OfLine;
        public int numX6OfLine;
        public int expOfLuckyWheel;
        public int numDropBlock;
        public int mulScoreBlockCount;
        public int mulScoreExp;
        public int singleScoreExp;
        public int singleScoreBlockCount;
        public int expOfSection;

        public void Reset()
        {
            this.numDropBlock = 0;
            this.expOfLuckyWheel = 0;
            this.numX1OfLine = 0;
            this.numX2OfLine = 0;
            this.numX3OfLine = 0;
            this.numX4OfLine = 0;
            this.numX5OfLine = 0;
            this.numX6OfLine = 0;
            this.numOfBlock = 0;
            this.mulScoreExp = 0;
            this.mulScoreBlockCount = 0;
            this.expOfSection = 0;
            this.singleScoreExp = 0;
            this.singleScoreBlockCount = 0;
        }

        public TrackingData DeepClone()
        {
            return new TrackingData()
            {
                numOfBlock = this.numOfBlock,
                numX1OfLine = this.numX1OfLine,
                numX2OfLine = this.numX2OfLine,
                numX3OfLine = this.numX3OfLine,
                numX4OfLine = numX4OfLine,
                numX5OfLine = this.numX5OfLine,
                numX6OfLine = numX6OfLine,
                expOfLuckyWheel = expOfLuckyWheel,
                numDropBlock = numDropBlock,
                mulScoreBlockCount = mulScoreBlockCount,
                mulScoreExp = mulScoreExp,
                singleScoreExp = singleScoreExp,
                singleScoreBlockCount = singleScoreBlockCount,
                expOfSection = expOfSection
            };
        }

        public override string ToString()
        {
            return $"num Of Block: {numOfBlock}\n" +
                   $"num Of Line: {numX1OfLine}: {numX2OfLine}: {numX3OfLine}: {numX4OfLine}: {numX5OfLine}: {numX6OfLine}\n" +
                   $"exp Lucky Wheel: {expOfLuckyWheel}\n" +
                   $"num Of Drop: {numDropBlock}\n";
        }

        [System.Reflection.Obfuscation(Exclude = false)]
        public string GetH()
        {
            var json = JsonConvert.SerializeObject(this, Formatting.None);
            using SHA256 sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }

    [Serializable]
    public class PreBoosterData
    {
        public List<PreBoosterValue> lstPreBoosterData = new List<PreBoosterValue>();
        public PreBoosterData(bool init)
        {
            if (init)
            {
                lstPreBoosterData.Clear();
                lstPreBoosterData.Add(new PreBoosterValue() { preBoosterType = PreBoosterType.Rocket, value = 2, timeFree = 0 });
                lstPreBoosterData.Add(new PreBoosterValue() { preBoosterType = PreBoosterType.Glass, value = 2, timeFree = 0 });
                Debug.Log($"PreBoosterCount {lstPreBoosterData.Count}");
            }
            else
            {
                lstPreBoosterData.Clear();
                lstPreBoosterData.Add(new PreBoosterValue() { preBoosterType = PreBoosterType.Rocket, value = 0, timeFree = 0 });
                lstPreBoosterData.Add(new PreBoosterValue() { preBoosterType = PreBoosterType.Glass, value = 0, timeFree = 0 });
                Debug.Log($"PreBoosterCount {lstPreBoosterData.Count}");
            }
        }
        //public PreBoosterData()
        //{
        //    lstPreBoosterData.Clear();
        //    lstPreBoosterData.Add(new PreBoosterValue() { preBoosterType = PreBoosterType.Rocket, value = 0, timeFree = 0 });
        //    lstPreBoosterData.Add(new PreBoosterValue() { preBoosterType = PreBoosterType.Glass, value = 0, timeFree = 0 });
        //    Debug.Log($"PreBoosterCount {lstPreBoosterData.Count}");
        //}
        public void AddValue(PreBoosterType type, int value)
        {
            var preBoosterClone = Db.storage.PreBoosterData.Deepclone();
            var preBooster = preBoosterClone.lstPreBoosterData.Find(x => x.preBoosterType == type);
            if (preBooster == null)
            {
                Console.WriteLine($"Null preBooster type {type}");
                return;
            }
            preBooster.value += value;
            if (preBooster.value < 0)
                preBooster.value = 0;
            SaveToDB(preBoosterClone);
        }
        public void SaveBooster(PreBoosterType preBoosterType)
        {
            var preBoosterClone = Db.storage.PreBoosterData.Deepclone();

            var preBooster = preBoosterClone.lstPreBoosterData.Find(x => x.preBoosterType == preBoosterType);
            preBooster.isSaveUsing = true;
            SaveToDB(preBoosterClone);
        }
        public int CountValue(PreBoosterType type)
        {
            var preBooster = lstPreBoosterData.Find(x => x.preBoosterType == type);
            if (preBooster == null)
            {
                Console.WriteLine($"Null preBooster type {type}");
                return 0;
            }
            return preBooster.value;
        }
        public void AddFreeTime(PreBoosterType preBoosterType, int time)
        {
            var preBoosterClone = Db.storage.PreBoosterData.Deepclone();

            var preBooster = preBoosterClone.lstPreBoosterData.Find(x => x.preBoosterType == preBoosterType);
            if (preBooster == null)
            {
                Console.WriteLine($"Null preBooster type {preBoosterType}");
                return;
            }
            preBooster.timeFree += time;
            SaveToDB(preBoosterClone);
        }
        public void SaveToDB(PreBoosterData data)
        {
            Db.storage.PreBoosterData = data;
        }
        public void UseFree(PreBoosterType type)
        {
            var preBoosterClone = Db.storage.PreBoosterData.Deepclone();

            var preBooster = preBoosterClone.lstPreBoosterData.Find(x => x.preBoosterType == type);
            if (preBooster == null)
            {
                Console.WriteLine($"Null preBooster type {type}");
                return;
            }
            preBooster.isFree = false;
            SaveToDB(preBoosterClone);
        }
        public bool IsFree(PreBoosterType type)
        {
            var preBooster = lstPreBoosterData.Find(x => x.preBoosterType == type);
            if (preBooster == null)
            {
                Console.WriteLine($"Null preBooster type {type}");
                return false;
            }
            return preBooster.isFree || preBooster.timeFree > 0;
        }
        public bool IsFreeTime(PreBoosterType type)
        {
            var preBooster = lstPreBoosterData.Find(x => x.preBoosterType == type);
            if (preBooster == null)
            {
                Console.WriteLine($"Null preBooster type {type}");
                return false;
            }
            return preBooster.timeFree > 0;
        }
        public bool IsSaveUsing(PreBoosterType type)
        {
            var preBooster = lstPreBoosterData.Find(x => x.preBoosterType == type);
            if (preBooster == null)
            {
                Console.WriteLine($"Null preBooster type {type}");
                return false;
            }
            return preBooster.isSaveUsing;
        }
        public void OnNewGame()
        {
            var preBoosterClone = Db.storage.PreBoosterData.Deepclone();

            foreach (var preBooster in preBoosterClone.lstPreBoosterData)
            {
                preBooster.isSaveUsing = false;
            }
            SaveToDB(preBoosterClone);
        }
        public void CheckOfflineTime(long currentTime, PreBoosterType preBoosterType)
        {
            var preBoosterClone = Db.storage.PreBoosterData.Deepclone();

            var data = preBoosterClone.lstPreBoosterData.Find(x => x.preBoosterType == preBoosterType);
            long offsetTime = 0;
            if (data.lastTime > 0)
            {
                offsetTime = currentTime - data.lastTime;

                if (data.timeFree > 0)
                {
                    data.timeFree -= offsetTime;
                    if (data.timeFree <= 0)
                    {
                        data.lastTime = currentTime;
                        data.timeFree = 0;
                        
                    }
                }
            }
            data.lastTime = currentTime;

            SaveToDB(preBoosterClone);
        }

        [System.Reflection.Obfuscation(Exclude = false)]
        public string GetH()
        {
            var arr = new List<object>();

            foreach (var p in lstPreBoosterData)
            {
                arr.Add(new
                {
                    type = (int)p.preBoosterType,
                    lastTime = p.lastTime.GetDecrypted(),
                    value = p.value.GetDecrypted(),
                    isFree = p.isFree.GetDecrypted(),
                    timeFree = p.timeFree.GetDecrypted(),
                    isSaveUsing = p.isSaveUsing.GetDecrypted()
                });
            }

            string json = JsonConvert.SerializeObject(arr, Formatting.None);

            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));

            StringBuilder sb = new();
            foreach (byte b in bytes)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }



        public void RandomizeCryptoKey()
        {
            for (int i = 0; i < lstPreBoosterData.Count; i++)
            {
                lstPreBoosterData[i].RandomizeCryptoKey();
            }
        }

        public PreBoosterData Deepclone()
        {
            PreBoosterData clone = new PreBoosterData(false);
            clone.lstPreBoosterData.Clear();
            for (int i = 0; i < this.lstPreBoosterData.Count; i++)
            {
                PreBoosterValue value = new PreBoosterValue()
                {
                    preBoosterType = this.lstPreBoosterData[i].preBoosterType,
                    lastTime = this.lstPreBoosterData[i].lastTime,
                    value = this.lstPreBoosterData[i].value,
                    isFree = this.lstPreBoosterData[i].isFree,
                    timeFree = this.lstPreBoosterData[i].timeFree,
                    isSaveUsing = this.lstPreBoosterData[i].isSaveUsing
                };

                //PreBoosterValue preBoosterValue = new PreBoosterValue(
                //    this.lstPreBoosterData[i].preBoosterType,
                //    this.lstPreBoosterData[i].lastTime.GetDecrypted(),
                //    this.lstPreBoosterData[i].value.GetDecrypted(),
                //    this.lstPreBoosterData[i].isFree.GetDecrypted(),
                //    this.lstPreBoosterData[i].timeFree.GetDecrypted(),
                //    this.lstPreBoosterData[i].isSaveUsing.GetDecrypted()
                //    );

                clone.lstPreBoosterData.Add(value);
            }
            return clone;
        }

        public string ToString()
        {
            string debugStr = "";
            for (int i = 0; i < lstPreBoosterData.Count; i++)
            {
                debugStr += $"PreBoosterType: {lstPreBoosterData[i].preBoosterType} - Value: {lstPreBoosterData[i].value}\n";
            }
            return debugStr;
        }
    }

    [Serializable]
    public class PreBoosterValue
    {
        public PreBoosterType preBoosterType;
        public ObscuredLong lastTime = 0;

        public ObscuredInt value = 0;
        public ObscuredBool isFree = true;
        public ObscuredLong timeFree = 0;
        public ObscuredBool isSaveUsing = false;

        public PreBoosterValue()
        {
            lastTime = 0;
            value = 0;
            isFree = true;
            timeFree = 0;
            isSaveUsing = false;

            RandomizeCryptoKey();
        }

        public PreBoosterValue(PreBoosterType preBoosterType, ObscuredLong lastTime, ObscuredInt value, ObscuredBool isFree, ObscuredLong timeFree, ObscuredBool isSaveUsing)
        {
            this.preBoosterType = preBoosterType;
            this.lastTime = lastTime;
            this.value = value;
            this.isFree = isFree;
            this.timeFree = timeFree;
            this.isSaveUsing = isSaveUsing;
        }

        public void RandomizeCryptoKey()
        {
            lastTime.RandomizeCryptoKey();
            value.RandomizeCryptoKey();
            isFree.RandomizeCryptoKey();
            timeFree.RandomizeCryptoKey();
            isSaveUsing.RandomizeCryptoKey();
        }

        public string GetH()
        {
            var dict = new SortedDictionary<string, object>()
            {
                ["type_prebooster"] = (int)preBoosterType,
                ["last_time"] = lastTime.GetDecrypted(),
                ["value"] = value.GetDecrypted(),
                ["is_free"] = isFree.GetDecrypted(),
                ["time_free"] = timeFree.GetDecrypted(),
                ["is_save_using"] = isSaveUsing.GetDecrypted()
            };
            var json = JsonConvert.SerializeObject(dict, Formatting.None);
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }


    }

    public class LuckySpinData
    {
        public ObscuredInt collectedScrew;
        public ObscuredInt collectedInGamplayScrew;
        public ObscuredInt dailySpinByADS;
        public ObscuredInt adsWatchedDay;
        public ObscuredInt adsWatchedMonth;
        public ObscuredInt adsWatchedYear;
        public ObscuredInt spinCount;
        public ObscuredBool isShowTutorial;
        public ObscuredBool isReveal;
    }

    public class ReviveData
    {
        public ObscuredInt count;

        public ReviveData DeepClone()
        {
            return new ReviveData()
            {
                count = this.count
            };
        }

        [System.Reflection.Obfuscation(Exclude = false)]
        public string GetH()
        {
            var dict = new SortedDictionary<string, object>()
            {
                ["count"] = count.GetDecrypted()
            };
            var json = JsonConvert.SerializeObject(dict, Formatting.None);
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }

    [System.Serializable]
    public class LevelBonusData
    {
        public List<int> lstLevelBonusUsed;
        public List<int> lstLevelBonusStart;
    }
}
