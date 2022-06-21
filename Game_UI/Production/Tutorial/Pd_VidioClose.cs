using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pd_VidioClose : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        StartCoroutine("IRun");
        CWBgmManager.Instance.PlayLobby();
    }


    IEnumerator IRun()
    {
        AudioSource m_gSource = GetComponentInParent<AudioSource>();
        if (m_gSource != null)
        {

            float t = m_gSource.volume;
            while (t > 0.0f)
            {
                t -= Time.deltaTime * 0.5f;
                m_gSource.volume = t;
                yield return new WaitForSeconds(0.01f);
            }
        }

        yield return null;
        CWProductionPage pt = GetComponentInParent<CWProductionPage>();
        pt.OnClose();

        CWGlobal.g_bBgmOn = true;

    }

}
