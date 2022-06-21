using UnityEngine;
using System.Collections;

public class NormalPopup<T> : BaseWindow<T>
{

    public override void Open(Returnfuction fuc = null)
    {
        base.Open(fuc);

        NWinType = WINDOWTYPE.POPUP;
    }
}
