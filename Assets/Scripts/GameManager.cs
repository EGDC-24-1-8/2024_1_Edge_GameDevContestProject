using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    //디자인패턴중 싱글톤 패턴

    public GameObject[] Player;
    //public string[] playerName;
    public int[] playerCardNum;
    public int[] playerCardSum;
    public Text[] playerCardText;
    public Text[] playerSumText;
    public Text TopCardText;
    public Text BottomCardText;
    public Text PalmCardText;
    public int palmCardNum;
    
    public int dealOrder = 0;
    public List<int> CardDeck = null;

    private void Awake()
    {
        if (null == Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
         
            Destroy(this.gameObject);
        }
    }

    void Start()
    {


        int random1, random2;
        int temp;
        
        for (int i = 0; i < 52; i++)
        {
            temp = i % 13 + 1;
            temp = temp > 10 ? 10 : temp;
            CardDeck.Add(temp);
        }

        for (int i = 0; i < CardDeck.Count; ++i)
        {
            random1 = Random.Range(0, CardDeck.Count);
            random2 = Random.Range(0, CardDeck.Count);

            temp = CardDeck[random1];
            CardDeck[random1] = CardDeck[random2];
            CardDeck[random2] = temp;
        }
        TopCardText.text = CardDeck[0].ToString();
        BottomCardText.text = CardDeck[CardDeck.Count - 1].ToString();
        palmCardNum = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if((dealOrder % 4) == 0)
            BottomCardText.text = CardDeck[CardDeck.Count - 1].ToString();
        TopCardText.text = CardDeck[0].ToString();

    }


    public void NormalDeal()
    {
        playerCardNum[dealOrder] = CardDeck[0];
        playerCardText[dealOrder].text = playerCardNum[dealOrder].ToString();
        CardDeck.Remove(CardDeck[0]);
        dealOrder++;
        TopCardText.text = CardDeck[0].ToString();
        if (dealOrder > 11)
        {
            for (int i = 0; i < 12; i++)
            {
                playerCardSum[i % 4] += playerCardNum[i];
                playerSumText[i % 4].text = playerCardSum[i % 4].ToString();
            }
        }
    }

    public void BottomDeal()
    {
        playerCardNum[dealOrder] = CardDeck[CardDeck.Count-1];
        playerCardText[dealOrder].text = playerCardNum[dealOrder].ToString();
        CardDeck.RemoveAt(CardDeck.Count-1);
        dealOrder++;
        BottomCardText.text = "Undefined";
        if (dealOrder > 11)
        {
            for (int i = 0; i < 12; i++)
            {
                playerCardSum[i % 4] += playerCardNum[i];
                playerSumText[i % 4].text = playerCardSum[i % 4].ToString();
            }
        }
    }

    public void Palm()
    {
        int temp;
        if(palmCardNum == 0)
        {
            palmCardNum = CardDeck[0];
            CardDeck.Remove(CardDeck[0]);
        }
        else
        {
            temp = palmCardNum;
            palmCardNum = CardDeck[0];
            CardDeck[0] = temp;
        }
        PalmCardText.text = palmCardNum.ToString();
    }

    public void GameOver()
    {
        Debug.Log("게임 끝!");
    }
}
