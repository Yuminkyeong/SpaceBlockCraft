using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pd_ChangeMap : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Space_Map.Instance.MoveStage(0);
    }

}
