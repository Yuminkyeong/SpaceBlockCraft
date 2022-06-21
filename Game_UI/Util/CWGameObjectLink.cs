using UnityEngine;
using System.Collections;

public class CWGameObjectLink : MonoBehaviour
{

    public GameObject[] m_gActive;
    public GameObject[] m_gDactive;


    public GameObject[] m_gDactiveToggle;// 
    public GameObject[] m_gActiveToggle;// 

    enum TYPE { NONE, TRUE, FALSE };

    TYPE[] m_nFlag;

    void Show(GameObject gg, bool flag)
    {

        
        BaseUI bs = gg.GetComponent<BaseUI>();
        if (bs)
        {
            if (flag) bs.Open();
            else bs.Close();
        }
        else
        {
            gg.SetActive(flag);
        }

      //  Debug.Log(string.Format("{0} {1}", gg.name,flag));

    }

    void Run()
    {
        StartCoroutine("IRun");
    }
    void OnEnable()
    {
        Run();
    }
    void OnDisable()
    {

        if (m_nFlag == null) return;
        for (int i = 0; i < m_gDactiveToggle.Length; i++)
        {
            if (m_nFlag[i] == TYPE.NONE) continue;
            if (m_gDactiveToggle[i] != null)
            {
                if (m_nFlag[i] == TYPE.TRUE)
                {

                    //m_gDactiveToggle[i].SetActive(true);
                    Show(m_gDactiveToggle[i], true);
                }
                else
                {
                    //m_gDactiveToggle[i].SetActive(false);
                    Show(m_gDactiveToggle[i], false);
                }

            }

        }
        foreach (var v in m_gActiveToggle)
        {
            if (v != null)
            {
                Show(v, false);
            }
            //v.SetActive(false);
        }


    }

    IEnumerator IRun()
    {
        yield return new WaitForSeconds(0.1f);
        m_nFlag = new TYPE[m_gDactiveToggle.Length];

        foreach (var v in m_gActive)
        {
            if (v != null)
            {
                Show(v, true);
            }
            else
            {
               // Debug.Log("");
            }
        }
        foreach (var v in m_gDactive)
        {

            if (v != null)
            {
                Show(v, false);
            }
            else
            {
                //Debug.Log("");
            }

        }

        for (int i = 0; i < m_gDactiveToggle.Length; i++)
        {
            if (m_gDactiveToggle[i] != null)
            {
                if (m_gDactiveToggle[i].activeInHierarchy == false)
                {
                    m_nFlag[i] = TYPE.NONE;
                }
                else
                {
                    if (m_gDactiveToggle[i].activeSelf)
                    {
                        m_nFlag[i] = TYPE.TRUE;
                    }
                    else
                    {
                        m_nFlag[i] = TYPE.FALSE;
                    }

                }
            }
            else
            {
                //        Debug.Log("");
            }


        }
        for (int i = 0; i < m_gDactiveToggle.Length; i++)
        {
            if (m_nFlag[i] == TYPE.NONE) continue;

            if (m_gDactiveToggle[i] != null)
            {
                Show(m_gDactiveToggle[i], false);
            }
            else
            {
                //Debug.Log("");
            }

        }

        foreach (var v in m_gActiveToggle)
        {
            if (v != null)
            {

                Show(v, true);
            }
            else
            {
              //  Debug.Log("");
            }

        }





    }
}
