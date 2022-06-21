using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginBaseWin<T> : MonoBehaviour
{

    public GameObject m_gvisible;
    bool bShow = false;
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
    protected bool m_bShow
    {
        get
        {
            return bShow;
        }
        set
        {
            bShow = value;
            //print(string.Format("Show {0}  -{1}",bShow,name));
        }
    }

    public virtual void Open()
    {
        m_gvisible.SetActive(true);
    }
    
    public virtual void Close()
    {

        m_gvisible.SetActive(false);

    }



}
