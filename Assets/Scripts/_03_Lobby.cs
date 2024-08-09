namespace SoundOfSlash
{
    // System
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;

    // Unity
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using static UnityEngine.InputSystem.InputSettings;


    public class _03_Lobby : MonoBehaviour
    {
        [Header("Menu Buttons")]
        [SerializeField] private Button btn_start = null;
        [SerializeField] private Button btn_profile = null;
        [SerializeField] private Button btn_music_editor = null;
        [SerializeField] private Button btn_shop = null;
        [SerializeField] private Toggle btn_mission = null;
        [SerializeField] private Button btn_credit = null;

        [SerializeField] private Button btn_calibration = null;
        [SerializeField] private Text text_nowOffset = null;

        [SerializeField] private Button btn_setting = null;

        [SerializeField] private Button btn_exit = null;
        [SerializeField] private GameObject pnl_Option = null;

        [SerializeField] private GameObject pnl_KeyBinder = null;

        [SerializeField] AudioClip hoverSoundClip = null;
        public int selectedLobbyIndex = 0;
        [SerializeField] private bool isSetting = false;


        [SerializeField] private Button[] btnList;
        [SerializeField] private Sprite img_hover;
        [SerializeField] private Sprite img_normal;


        private AudioManager audioManager;
        bool isPaused = false;

        private void Awake()
        {
            AddListeners();
            selectedLobbyIndex = 0;

            Invalidate();
            audioManager = AudioManager.GetOrCreate();
            audioManager.SetBGMVolume(PlayerPrefs.GetFloat("BgmSound"));

            audioManager.SetEffectVolume(PlayerPrefs.GetFloat("EffectSound"));
            text_nowOffset.text = "now Offset = "+ ((float)PlayerPrefs.GetInt("CALIBRATION_RESULT_MS", 0) / 1000.0f).ToString() + "s";
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
                OnUpAction();

            if (Input.GetKeyDown(KeyCode.DownArrow))
                OnDownAction();

            float wheelInput = Input.GetAxis("Mouse ScrollWheel");
            if (wheelInput > 0)
                OnUpAction();

            if (wheelInput < 0)
                OnDownAction();

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                ExitSettingButtonClicked();
            }

            
            if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && !isSetting)
            {

                switch (selectedLobbyIndex)
                {
                    case 0:
                        OnStartButtonClicked();
                        break;
                    case 1:
                        OnProfileButtonClicked();
                        break;
                    case 2:
                        OnMusicEditorButtonClicked();
                        break;
                    case 3:
                        OnCreditButtonClicked(); 
                        break;
                    case 4:
                        OnExitButtonClicked();
                        break;

                }
            }
        }
        public void OnUpAction()
        {
            AudioManager.GetOrCreate().PlayEffectSound(hoverSoundClip);
            if (selectedLobbyIndex > 0)
            {
                selectedLobbyIndex--;
                for (int i = 0; i < btnList.Length; i++)
                {
                    btnList[i].image.sprite = img_normal;
                }
                btnList[selectedLobbyIndex].image.sprite = img_hover;
            }
            
        }

        public void OnDownAction()
        {
            AudioManager.GetOrCreate().PlayEffectSound(hoverSoundClip);
            if (selectedLobbyIndex < 4)
            {
                selectedLobbyIndex++;
                for (int i = 0; i < btnList.Length; i++)
                {
                    btnList[i].image.sprite = img_normal;
                }
                btnList[selectedLobbyIndex].image.sprite = img_hover;
            }
        }

        public void Invalidate()
        {
            for (int i = 0; i < btnList.Length; i++)
            {
                btnList[i].image.sprite = img_normal;
            }
            btnList[selectedLobbyIndex].image.sprite = img_hover;
        }

        private void AddListeners()
        {
            btn_start.onClick.AddListener(OnStartButtonClicked);
            btn_profile.onClick.AddListener(OnProfileButtonClicked);
            btn_music_editor.onClick.AddListener(OnMusicEditorButtonClicked);
            btn_shop.onClick.AddListener(OnShopButtonClicked);
            btn_credit.onClick.AddListener(OnCreditButtonClicked);
            btn_exit.onClick.AddListener(OnExitButtonClicked);
            btn_setting.onClick.AddListener(OnSettingButtonClicked);
            btn_calibration.onClick.AddListener(OnCalibrationButtonClicked);
        }

        private void OnCalibrationButtonClicked()
        {
            Fade.Out(0.5f, () =>
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(SceneNameString._98_Calibration);
            });
        }


        private void OnStartButtonClicked()
        {
            Fade.Out(0.5f, () =>
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(SceneNameString._04_GameMode);
            });
        } 
        
        private void OnSettingButtonClicked()
        {
            isSetting = true;
            pnl_Option.SetActive(true);

        }

        public void ExitSettingButtonClicked()
        {
            isSetting = false;
            pnl_Option.SetActive(false);
            pnl_KeyBinder.SetActive(false);
        }


        private void OnExitButtonClicked()
        {
            Fade.Out(0.5f, () =>
            {
        #if UNITY_EDITOR
             UnityEditor.EditorApplication.isPlaying = false;
        #else
             Application.Quit(); // 어플리케이션 종료
        #endif  


            });
        }

        private void OnProfileButtonClicked()
        {
            Fade.Out(0.5f, () =>
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(SceneNameString._10_Profile);
            });
        }

        private void OnMusicEditorButtonClicked()
        {
            string editorPath = Path.Combine(Environment.CurrentDirectory, "SoundOfSlashEditor", "SoundOfSlashEditor.exe");
            Process.Start(editorPath);
        }

        private void OnShopButtonClicked()
        {
            Fade.Out(0.5f, () =>
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(SceneNameString._11_Shop);
            });
        }
        private void OnCreditButtonClicked()
        {
            Fade.Out(0.5f, () =>
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(SceneNameString._40_Credit);
            });
        }
        void OnApplicationFocus(bool hasFocus)
        {
            isPaused = !hasFocus;
        }

        void OnApplicationPause(bool pauseStatus)
        {
            isPaused = pauseStatus;
        }

        void OnGUI()
        {
            if (isPaused)
            {
                GUI.Label(new Rect(100, 100, 50, 30), "Game paused");
                AudioManager.GetOrCreate().PauseBGM();
            }
            else
            {
                AudioManager.GetOrCreate().ResumeBGM();
            }
        }
    }
}