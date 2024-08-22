using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BtnUndo : MonoBehaviour
{
    public Button undoButton;
    public GameObject mainPanel;
    public GameObject optionPanel;
    public GameObject creditPanel;
    void Start()
    {
        undoButton.onClick.AddListener(onUndoBtnClicked);
    }

    public void onUndoBtnClicked()
    {
        optionPanel.SetActive(false);
        creditPanel.SetActive(false);
        mainPanel.SetActive(true);
    }
}
