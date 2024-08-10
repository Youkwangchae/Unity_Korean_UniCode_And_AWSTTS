using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class Text_Info
{
    char Text;
    bool isConsonant;
    bool isDouble;
    bool isChosoug;
    bool isEnd;
    bool isSolo;
    bool isSpace;

    public Text_Info(char _Text, bool _isConsonant, bool _isDouble, bool _isChosung) 
    { 
        Text = _Text;
        isConsonant = _isConsonant;
        isDouble = _isDouble;
        isChosoug = _isChosung;
        isEnd = false;
        isSolo = false;
        isSpace = false;
    }

    public bool _isSpace()
    {
        return isSpace;
    }

    public void setSpace(bool _isSpace)
    {
        isSpace = _isSpace;
    }

    public void setSolo(bool _isSolo)
    {
        isSolo = _isSolo;
    }

    public bool _isSolo()
    {
        return isSolo;
    }

    public bool _isChosung()
    {
        return isChosoug;
    }

    public void setEnd(bool _isEnd)
    {
        isEnd = _isEnd;
    }

    public bool getEnd()
    {
        return isEnd;
    }

    public char getJaso()
    {
        return Text;
    }

    public bool _isConsonant()
    {
        return this.isConsonant;
    }

    public bool _isDouble()
    {
        return isDouble;
    }
}

public class Unicode_Hangul : MonoBehaviour
{
    public Transform target;
    public bool _isConsonant;            // 자음인지 체크
    public bool _isDoublePossible;       // 쌍자음 가능한지 체크
    public bool _isSpace;
    public string jaso;

    public string[] checkList;           // 종성으로 앞에 올 경우 겹자음이 가능한 자음들
    char[] _checkList;

    public string[] backList;            // 종성으로 뒤에 올 경우 겹자음이 가능한 자음들
    public char[] _backList;
    TextMeshProUGUI text;

    // First '가' : 0xAC00(44032), 끝 '힣' : 0xD79F(55199)

    private const int FIRST_HANGUL = 44032;

    // 14 Consonants
    private char[] CON_LIST = {'ㄱ', 'ㄴ', 'ㄷ', 'ㄹ', 'ㅁ', 'ㅂ', 'ㅅ', 'ㅇ', 'ㅈ','ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ'};

    // 11 Vowels
    private char[] VOW_LIST = {'ㅏ', 'ㅑ', 'ㅓ', 'ㅕ','ㅗ', 'ㅛ', 'ㅜ', 'ㅠ', 'ㅡ', 'ㅢ','ㅣ'};

    // 19 initial consonants
    private char[] CHOSUNG_LIST = {'ㄱ', 'ㄲ', 'ㄴ', 'ㄷ', 'ㄸ', 'ㄹ', 'ㅁ', 'ㅂ', 'ㅃ', 'ㅅ','ㅆ', 'ㅇ', 'ㅈ', 'ㅉ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ'};

    private int JUNGSUNG_COUNT = 21;

    // 21 vowels
    private char[] JUNGSUNG_LIST = {'ㅏ', 'ㅐ', 'ㅑ', 'ㅒ', 'ㅓ', 'ㅔ', 'ㅕ', 'ㅖ', 'ㅗ', 'ㅘ','ㅙ', 'ㅚ', 'ㅛ', 'ㅜ', 'ㅝ', 'ㅞ', 'ㅟ', 'ㅠ', 'ㅡ', 'ㅢ','ㅣ'};

    private static int JONGSUNG_COUNT = 28;

    // 28 consonants placed under a vowel(plus one empty character)
    private char[] JONGSUNG_LIST = {' ', 'ㄱ', 'ㄲ', 'ㄳ', 'ㄴ', 'ㄵ', 'ㄶ', 'ㄷ', 'ㄹ', 'ㄺ','ㄻ', 'ㄼ', 'ㄽ', 'ㄾ', 'ㄿ', 'ㅀ', 'ㅁ', 'ㅂ', 'ㅄ', 'ㅅ','ㅆ', 'ㅇ', 'ㅈ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ'};

    TTS_Controller actController;

    List<Text_Info> jasoList;

