using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Username : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _username;
    [SerializeField] private Transform _target;
    [SerializeField] private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    public void SetUsername(string username)
    {
        _username.text  = username;
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    private void Update()
    {
        if (_target != null)
        {
            var usernamePosition  = _camera.WorldToScreenPoint(_target.position + new Vector3(0,2,0));
            transform.position = usernamePosition;
        }
    }
}
