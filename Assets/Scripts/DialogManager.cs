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
    //public event Action DialogHighPriorityCreated;
    //public event Action DialogMiddlePriorityCreated;
    //public event Action DialogLowPriorityCreated;
    public Coroutine CurrentCoroutine;

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
        //DialogHighPriorityCreated?.Invoke();                  //이벤트 발생시키고
        if (isDialogHighPriority)
        {
            StopCoroutine(CurrentCoroutine);
            isDialogHighPriority = false;
        }
        if (isDialogMiddlePriority)
        {
            StopCoroutine(CurrentCoroutine);
            isDialogMiddlePriority = false;
        }
        if (isDialogLowPriority)
        {
            StopCoroutine(CurrentCoroutine);
            isDialogLowPriority = false;
        }
        CurrentCoroutine = StartCoroutine(NextSentence_HighPriority(playerIdx, type)); //코루틴 실행
    }

    public void TriggerNextSentence_MiddlePriority(int playerIdx, TextType type)
    {
        //DialogMiddlePriorityCreated?.Invoke();
        if (isDialogHighPriority)
        {
            return;
        }
        if (isDialogMiddlePriority)
        {
            StopCoroutine(CurrentCoroutine);
            isDialogMiddlePriority = false;
        }
        if (isDialogLowPriority)
        {
            StopCoroutine(CurrentCoroutine);
            isDialogLowPriority = false;
        }
        CurrentCoroutine = StartCoroutine(NextSentence_MiddlePriority(playerIdx, type));
    }

    public void TriggerNextSentence_LowPriority(int playerIdx, TextType type)
    {
        //DialogLowPriorityCreated?.Invoke();
        if (isDialogHighPriority)
        {
            return;
        }
        if (isDialogMiddlePriority)
        {
            return;
        }
        if (isDialogLowPriority)
        {
            StopCoroutine(CurrentCoroutine);
            isDialogLowPriority = false;
        }
        CurrentCoroutine = StartCoroutine(NextSentence_LowPriority(playerIdx, type));
    }
    #endregion

    #region Wait For Coroutine To End
    public IEnumerator WaitForHighDialog()
    {
        yield return new WaitUntil(() => isDialogHighPriority == false); //High Priority의 코루틴이 실행중일 때, 종료될 때까지 기다리는 코루틴
        yield return new WaitForSeconds(1f);
    }

    public IEnumerator WaitForMiddleDialog()
    {
        yield return new WaitUntil(() => isDialogMiddlePriority == false);
    }
    #endregion

    #region Coroutine
    public IEnumerator NextSentence_HighPriority(int playerIdx, TextType type)
    {
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
