using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CWUnityLib;
public class test2 : MonoBehaviour {

    public Text kTest;
    void Start () {

        
        string str = "안녕하세요 @User님! 스페이스 블록 크래프트는 재밌게 하고 계신가요? 저희는 무엇보다도 @User님이 게임에 많은 관심 가지고 즐겨주신다면 가장 행복할 거예요  (,,•́ . •̀,,）별것 아니지만 선물 받으시구 좋은하루 되세요:) 아 그리구 혹시라도 수정했으면 하는 부분이나 궁금한 점이 있으시면 언제든지 편하게 개발자 이메일로 연락주세요 ✦‿✦ ( cwgames127@gmail.com )";
        kTest.text =string.Format("{0}", CWLib.ChangeString(str, "@User", "홍길동"));

	}


}
