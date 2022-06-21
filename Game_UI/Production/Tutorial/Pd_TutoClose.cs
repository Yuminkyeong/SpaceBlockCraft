using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pd_TutoClose : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CWSocketManager.Instance.UpdateUser("IsTuto", "1");
        TutoMissionDlg.Instance.Close();

        Space_Map.Instance.ShowOnce();
    }

}
