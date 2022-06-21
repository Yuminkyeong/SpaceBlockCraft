using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ctrltest : MonoBehaviour {

    CWTouchEvent m_Event ;

    public UILabel m_kLabel;

    public void DgTouchUpFuc(Vector3 vPos)
    {

    }
    public void DgTouchLongDowning(Vector3 vPos)
    {

    }
    public void DgTouchDraging(Vector3 vDrag)
    {
        m_kLabel.text = "draging";
        print(m_kLabel.text);

    }
    public void DgTouchScale(bool bflag)
    {
        float fdelta = 1;
        if(bflag) fdelta = 1;
        else fdelta = -1;
        Camera.main.transform.position += Camera.main.transform.forward * 10f* fdelta;

        m_kLabel.text = "scale!!";
        print(m_kLabel.text);
    }
    void Start () {


        m_Event = GetComponent<CWTouchEvent>();
        m_Event.TouchUpFuc = DgTouchUpFuc;
        m_Event.TouchLongDowning = DgTouchLongDowning;
        m_Event.TouchDraging = DgTouchDraging;
        m_Event.TouchScaleFuc = DgTouchScale;



    }
	
	// Update is called once per frame
	void Update () {




    }
}
