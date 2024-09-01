using UnityEngine;
using UnityEngine.UI;
using System;

public class BtnPlay : MonoBehaviour
{
    [SerializeField] private AudioClip BGM;
    public Button playButton;
    public GameObject newGameButton;
    public GameObject loadGameButton;
    public GameObject optionButton;
    public GameObject creditButton;
    public event Action playBtnOn;

    void Start()
    {
        AudioManager.GetOrCreate().SetBGMVolume(0.7f);
        AudioManager.GetOrCreate().PlayBGM(BGM);
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