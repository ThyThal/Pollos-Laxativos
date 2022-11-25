using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _attackCooldown = 0.4f;
    [SerializeField] private float _dashCooldown = 1f;

    //private float _originalAttackCooldown;
    //private float _originalDashCooldown;

    [SerializeField] private KeyCode _dashKey = KeyCode.LeftShift;
    public bool GameStarted => GameManager.Instance.LevelManager.GameStarted;
    public bool CanAttack => GameStarted && _attackCooldown < 0;
    public bool CanDash => GameStarted && _dashCooldown < 0;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //Destroy(Camera.main.gameObject);
            Destroy(this);
        }
    }
    void Update()
    {
        // Attack
        //if (CanAttack)
        //{
        //if (Input.GetKeyDown(KeyCode.A))
        //    MasterManager.Instance.RPCMaster("RequestAttack", PhotonNetwork.LocalPlayer);
        //}

        // Dash
        //if (CanDash && Input.GetKeyDown(_dashKey)) 
        //{
        if (Input.GetKeyDown(_dashKey))
            MasterManager.Instance.RPCMaster("RequestDash", PhotonNetwork.LocalPlayer);
        //}
    }

    private void FixedUpdate()
    {
        if (!GameManager.Instance.LevelManager.GameStarted) return;

        // Create Movement Vector2
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 direction = new Vector2(h, v).normalized;

        // Call Move and Rotate from Model.
        if (direction != Vector2.zero)
        {
            MasterManager.Instance.RPCMaster("RequestMove", PhotonNetwork.LocalPlayer, direction);
        }
    }
}
