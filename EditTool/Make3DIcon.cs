using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
using System.IO;

public class Make3DIcon : MonoBehaviour {


    public Camera m_Camera;
    public GameObject m_gBox;
    public GameObject m_gDir;
    public Color m_kAlpha;
    public GameObject m_Pan;
    public int m_Size = 128;

    public Transform m_tIconDir;// 이 디렉토리 밑에는 모두 3D 아이콘으로 바꾼다 
  
	
    private void OnGUI()
    {
        if(GUI.Button(new Rect(0,0,200,200),"Make"))
        {
            StartCoroutine("Run");
        }
        if (GUI.Button(new Rect(0, 200, 200, 200), "test"))
        {
            m_Camera.depth = 2;
            m_Camera.transform.position = Camera.main.transform.position;
            m_Camera.transform.rotation = Camera.main.transform.rotation;

            string szPath = string.Format("{0}/Resources/ItemIcon/test.png", Application.dataPath);
            CWLib.MakeImage( m_Camera, szPath, m_kAlpha);
        }

    }
    class IData
    {
        public int nID;
        public string strtype;
        public string strname;
    }
    IEnumerator Run()
    {

    
        Dictionary<int, IData> kIconData = new Dictionary<int, IData>();
        int tcnt = CWTableManager.Instance.GetTableCount("BAC_아이템 - 아이템");
        for (int i = 1; i < tcnt+1; i++)
        {
            
            IData kData = new IData();
            kData.nID = CWTableManager.Instance.GetTableInt("BAC_아이템 - 아이템", "ID", i);
            kData.strtype = CWTableManager.Instance.GetTable("BAC_아이템 - 아이템", "type", i);
            kData.strname = CWTableManager.Instance.GetTable("BAC_아이템 - 아이템", "iconame", i);

            if (kIconData.ContainsKey(kData.nID)) continue;
            kIconData.Add(kData.nID, kData);
        }


        yield return null;


        m_gDir.SetActive(false);
        // m_Pan.transform.childCount
        for(int i=0;i< m_Pan.transform.childCount; i++)
        {
            m_Pan.transform.GetChild(i).gameObject.SetActive(false);
        }

        m_Camera.depth = 2;
        m_Camera.transform.position = Camera.main.transform.position;
        m_Camera.transform.rotation = Camera.main.transform.rotation;

        foreach(var v in kIconData)
        {
            GameObject gTarget = null;
            string strtype = v.Value.strtype;
            string strname = v.Value.strname;
            if (v.Key == 0) continue;


            print(string.Format("begin=> {0}", strname));

            if (strtype == "shipblock" || strtype == "resblock"  )
            {
                string szBlock = strname;
                if (szBlock != null && szBlock.Length > 1)
                {
                    // 블록
                    //m_gBox
                    m_gBox.SetActive(true);
                    gTarget = m_gBox;
                    Renderer rr = gTarget.GetComponent<Renderer>();
                    rr.material.mainTexture = CWResourceManager.Instance.GetTile(szBlock);
                }
            }
            if(strtype=="color")
            {

            }
            if (strtype == "weapon")
            {
                m_gBox.SetActive(false);
                gTarget = CWResourceManager.Instance.GetItemObject(v.Key);


            }
           
            if (gTarget == null)
            {
                print("error " + strname);
                continue;
            }


            gTarget.transform.parent = m_Pan.transform;
            gTarget.transform.localPosition = Vector3.zero;
            gTarget.transform.localScale = Vector3.one;
            string szPath = string.Format("{0}/Resources/ItemIcon/{1}.png", Application.dataPath, strname);
            yield return new WaitForEndOfFrame();

            print(string.Format("make=> {0}",strname));


            CWLib.MakeImage( m_Camera, szPath, m_kAlpha);

            gTarget.SetActive(false);
            yield return null;





        }

        // 특정 디렉토리에 있으면 모두 아이콘으로 바꾸는 작업 
        //
        m_gBox.SetActive(false);
        List<Transform> kTemp = new List<Transform>();

        for (int i = 0; i < m_tIconDir.childCount; i++)
        {
            kTemp.Add(m_tIconDir.GetChild(i));
        }


        foreach(var v in kTemp)
        {
            Transform gTarget = v;
            gTarget.parent = m_Pan.transform;
            gTarget.localPosition = Vector3.zero;
            gTarget.localScale = Vector3.one;
            gTarget.localEulerAngles = new Vector3(0, 0, 0);
            yield return new WaitForEndOfFrame();
            string szPath = string.Format("{0}/Resources/Texture/{1}.png", Application.dataPath, gTarget.name);
            CWLib.MakeImage( m_Camera, szPath, m_kAlpha);
            gTarget.gameObject.SetActive(false);
            

        }

        print("End Task!!");
        yield return null;

    }

    

}
