using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider effectSlider;

    void Start()
    {
        bgmSlider.value = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
        effectSlider.value = PlayerPrefs.GetFloat("EffectVolume", 0.5f);

        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        effectSlider.onValueChanged.AddListener(SetEffectVolume);
    }

    public void SetBGMVolume(float volume)
    {
        PlayerPrefs.SetFloat("BGMVolume", volume);
        AudioManager.GetOrCreate().SetBGMVolume(0.7f * PlayerPrefs.GetFloat("BGMVolume"));
    }

    public void SetEffectVolume(float volume)
    {
        PlayerPrefs.SetFloat("EffectVolume", volume);
        AudioManager.GetOrCreate().SetEffectVolume(PlayerPrefs.GetFloat("EffectVolume"));
    }
}