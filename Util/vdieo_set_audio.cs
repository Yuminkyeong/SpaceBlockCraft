using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
public class vdieo_set_audio : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {

        VideoPlayer videoPlayer = gameObject.GetComponent<VideoPlayer>();
        if (videoPlayer != null)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            videoPlayer.SetTargetAudioSource(0, audioSource);
            audioSource.Play();

        }

        CWBgmManager.Instance.PlayStop();
    }
    private void OnDisable()
    {
        CWBgmManager.Instance.PlayStage();
    }

}
