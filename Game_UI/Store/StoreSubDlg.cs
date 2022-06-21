using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Newtonsoft.Json;
using System.IO;
using SimpleJSON;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;
using CWStruct;
using CWEnum;
using CWUnityLib;
using System;

public class StoreSubDlg : WindowUI<StoreSubDlg>
{
    public enum STORETYPE
    {
        DISCOUNT,PAKAGE,COIN,CRYSTAL,BLOCK,PLANE,WEAPON,ITEM,SHAPE,CHARACTER,COLOR
    }
    public STORETYPE StoreType;



    #region Override 함수
    public override void OnButtonClick(int num)
    {
        base.OnButtonClick(num);
    }
    public override void OnEscKey()
    {

        base.OnEscKey();
    }
    public override void OnBuy(int num)
    {
        base.OnBuy(num);
    }
    public override void OnSelect(int num)
    {
        base.OnSelect(num);
    }
    protected override void OnOpen()
    {
        base.OnOpen();
    }
    #endregion



}
