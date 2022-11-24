using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Timer : MonoBehaviour
{
    [SerializeField] private TMP_Text _timer;
    private float _currentTime;

    // Update is called once per frame
    void Update()
    {
        if (_timer == null) return;
        _currentTime = GameManager.Instance.LevelManager.currentTime;
        var roundedTime = Mathf.RoundToInt(_currentTime);
        _timer.text = $"Time: {roundedTime} s";
    }
}
