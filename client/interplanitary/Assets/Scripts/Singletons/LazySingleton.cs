
using UnityEngine;

public class LazySingleton<T> : MonoBehaviour where T : MonoBehaviour
{

    static T _instance;

    public static T instance
    {
        get
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("[LazySingleton] Instance '" + typeof(T) +
                    "' being accessed while application is not playing");
                return null;
            }

            if (_instance == null)
            {
                _instance = (T)FindObjectOfType(typeof(T));

                if(_instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).ToString());
                    _instance = obj.AddComponent<T>();
                }
            }

            return _instance;
        }
    }

}
