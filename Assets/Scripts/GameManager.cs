using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    [SerializeField] public int foldPlayerCnt = 0;
    [SerializeField] public int IngamePlayerCnt = 4;

    [SerializeField] private int gameTurn = 0;
    [SerializeField] private int dealtCardCount = 0; // 카드 나눠주는 턴
    [SerializeField] private int dealOrder = 0; // 현재 카드 나눠줄 플레이어 순서
    [SerializeField] private List<int> CardDeck = null;

    [SerializeField] private List<PlayerData> playerDataSet = null;
    [SerializeField] public Player[] playerArray = null;
    [SerializeField] public BettingManager betMan;


    [SerializeField] public bool isAbleToDeal = true;
    [SerializeField] public bool isCheat = false;

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


    public void SetIsAbleToDeal(bool isValue)
    {
        isAbleToDeal = isValue;
    }
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
        isAbleToDeal = true;
        TopCardText.text = CardDeck[0].ToString();

        IngamePlayerCnt = 4;

        BottomCardText.text = CardDeck[CardDeck.Count - 1].ToString();
    }

    public void Update()
    {
        if (betMan.isBetOver) //딜링 끝
        {
            ShowCardSum();
            betMan.calculateResult(betMan.decideWinner(playerCardSum));
            dealtCardCount = 0;
            betMan.isBetOver = false;
        }
    }
    public void CheckWinnerByFold()
    {
        if (foldPlayerCnt == IngamePlayerCnt-1)
        {
            for (int j = 0; j < betMan.isFold.Length; j++)
            {
                if (betMan.isFold[j] == false)
                {
                    betMan.calculateResult(betMan.decideWinnerByFold(j));
                }
            }
        }
    }

    private IEnumerator nextTurn()
    {
        yield return new WaitForSeconds(5f);
        gameTurn++;
        ResetDealedCard();
        for (int i = 0; i < playerArray.Length; i++) //입장 베팅
        {
            betMan.entranceBet(i);
        }
        palmCardNum = 0;

        TopCardText.text = CardDeck[0].ToString();
        BottomCardText.text = CardDeck[CardDeck.Count - 1].ToString();
    }
    public void TriggerNextTurn()
    {
        StartCoroutine(nextTurn());
    }

    public void CheckDealOrder()
    {

        if(dealOrder >= IngamePlayerCnt) //카드를 다 나눠줌 이미 나눠주고 더했음 즉 지금 마지막 플레이어한테 카드를 주고, 이 함수로 넘어온 상황
        {
            ++dealtCardCount;
            StartCoroutine(betMan.Betting());
            dealOrder = 0;
        }


        if (betMan.isFold.Length > dealOrder) //지금 dealOrder번의 플레이어가 fold면 한칸 뒤로 옮기기
        {
            if (betMan.isFold[dealOrder] == true)
            {
                dealOrder++;
                CheckDealOrder();
                return;
            }
        }

        // 1. deal Order이 fold 플레이어를 넘겨야합니다. (전처리)


        // 2. deal Order이 IngamePlayerCnt 이상이 되면 플레이어에게 카드를 다 나눠준것이므로... 베팅을 시작합니다. 그리고 카드 턴을 더합니다.

        //동시에 처리를 해야


        if (dealOrder == 0) // 모든 플레이어게 카드 배분할 때 마다 Bottom 카드가 보이게 수정
        {
            BottomCardText.text = CardDeck[CardDeck.Count - 1].ToString();
        }


        if (dealtCardCount == 2) // 애들이 카드를 1장 받고 치트를 쓸수없으니까
        {
            if(!isCheat)
            {
                isCheat = true;
                StartCoroutine(CheatCycle());
            }
        }
    }

    private void ResetDealedCard()
    {
        Array.Clear(playerCard0Num, 0, playerCard0Num.Length);
        Array.Clear(playerCard1Num, 0, playerCard1Num.Length);
        Array.Clear(playerCard2Num, 0, playerCard2Num.Length);
        Array.Clear(playerCardSum, 0, playerCardSum.Length);
        foldPlayerCnt = 0;
        dealOrder = 0;
        dealtCardCount = 0;
        for (int i =0; i < 4; i++)
        {
            playerCard0Text[i].text = "";
            playerCard1Text[i].text = "";
            playerCard2Text[i].text = "";
            playerSumText[i].text   = "";
            betMan.isFold[i] = false;
            betMan.resetBet();
        }
        betMan.UpdateUIText();
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

    private IEnumerator CheatCycle() //코루틴
    {
        while (true)
        {
            for(int i = 0; i < 4; i++)
            {
                DecideToSwitch(i);
            }
            yield return new WaitForSeconds(UnityEngine.Random.Range(10, 30));
        }
    }

    private void InitPlayer()
    {
        for (int i = 0; i < 4; i++)
        {
            int temp = UnityEngine.Random.Range(0, playerDataSet.Count);
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
            random1 = UnityEngine.Random.Range(0, deck.Count);
            random2 = UnityEngine.Random.Range(0, deck.Count);

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
        if(!isAbleToDeal)
        {
            return; 
        }

        
        switch (dealtCardCount)
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
        CheckDealOrder();
    }

    public void BottomDeal()
    {
        if (!isAbleToDeal)
        {
            return;
        }
        
        switch (dealtCardCount)
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
        CheckDealOrder();
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

            if(20f + playerArray[idx].cheatFrequency < UnityEngine.Random.Range(0,101))
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

    public int GetDealtCardCount()
    {
        return dealtCardCount;
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
        IngamePlayerCnt--;


        //idx 플레이어 제거처리
    }
}
