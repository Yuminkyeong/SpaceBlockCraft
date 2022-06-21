using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
public class Pd_SkipMovie : MonoBehaviour
{

    public void OnStop()
    {
        VideoPlayer videoPlayer = gameObject.GetComponent<VideoPlayer>();
        videoPlayer.Stop();
        CWProductionPage pt = GetComponentInParent<CWProductionPage>();
        pt.OnClose();
        Destroy(gameObject);

    }
}
