using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CWUnityLib;
using CWEnum;
public class BaseUnitUI : MonoBehaviour
{


    public GameObject m_gVisible;
    public Text m_kDamageText;
    public Text m_kName;
    public Image m_kSliderbar;

    public GameObject m_gWarmode;
    public GameObject m_gMusuk;

    int m_nHP = 0;

    public void Show()
    {
        if (m_gVisible == null) return;
        m_gVisible.SetActive(true);
        CWAirObject kUnit = GetComponentInParent<CWAirObject>();
        int nHP = kUnit.KPower.GetHP();
        
        m_kDamageText.text = nHP.ToString()+"/"+ kUnit.KPower.m_nHp.ToString();
        m_nHP = kUnit.KPower.GetHP();

        if (m_kName != null)
        {
            m_kName.text = kUnit.GetName();
        }

        m_gWarmode.SetActive(false);
        m_gMusuk.SetActive(false);
    }
    public void Hide()
    {
        if (m_gVisible == null) return;
        m_gVisible.SetActive(false);
    }

    private void OnEnable()
    {
        if (CWHero.Instance == null) return;
        if (m_gVisible == null) return;
        m_gVisible.SetActive(true);
        StartCoroutine("IRun");
        
    }
    virtual public void UpdateData()
    {
        CWAirObject kUnit = GetComponentInParent<CWAirObject>();
        if(kUnit==null)
        {
            return;
        }
        if (kUnit.KPower == null)
        {

            return;
        }
        Slider sl = GetComponentInChildren<Slider>();
        if(sl!=null)
        {
            
            sl.value = kUnit.KPower.FhpRate;
        }
        if(m_kSliderbar!=null)
        {
            m_kSliderbar.fillAmount = kUnit.KPower.FhpRate;
        }

        int nHP = kUnit.KPower.GetHP();
        if(m_nHP!=nHP)
        {

            m_kDamageText.text = nHP.ToString() + "/" + kUnit.KPower.m_nHp.ToString();

            m_kDamageText.transform.localScale = Vector3.one;
            m_kDamageText.transform.DOPunchScale(new Vector3(2, 2, 2), 0.25f).OnComplete(()=> {
                m_kDamageText.transform.localScale = Vector3.one;
            });
        }
        m_nHP = nHP;
        if (m_kName != null)
        {
            m_kName.text = kUnit.GetName();
        }




    }
    IEnumerator IRun()
    {
        while(true)
        {
            UpdateData();
            yield return new WaitForSeconds(0.2f);
        }
    }
    #region 메세지 

    // 메세지를 띄우고 시간이 지나면 사라진다

    
    public Text m_kMessage;
    public GameObject m_gMessage;

    float m_fLifetime=5f;
    public void Message( string szstr,float ftime)
    {
        if (m_gMessage == null) return;
        m_fLifetime = ftime;
        m_kMessage.text = szstr;
        if (m_gMessage.activeSelf==false)
        {
            m_gMessage.SetActive(true);
            StartCoroutine("IRunMessage");
        }
    }
    public void StopMessage()
    {
        m_fLifetime = 0;
        
    }

    private void Awake()
    {
        if (m_gVisible == null) return;
        m_gVisible.SetActive(false);
        if (m_gMessage == null) return;
        m_gMessage.SetActive(false);
    }
    public float m_fMin = -400;
    public float m_fStart = 400;
    public float m_fSpeed = 1f;
    IEnumerator IRunMessage()
    {
        RectTransform rt = m_kMessage.GetComponent<RectTransform>();

        float ftime = Time.time;

        while (true)
        {
            if(Time.time - ftime>=m_fLifetime)
            {
                break;
            }
            Vector2 vPos = rt.anchoredPosition;
            if (vPos.x < m_fMin)
            {
                vPos.x = m_fStart;
            }
            else
            {
                vPos.x -= m_fSpeed * Time.deltaTime;
            }
            rt.anchoredPosition = vPos;
            yield return null;
        }
        if (m_gMessage != null)
            m_gMessage.SetActive(false);
    }

    #endregion
    #region 이모티콘

    void HideWoarmode()
    {
        m_gWarmode.SetActive(false);
    }
    public void SetWarmode()
    {
        if (m_gWarmode.activeSelf) return;
        m_gWarmode.SetActive(true);

        Invoke("HideWoarmode", 12f);

    }
    public void SetMusukBar(int state)
    {
        if (state == 1) m_gMusuk.SetActive(true);
        else m_gMusuk.SetActive(false);
    }
    #endregion
}
