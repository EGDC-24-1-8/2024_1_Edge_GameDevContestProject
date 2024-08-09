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
    private string[] textData;
    public string[] TextData { get { return textData; } }



}
