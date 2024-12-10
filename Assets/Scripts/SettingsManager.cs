using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Slider volumeSlider;   // Master volume slider
    public Slider musicSlider;    // Music volume slider
    public Slider sfxSlider;      // SFX volume slider
    public Slider dialogueSlider; // Dialogue volume slider

    public AudioSource musicAudioSource;    // Reference to the music AudioSource
    public AudioSource windAudioSource;     // Reference to the SFX AudioSource (e.g., wind sound)
    public AudioSource dialogueAudioSource; // Reference to the dialogue AudioSource

    void Start()
    {
        Debug.Log("Master Volume Loaded: " + PlayerPrefs.GetFloat("Volume", 0.5f));
        Debug.Log("Music Volume Loaded: " + PlayerPrefs.GetFloat("MusicVolume", 0.5f));
        Debug.Log("SFX Volume Loaded: " + PlayerPrefs.GetFloat("SFXVolume", 0.5f));
        Debug.Log("Dialogue Volume Loaded: " + PlayerPrefs.GetFloat("DialogueVolume", 0.5f));

        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 0.5f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        dialogueSlider.value = PlayerPrefs.GetFloat("DialogueVolume", 0.5f);

        // Apply the loaded settings
        UpdateAudioSources();
    }


    public void OnVolumeSliderChanged()
    {
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);

        // Apply the master volume to all audio sources
        UpdateAudioSources();

        Debug.Log("Master Volume: " + volumeSlider.value);
    }

    public void OnMusicSliderChanged()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);

        // Apply the music volume
        if (musicAudioSource != null)
        {
            musicAudioSource.volume = musicSlider.value * volumeSlider.value;
        }

        Debug.Log("Music Volume: " + musicSlider.value);
    }

    public void OnSFXSliderChanged()
    {
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);

        // Apply the SFX volume
        if (windAudioSource != null)
        {
            windAudioSource.volume = sfxSlider.value * volumeSlider.value;
        }

        Debug.Log("SFX Volume: " + sfxSlider.value);
    }

    public void OnDialogueSliderChanged()
    {
        PlayerPrefs.SetFloat("DialogueVolume", dialogueSlider.value);

        // Apply the dialogue volume
        if (dialogueAudioSource != null)
        {
            dialogueAudioSource.volume = dialogueSlider.value * volumeSlider.value;
        }

        Debug.Log("Dialogue Volume: " + dialogueSlider.value);
    }

    public void ResetSettings()
    {
        PlayerPrefs.SetFloat("Volume", 0.5f);
        PlayerPrefs.SetFloat("MusicVolume", 0.5f);
        PlayerPrefs.SetFloat("SFXVolume", 0.5f);
        PlayerPrefs.SetFloat("DialogueVolume", 0.5f);

        volumeSlider.value = 0.5f;
        musicSlider.value = 0.5f;
        sfxSlider.value = 0.5f;
        dialogueSlider.value = 0.5f;

        // Apply the reset settings
        UpdateAudioSources();

        Debug.Log("Settings reset to default values.");
    }

    private void UpdateAudioSources()
    {
        float masterVolume = volumeSlider.value;

        // Apply master volume combined with individual volumes
        if (musicAudioSource != null)
        {
            musicAudioSource.volume = musicSlider.value * masterVolume;
        }

        if (windAudioSource != null)
        {
            windAudioSource.volume = sfxSlider.value * masterVolume;
        }

        if (dialogueAudioSource != null)
        {
            dialogueAudioSource.volume = dialogueSlider.value * masterVolume;
        }
    }
}
