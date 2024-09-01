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

    public Slider BgmSlider;
    public Slider EffectSlider;


    void Start()
    {
        AudioManager.GetOrCreate().SetBGMVolume(0.7f * PlayerPrefs.GetFloat("PlayerBgm"));
        AudioManager.GetOrCreate().SetEffectVolume(PlayerPrefs.GetFloat("PlayerEffect"));
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

    public void UpdateBgmSlider()
    {
        PlayerPrefs.SetFloat("PlayerBgm" , BgmSlider.value);
        AudioManager.GetOrCreate().SetBGMVolume(0.7f * PlayerPrefs.GetFloat("PlayerBgm"));
    }
    public void UpdateEffectSlider()
    {
        PlayerPrefs.SetFloat("PlayerEffect", EffectSlider.value);
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