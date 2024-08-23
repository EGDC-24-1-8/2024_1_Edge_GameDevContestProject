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

    public bool isDialog = false;
    public GameObject TextPanel;
    public Player[] playerArray = null;



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
        //TextData = playerArray[1].textData;
        TextIndex = TextData.Length;
        //NextSentence();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isDialog)
                return;
            if (now_Sentence < TextIndex)
            {
                //NextSentence();
            }
        }
    }

    public void TriggerNextSentence(int playerIdx, TextType type)
    {
        StartCoroutine(NextSentence(playerIdx, type));
    }

    public IEnumerator NextSentence(int playerIdx, TextType type)
    {
        isDialog = true;
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

        now_Sentence = Random.Range(0, TextData.Length);
        TextLen = TextData[now_Sentence].Length;
        int temp = 0;

        while (temp < TextLen)
        {
            DialogString += TextData[now_Sentence][temp];
            temp++;

            DialogText.text = DialogString;
            yield return new WaitForSeconds(delay);
        }
        isDialog = false;
    }
}
