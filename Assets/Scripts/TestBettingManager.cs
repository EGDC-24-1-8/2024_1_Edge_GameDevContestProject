using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestBettingManager : MonoBehaviour
{
    [SerializeField] private Player[] playerArray = null;
    [SerializeField] private int[] playerCardSum = null;

    [Header("UI")]
    [SerializeField] private Text[] playerBetText = null;
    [SerializeField] private Text potText = null;
    [SerializeField] private Text[] winnerText = null;
    [SerializeField] private Text[] playerFoldText = null;
    [SerializeField] private Text[] playerSeedText = null;

    [Header("Betting Amount")]
    [SerializeField] private int ante = 10; //참여금
    [SerializeField] private int defaultBet = 10; //기본 베팅 단위
    [SerializeField] private int roundBet = 0; //해당 베팅 페이즈의 베팅 단위
    [SerializeField] private int maxBet = 10; //라운드 최대 베팅 금액
    [SerializeField] private int pot = 0; //판돈
    [SerializeField] private int prize = 0; //라운드 상금
    [SerializeField] private int eliminationCriteria = 0; //소유금이 여기까지 줄어들면 해당 플레이어가 테이블을 떠남

    [Header("In Game Data")]
    [SerializeField] private List<int> winner = null;
    [SerializeField] public int dealtCardCount = 0;
    [SerializeField] private int endOrder = 0;
    [SerializeField] public bool[] isFold = null;
    [SerializeField] public bool isBetOver = false;

    void Start()
    {
        playerArray = TestGameManager.Instance.playerArray;
        playerCardSum = TestGameManager.Instance.playerCardSum;
        roundBet = defaultBet;
    }

    //베팅 페이즈 전반 관리
    //1. 베팅 관리
    //2. Winner By Fold 상시 체크

    /*
    public void CheckWinnerByFold()
    {
        if (foldPlayerCnt == IngamePlayerCnt-1)
        {
            for (int j = 0; j < betMan.isFold.Length; j++)
            {
                if (betMan.isFold[j] == false)
                {
                    betMan.CalculateResult(betMan.DecideWinnerByFold(j));
                }
            }
        }
    }
    */
    //3. 베팅 페이즈 종료 후 gameState afterBet으로 변경
    //4. 돈 정산까지 얘가 하자

    void Update()
    {
        if (isBetOver)
        {
            StopCoroutine("Betting");
            UpdateUIText();
        }
    }

    public void UpdateUIText()
    {
        for (int i = 0; i < playerArray.Length; i++)
        {
            playerSeedText[i].text = playerArray[i].playerMoney.ToString();
            playerBetText[i].text = playerArray[i].playerBettingMoney.ToString();
            playerFoldText[i].text = isFold[i] ? "FOLD" : "IN";
            potText.text = pot.ToString();
        }
    }

    public void TriggerBetting()
    {
        StartCoroutine(Betting());
    }

    public IEnumerator Betting()
    {
        if(TestGameManager.Instance.gameState != TestGameManager.GameState.bet || isBetOver == true)
        {
            yield break;
        }
        ++dealtCardCount;
        endOrder = playerArray.Length;
        for (int i = 0; i != endOrder && !isBetOver; i = (i+1)%4) //한 턴이 돌 때마다 베팅 진행
        {
            yield return new WaitForSeconds(1);
            bet(i);
            playerSeedText[i].text = playerArray[i].playerMoney.ToString();
        }
        yield return new WaitForSeconds(1);
        TestGameManager.Instance.SetStateAfterBet();
    }

    #region betting options
    public void entranceBet(int playerIdx)
    {
        maxBet = ante;
        playerArray[playerIdx].playerBettingMoney += ante;
        playerArray[playerIdx].playerMoney -= ante;
        pot += ante;
        UpdateUIText();
    }

    public void bet(int playerIdx)
    {
        if (isBetOver)
            return;

        if (isFold[playerIdx] == true)
            return;

        if (dealtCardCount == 2
            && (playerCardSum[playerIdx] < 8 || 16 < playerCardSum[playerIdx]))
        {
            fold(playerIdx);
            return;
        }
        else if (dealtCardCount == 3)
        {
            if (playerCardSum[playerIdx] < 17 || 21 < playerCardSum[playerIdx])
            {
                fold(playerIdx);
                return;
            }
            if ((playerCardSum[playerIdx] == 21) && (defaultBet == roundBet))
            {
                raise(playerIdx);
                return;
            }
        }
        call(playerIdx);
    }

    public void call(int playerIdx)
    {
        if (playerArray[playerIdx].playerBettingMoney == maxBet)
        {
            playerArray[playerIdx].playerBettingMoney += roundBet;
            playerArray[playerIdx].playerMoney -= roundBet;
            pot += roundBet;
            maxBet += roundBet;
            endOrder = playerIdx;
        }
        else
        {
            playerArray[playerIdx].playerMoney += playerArray[playerIdx].playerBettingMoney;
            pot -= playerArray[playerIdx].playerBettingMoney;
            playerArray[playerIdx].playerBettingMoney = maxBet;
            playerArray[playerIdx].playerMoney -= maxBet;
            pot += maxBet;
        }
        UpdateUIText();
    }

    public void raise(int playerIdx)
    {
        roundBet += defaultBet;
        endOrder = playerIdx;

        playerArray[playerIdx].playerBettingMoney += roundBet;
        playerArray[playerIdx].playerMoney -= roundBet;
        pot += roundBet;
        maxBet = playerArray[playerIdx].playerBettingMoney;
        endOrder = playerIdx;
        UpdateUIText();
    }

    public void fold(int playerIdx)
    {
        isFold[playerIdx] = true;
        TestGameManager.Instance.foldPlayerCnt++;
        CheckWinnerByFold();
        UpdateUIText();
    }
    #endregion

    #region win by fold

    private void CheckWinnerByFold()
    {
        if (TestGameManager.Instance.foldPlayerCnt == TestGameManager.Instance.IngamePlayerCnt - 1)
        {
            isBetOver = true;
        }
    }

    public List<int> DecideWinnerByFold(int playerIdx) //winner list 반환
    {
        isBetOver = true;
        winner.Clear();
        winner.Add(playerIdx);
        winnerText[0].text = playerIdx.ToString();
        return winner;
    }

    #endregion

    #region afterBet

    public void ResetBet() //베팅금, 팟 등 데이터 리셋
    {
        dealtCardCount = 0;
        endOrder = playerArray.Length;
        isBetOver = false;
        for (int i = 0; i < playerArray.Length; i++)
        {
            playerArray[i].playerBettingMoney = 0;
            isFold[i] = false;
        }
        roundBet = defaultBet;
        pot = 0;
        prize = 0;
    }

    public List<int> DecideWinner(int[] sum) //winner list 반환
    {
        int max = 0;
        for (int i = 0; i < sum.Length; i++)
        {
            if (sum[i] > 21 || 21 - sum[i] > 21 - max || isFold[i] == true)
                continue;
            else if (21 - sum[i] < 21 - max)
            {
                max = sum[i];
                winner.Clear();
            }
            winner.Add(i);
        }
        for (int i = 0; i < winner.Count; i++)
            winnerText[i].text = winner[i].ToString();
        return winner;
    }

    public void CalculateResult(List<int> playerIdx) //winner에게 prize 전달, 
    {
        isBetOver = true;
        if (winner.Count != 0)
            prize = pot / winner.Count;
        for (; winner.Count > 0; winner.RemoveAt(0))
            playerArray[winner[0]].playerMoney += prize;
        UpdateUIText();
        for (int i = 0; i < playerArray.Length; i++)
        {
            if (eliminationCriteria > playerArray[i].playerMoney)
            {
                TestGameManager.Instance.EliminatePlayer(i, TestGameManager.NO_MONEY_ELIMINATED);
            }
        }
        TestGameManager.Instance.TriggerNextTurn();
    }

    #endregion
}
