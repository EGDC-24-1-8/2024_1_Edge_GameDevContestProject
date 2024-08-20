using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnNewGame : MonoBehaviour
{
    public Button newGameButton;
    // Start is called before the first frame update
    void Start()
    {
        newGameButton.onClick.AddListener(onNewGameButtonClicked);
    }

    void onNewGameButtonClicked()
    {
        Debug.Log("NewGameButton is Clicked");
    }
}
