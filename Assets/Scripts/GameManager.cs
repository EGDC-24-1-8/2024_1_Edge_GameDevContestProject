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
    public Text TopCard;
    public Text BottomCard;

    
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
        TopCard.text = CardDeck[0].ToString();
        BottomCard.text = CardDeck[CardDeck.Count - 1].ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if((dealOrder % 4) == 0)
            BottomCard.text = CardDeck[CardDeck.Count - 1].ToString();

    }


    public void NormalDeal()
    {
        playerCardNum[dealOrder] = CardDeck[0];
        playerCardText[dealOrder].text = CardDeck[0].ToString();
        CardDeck.Remove(CardDeck[0]);
        dealOrder++;
        TopCard.text = CardDeck[0].ToString();
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
        playerCardText[dealOrder].text = CardDeck[CardDeck.Count - 1].ToString();
        CardDeck.Remove(CardDeck[CardDeck.Count-1]);
        dealOrder++;
        BottomCard.text = "Undefined";
        if (dealOrder > 11)
        {
            for (int i = 0; i < 12; i++)
            {
                playerCardSum[i % 4] += playerCardNum[i];
                playerSumText[i % 4].text = playerCardSum[i % 4].ToString();
            }
        }
    }

    public void GameOver()
    {
        Debug.Log("게임 끝!");
    }
}
