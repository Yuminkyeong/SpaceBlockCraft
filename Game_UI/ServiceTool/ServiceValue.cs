using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
using UnityEngine.UI;
using DG.Tweening;
using CWEnum;
using CWStruct;

public class ServiceValue : MonoBehaviour
{
    static public float g_fStartTimer = 0;
    static public float g_fLifeTimer = 0; // 정해진시간

    static public GITEMDATA g_kSelectItemData = new GITEMDATA();


    public string m_szValues;
    public string m_szFormat;
    public string m_szMaxValues; // 최대치가 있다

    float m_fDelay = 0.3f;



    public enum UITYPE { AUTO, TEXT, SLIDER, FACE, TEXTURE, ICON, SPRITE, SPRITESLIDER, ITEMSLOT, SHOWHIDE, INC_NUMBER };// INC_NUMBER : 증가하는 숫자
    public UITYPE m_Type = UITYPE.AUTO;

    Text m_kText;
    Slider m_kSlider;
    RawImage m_kRawImage;
    Image m_kSprite;

    ItemInfoSlot m_kItemInfo;


    int m_nTempValue = 0;
    int m_nTempMaxValue = 0;
    bool m_bTempRun = false;
    private void Awake()
    {

        m_kItemInfo = GetComponent<ItemInfoSlot>();
        if (m_kItemInfo)
        {
            if (m_Type == UITYPE.AUTO)
                m_Type = UITYPE.ITEMSLOT;

        }

        m_kText = GetComponent<Text>();
        if (m_kText != null)
        {
            if (m_Type == UITYPE.AUTO)
                m_Type = UITYPE.TEXT;
        }
        m_kSlider = GetComponent<Slider>();
        if (m_kSlider != null)
        {
            if (m_Type == UITYPE.AUTO)
                m_Type = UITYPE.SLIDER;
        }
        m_kRawImage = GetComponent<RawImage>();
        if (m_kRawImage != null)
        {
            if (m_Type == UITYPE.AUTO)
                m_Type = UITYPE.TEXTURE;
        }
        m_kSprite = GetComponent<Image>();
        if (m_kSprite != null)
        {
            if (m_Type == UITYPE.AUTO)
            {

                if (m_kSprite.type == Image.Type.Filled)
                {
                    m_Type = UITYPE.SPRITESLIDER;
                }
                else
                {
                    m_Type = UITYPE.SPRITE;
                }

            }


        }


    }
    private void OnEnable()
    {
        StopCoroutine("IRun");
        StartCoroutine("IRun");

    }
    private void OnDisable()
    {
        EndTask();
    }
    string GetValueString(string szValue)
    {
        if (szValue == "현재접속유저")
        {
            return ServiceMain.Instance.NowCount.ToString();

        }
        if (szValue == "오늘 접속 수")
        {
            return ServiceMain.Instance.TodayPlayUser.ToString();
        }
        if (szValue == "누적 유저 수")
        {
            return ServiceMain.Instance.AllCount.ToString();
        }
        if (szValue == "오늘 광고 수")
        {
            return ServiceMain.Instance.TodayAD.ToString();
        }
        if (szValue == "누적 광고수")
        {
            return ServiceMain.Instance.AllAD.ToString();
        }
        if (szValue == "오늘 현금 상점")
        {
            return ServiceMain.Instance.TodayBuiling.ToString();
        }
        if (szValue == "오늘 최대 소비 품목")
        {

        }
        if (szValue == "오늘 최대 소비 품목")
        {

        }
        if (szValue == "오늘 평균 게임 시간")
        {

        }
        if (szValue == "누적 최대 소비 품목")
        {

        }

        if (szValue == "10이상 출석")
        {
            return ServiceMain.Instance.Day10.ToString();
        }
        if (szValue == "2일 출석")
        {
            return ServiceMain.Instance.Day2.ToString();
        }
        if (szValue == "3일 출석")
        {
            return ServiceMain.Instance.Day3.ToString();
        }
        if (szValue == "튜토리얼스킵")
        {
            return ServiceMain.Instance.Tutoskipcount.ToString();
        }
        if (szValue == "스토리1")
        {
            return ServiceMain.Instance.Storycount1.ToString();
        }
        if (szValue == "스토리4")
        {
            return ServiceMain.Instance.Storycount4.ToString();
        }
        if (szValue == "스토리5")
        {
            return ServiceMain.Instance.Storycount5.ToString();
        }
        if (szValue == "스토리6")
        {
            return ServiceMain.Instance.Storycount6.ToString();
        }
        if (szValue == "1:1대전")
        {
            return ServiceMain.Instance.PVPCount.ToString();
        }
        if (szValue == "마이룸")
        {
            return ServiceMain.Instance.MyroomCount.ToString();
        }
        if (szValue == "멀티")
        {
            return ServiceMain.Instance.MultiCount.ToString();
        }



        return "";
    }
   
