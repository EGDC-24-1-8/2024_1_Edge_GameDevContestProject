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

    public bool isDialogEssential = false;
    public bool isDialogNonEssential = false;
    public GameObject TextPanel;
    public Player[] playerArray = null;
    private IEnumerator CurDialog;
    public event Action DialogCreated;

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
            if (isDialogEssential)
                return;
            if (now_Sentence < TextIndex)
            {
                //NextSentence();
            }
        }
    }

    public void TriggerNextSentenceEssential(int playerIdx, TextType type)
    {
        DialogCreated?.Invoke();
        StartCoroutine(NextSentenceEssential(playerIdx, type));
    }

    public void TriggerNextSentenceNonEssential(int playerIdx, TextType type)
    {
        DialogCreated?.Invoke();
        StartCoroutine(NextSentenceNonEssential(playerIdx, type));
    }

    public IEnumerator NextSentenceEssential(int playerIdx, TextType type)
    {
        isDialogEssential = true;
        DialogString = "";
        switch(type)
        {
            case TextType.start:
                TextData = playerArray[playerIdx].textDataStart;
                break;
            case TextType.recieveCard:
                TextData = playerArray[playerIdx].textDataRecieveCard;
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
        Debug.Log("Dialog " + playerIdx);
        isDialogEssential = false;
    }

    public IEnumerator NextSentenceNonEssential(int playerIdx, TextType type)
    {
        isDialogNonEssential = true;
        DialogCreated += () =>
        {
            isDialogNonEssential = false;
            StopAllCoroutines();
        };
        DialogString = "";
        switch (type)
        {
            case TextType.start:
                TextData = playerArray[playerIdx].textDataStart;
                break;
            case TextType.recieveCard:
                TextData = playerArray[playerIdx].textDataRecieveCard;
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
            if (isDialogEssential)
                yield break;
            DialogString += TextData[now_Sentence][temp];
            temp++;

            DialogText.text = DialogString;
            yield return new WaitForSeconds(delay);
        }
        isDialogNonEssential = false;
    }
}
