using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
public class DebugTest : CWSingleton<DebugTest> {


    public static bool g_bTest1 = true;
    public static bool g_bTest2 = true;
    public static bool g_bTest3 = true;
    public static bool g_bTest4 = true;
    public static bool g_bTest5 = true;

    public bool m_bDontObject = false;
    public UILabel m_kLabel;

    public bool m_StopWorld;

	// Use this for initialization
	void Start () {

    

    }
	
	// Update is called once per frame
	void Update () {

        m_kLabel.text = string.Format("{0} {1} {2} {3}", g_bTest1, g_bTest2, g_bTest3, g_bTest4);

    }
    //mesh
    public void OnButton1()
    {
        ///GPGSManager.Instance.ShowAchievements();

        //g_bTest1 = !g_bTest1;
//        GameObject gg = CWLib.FindChild(CWMapManager.Instance.gameObject, "MapSellDir");
  //      gg.SetActive(g_bTest1);

    }
    public void OnButton2()
    {
       // GPGSManager.Instance.ShowRanking();
        //        g_bTest2 = !g_bTest2;
    }
    public void OnButton3()
    {
        g_bTest3 = !g_bTest3;
    }
    public void OnButton4()
    {
        g_bTest4 = !g_bTest4;
    }
    public void OnButton5()
    {
        g_bTest5 = !g_bTest5;
    }
    public void OnButton6()
    {
       
    }

    IEnumerator Run()
    {
        while(true)
        {

            Vector3 vDir = new Vector3();
            vDir.x = Random.Range(-1f, 1f);
            vDir.z = Random.Range(-1f, 1f);
            CWObject kObject = CWHeroManager.Instance.GetHero();
            
            yield return new WaitForSeconds(5f);
        }
    }


    
}
