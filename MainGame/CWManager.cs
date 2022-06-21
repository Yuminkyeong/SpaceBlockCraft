using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWBaseManager : MonoBehaviour
{
    protected bool m_bCreated=false;
    public virtual void Create()
    {
        m_bCreated = true;
    }

}

// 자동 등록 
public class CWManager<T> : CWBaseManager
{

    
    #region Singleton
    static T _Instance;
    public static T Instance
    {
        get
        {
            return _Instance;
        }
    }
    protected void Awake()
    {

        _Instance = this.GetComponent<T>();
    }

    #endregion


}
