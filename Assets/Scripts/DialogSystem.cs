using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{
    private static DialogSystem instance;
    public static DialogSystem Instance { get { return instance; } }

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

    private bool isDialog = false;
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
        TextData = playerArray[0].textData;
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
                NextSentence();
            }
        }
    }

    void NextSentence()
    {
        DialogString = "";
        TextLen = TextData[now_Sentence].Length;
        StartCoroutine(NextSentence_Play());
    }

    IEnumerator NextSentence_Play()
    {
        isDialog = true;
        int temp = 0;

        while (temp < TextLen)
        {
            DialogString += TextData[now_Sentence][temp];
            temp++;

            DialogText.text = DialogString;
            yield return new WaitForSeconds(delay);
        }
        now_Sentence++;
        isDialog = false;
    }
}
