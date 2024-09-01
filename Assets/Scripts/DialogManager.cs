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
        preGame,
        start,
        recieveCard,
        call,
        raise,
        fold,
        win,
        time,
        detected,
        missDetected,
        busted,
        suspicion,
        dayWin,
        dayLose
    };

    [TextArea]
    public string[] TextData;
    public string[] EndTextData;
    public int TextIndex;

    public int TextLen;
    public string DialogString;
    public Text DialogText;
    public Text NameText;
    public float delay = 0.1f;

    public int now_Sentence = 0;
    public bool isStart = false;
    public bool isEnd = false;

    public GameObject TextPanel;
    public GameObject NamePanel;

    public bool isDialogHighPriority = false;
    public bool isDialogMiddlePriority = false;
    public bool isDialogLowPriority = false;
    public Coroutine CurrentCoroutine;

    [Header("SoundArea")]
    [SerializeField] private AudioClip DialogSound;

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
        Debug.Log("High Wait Ended");
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
                TextData = GameManager.Instance.playerArray[playerIdx].textDataDetected;
                break;
            case TextType.missDetected:
                TextData = GameManager.Instance.playerArray[playerIdx].textDataMissDetected;
                break;
            case TextType.busted:
                TextData = GameManager.Instance.playerArray[playerIdx].textDataBusted;
                break;
            case TextType.suspicion:
                TextData = GameManager.Instance.playerArray[playerIdx].textDataSuspicion;
                break;
            case TextType.dayWin:
                TextData = GameManager.Instance.playerArray[playerIdx].textDataDayWin;
                break;
            case TextType.dayLose:
                TextData = GameManager.Instance.playerArray[playerIdx].textDataDayLose;
                break;
        }

        now_Sentence = UnityEngine.Random.Range(0, TextData.Length);
        TextLen = TextData[now_Sentence].Length;
        int temp = 0;
        NameText.text = GameManager.Instance.playerArray[playerIdx].playerName;

        while (temp < TextLen)
        {
            if (TextData[now_Sentence][temp] != ' ')
            {
                AudioManager.GetOrCreate().PlayEffectSound(DialogSound);
            }
            DialogString += TextData[now_Sentence][temp];
            temp++;

            DialogText.text = DialogString;
            yield return new WaitForSeconds(delay);
        }
        if(type == TextType.detected)
            GameManager.Instance.Player[playerIdx].transform.GetChild(0).gameObject.SetActive(false);
        isDialogHighPriority = false;


    }

    public IEnumerator NextSentence_MiddlePriority(int playerIdx, TextType type)
    {
        isDialogMiddlePriority = true;
        DialogString = "";
        switch (type)
        {
            case TextType.start:
                TextData = GameManager.Instance.playerArray[playerIdx].textDataStart;
                break;
            case TextType.call:
                TextData = GameManager.Instance.playerArray[playerIdx].textDataCall;
                break;
            case TextType.raise:
                TextData = GameManager.Instance.playerArray[playerIdx].textDataRaise;
                break;
            case TextType.fold:
                TextData = GameManager.Instance.playerArray[playerIdx].textDataFold;
                break;
            case TextType.win:
                TextData = GameManager.Instance.playerArray[playerIdx].textDataWin;
                break;
        }

        NameText.text = GameManager.Instance.playerArray[playerIdx].playerName;
        now_Sentence = UnityEngine.Random.Range(0, TextData.Length);
        TextLen = TextData[now_Sentence].Length;
        int temp = 0;

        while (temp < TextLen)
        {
            if (TextData[now_Sentence][temp] != ' ')
            {
                AudioManager.GetOrCreate().PlayEffectSound(DialogSound);
            }
            DialogString += TextData[now_Sentence][temp];
            temp++;

            DialogText.text = DialogString;
            yield return new WaitForSeconds(delay);
        }
        isDialogMiddlePriority = false;
        if (type == TextType.start)
        {
            for (int i = 0; i < GameManager.Instance.playerArray.Length; i++)
            {
                GameManager.Instance.betMan.entranceBet(i);
                yield return new WaitForSeconds(0.5f);
            }
            GameManager.Instance.SetStateDeal();
        }
    }

    public IEnumerator NextSentence_LowPriority(int playerIdx, TextType type)
    {
        isDialogLowPriority = true;
        DialogString = "";
        yield return new WaitUntil(() => GameManager.Instance.playerArray.Length == 4);
        switch (type)
        {
            case TextType.recieveCard:
                TextData = GameManager.Instance.playerArray[playerIdx].textDataRecieveCard;
                break;
            case TextType.time:
                TextData = GameManager.Instance.playerArray[playerIdx].textDataTime;
                break;
        }

        now_Sentence = UnityEngine.Random.Range(0, TextData.Length);
        TextLen = TextData[now_Sentence].Length;
        int temp = 0;
        NameText.text = GameManager.Instance.playerArray[playerIdx].playerName;

        while (temp < TextLen)
        {
            if (TextData[now_Sentence][temp] != ' ')
            {
                AudioManager.GetOrCreate().PlayEffectSound(DialogSound);
            }
            DialogString += TextData[now_Sentence][temp];
            temp++;

            DialogText.text = DialogString;
            yield return new WaitForSeconds(delay);
        }
        isDialogLowPriority = false;
    }
    #endregion
}