    float GetValueRate(string szValue)//0~1으로 처리 하는 개념만
    {
        if (szValue == "남은시간")
        {
            return (Time.time - g_fStartTimer) / g_fLifeTimer;//0~1
        }



        string szval = GetValueString(szValue);
        return CWLib.ConvertFloat(szval);
    }
    bool m_bDWTFlag = false;
    void UpdateData()
    {
        if (CWArrayManager.Instance == null) return;
        if (CWHero.Instance == null) return;
        if (CWHeroManager.Instance == null) return;
        if (m_Type == UITYPE.TEXT)
        {
            // 개념 설정
            // 쉼표등 표현이 필요한 문자는, 정수로 바꿔서 출력한다
            if (CWLib.IsString(m_szMaxValues))// 최대치가 있다면
            {
                string sz1 = GetValueString(m_szValues);
                string sz2 = GetValueString(m_szMaxValues);
                m_kText.text = string.Format("{0}/{1}", sz1, sz2);
            }
            else
            {
                if (CWLib.IsString(m_szFormat))
                {
                    m_kText.text = string.Format(m_szFormat, GetValueString(m_szValues));
                }
                else
                {
                    m_kText.text = GetValueString(m_szValues);
                }

            }

            return;
        }
        if (m_Type == UITYPE.SLIDER)
        {
            if (CWLib.IsString(m_szMaxValues))// 최대치가 있다면
            {
                string sz1 = GetValueString(m_szValues);
                string sz2 = GetValueString(m_szMaxValues);
                float f1 = CWLib.ConvertFloat(sz1);
                float f2 = CWLib.ConvertFloat(sz2);

                m_kSlider.value = f1 / f2;
                if (m_kSlider.value > 1) m_kSlider.value = 1;
                if (m_kSlider.value < 0) m_kSlider.value = 0;

            }
            else
            {
                m_kSlider.value = GetValueRate(m_szValues);
            }



        }
        if (m_Type == UITYPE.TEXTURE)
        {
            m_kRawImage.texture = CWResourceManager.Instance.GetTexture(GetValueString(m_szValues)); //.GetFaceTexture(GetValueString(m_szValues));
        }
        if (m_Type == UITYPE.ICON)
        {
            int nID = CWLib.ConvertInt(GetValueString(m_szValues));
            m_kRawImage.texture = CWResourceManager.Instance.GetItemIcon(nID);
        }
        if (m_Type == UITYPE.SPRITE)
        {
            m_kSprite.sprite = CWResourceManager.Instance.GetSprite(GetValueString(m_szValues));
        }
        if (m_Type == UITYPE.SPRITESLIDER)
        {
            m_kSprite.fillAmount = GetValueRate(m_szValues);
        }
        if (m_Type == UITYPE.ITEMSLOT)
        {
            m_kItemInfo.m_nItem = CWLib.ConvertInt(GetValueString(m_szValues));
            m_kItemInfo.UpdateData();
        }
        if (m_Type == UITYPE.SHOWHIDE)
        {


            GameObject gg = CWLib.FindChild(gameObject, "visible");
            if (gg == null) return;
            int nRet = CWLib.ConvertInt(GetValueString(m_szValues));
            if (nRet == 1)
            {
                if (gg.activeSelf == false)
                {
                    gg.SetActive(true);

                    if (!m_bDWTFlag)
                    {
                        gg.transform.DOScale(0, 0).OnComplete(() => {
                            gg.transform.DOScale(1, 0.2f).OnComplete(() => {

                                m_bDWTFlag = false;
                            });

                        });

                    }
                    m_bDWTFlag = true;

                }


            }
            else
            {
                if (gg.activeSelf == true)
                {
                    if (!m_bDWTFlag)
                    {
                        gg.transform.DOScale(0, 0.2f).OnComplete(() => {
                            gg.SetActive(false);
                            gg.transform.DOScale(1, 0);
                            m_bDWTFlag = false;
                        });

                    }
                    m_bDWTFlag = true;
                }

            }
        }
        if (m_Type == UITYPE.INC_NUMBER)
        {
            // 증가하는 텍스쳐
            if (m_bTempRun == false)
            {
                string szValues = GetValueString(m_szValues);
                //m_kText.text = szValues;

                int nValue = CWLib.ConvertInt(szValues);
                if (nValue == 0 || nValue != m_nTempValue)
                {
                    m_nTempMaxValue = nValue;
                    m_bTempRun = true;

                    // EndTask();
                    StopCoroutine("IncRun");
                    StartCoroutine("IncRun");
                }
            }

        }

    }
    void EndTask()
    {
        if (m_bTempRun)
        {
            m_kText.text = m_nTempMaxValue.ToString();
            m_nTempValue = m_nTempMaxValue;
        }
        m_nTempMaxValue = 0;
        m_bTempRun = false;

    }
    IEnumerator IncRun()
    {
        // 0.3초 변화 
        // 맥스 시간 1초 
        // 간격 
        float fMaxTime = 0.5f;
        float fStartTime = Time.time;
        float fMaxValue = m_nTempMaxValue - m_nTempValue;
        float fStartValue = m_nTempValue;

        while (true)
        {
            float ftime = Time.time - fStartTime;
            if (ftime > fMaxTime)
            {

                break;
            }
            float fRate = ftime / fMaxTime;
            float fValue = fStartValue + fMaxValue * fRate;
            m_kText.text = ((int)fValue).ToString();


            yield return null;
        }
        EndTask();

    }
    IEnumerator IRun()
    {
        while (true)
        {
            if (CWGlobal.G_bGameStart)
            {
                UpdateData();

            }
            yield return new WaitForSeconds(m_fDelay);
        }
    }
    private void Start()
    {
        StartCoroutine("IRun");
    }


}
