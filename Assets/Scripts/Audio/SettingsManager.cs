using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider ambientSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("UI Settings")]
    [SerializeField] private GameObject settingsPanel;

    private Animator animator;
    private bool isPanelOpen = false;

    private void Awake()
    {
        if (settingsPanel == null)
        {
            Debug.LogError("Settings panel not assigned.");
            return;
        }

        animator = settingsPanel.GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator not found on settings panel.");
            return;
        }

        animator.enabled = false;
        LoadVolume();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ToggleSettingsPanel();
        }
    }

    public void SetMusicVolume() => SetVolume("Music", musicSlider, "musicVolume");
    public void SetAmbientVolume() => SetVolume("Ambient", ambientSlider, "ambientVolume");
    public void SetSFXVolume() => SetVolume("SFX", sfxSlider, "sfxVolume");

    private void SetVolume(string exposedParam, Slider slider, string playerPrefKey)
    {
        if (audioMixer == null || slider == null) return;

        float value = Mathf.Clamp(slider.value, 0.0001f, 1f);
        audioMixer.SetFloat(exposedParam, Mathf.Log10(value) * 20f);
        PlayerPrefs.SetFloat(playerPrefKey, value);
    }

    public void LoadVolume()
    {
        float musicValue = PlayerPrefs.GetFloat("musicVolume", 0.5f);
        musicSlider.value = musicValue;
        SetMusicVolume();

        float ambientValue = PlayerPrefs.GetFloat("ambientVolume", 0.5f);
        ambientSlider.value = ambientValue;
        SetAmbientVolume();

        float sfxValue = PlayerPrefs.GetFloat("sfxVolume", 0.5f);
        sfxSlider.value = sfxValue;
        SetSFXVolume();
    }

    private void SetSliderValue(Slider slider, string key, string exposedParam)
    {
        if (audioMixer == null || slider == null) return;

        float value = PlayerPrefs.GetFloat(key, .5f);
        slider.value = value;
        audioMixer.SetFloat(exposedParam, Mathf.Log10(value) * 20f);
    }

    public void ToggleSettingsPanel()
    {
        if (isPanelOpen)
            CloseSettingsPanel();
        else
            OpenSettingsPanel();
    }

    public void OpenSettingsPanel()
    {
        if (settingsPanel == null || animator == null) return;

        settingsPanel.SetActive(true);
        animator.enabled = true;
        animator.Play("Open_setting_UI_anim");
        isPanelOpen = true;
    }

    public void CloseSettingsPanel()
    {
        if (animator == null) return;

        animator.Play("Close_Setting_ui_anim");
        StartCoroutine(DeactivatePanelAfterAnimation());
    }

    private IEnumerator DeactivatePanelAfterAnimation()
    {
        yield return null; // ensure animator state updates first

        float animLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSecondsRealtime(animLength);

        settingsPanel.SetActive(false);
        animator.enabled = false;
        isPanelOpen = false;
    }

    public void Resume()
    {
        CloseSettingsPanel();
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
