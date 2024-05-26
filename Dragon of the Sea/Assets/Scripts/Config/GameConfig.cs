using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameConfig : MonoBehaviour {
    public AudioMixer audioMixer;

    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown windowModeDropdown;
    public Slider BGMSlider;
    public Slider SFXSlider;

    public CanvasGroup pauseCanvas;

    Resolution[] resolutions;
    List<Resolution> filteredResoltions;

    double currentRefreshRate;

    private void Start() {
        UpdateSoundVolume();
        CreateResolutionList();
        SetResolutionValueInUI();
    }

    public void SetResolutionValueInUI() {
        if (PlayerPrefs.GetInt("Resolution", 0) >= filteredResoltions.Count)
            PlayerPrefs.SetInt("Resolution", filteredResoltions.Count - 1);
        
        resolutionDropdown.value = PlayerPrefs.GetInt("Resolution", 0);
        windowModeDropdown.value = PlayerPrefs.GetInt("Resolution", 0);        

        SetResolution(PlayerPrefs.GetInt("Resolution", 0));
    }

    public void CreateResolutionList() {
        resolutions = Screen.resolutions;
        filteredResoltions = new List<Resolution>();
        resolutionDropdown.ClearOptions();

        currentRefreshRate = Screen.currentResolution.refreshRateRatio.value;

        for (int i = 0; i < resolutions.Length; i++) {
            if (resolutions[i].refreshRateRatio.value == currentRefreshRate) {
                filteredResoltions.Add(resolutions[i]);
            }
        }

        List<string> options = new();
        foreach (var item in filteredResoltions) {
            string resolutionOption = $"Resolução: {item.width} x {item.height}";
            options.Add(resolutionOption);
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int id) {
        Resolution resolution = filteredResoltions[id];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
        PlayerPrefs.SetInt("Resolution", id);
    }

    public void ToggleFullScreen(int id) {
        var fullscreen = id == 0 ? true : false;
        Screen.fullScreen = fullscreen;
        PlayerPrefs.SetInt("FullScreen", id);
    }

    public void UpdateSoundVolume() {
        audioMixer.SetFloat("BGM", LinearToDecibel(PlayerPrefs.GetFloat("BGMVolume", 0.3f)));
        audioMixer.SetFloat("SFX", LinearToDecibel(PlayerPrefs.GetFloat("SFXVolume", 0.3f)));
        BGMSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("BGMVolume"));
        SFXSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("SFXVolume"));
    }

    public void SetBGMVolume(float value) {
        audioMixer.SetFloat("BGM", LinearToDecibel(value));
        PlayerPrefs.SetFloat("BGMVolume", value);
    }

    public void SetSFXVolume(float value) {
        audioMixer.SetFloat("SFX", LinearToDecibel(value));
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public void PauseGame() {
        Pause();
    }

    public void PauseGame(InputAction.CallbackContext context) {
        if (context.performed) {
            Pause();
        }        
    }

    void Pause() {
        if (Time.timeScale == 0) {
            Player.instance.EnablePlayerControls();
            Time.timeScale = 1;
            pauseCanvas.blocksRaycasts = false;
            pauseCanvas.alpha = 0;
            Player.instance.playlist.PlaySFX("Close Menu");
        } else {
            Player.instance.EnableUIControls();
            Time.timeScale = 0;
            pauseCanvas.blocksRaycasts = true;
            pauseCanvas.alpha = 1;
            Player.instance.playlist.PlaySFX("Open Menu");
        }
    }

    private float LinearToDecibel(float linear) {
        float dB;

        if (linear != 0)
            dB = 20.0f * Mathf.Log10(linear);
        else
            dB = -144.0f;

        return dB;
    }

    private float DecibelToLinear(float dB) {
        float linear = Mathf.Pow(10.0f, dB / 20.0f);

        return linear;
    }

}