    public void Unicode()        // 유니코드로 텍스트 입력
    {
        bool _isChosung = false;                    // 초성인지. true면 초성, 아니면 종성
        bool _isEnd = false;                        // 지금 글자가 끝나는 부분인지
        bool _isDouble = false;        // __isDouble은 종성의 쌍자음인지 알려줌. 쌍자음인지 겹자음인지 구분 필요. 갂 + ㅏ => 가까 / 갅 + ㅏ => 간자
        bool _isSolo = false;                       // 지금 자음이 겹자음인지 아닌지
        char _jaso = char.Parse(jaso);

        if (_isSpace)
        {
            if (jasoList.Count > 0)
                jasoList[jasoList.Count - 1].setEnd(true);

            jasoList.Add(new Text_Info(_jaso, _isConsonant, _isDouble, _isChosung));
            jasoList[jasoList.Count - 1].setEnd(true);
            jasoList[jasoList.Count - 1].setSpace(true);
            setText();
            return;
        }

        if (_isConsonant) //자음일 경우, 바로 앞이 자음인지 모음인지 체크.
        {
            if (jasoList.Count > 0)
            {
                if(jasoList[jasoList.Count - 1]._isSpace())
                {
                    _isChosung = true;
                    // _isEnd = true;

                    jasoList.Add(new Text_Info(_jaso, _isConsonant, _isDouble, _isChosung));

                    if (_isConsonant && _isEnd)
                        jasoList[jasoList.Count - 1].setEnd(true);

                    setText();
                    return;
                }

                if (jasoList[jasoList.Count - 1]._isConsonant())     // 바로 앞이 자음
                {
                    char _check = jasoList[jasoList.Count - 1].getJaso();

                    var _temp = Array.Exists(_checkList, x => x == _check); // 앞에 있는 자음하고 겹자음이 가능한지 체크.
                    if (_temp && jasoList[jasoList.Count - 1].getEnd())     // 앞에 있는 자음이 종성이었다면 이번 자음과 합쳐져서 종성으로 들어가야 함. 각 + ㄱ => 갂
                    {
                        if (jasoList[jasoList.Count - 1].getJaso() == _jaso) // 앞에 있는 자음과 똑같음 => 쌍자음
                        {
                            _isDouble = true;

                            jasoList.RemoveAt(jasoList.Count - 1);      // 바로 앞 자음은 지워야 함.

                            _isChosung = false;
                            int _Idx = Array.IndexOf(JONGSUNG_LIST, _check);    // 바로 앞 자음의 위치.

                            int _idx = Array.IndexOf(CON_LIST, _check);

                            int _index = Array.IndexOf(actController.Consonants[_idx]._backList, _jaso);

                            _jaso = JONGSUNG_LIST[_Idx + _index + 1];
                        }
                        else                                                 // 앞에 있는 자음과 다름 => 겹자음
                            _isDouble = false;

                        jasoList[jasoList.Count - 1].setEnd(false);

                        _isEnd = true;
                    }
                    else                                      // 앞에 있는 자음과 겹자음 불가.
                    {
                        _isChosung = true;                              //그럼 이번 자음은 초성으로 들어가야 함.

                        if (jasoList[jasoList.Count - 1].getJaso() == _jaso && _isDoublePossible) // 바로 앞 자음이 나와 똑같음. && 쌍자음이 가능
                        {
                            jasoList.RemoveAt(jasoList.Count - 1);      // 바로 앞 자음은 지워야 함.

                            int _index = Array.IndexOf(CHOSUNG_LIST, _jaso);
                            _jaso = CHOSUNG_LIST[_index + 1];           // 쌍자음으로 바꿈.
                        }
                        else
                        {
                            jasoList[jasoList.Count - 1].setEnd(true);  // ㄱㄱㅏ => ㄱ가
                        }
                    }
                }
                else                                                // 바로 앞이 모음
                {
                    if(jasoList.Count > 1)
                    {
                        if(!jasoList[jasoList.Count - 2]._isConsonant()) // 그 앞도 모음일 경우
                        {
                            _isChosung = true;// 이번 자음은 초성으로 들어가야 함.
                            
                        }
                        else                                                // 그 앞이 자음일 경우
                        {
                            _isChosung = false;                             // 그럼 이번 자음은 종성으로 들어가야 함.
                            jasoList[jasoList.Count - 1].setEnd(false);     // 앞 모음의 isEnd는 해제해야 함.
                            _isEnd = true;
                            _isSolo = true;
                        }
                    }
                    else
                    {
                        _isChosung = true;
                    }                    
                }
            }
            else                                                    // 맨 처음으로 입력
            {
                _isChosung = true;
            }
        }
        else // 모음일 경우, 무조건 중성.
        {
            _isChosung = false;

            if (jasoList.Count > 0)
            {
                if (jasoList[jasoList.Count - 1]._isConsonant())            // 바로 앞이 자음일 때
                {
                    if (jasoList[jasoList.Count - 1]._isDouble())           // 바로 앞이 쌍자음
                    {
                        jasoList[jasoList.Count - 1].setEnd(false);
                        jasoList[jasoList.Count - 2].setEnd(true);
                    }
                    else                                                    // 바로 앞이 쌍자음이 아닐 경우
                    {
                        if (!jasoList[jasoList.Count - 1]._isChosung())     // 그 자음이 종성일 경우
                        {
                            if (jasoList[jasoList.Count - 1]._isSolo())      // 그 자음이 혼자일 경우
                            {
                                jasoList[jasoList.Count - 1].setEnd(false);
                                jasoList[jasoList.Count - 2].setEnd(true);
                            }
                            else                                            // 그 자음이 겹자음일 경우
                            {
                                jasoList[jasoList.Count - 1].setEnd(false);
                                jasoList[jasoList.Count - 2].setEnd(true);
                            }
                        }
                    }
                }
                else // 앞에 모음이 온 경우 겹모음 처리 필요 ㅏ + ㅣ => ㅐ
                {
                    char _check = jasoList[jasoList.Count - 1].getJaso();
                   
                    if (_check == 'ㅣ' && _jaso == 'ㅓ')
                    {
                        jasoList.RemoveAt(jasoList.Count - 1);
                        _jaso = 'ㅐ';
                    }
                    else if (_check == 'ㅘ' && _jaso == 'ㅣ')
                    {
                        jasoList.RemoveAt(jasoList.Count - 1);
                        _jaso = 'ㅙ';
                    }
                    else if(_check == 'ㅝ' && _jaso == 'ㅣ')
                    {
                        jasoList.RemoveAt(jasoList.Count - 1);
                        _jaso = 'ㅞ';
                    }
                    else
                    {
                        int _Idx = Array.IndexOf(JUNGSUNG_LIST, _check);    // 바로 앞 모음의 위치.

                        var _temp = Array.Exists(VOW_LIST, x => x == _check);

                        if (_temp)
                        {
                            int _idx = Array.IndexOf(VOW_LIST, _check);

                            if (actController.Vowels[_idx]._backList.Length > 0)
                            {
                                int _index = Array.IndexOf(actController.Vowels[_idx]._backList, _jaso);

                                if (_index > -1)
                                {
                                    _jaso = JUNGSUNG_LIST[_Idx + _index + 1];

                                    jasoList.RemoveAt(jasoList.Count - 1);
                                }
                            }
                        }
                    }                                   
                }
            }
            
        }

        jasoList.Add(new Text_Info(_jaso, _isConsonant, _isDouble, _isChosung));

        if(!_isConsonant)
            jasoList[jasoList.Count - 1].setEnd(true);

        if (_isConsonant && _isEnd)
            jasoList[jasoList.Count - 1].setEnd(true);

        if(_isConsonant && _isSolo)
            jasoList[jasoList.Count - 1].setSolo(true);

        for (int i = 0; i < jasoList.Count; i++)
            Debug.Log((i + 1) + " : " + jasoList[i].getJaso() + ", End : " + jasoList[i].getEnd());


        setText();
    }

