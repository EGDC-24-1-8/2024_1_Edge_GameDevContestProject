using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    private static DialogManager instance;
    public static DialogManager Instance { get { return instance; } }

    public enum TextType
    {
        start,
        recieveCard,
        call,
        raise,
        fold,
        win,
        detected,
        suspicion
    };

    [TextArea]
    public string[] TextData;
    public string[] EndTextData;
    public int TextIndex;

    public int TextLen;
    public string DialogString;
    public Text DialogText;
    public float delay = 0.1f;

    public int now_Sentence = 0;
    public bool isStart = false;
    public bool isEnd = false;

    public GameObject TextPanel;
    public Player[] playerArray = null;

    public bool isDialogHighPriority = false;
    public bool isDialogMiddlePriority = false;
    public bool isDialogLowPriority = false;
    public event Action DialogHighPriorityCreated;
    public event Action DialogMiddlePriorityCreated;
    public event Action DialogLowPriorityCreated;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    void Start()
    {
        playerArray = GameManager.Instance.playerArray;
        TextIndex = TextData.Length;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isDialogMiddlePriority)
                return;
            if (now_Sentence < TextIndex)
            {
                //NextSentence();
            }
        }
    }

    //TODO: 여러 오류 수정
    //하고 싶은 것: 대사별로 우선순위를 매기고 싶음.
    //카드 받으면서 치는 대사: Low Priority, 다른 대사들이 끼어들면 끊김, 게임 플레이에 영향 안 줌
    //시작, 베팅 등의 대사: Middle Priority, 여기부터는  게임 플레이도 멈추면서 대사를 침
    //고발, 의심도Max시 대사: High Priority, 게임 플레이 도중 비동기적으로 발생하는 이벤트에 대한 대사

    #region Trigger Coroutine
    public void TriggerNextSentence_HighPriority(int playerIdx, TextType type)
    {
        DialogHighPriorityCreated?.Invoke();                  //이벤트 발생시키고
        StartCoroutine(NextSentence_HighPriority(playerIdx, type)); //코루틴 실행
    }

    public void TriggerNextSentence_MiddlePriority(int playerIdx, TextType type)
    {
        DialogMiddlePriorityCreated?.Invoke();
        StartCoroutine(NextSentence_MiddlePriority(playerIdx, type));
    }

    public void TriggerNextSentence_LowPriority(int playerIdx, TextType type)
    {
        DialogLowPriorityCreated?.Invoke();
        StartCoroutine(NextSentence_LowPriority(playerIdx, type));
    }
    #endregion

    #region Wait For Coroutine To End
    private IEnumerator WaitForHighDialog()
    {
        yield return new WaitUntil(() => isDialogHighPriority == false); //High Priority의 코루틴이 실행중일 때, 종료될 때까지 기다리는 코루틴
    }

    private IEnumerator WaitForMiddleDialog()
    {
        yield return new WaitUntil(() => isDialogMiddlePriority == false);
    }
    #endregion

    #region Coroutine
    public IEnumerator NextSentence_HighPriority(int playerIdx, TextType type)
    {
        DialogHighPriorityCreated += () =>                        //다른 High Priority 대사가 실행되면
        {
            isDialogHighPriority = false;                          //지금 실행중인 코루틴을 중단하고
            StopAllCoroutines();                                  //외부에서 실행된 High Priority 코루틴 실행
        };
        isDialogHighPriority = true;
        DialogString = "";
        switch (type)
        {
            case TextType.detected:
                TextData = playerArray[playerIdx].textDataDetected;
                break;
            case TextType.suspicion:
                TextData = playerArray[playerIdx].textDataSuspicion;
                break;
        }

        now_Sentence = UnityEngine.Random.Range(0, TextData.Length);
        TextLen = TextData[now_Sentence].Length;
        int temp = 0;

        while (temp < TextLen)
        {
            DialogString += TextData[now_Sentence][temp];
            temp++;

            DialogText.text = DialogString;
            yield return new WaitForSeconds(delay);
        }
        Debug.Log("High Dialog " + playerIdx);
        isDialogHighPriority = false;
    }

    public IEnumerator NextSentence_MiddlePriority(int playerIdx, TextType type)
    {
        DialogHighPriorityCreated += () =>                       //다른 High Priority 대사가 실행되면
        {
            isDialogMiddlePriority = false;                      //지금 실행중인 코루틴을 중단하고
            StopAllCoroutines();                                 //외부에서 실행된 High Priority 코루틴 실행
        };
        if (isDialogHighPriority)                                //다른 High Priority 대사가 실행 중이라면
        {
            yield return StartCoroutine(WaitForHighDialog());    //그 코루틴이 끝날 때까지 대기하고
            //StopAllCoroutines();                                 //이 코루틴을 파괴(안 하면 끊긴 대사가 중간부터 출력됨, 다음 대사로 자연스럽게 넘어가고자 함)
        }                                                        //근데 생각해보니까 파괴할 필요는 없을 것도 같은데?
        isDialogMiddlePriority = true;
        DialogString = "";
        switch(type)
        {
            case TextType.start:
                TextData = playerArray[playerIdx].textDataStart;
                break;
            case TextType.call:
                TextData = playerArray[playerIdx].textDataCall;
                break;
            case TextType.raise:
                TextData = playerArray[playerIdx].textDataRaise;
                break;
            case TextType.fold:
                TextData = playerArray[playerIdx].textDataFold;
                break;
            case TextType.win:
                TextData = playerArray[playerIdx].textDataWin;
                break;
        }

        now_Sentence = UnityEngine.Random.Range(0, TextData.Length);
        TextLen = TextData[now_Sentence].Length;
        int temp = 0;

        while (temp < TextLen)
        {
            /*if (isDialogHighPriority)          */                     //대사 출력 중에 High Priority 대사 코루틴이 실행되면
            /*{                                  */                     //이 대사 끊고 바로 출력할 수 있게끔
            /*    isDialogMiddlePriority = false;*/                     //코루틴 파괴
                //yield break;
            //}
            DialogString += TextData[now_Sentence][temp];
            temp++;

            DialogText.text = DialogString;
            yield return new WaitForSeconds(delay);
        }
        Debug.Log("Middle Dialog " + playerIdx);
        isDialogMiddlePriority = false;
    }

    public IEnumerator NextSentence_LowPriority(int playerIdx, TextType type)
    {
        DialogHighPriorityCreated += () =>
        {
            isDialogLowPriority = false;
            StopAllCoroutines();
        };
        DialogMiddlePriorityCreated += () =>
        {
            isDialogLowPriority = false;
            StopAllCoroutines();
        };
        DialogLowPriorityCreated += () =>
        {
            isDialogLowPriority = false;
            StopAllCoroutines();
        };
        if (isDialogHighPriority)
        {
            yield return StartCoroutine(WaitForHighDialog());
            //StopAllCoroutines();
        }
        if (isDialogMiddlePriority)
        {
            yield return StartCoroutine(WaitForMiddleDialog());
            //StopAllCoroutines();
        }
    isDialogLowPriority = true;
        DialogString = "";
        switch (type)
        {
            case TextType.recieveCard:
                TextData = playerArray[playerIdx].textDataRecieveCard;
                break;
        }

        now_Sentence = UnityEngine.Random.Range(0, TextData.Length);
        TextLen = TextData[now_Sentence].Length;
        int temp = 0;
        while (temp < TextLen)
        {
            //if (isDialogHighPriority ||
                //isDialogMiddlePriority)
            //{
                //isDialogLowPriority = false;
                //yield break;
            //}
            DialogString += TextData[now_Sentence][temp];
            temp++;

            DialogText.text = DialogString;
            yield return new WaitForSeconds(delay);
        }
        Debug.Log("Low Dialog " + playerIdx);
        isDialogLowPriority = false;
    }
    #endregion
}
