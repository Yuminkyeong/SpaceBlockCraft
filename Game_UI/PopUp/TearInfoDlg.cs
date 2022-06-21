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

public class TearInfoDlg : WindowUI<TearInfoDlg>
{

    public GameObject[] m_gPos;
    public GameObject m_gMyGrade;

    public override void Open()
    {
        int grade = CWHeroManager.Instance.m_nGrade;

        m_gMyGrade.transform.localPosition = m_gPos[grade].transform.localPosition;

        base.Open();
    }

}
