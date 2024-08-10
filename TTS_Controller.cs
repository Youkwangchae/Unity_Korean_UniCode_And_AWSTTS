using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Amazon.Polly;
using Amazon.Polly.Model;
using System.IO;
using UnityEngine.Networking;
// using myPoliy;

public class TTS_Controller : MonoBehaviour
{
    AudioSource _audio;
    public Unicode_Hangul[] Consonants;

    public Unicode_Hangul[] Vowels;
    public List<Text_Info> jasoList;
    public AudioClip AudioClip { get; private set; }

    public GameObject howPopup;

    string _Text;
    bool isTTSNow = false;              // TTS 중복 방지 변수
    // myTTS tts;                       // myPolly.dll이라는 사용자 dll로 aws.core, aws.polly를 가져오려 했었음. (삭제 예정)

    public void openHow()               // 사용방법 팝업
    {
        howPopup.SetActive(true);
    }

    public void closeHow()              // 사용방법 팝업 닫기
    {
        howPopup.SetActive(false);
    }

    public void Btn_New()               // 다시하기 버튼
    {
        jasoList.Clear();
        Consonants[0].setText();
        return;
    }

    public void Erase()
    {
        int _temp = 0;

        if (jasoList.Count == 0)
            return;

        if (jasoList.Count == 1)
        {
            jasoList.RemoveAt(jasoList.Count - 1);
            Consonants[0].setText();
            return;
        }

        jasoList[jasoList.Count - 1].setEnd(true);

        for (int i = jasoList.Count - 1; i > -1; i--)
        {
            _temp++;

            if (jasoList[i].getEnd())
            {
                for (int j = 0; j < _temp; j++)
                {
                    jasoList.RemoveAt(jasoList.Count - 1);
                }

                Consonants[0].setText();
                break;
            }

            if(_temp == jasoList.Count)
            {
                for (int j = 0; j < _temp; j++)
                {
                    jasoList.RemoveAt(jasoList.Count - 1);
                }

                Consonants[0].setText();
                break;
            }

        }
    }

    public void AWS_TTS()
    {
        if (isTTSNow) return;

        if (String.IsNullOrEmpty(_Text)) return;        

        Load(_Text);
    }

    public async void Load(string Text)
    {
        isTTSNow = true;
        AmazonPollyClient pc = new AmazonPollyClient("Client Key", "API Key", Amazon.RegionEndpoint.EUWest2);

        string _text = System.Text.RegularExpressions.Regex.Replace(Text, "[ㄱ-ㅎ]+", "");
        string _Text = System.Text.RegularExpressions.Regex.Replace(_text, " ", ",");

        if(!String.IsNullOrEmpty(_Text))
        {
            if (!GameObject.Find("UI").GetComponent<Exit_Popup>().isClear) GameObject.Find("UI").GetComponent<Exit_Popup>().isClear = true;
        }        

        SynthesizeSpeechRequest sreq = new SynthesizeSpeechRequest
        {
            Text = $"<speak>{ _Text }</speak>",
            OutputFormat = OutputFormat.Mp3,
            VoiceId = VoiceId.Seoyeon,
            // VoiceId = VoiceId.Amy,
            LanguageCode = "ko-KR",
            TextType = TextType.Ssml
        };

        var sres = await pc.SynthesizeSpeechAsync(sreq);

        string filename = Application.persistentDataPath+".mp3";

        using (var fileStream = File.Create(filename))
        {
            sres.AudioStream.CopyTo(fileStream);
            fileStream.Flush();
            fileStream.Close();
        }

        StartCoroutine("playClip", filename);

        Debug.Log("요청 : " + Text);

        //try
        //{
        //    tts._load(text);
        //}
        //catch (typeloadexception e)
        //{
        //    debug.log(e);
        //}

    }

    IEnumerator playClip(string filename)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(filename, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                Debug.Log("인터넷 연결을 확인해주세요 : " + www.error);
                yield break;
            }
            else
                AudioClip = DownloadHandlerAudioClip.GetContent(www);
        }

        _audio.clip = AudioClip;
        _audio.Play();
        isTTSNow = false;
    }

    public void getText(string text)
    {
        _Text = text;
    }

    // Start is called before the first frame update
    void Awake()
    {
        jasoList = new List<Text_Info>();
        _audio = transform.GetComponent<AudioSource>();
        howPopup.SetActive(false);
        Load("");
        // tts = new myTTS();
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}
}
