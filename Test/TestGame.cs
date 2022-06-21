using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
using DG.Tweening;
using UnityEngine.UI;
using CWStruct;
using CWEnum;
using CWUnityLib;
// 실험적인 게임 
public class TestGame : CWBehaviour
{

    public GameObject m_gTarget;
    //public string m_szFileName;
    public BaseUI m_gWindow;

    public void RefreshServerFile(string szFile)
    {
        CWSocketManager.Instance.SendRefreshFile(szFile);
    }
    public void ChangeDir()
    {
        if (!m_gTarget) return;


        Vector3 vTaregt = m_gTarget.transform.position;
        vTaregt.y = CWHero.Instance.transform.position.y;
        CWHero.Instance.transform.DOLookAt(vTaregt, 2f);   //DORotate(m_gTarget.transform.position, 2f, RotateMode.FastBeyond360);
        //CWHero.Instance.TurntoEnmey()
    }
    public void ChagneFile(string szFile)
    {
        if (CWHero.Instance == null) return;
        //CWHero.Instance.Ch
        CWHero.Instance.Load(szFile);
    }
   
#if UNITY_EDITOR

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.F1))
        {
            CWLocalization.Instance.Save();
            //(int nWtype, TALKTYPE tType,string szText,Vector3 vDummypos  , Vector2 vUIPos,float fDist=10 ,float fDelaytime=0f)
            //TalkMessageDlg.Instance.Show(1, TALKTYPE.HAPPY,"하하하", vDummypos, vUIPos,fDist,100);
         //   TalkMessageDlg.Instance.Show("행성을 정복을 축하해요!&이제! 행성의 모든 블록을 캘 수 있어요~");
            //MultiRewardDlg.Instance.Open();
            //      Application.OpenURL("https://play.google.com/store/apps/details?id=com.cwgames.spaceblock4");
            //Application.OpenURL("http://www.google.com");
            //TalkMessageDlg.Instance.Show(1,
            //    TALKTYPE.HAPPY, "행성을 정복을 축하해요!&이제! 행성의 모든 블록을 캘 수 있어요~", new Vector3(-2.64f, -2.5f, 4.11f), new Vector2(0, 354), 10);

            //CWDemageManager.Instance.ShowDamage("+100", Color.yellow);
            //m_gWindow.Open();

            //   HelpMessageDlg.Instance.Show("안녕하세요!!!!");
        }
    }

#endif
}
