using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        Debug.Log("LoadGameButton is Clicked");
    }
}