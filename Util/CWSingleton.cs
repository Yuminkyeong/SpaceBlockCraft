using UnityEngine;
using System.Collections;

public class CWSingleton<T> : MonoBehaviour where T : MonoBehaviour
{

    protected static T instance = null;

    void Awake()
    {
        instance = this as T;
    }
    public static T Instance
    {
        get
        {
            return instance;
        }
    }
   
}
