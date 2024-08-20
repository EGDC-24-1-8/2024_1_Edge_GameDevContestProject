using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using TMPro;

public class BtnCredit : MonoBehaviour
{
    public Button creditButton;
    public Button playButton;
    private bool flag;
    private Color originalColor;
    public event Action creditButtonOn;
    void Start()
    {
        playButton.GetComponent<BtnPlay>().playBtnOn += setButtonActive;
        creditButton.onClick.AddListener(onCreditBtnClicked);
        flag = true;
        originalColor = creditButton.colors.normalColor;
    }
    
    void setButtonActive()
    {
        creditButton.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().color = Color.gray;
        flag = false;
    }

    void onCreditBtnClicked()
    {
        if (!flag)
        {
            creditButtonOn?.Invoke();
            creditButton.GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().color = originalColor;
            flag = true;
            return;
        }
        SceneManager.LoadScene("CreditScene");
    }
}
