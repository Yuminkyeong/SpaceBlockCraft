using UnityEngine;
using System.Collections;

public class DynamicScrollItem : MonoBehaviour
{

    
    public delegate void CBFuction();
    public CBFuction CBDisplayFuc;
    public CWWindow.Returnfuction CBClick;
    protected CWWindow m_gParentWindow;

    protected DynamicScroll _Parent;

    public int m_Num=-1; //0 부터 시작하는 정수 

	void Awake () 
    {
        Create();

    }
    public void Create()
    {
        if (_Parent != null) return;
        if (transform.parent == null) return;
        if (transform.parent.parent == null) return;
        _Parent = transform.parent.parent.GetComponent<DynamicScroll>();
        if (_Parent == null) return;

        _Parent.AddList(this);

    }
    public virtual int GetGroup()
    {
        return 0;
    }
    
    public int GetNum()
    {
        return m_Num;
    }

    public void SetBinding(int _Num)
    {
        transform.SetSiblingIndex(_Num);
    }

    public void SetItem(int _Num,CWWindow gParent)
    {
        this.m_Num = _Num;
        m_gParentWindow = gParent;

        if (_Num < _Parent.GetCnt())
        {
            gameObject.SetActive(true);
            if (CBDisplayFuc != null) CBDisplayFuc();
            SetScreen();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public virtual void SetScreen()
    {
        
    }

    public virtual void OnClick()
    {
        if (CBClick != null)
        {
            UILabel[] array = gameObject.GetComponentsInChildren<UILabel>();
            foreach (var v in array)
            {
                if (v.name.ToUpper() == "NAME")
                {
                    m_gParentWindow.name = v.text;
                }
            }
            CBClick(m_Num);
            m_gParentWindow.Close();
        }
        if(m_gParentWindow)
        {
            m_gParentWindow.SelectItem(m_Num,gameObject);
        }

    }
    public void ParentClose()
    {
        m_gParentWindow.Close();
    }
    
}
