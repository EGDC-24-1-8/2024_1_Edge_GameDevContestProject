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
    public string[] playerName;
    public int[] playerCardNum;
    public Text[] playerCardText;
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
        for(int i = 1; i< 11; i++)
        {
            CardDeck.Add(i);
        }

        int random1, random2;
        int temp;

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

    }


    public void NormalDeal()
    {
        playerCardNum[dealOrder] = CardDeck[0];
        playerCardText[dealOrder].text = CardDeck[0].ToString();
        CardDeck.Remove(CardDeck[0]);
        dealOrder++;
        TopCard.text = CardDeck[0].ToString();
    }

    public void BottomDeal()
    {
        playerCardNum[dealOrder] = CardDeck[CardDeck.Count-1];
        playerCardText[dealOrder].text = CardDeck[CardDeck.Count - 1].ToString();
        CardDeck.Remove(CardDeck[CardDeck.Count-1]);
        dealOrder++;
        BottomCard.text = "Undefined";
    }

    public void GameOver()
    {
        Debug.Log("게임 끝!");
    }
}
