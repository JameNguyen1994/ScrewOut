using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PS.Ad.Utils
{
    public interface IAnimationInform
    {
        UnityAction onCompleted { get; set; }
        void Open();
        void Close();
    }

}
