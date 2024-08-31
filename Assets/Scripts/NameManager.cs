using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameManager : MonoBehaviour
{
    private static NameManager instance;
    public static NameManager Instance { get { return instance; } }

    [TextArea]
    public Text NameText;
    public GameObject TextPanel;
    public Player[] playerArray = null;


    void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    void Start()
    {
        playerArray = GameManager.Instance.playerArray;
    }

    public void ShowName(int playerIdx)
    {
        NameText.text = playerArray[playerIdx].playerName;
    }
}
