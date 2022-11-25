using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Room Settings")]
    [SerializeField] private TMP_InputField _inputUsername;
    [SerializeField] private TMP_InputField _inputRoomName;
    [SerializeField] private Slider _sliderMaxPlayersSlider;
    [SerializeField] private TMP_Text _sliderMaxPlayersLabel;

    public string Username => _inputUsername.text;
    public string RoomName => _inputRoomName.text;
    public byte MaxPlayer => (byte)(_sliderMaxPlayersSlider.value + 1);

    /// <summary>
    /// Changes if Room Settings are Interactable.
    /// </summary>
    public void InteractableRoomSettings(bool interactable)
    {
        _inputUsername.interactable = interactable;
        _inputRoomName.interactable = interactable;
        _sliderMaxPlayersSlider.interactable = interactable;
        _sliderMaxPlayersLabel.gameObject.SetActive(interactable);
    }

    /// <summary>
    /// Updates the Max Players Label.
    /// </summary>
    public void UpdateMaxPlayersLabel()
    {
        _sliderMaxPlayersLabel.text = _sliderMaxPlayersSlider.value.ToString();
    }

    public string GetUsername()
    {
        return _inputUsername.text;
    }
}
