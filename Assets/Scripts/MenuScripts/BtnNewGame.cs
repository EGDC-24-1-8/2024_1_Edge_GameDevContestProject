using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
        PlayerPrefs.SetInt("Day", 1);
        PlayerPrefs.SetInt("CutSceneBegin", 0);
        PlayerPrefs.SetInt("CutSceneEnd", 11);
        SceneManager.LoadScene("CutScene");
        Debug.Log("NewGameButton is Clicked");
    }
}
