using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class StartGame : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("IntroSequence");
        
    }

    IEnumerator IntroSequence()
    {
     //   SceneManager.LoadSceneAsync
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene("GLogin");
    }

}
