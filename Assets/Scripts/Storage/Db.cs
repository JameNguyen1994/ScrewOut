using UnityEngine;

namespace Storage
{
    public class Db : MonoBehaviour
    {
        private static Db instance;
        public static Db Intance => instance;

        public static LocalDb storage;

        private void Awake()
        {
            if (instance)
            {
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
                storage = new LocalDb();
                instance = this;
            }
        }
    }
}
