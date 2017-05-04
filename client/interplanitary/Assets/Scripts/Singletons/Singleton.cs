
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{

    static T _instance;

    public static T instance
    {
        get
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                    "' being accessed while application is not playing");
                return null;
            }

            if (_instance == null)
            {
                _instance = (T)FindObjectOfType(typeof(T));
            }

            return _instance;
        }
    }

}
