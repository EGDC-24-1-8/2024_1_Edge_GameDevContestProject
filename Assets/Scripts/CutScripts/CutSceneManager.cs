using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class CutSceneManager : MonoBehaviour
{
    [SerializeField] private AudioClip BGM;
    [Header("Cutscene Elements")]
    public Image displayImage;
    public Text displayText;
    public Button nextButton;

    [Header("Cutscene Content")]
    public Sprite[] images;       //이미지 배열
    [TextArea(3, 10)]
    public string[] dialogue;     //텍스트 배열

    private int currentIndex = 0; //컷씬 인덱스
    private Coroutine typingCoroutine;

    void Start()
    {
        AudioManager.GetOrCreate().SetBGMVolume(0.1f * PlayerPrefs.GetFloat("PlayerBgm"));
        AudioManager.GetOrCreate().PlayBGM(BGM);
        currentIndex = PlayerPrefs.GetInt("CutSceneBegin");
        nextButton.onClick.AddListener(ShowNext);
        UpdateCutscene();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShowNext();
        }
    }

    void ShowNext()
    {
        if (currentIndex == images.Length - 1)
        {
            //게임 클리어
            SceneManager.LoadScene("MainMenuScene");
        }
        else if (currentIndex < PlayerPrefs.GetInt("CutSceneEnd"))
        {
            currentIndex++;
            UpdateCutscene();
        }
        else
        {
            ChangeScene();
        }
    }

    void UpdateCutscene()
    {
        if (currentIndex == PlayerPrefs.GetInt("CutSceneEnd"))
        {
            nextButton.GetComponentInChildren<Text>().text = "END";
        }

        displayImage.sprite = images[currentIndex];

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeText(dialogue[currentIndex]));
    }

    IEnumerator TypeText(string text)
    {
        displayText.text = ""; 

        foreach (char letter in text.ToCharArray())
        {
            displayText.text += letter;
            yield return new WaitForSeconds(0.03f); 
        }
    }

    //게임 씬으로 전환
    void ChangeScene()
    {
        SceneManager.LoadScene("Game Scene");
        Debug.Log("Change to game scene.");
    }
}
