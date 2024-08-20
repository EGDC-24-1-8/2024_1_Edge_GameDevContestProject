using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; 
using System;
public class BtnOption : MonoBehaviour
{
    public Button optionButton;
    public Button playButton;
    private bool flag;
    private Color originalColor;
    public event Action optionButtonOn;
    void Start()
    {
        playButton.GetComponent<BtnPlay>().playBtnOn += setButtonActive;
        optionButton.onClick.AddListener(onOptionBtnClicked);
        flag = true;
        originalColor = optionButton.colors.normalColor;
    }

    void setButtonActive()
    {
        optionButton.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().color = Color.gray;
        flag = false;
    }

    void onOptionBtnClicked()
    {
        if (!flag)
        {
            optionButtonOn?.Invoke();
            optionButton.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().color = originalColor;
            flag = true;
            return;
        }
        SceneManager.LoadScene("OptionScene");
    }
}