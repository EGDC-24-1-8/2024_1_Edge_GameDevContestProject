using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BtnUndo : MonoBehaviour
{
    public Button undoButton;
    void Start()
    {
        undoButton.onClick.AddListener(onUndoBtnClicked);
    }

    public void onUndoBtnClicked()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
