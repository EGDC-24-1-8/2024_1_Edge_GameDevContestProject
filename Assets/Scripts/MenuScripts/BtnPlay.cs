using UnityEngine;
using UnityEngine.UI;
using System;

public class BtnPlay : MonoBehaviour
{
    public Button playButton;
    public GameObject newGameButton;
    public GameObject loadGameButton;
    public GameObject optionButton;
    public GameObject creditButton;
    public event Action playBtnOn;

    void Start()
    {
        newGameButton.SetActive(false);
        loadGameButton.SetActive(false);
        playButton.onClick.AddListener(onPlayBtnClicked);
        optionButton.GetComponent<BtnOption>().optionButtonOn += BtnHidden;
        creditButton.GetComponent<BtnCredit>().creditButtonOn += BtnHidden;
    }

    void BtnHidden()
    {
        newGameButton.SetActive(false);
        loadGameButton.SetActive(false);
    }

    public void onPlayBtnClicked()
    {
        newGameButton.SetActive(true);
        loadGameButton.SetActive(true);
        
        playBtnOn?.Invoke();
        //optionButton.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().color = Color.gray;
        //creditButton.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().color = Color.gray;
    }
}