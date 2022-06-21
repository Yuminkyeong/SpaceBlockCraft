using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using UnityEngine.UI;

public class DynamicScroll : MonoBehaviour
{

    public Transform m_SlotPreb;

    List<DynamicScrollItem> _ItemPool = new List<DynamicScrollItem>();
    LinkedList<DynamicScrollItem> _ItemList = new LinkedList<DynamicScrollItem>();

    protected CWWindow m_gParent;
    protected CWWindow.Returnfuction Resultfuc;

    UIPanel _UIPanel;
    UIScrollView _UIScrollView;
    protected UIGrid _UIGrid;
    SpringPanel _SpringPanel;

    bool _Loop;
    protected int _ItemCnt;

    Vector3 _LocalPosition;
    Vector2 _OffSet;
    float _SizeDelta;
    private int nCursor;

    void Awake()
    {
        _UIPanel = GetComponent<UIPanel>();
        _UIScrollView = GetComponent<UIScrollView>();
        _UIGrid = GetComponentInChildren<UIGrid>();
        _SpringPanel = GetComponent<SpringPanel>();

        _UIGrid.sorting = UIGrid.Sorting.None;
    }

    

    public void AddList(DynamicScrollItem _Item)
    {
        _ItemPool.Add(_Item);
    }

    public void SetLoop(bool _OnOff)
    {
        if (_ItemCnt <= 1)
        {
            _Loop = false;
        }
        else
        {
            _Loop = _OnOff;
        }
    }

    public virtual void SetScroll(int _Cnt)
    {
        if(_UIGrid.transform.childCount <_Cnt)
        {
            if(m_SlotPreb!=null)
            {
                int tcnt = _Cnt ;
                for (int i = 0; i < tcnt; i++)
                {
                    Transform gNew = Instantiate(m_SlotPreb);
                    gNew.name = m_SlotPreb.name + i.ToString();
                    gNew.parent = _UIGrid.transform;
                    gNew.localPosition = Vector3.zero;
                    gNew.localScale = Vector3.one;
                    DynamicScrollItem dSlot = gNew.GetComponent<DynamicScrollItem>();
                    dSlot.Create();
                }

            }
        }
       
        SetList(_Cnt);
        
    }
    

    public int GetCnt()
    {
        return _ItemCnt;
    }
    public virtual void Close()
    {
        _ItemCnt = 0;
        _ItemList.Clear();
        gameObject.SetActive(false);
    }
    public void SetList(int cnt)
    {
        _ItemCnt = cnt;
        _ItemList.Clear();
        for (int i = 0; i < _ItemPool.Count; i++)
        {
            if (_ItemList.Count < _ItemCnt)
            {
                _ItemPool[i].gameObject.SetActive(true);

                _ItemList.AddLast(_ItemPool[i]);
                _ItemList.Last.Value.SetBinding(_ItemList.Count - 1);
                _ItemPool[i].SetItem(_ItemList.Count - 1,m_gParent);
                if(Resultfuc!=null)
                    _ItemPool[i].CBClick = Resultfuc;


            }
            else
            {
                _ItemPool[i].gameObject.SetActive(false);
            }
        }

        Sorting();
    }
    Vector2 m_vBackupOffset = Vector2.zero;
    Vector3 m_LocalPosition;

    public int NCursor { get => nCursor; set => nCursor = value; }

    public void SetCursorCapture()
    {
        m_vBackupOffset = _UIPanel.clipOffset;
        m_LocalPosition = _UIPanel.transform.localPosition;
    }
    public void BackupCursorCapture()
    {
        _UIPanel.clipOffset= m_vBackupOffset;
        _UIPanel.transform.localPosition = m_LocalPosition;

    }
    public void PositionReset()
    {
        if (_UIPanel == null) return;
        _LocalPosition = _UIPanel.transform.localPosition;
        _OffSet = _UIPanel.clipOffset;

        switch (_UIScrollView.movement)
        {
            case UIScrollView.Movement.Horizontal:
                _SpringPanel.target.x = 0;

                _LocalPosition.x = 0;
                _OffSet.x = 0;
                break;
            case UIScrollView.Movement.Vertical:
                _SpringPanel.target.y = 0;

                _LocalPosition.y = 0;
                _OffSet.y = 0;
                break;
        }

        _UIPanel.transform.localPosition = _LocalPosition;
        _UIPanel.clipOffset = _OffSet;
    }
    public void SetClick(int index)
    {
        Transform tChild = _UIGrid.GetChild(index);
        if (tChild)
        {
            CWSlotItem cs = tChild.GetComponent<CWSlotItem>();
            cs.OnClick();
        }

    }
    public void SetCursor(int index)
    {
        if (index <= 0) index = 0;
        _SpringPanel.enabled = true;
        switch (_UIScrollView.movement)
        {
            case UIScrollView.Movement.Horizontal:

                _SpringPanel.target.x = -(_UIGrid.cellWidth) * index;
                break;
            case UIScrollView.Movement.Vertical:

                _SpringPanel.target.y = (_UIGrid.cellHeight) * index;
                break;
        }

        Transform tChild= _UIGrid.GetChild(index);
        if(tChild)
        {
            CWSlotItem cs = tChild.GetComponent<CWSlotItem>();
            cs.OnClick();
        }
    }
    
    void Sorting()
    {
        if (_UIGrid == null) return;
        _UIGrid.Reposition();
    }

    public DynamicScrollItem GetItem(int _Num)
    {
        
        return _ItemPool.Find(_Item => _Item.m_Num == _Num);
    }
}
