using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public PlayerData playerData;
    public Image playerImage;
    public int playerMoney;
    public int playerBettingMoney;
    public string playerName;
    public int highPreference;
    public int lowPreference;
    public float cheatFrequency;
    public int dealtCardCount;
    public string[] textDataStart;
    public string[] textDataRecieveCard;
    public string[] textDataCall;
    public string[] textDataRaise;
    public string[] textDataFold;
    public string[] textDataWin;
    public string[] textDataDetected;
    public string[] textDataDoubt;


    public bool isAlly = false; 

    //public bool isCheat = false;

    public void initData()
    {
        playerMoney = playerData.PlayerMoney;
        playerBettingMoney = playerData.PlayerBettingMoney;
        playerName = playerData.PlayerName;
        highPreference = playerData.HighPreference;
        lowPreference = playerData.LowPreference;
        cheatFrequency = playerData.CheatFrequency;
        textDataStart = playerData.TextDataStart;
        textDataRecieveCard = playerData.TextDataRecieveCard;
        textDataCall = playerData.TextDataCall;
        textDataRaise = playerData.TextDataRaise;
        textDataFold = playerData.TextDataFold;
        textDataWin = playerData.TextDataWin;
        textDataDetected = playerData.TextDataDetected;
        textDataDoubt = playerData.TextDataDoubt;
        dealtCardCount = 0;
    }
    public void Start_DoCheat(int idx)
    {
        StartCoroutine(DoCheat(idx));
    }

    private IEnumerator DoCheat(int idx)
    {
        GameManager.Instance.playerIsDetectable[idx] = true;
        //playerImage.color = Color.red;
        yield return new WaitForSeconds(2f);
        //playerImage.color = Color.white;
        GameManager.Instance.playerIsDetectable[idx] = false;
    }
}