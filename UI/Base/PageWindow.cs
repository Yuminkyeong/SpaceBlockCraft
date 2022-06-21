using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWStruct;
using CWUnityLib;
/*
 * 무조건 하나만 열 수 있다.
 * 전에 연 페이지는 자동으로 닫힌다 
 * 
 * 
 * */


public class PageWindow<T> : BaseWindow<T>
{


    public override void Open(Returnfuction fuc = null)
    {
        g_kPageWindow = this;
        base.Open(fuc);
    }

}
