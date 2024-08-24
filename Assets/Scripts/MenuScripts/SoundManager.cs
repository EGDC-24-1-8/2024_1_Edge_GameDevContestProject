using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider effectSlider;

    void Start()
    {
        bgmSlider.value = PlayerPrefs.GetFloat("BGMVolume", 1.0f);
        effectSlider.value = PlayerPrefs.GetFloat("EffectVolume", 1.0f);
        
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        effectSlider.onValueChanged.AddListener(SetEffectVolume);
    }

    public void SetBGMVolume(float volume)
    {
        PlayerPrefs.SetFloat("BGMVolume", volume);
    }

    public void SetEffectVolume(float volume)
    {
        PlayerPrefs.SetFloat("EffectVolume", volume);
    }
}