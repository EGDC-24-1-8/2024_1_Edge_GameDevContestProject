using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BtnLoadGame : MonoBehaviour
{
    public Button loadGameButton;
    // Start is called before the first frame update
    void Start()
    {
        loadGameButton.onClick.AddListener(onLoadGameButtonClicked);
    }

    void onLoadGameButtonClicked()
    {
        SceneManager.LoadScene("CutScene");
        Debug.Log("LoadGameButton is Clicked");
    }
}