using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public PlayerData playerData;
    public GameObject playerSprite;
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
    public string[] textDataTime;
    public string[] textDataWin;
    public string[] textDataDetected;
    public string[] textDataMissDetected;
    public string[] textDataBusted;
    public string[] textDataSuspicion;


    public bool isAlly; 

    //public bool isCheat = false;

    public void initData()
    {
        playerMoney = playerData.PlayerMoney;
        playerSprite = playerData.PlayerSprite;
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
        textDataTime = playerData.TextDataTime;
        textDataWin = playerData.TextDataWin;
        textDataDetected = playerData.TextDataDetected;
        textDataMissDetected = playerData.TextDataMissDetected;
        textDataBusted = playerData.TextDataBusted;
        textDataSuspicion = playerData.TextDataSuspicion;
        dealtCardCount = 0;
    }
    public void Start_DoCheat(int idx)
    {
        StartCoroutine(DoCheat(idx));
    }

    private IEnumerator DoCheat(int idx)
    {
        GameManager.Instance.playerIsDetectable[idx] = true;
        this.GetComponentInChildren<Animator>().SetTrigger("doCheat");
        //playerSprite.color = Color.red;
        yield return new WaitForSeconds(2f);
        //playerSprite.color = Color.white;
        GameManager.Instance.playerIsDetectable[idx] = false;
    }
}