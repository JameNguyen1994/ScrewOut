using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UserProfile
{
    public interface TabBase
    {
        public void Init();
        public void OnOpenTab();
        public void OnCloseTab();
    }
}
