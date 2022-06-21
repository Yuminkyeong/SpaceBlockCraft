using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using CWUnityLib;
using CWStruct;
using CWEnum;

public class ServiceMain : PageUI<ServiceMain>
{

    public int NowCount;//접속 유저수
    public int AllCount; // 전체유저
    public int TodayRegUser; //오늘 가입한 유저
    public int TodayPlayUser;//오늘 플레이한 유저
    public int AllBuiling;//결체 총합
    public int TodayBuiling;//오늘 결제
    public int AllAD;  // 광고 총수
    public int TodayAD; //오늘 광고
    public int Day10; //10회 출석
    public int Day2;//2회 출석
    public int Day3;//3회 출석


    
    
    //1: 1행성 접속 햇수
    //내행성 : 접속 햇수
    //멀티행성 접속 햇수

    public int Tutoskipcount ;//        튜토리얼 스킵 숫자
    public int Storycount1;   // 행성 5 갠수
    public int Storycount4;   // 행성 5 갠수
    public int Storycount5;   // 행성 5 갠수
    public int Storycount6;   // 행성 6 갠수

    public int PVPCount;   //1:1행성 접속 햇수
    public int MyroomCount;   //내행성 접속 햇수
    public int MultiCount;//멀티행성 접속 햇수


    DateTime m_DatePoint;
    public Text m_kDay;

    string m_szDay;
    public void OnLeft()
    {
        DateTime tt = m_DatePoint.AddDays(-1);
        m_DatePoint = tt;
        m_szDay = string.Format("{0}-{1:00}-{2:00}", tt.Year,tt.Month,tt.Day);
        m_kDay.text = m_szDay;
        CWSocketManager.Instance.SendManagerTool(m_szDay, ReceiveData, "ReceiveData");
    }
    public void OnRight()
    {
        DateTime tt = m_DatePoint.AddDays(1);
        m_DatePoint = tt;
        m_szDay = string.Format("{0}-{1:00}-{2:00}", tt.Year, tt.Month, tt.Day);
        m_kDay.text = m_szDay;
        CWSocketManager.Instance.SendManagerTool(m_szDay, ReceiveData, "ReceiveData");
    }

    public override void Open()
    {
        CWGlobal.G_bGameStart = true;
        DateTime tt = DateTime.Now;
        m_DatePoint = tt;
        m_szDay = string.Format("{0}-{1:00}-{2:00}", tt.Year, tt.Month, tt.Day);
        m_kDay.text = m_szDay;
        StartCoroutine("IRun");

        base.Open();
    }
    void ReceiveData(JObject jData)
    {
        NowCount = CWLib.ConvertIntbyJson(jData["NowCount"])-1;
        AllCount = CWLib.ConvertIntbyJson(jData["AllCount"]);
        TodayRegUser = CWLib.ConvertIntbyJson(jData["TodayRegUser"]);
        TodayPlayUser = CWLib.ConvertIntbyJson(jData["TodayPlayUser"]);
        AllBuiling = CWLib.ConvertIntbyJson(jData["AllBuiling"]);
        TodayBuiling = CWLib.ConvertIntbyJson(jData["TodayBuiling"]);
        AllAD = CWLib.ConvertIntbyJson(jData["AllAD"]);
        TodayAD = CWLib.ConvertIntbyJson(jData["TodayAD"]);


        Day10 = CWLib.ConvertIntbyJson(jData["Day10"]);// 7일 출석 받은 유저 수
        Day2 = CWLib.ConvertIntbyJson(jData["Day2"]); // 그다음날 들어 왔는가?
        Day3 = CWLib.ConvertIntbyJson(jData["Day3"]);


        Tutoskipcount = CWLib.ConvertIntbyJson(jData["Tutoskipcount"]);//        튜토리얼 스킵 숫자
        Storycount5 = CWLib.ConvertIntbyJson(jData["Storycount5"]);   // 행성 5 갠수
        Storycount6 = CWLib.ConvertIntbyJson(jData["Storycount6"]);   // 행성 6 갠수

        PVPCount = CWLib.ConvertIntbyJson(jData["PVPCount"]);   //1:1행성 접속 햇수
        MyroomCount = CWLib.ConvertIntbyJson(jData["MyroomCount"]);   //내행성 접속 햇수
        MultiCount = CWLib.ConvertIntbyJson(jData["MultiCount"]);//멀티행성 접속 햇수

        





    }
IEnumerator IRun()
    {


        yield return null;

        while (true)
        {
            CWSocketManager.Instance.SendManagerTool(m_szDay, ReceiveData, "ReceiveData");
            yield return new WaitForSeconds(10f);
        }
    }

    public void OnResetFunction()
    {
        FuncReset.Instance.Open();
    }
    // 보석 메일
    public void OnSendGem()
    {
        FindUserDlg.Instance.Open();
    }
    // 함수리셋
    public void OnFunctionReset()
    {

    }

    // 차단유저
    public void OnBlockUser()
    {
        ListDBDlg.Instance.ShowBlockUser();
    }

    // 결제 한 유저 
    public void OnCashUser()
    {
        ListDBDlg.Instance.ShowCashUser();
    }
    //메일 주기

    public void OnMailSend(int nID, string szMessage,int gemcount)
    {
    //    CWSocketManager.Instance.SendMail(nID, szMessage, gemcount);
    }

    // 함수 리셋




}
