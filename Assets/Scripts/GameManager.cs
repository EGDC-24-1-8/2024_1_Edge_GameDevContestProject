using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public const int NO_MONEY_ELIMINATED = 0;
    public static GameManager Instance = null;//싱글톤 패턴

    //Card UI
    [Header("UI")]
    [SerializeField] private Text[] playerCard0Text;
    [SerializeField] private Text[] playerCard1Text;
    [SerializeField] private Text[] playerCard2Text;
    [SerializeField] private Text[] playerSumText;
    [SerializeField] private Text TopCardText;
    [SerializeField] private Text BottomCardText;
    [SerializeField] private Text PalmCardText;
    [SerializeField] private GameObject[] Player;

    //Card Value
    [Header("card Value")]
    //public string[] playerName;
    //[SerializeField] private int[] playerCardNum;
    [SerializeField] private int[] playerCard0Num;
    [SerializeField] private int[] playerCard1Num;
    [SerializeField] private int[] playerCard2Num;
    [SerializeField] public int[] playerCardSum;
    [SerializeField] private int palmCardNum;

    [SerializeField] private int nowGameTurn = 0; // 카드 나눠주는 턴
    [SerializeField] private int dealOrder = 0; // 현재 카드 나눠줄 플레이어 순서
    [SerializeField] private List<int> CardDeck = null;

    [SerializeField] private List<PlayerData> playerDataSet = null;
    [SerializeField] public Player[] playerArray = null;
    [SerializeField] public BettingManager betMan;


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
        CardDeck = InitDeck();
        CardDeck = shuffleDeck(CardDeck);
        InitPlayer();
        for (int i = 0; i < playerArray.Length; i++) //입장 베팅
        {
            betMan.entranceBet(i);
        }
        palmCardNum = 0;

        TopCardText.text = CardDeck[0].ToString();
        BottomCardText.text = CardDeck[CardDeck.Count - 1].ToString();
    }

    void Update()
    {
        if(dealOrder == 4)
        {
            dealOrder = 0;
            ++nowGameTurn;
            StartCoroutine(betMan.Betting());
        }
        #region Win by Fold
        if (!betMan.isFold[0] && betMan.isFold[1] && betMan.isFold[2] && betMan.isFold[3])
        {
            betMan.calculateResult(betMan.decideWinnerByFold(0));
        }

        if (betMan.isFold[0] && !betMan.isFold[1] && betMan.isFold[2] && betMan.isFold[3])
        {
            betMan.calculateResult(betMan.decideWinnerByFold(1));
        }

        if (betMan.isFold[0] && betMan.isFold[1] && !betMan.isFold[2] && betMan.isFold[3])
        {
            betMan.calculateResult(betMan.decideWinnerByFold(2));
        }

        if (betMan.isFold[0] && betMan.isFold[1] && betMan.isFold[2] && !betMan.isFold[3])
        {
            betMan.calculateResult(betMan.decideWinnerByFold(3));
        }
        #endregion
        if (dealOrder == 0) //한 턴이 돌때마다 Bottom 카드가 보이게 수정
        {
            BottomCardText.text = CardDeck[CardDeck.Count - 1].ToString();
        }

        if (betMan.isFold[dealOrder])
            dealOrder++;

        TopCardText.text = CardDeck[0].ToString();
        

        if(nowGameTurn == 2)
        {
            int cheatOrder = dealOrder == 0 ? 3 : (dealOrder - 1);
            StartCoroutine(CheatCycle(cheatOrder));
        }

        if (nowGameTurn > 2 && betMan.isBetOver) //딜링 끝
        {
            ShowCardSum();
            betMan.calculateResult(betMan.decideWinner(playerCardSum));
            nowGameTurn = 0;
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

        for (int i = 0; i < 4; i++)
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
        for (int i = 0; i < 4; i++)
            playerSumText[i].text = playerCardSum[i].ToString();
    }

    public void NormalDeal()
    {
        switch(nowGameTurn)
        {
            case 0:
                playerCard0Num[dealOrder] = CardDeck[0];
                playerCardSum[dealOrder] += playerCard0Num[dealOrder];
                playerCard0Text[dealOrder].text = playerCard0Num[dealOrder].ToString();
                break;
            case 1:
                playerCard1Num[dealOrder] = CardDeck[0];
                playerCardSum[dealOrder] += playerCard1Num[dealOrder];
                playerCard1Text[dealOrder].text = playerCard1Num[dealOrder].ToString();
                break;
            case 2:
                playerCard2Num[dealOrder] = CardDeck[0];
                playerCardSum[dealOrder] += playerCard2Num[dealOrder];
                playerCard2Text[dealOrder].text = playerCard2Num[dealOrder].ToString();
                break;
            default:
                break;
        }
        CardDeck.Remove(CardDeck[0]);
        dealOrder++;
        TopCardText.text = CardDeck[0].ToString();
    }

    public void BottomDeal()
    {
        switch(nowGameTurn)
        {
            case 0:
                playerCard0Num[dealOrder] = CardDeck[CardDeck.Count - 1];
                playerCardSum[dealOrder] += playerCard0Num[dealOrder];
                playerCard0Text[dealOrder].text = playerCard0Num[dealOrder].ToString();
                break;
            case 1:
                playerCard1Num[dealOrder] = CardDeck[CardDeck.Count - 1];
                playerCardSum[dealOrder] += playerCard1Num[dealOrder];
                playerCard1Text[dealOrder].text = playerCard1Num[dealOrder].ToString();
                break;
            case 2:
                playerCard2Num[dealOrder] = CardDeck[CardDeck.Count - 1];
                playerCardSum[dealOrder] += playerCard2Num[dealOrder];
                playerCard2Text[dealOrder].text = playerCard2Num[dealOrder].ToString();
                break;

        }
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

            if(20f + playerArray[idx].cheatFrequency < Random.Range(0,101))
            {
                SwitchCard(idx);
            }


        }
    }
    public void SwitchCard(int idx)
    {
        //사기치는 애니메이션 재생
        playerArray[idx].Start_DoCheat();
        playerCard0Num[idx] = 5;
        playerCard1Num[idx] = 6;
        playerCard2Num[idx] = 10; //숨긴 카드 3장을 가지고 특정 몇 장만 바꾸는 식으로 조작하도록 수정
    }
    #endregion

    public int GetNowGameTurn()
    {
        return nowGameTurn;
    }

    public int GetDealOrder()
    {
        return dealOrder;
    }

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
