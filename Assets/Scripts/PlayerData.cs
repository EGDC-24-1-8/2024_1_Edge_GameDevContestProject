using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Object/Player Data", order = int.MaxValue)]
public class PlayerData : ScriptableObject
{
    [SerializeField]
    private int playerMoney;
    public int PlayerMoney { get { return playerMoney; } }

    [SerializeField]
    private GameObject playerSprite;
    public GameObject PlayerSprite { get { return playerSprite; } }


    [SerializeField]
    private int playerBettingMoney;
    public int PlayerBettingMoney { get { return playerBettingMoney; } }


    [SerializeField]
    private string playerName;
    public string PlayerName { get { return playerName; } }



    [SerializeField]
    private int highPreference;
    public int HighPreference { get { return highPreference; } }
    [SerializeField]
    private int lowPreference;
    public int LowPreference { get { return lowPreference; } }

    [SerializeField]
    private int cheatFrequency;
    public int CheatFrequency { get { return cheatFrequency; } }

    [SerializeField]
    private string[] textDataStart;
    public string[] TextDataStart { get { return textDataStart; } }


    [SerializeField]
    private string[] textDataRecieveCard;
    public string[] TextDataRecieveCard { get { return textDataRecieveCard; } }

    [SerializeField]
    private string[] textDataCall;
    public string[] TextDataCall { get { return textDataCall; } }

    [SerializeField]
    private string[] textDataRaise;
    public string[] TextDataRaise { get { return textDataRaise; } }

    [SerializeField]
    private string[] textDataFold;
    public string[] TextDataFold { get { return textDataFold; } }

    [SerializeField]
    private string[] textDataTime;
    public string[] TextDataTime { get { return textDataTime; } }

    [SerializeField]
    private string[] textDataWin;
    public string[] TextDataWin { get { return textDataWin; } }

    [SerializeField]
    private string[] textDataDetected;
    public string[] TextDataDetected { get { return textDataDetected; } }

    [SerializeField]
    private string[] textDataMissDetected;
    public string[] TextDataMissDetected { get { return textDataMissDetected; } }

    [SerializeField]
    private string[] textDataBusted;
    public string[] TextDataBusted { get { return textDataBusted; } }

    [SerializeField]
    private string[] textDataSuspicion;
    public string[] TextDataSuspicion { get { return textDataSuspicion; } }

}
