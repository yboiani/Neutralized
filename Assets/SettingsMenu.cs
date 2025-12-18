using StarterAssets;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Slider sensitivitySlider;
    public Slider volumeSlider;

    public ThirdPersonController player;

    void Start()
    {
        // Set default slider values
        sensitivitySlider.value = player.mouseSensitivity;
        volumeSlider.value = AudioListener.volume;

        // Add listeners
        sensitivitySlider.onValueChanged.AddListener(UpdateSensitivity);
        volumeSlider.onValueChanged.AddListener(UpdateVolume);
    }

    public void UpdateSensitivity(float newValue)
    {
        player.mouseSensitivity = newValue;
    }

    public void UpdateVolume(float newValue)
    {
        AudioListener.volume = newValue;
    }
}
