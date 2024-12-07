using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Slider volumeSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider dialogueSlider;

    void Start()
    {
        // Load saved settings or set defaults
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 0.5f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        dialogueSlider.value = PlayerPrefs.GetFloat("DialogueVolume", 0.5f);
    }

    public void OnVolumeSliderChanged()
    {
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
        Debug.Log("Master Volume: " + volumeSlider.value);
    }
    public void OnMusicSliderChanged()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        Debug.Log("Music Volume: " + musicSlider.value);
    }

    public void OnSFXSliderChanged()
    {
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
        Debug.Log("SFX Volume: " + sfxSlider.value);
    }

    public void OnDialogueSliderChanged()
    {
        PlayerPrefs.SetFloat("DialogueVolume", dialogueSlider.value);
        Debug.Log("Dialogue Volume: " + dialogueSlider.value);
    }
}