    public void setText()          // 텍스트 입력 시 위에 뜨도록 처리.
    {
        string _text = "";
        int startIdx = 0;

        if(jasoList.Count > 0)
        {
            while (true)
            {
                if (startIdx < jasoList.Count)
                {
                    int size = getNextAssembleSize(startIdx);
                    if (size > 1)
                    {
                        if(size == 4)
                            _text += assemble(startIdx, true);
                        else
                            _text += assemble(startIdx);
                        startIdx += size;
                    }
                    else// 합칠 글자가 없을 때 (자음 혹은 모음만 온 경우)
                    {
                        _text += jasoList[startIdx].getJaso().ToString();
                        startIdx++;
                    }

                }
                else
                    break;
            }

            text.text = _text;
        }
        else
        {
            Debug.Log("입력해 주세요!");
            text.text = "";
        }

        actController.getText(text.text);
    }

    string assemble(int startIdx, bool isFour = false)
    {
        int _length = jasoList.Count - startIdx;

        int _uni = FIRST_HANGUL;

        if(_length > 0)     // 초성 검증
        {
            int chosungIndex = Array.IndexOf(CHOSUNG_LIST, jasoList[startIdx].getJaso());

            if (chosungIndex >= 0)
            {
                _uni += JONGSUNG_COUNT * JUNGSUNG_COUNT * chosungIndex;
            }
            else
            {
                Debug.Log((startIdx + 1) + "번째 글자가 초성이 아닙니다.");
            }
        }
        if (!jasoList[startIdx].getEnd())
        {
            if (_length > 1)  // 중성 검증
            {
                int jungsungIndex = Array.IndexOf(JUNGSUNG_LIST, jasoList[startIdx + 1].getJaso());

                if (jungsungIndex >= 0)
                {
                    _uni += JONGSUNG_COUNT * jungsungIndex;
                }
                else
                {
                    Debug.Log((startIdx + 2) + "번째 글자가 중성이 아닙니다.");              
                }
            }
        }

        if(!jasoList[startIdx + 1].getEnd())
        {
            if (_length > 2)  // 종성 검증
            {
                if (isFour)
                {

                    char _check = jasoList[startIdx + 2].getJaso();

                    int _Idx = Array.IndexOf(JONGSUNG_LIST, _check);    // 바로 앞 자음의 위치.

                    int _idx = Array.IndexOf(CON_LIST, _check);

                    int _index = Array.IndexOf(actController.Consonants[_idx]._backList, jasoList[startIdx + 3].getJaso());

                    int jongsungIndex = Array.IndexOf(JONGSUNG_LIST, JONGSUNG_LIST[_Idx + _index + 1]);

                    if (jongsungIndex >= 0)
                    {
                        _uni += jongsungIndex;
                    }
                    else
                    {
                        Debug.Log((startIdx + 3) + "번째 글자가 종성이 아닙니다.");
                    }
                }
                else
                {
                    if (jasoList[startIdx + 2].getEnd())
                    {
                        int jongsungIndex = Array.IndexOf(JONGSUNG_LIST, jasoList[startIdx + 2].getJaso());

                        if (jongsungIndex >= 0)
                        {
                            _uni += jongsungIndex;
                        }
                        else
                        {
                            Debug.Log((startIdx + 3) + "번째 글자가 종성이 아닙니다.");
                        }
                    }
                }
            }
        }
       
        
        Debug.Log("string : " + ((char)_uni).ToString());

        return ((char)_uni).ToString();
    }

    private int getNextAssembleSize(int startIdx)
    {
        int assembleSize = -1;

        int _temp = 0;

        for(int i=startIdx;i<jasoList.Count;i++)
        {
            _temp++;
            if (jasoList[i].getEnd())
            {
                assembleSize = _temp;
                break;
            }
        }

        // Debug.Log("★★★ assembleSize : " + assembleSize+", startIndex : "+startIdx);

        return assembleSize;
    }

    

    // Start is called before the first frame update
    void Start()
    {
        actController = FindObjectOfType<TTS_Controller>();

        text = target.GetComponent<TextMeshProUGUI>();

        jasoList = actController.jasoList;

        _checkList = new char[checkList.Length];
        for (int i=0;i<checkList.Length;i++)
        {
            _checkList[i] = char.Parse(checkList[i]);
        }

        _backList = new char[backList.Length];
        for (int i = 0; i < backList.Length; i++)
        {
            _backList[i] = char.Parse(backList[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
