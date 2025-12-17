using CodeStage.AntiCheat.ObscuredTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainMenuBar
{
    public class DBMainMenuBarModel : MonoBehaviour
    {
    }
    [System.Serializable]
    public class DBBarsItem
    {
        public List<int> lstIndexCompleted;
        public List<DBBarItem> lstDBBarItem;
        public DBBarsItem(List<ItemBarData> lstItemBarData)
        {
            lstDBBarItem = new List<DBBarItem>();
            for (int i = 0; i < lstItemBarData.Count; i++)
            {
                DBBarItem dBBar = new DBBarItem();
                dBBar.index = i;
                dBBar.isUnlock = lstItemBarData[i].levelUnlock == 0;
                dBBar.isNew = false;
                dBBar.levelUnlock = lstItemBarData[i].levelUnlock;
                lstDBBarItem.Add(dBBar);
            }
            lstIndexCompleted = new List<int>() { 0 };
        }
        public void CheckUnlock(int level)
        {
            for (int i = 0; i < lstDBBarItem.Count; i++)
            {
                lstDBBarItem[i].SetUnlock(level);
            }
        }
        public List<int> GetListIndexTutorial(int level)
        {
            var lstIndexNew = new List<int>();
            for (int i = 0; i < lstDBBarItem.Count; i++)
            {
                if (!lstIndexCompleted.Contains(lstDBBarItem[i].levelUnlock) && level >= lstDBBarItem[i].levelUnlock)
                {
                    lstIndexNew.Add(i);
                }
            }
            return lstIndexNew;
        }
        public void SetIndexCompleted(int index)
        {
            var level = lstDBBarItem[index].levelUnlock;
            if (!lstIndexCompleted.Contains(level))
            {
                lstIndexCompleted.Add(level);
            }
            DBMainMenuBarController.Instance.DB_MAIN_MENU_ITEMS = this;
        }

    }
    [System.Serializable]
    public class DBBarItem
    {
        public int index;
        public ObscuredBool isUnlock;
        public ObscuredBool isNew;
        public ObscuredInt levelUnlock;
        public DBBarItem()
        {
            isUnlock = false;
            isNew = false;
        }
        public void SetUnlock(int level)
        {
            if (!isUnlock && level >= levelUnlock)
            {
                isUnlock = true;
                isNew = true;
            }
        }
        public void SetNew(bool isNew)
        {
            this.isNew = isNew;
        }
    }
}
