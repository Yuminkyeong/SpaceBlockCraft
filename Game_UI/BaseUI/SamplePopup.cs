using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamplePopup  : MonoBehaviour
{

    public GameObject m_gvisible;
    public void Open()
    {
        m_gvisible.SetActive(true);
    }
    public void Close()
    {
        m_gvisible.SetActive(false);
    }
}
