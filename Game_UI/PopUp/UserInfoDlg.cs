using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CWStruct;
using CWEnum;
using CWUnityLib;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;

public class UserInfoDlg : WindowUI<UserInfoDlg>
{

    public Text m_kOnline;
    public Text m_kName;
    public Text m_kGrade;
    public Text m_kPvpInfo;
    public Text m_kStage;
    public RawImage m_kGradering;
    public Image m_kCharImage;

    bool m_bOnline;

    int m_UserID=0;
    public void Show(int ID)
    {

        m_UserID = ID;

        Open();
    }
    public override void Open()
    {
        // 온라인 중인가?
        CWSocketManager.Instance.AskUserInfo(m_UserID, (jData) => {

            
            if (CWJSon.GetString(jData,"Result")=="ok")
            {
                m_kName.text = CWJSon.GetString(jData, "Name");
                int RankPoint = CWJSon.GetInt(jData,"RankiPoint");
                int nGrade = CWGlobal.GetGrade(RankPoint);
                m_kGrade.text = CWGlobal.GetGradestring(nGrade);

                //v.texture = CWResourceManager.Instance.GetTexture(CWGlobal.GetGradeCircleFileName(nGrade));

                int PvpTotal = CWJSon.GetInt(jData, "PvpTotal");
                int PvpWin = CWJSon.GetInt(jData, "PvpWin");
                int CharNumber = CWJSon.GetInt(jData, "CharNumber");
                m_kPvpInfo.text = string.Format("{0}판중{1}승{2}패", PvpTotal,PvpWin,PvpTotal- PvpWin);

                int Stage = CWJSon.GetInt(jData, "Stage");
                int num = (Stage - 1) / 6+1;
                string szplanet= CWTableManager.Instance.GetTable("블록이름,스토리 - 행성", "name", num);///
                
                CWArrayManager.StageData kk = CWArrayManager.Instance.GetStageData(Stage);
                string szStage = kk.m_szName;
                m_kStage.text = string.Format("{0}-{1}  {2}_{3}",num,(Stage-1)%6+1,szplanet,szStage);
                m_kGradering.texture = CWResourceManager.Instance.GetTexture(CWGlobal.GetGradeCircleFileName(nGrade));

                string szchar= string.Format("char_{0}", CharNumber);
                m_kCharImage.sprite = CWResourceManager.Instance.GetSprite(szchar);

                if (CWJSon.GetInt(jData, "Online") ==0)
                {
                    m_bOnline = false;
                    m_kOnline.text = "Offline";
                    m_kOnline.color = Color.gray;
                }
                else
                {
                    m_bOnline = true;
                    m_kOnline.text = "Online";
                    m_kOnline.color = new Color(102/255f,236 / 255f, 21 / 255f);
                }

            }
            else
            {
                Debug.Log("정보 없음");
            }
            
        

        
        });



        base.Open();
    }

    public void OnPVP()
    {
        int nID = ScrollListUI.g_kSelectScrol.GetSelectValueInt("ID");
        
        if (!m_bOnline)
        {
            NoticeMessage.Instance.Show("온라인 상태가 아닙니다!");
            return;
        }

        if (nID == CWHero.Instance.m_nID) return;
        
        CWSocketManager.Instance.SendAskPvp(nID, (flag) => {

            ChattingDlg.Instance.Close();
            Close();
            if (flag)// 수락
            {

            }
            else
            {

            }
        
        });
    }
}
