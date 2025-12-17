using UnityEngine;

namespace VideoEditorHelper
{

    public class AutoDisableGameObject : MonoBehaviour
    {

        // Update is called once per frame
        void Update()
        {
            gameObject.SetActive(false);
        }
    }
}