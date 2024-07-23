using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public const int NO_MONEY_ELIMINATED = 0;
    public static GameManager Instance = null;//디자인패턴중 싱글톤 패턴



    //Card UI
    [Header("UI")]
    [SerializeField] private Text[] playerCardText;
    [SerializeField] private Text[] playerSumText;
    [SerializeField] private Text TopCardText;
    [SerializeField] private Text BottomCardText;
    [SerializeField] private Text PalmCardText;
    [SerializeField] private GameObject[] Player;

    //Card Value
    [Header("card Value")]
    //public string[] playerName;
    [SerializeField] private int[] playerCardNum;
    [SerializeField] private int[] playerCardSum;
    [SerializeField] private int palmCardNum;

    [SerializeField] private int nowGameTurn = 0;
    [SerializeField] private int dealOrder = 0; // 현재 카드 나눠줄 플레이어 순서
    [SerializeField] private List<int> CardDeck = null;

    [SerializeField] private List<PlayerData> playerDataSet = null;
    [SerializeField] private Player[] playerArray = null;


    private enum GameState
    {
        start,
        betting,
        deal,
        check, 
        end
    };

    //현재 턴 데이터
    //누구 차례인지
    //플레이어 애들한테 데이터 주는거(카드 데이터)
    //카드 뽑기 등등 다 담당

    //게임 플로우 담당


    private void Awake()
    {

        if (null == Instance) //디자인패턴중 싱글톤 패턴
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
        InitPlayer();
        CardDeck = InitDeck();
        CardDeck = shuffleDeck(CardDeck);
        palmCardNum = 0;

        TopCardText.text = CardDeck[0].ToString();
        BottomCardText.text = CardDeck[CardDeck.Count - 1].ToString();
        
    }

    void Update()
    {
        if((dealOrder % 4) == 0) //bottom카드가 한 턴이 돌때마다 수정
            BottomCardText.text = CardDeck[CardDeck.Count - 1].ToString();
        TopCardText.text = CardDeck[0].ToString();



        if(dealOrder > 11) //딜링 끝
        {
            ShowCardSum();
        }

        if(dealOrder > 8)
        {
            StartCoroutine(CheatCycle());
        }

    }



    #region Card
    private List<int> InitDeck()
    {
        int temp;
        List<int> initDeck = new List<int>();

        for (int i = 0; i < 52; i++) //카드 생성
        {
            temp = i % 13 + 1;
            temp = temp > 10 ? 10 : temp;
            initDeck.Add(temp);
        }

        return initDeck;
    }

    bool storyflag = false;

    private IEnumerator CheatCycle(int value) //코루틴
    {
        while (true)
        {
            for(int i = 0; i < 4; i++)
            {
                DecideToSwitch(i);
            }
            yield return new WaitForSeconds(Random.Range(10, 30));
        }
    }

    private void InitPlayer()
    {
        
        for(int i =0 ; i < 4; i++)
        {
            int temp = Random.Range(0, playerDataSet.Count);
            playerArray[i].playerData = playerDataSet[temp];
            playerArray[i].initData();
            playerDataSet.RemoveAt(temp);
        }
    }

    private List<int> shuffleDeck(List<int> deck)
    {
        int random1, random2;
        int temp;
        for (int i = 0; i < deck.Count; ++i)
        {
            random1 = Random.Range(0, deck.Count);
            random2 = Random.Range(0, deck.Count);

            temp = deck[random1];
            deck[random1] = deck[random2];
            deck[random2] = temp;
        }
        return deck;
    }

    private void ShowCardSum()
    {
        for (int i = 0; i < 12; i++)
        {
            playerCardSum[i % 4] += playerCardNum[i];
            playerSumText[i % 4].text = playerCardSum[i % 4].ToString();
        }
    }

    public void NormalDeal()
    {
        playerCardNum[dealOrder] = CardDeck[0];
        playerCardText[dealOrder].text = playerCardNum[dealOrder].ToString();
        CardDeck.Remove(CardDeck[0]);
        dealOrder++;
        TopCardText.text = CardDeck[0].ToString();
    }

    public void BottomDeal()
    {
        playerCardNum[dealOrder] = CardDeck[CardDeck.Count-1];
        playerCardText[dealOrder].text = playerCardNum[dealOrder].ToString();
        CardDeck.RemoveAt(CardDeck.Count-1);
        dealOrder++;
        BottomCardText.text = "Undefined";
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
    #endregion

    #region Cheat
    public void DecideToSwitch(int idx)
    {
        if (playerCardSum[idx] > 21)
        {

            if(20f + playerArray[idx].cheatFrequnce < Random.Range(0,101))
            {
                SwitchCard(idx);
            }


        }
    }
    public void SwitchCard(int idx)
    {
        //사기치는 애니메이션 재생
        playerArray[idx].Start_DoCheat();
        playerCardNum[1] = 5;
        playerCardNum[5] = 6;
        playerCardNum[9] = 10;
    }
    #endregion



    public void GameOver()
    {
        Debug.Log("게임 끝!");
    }


    public void EliminatePlayer(int idx , int type) //0이면 베팅금 부족, 1이면 고발
    {
        switch (type)
        {
            case NO_MONEY_ELIMINATED:

                break;

            default:
                Debug.Log("-");
                break;
        }
        
        
        //idx 플레이어 제거처리
    }
}
