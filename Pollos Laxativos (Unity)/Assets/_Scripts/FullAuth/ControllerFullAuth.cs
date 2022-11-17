using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ControllerFullAuth : MonoBehaviour
{
    [SerializeField] private float _attackCooldown = 0.4f;
    [SerializeField] private float _dashCooldown = 1f;

    private float _originalAttackCooldown;
    private float _originalDashCooldown;

    [SerializeField] private KeyCode _dashKey = KeyCode.LeftShift;
    public bool CanAttack => _attackCooldown < 0;
    public bool CanDash => _dashCooldown < 0;
    void Start()
    {
        MasterManager.Instance.RPCMaster("RequestConnectPlayer", PhotonNetwork.LocalPlayer);
    }

    // Update is called once per frame
    void Update()
    {
        if (CanAttack) MasterManager.Instance.RPCMaster("RequestAttack", PhotonNetwork.LocalPlayer);
        //_playerModel.Attack(photonView);
        if (CanDash && Input.GetKeyDown(_dashKey)) MasterManager.Instance.RPCMaster("RequestDash", PhotonNetwork.LocalPlayer);

        DashTimer();
        AttackTimer();
    }

    private void FixedUpdate()
    {
        if (!GameManagerFullAuth.Instance.LevelManager.GameStarted) return;

        // Create Movement Vector2
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 direction = new Vector2(h, v).normalized;

        // Call Move and Rotate from Model.
        if (direction != Vector2.zero)
        {
            MasterManager.Instance.RPCMaster("RequestMove", PhotonNetwork.LocalPlayer, direction);

            //_playerModel.Move(direction);
            //_playerModel.Rotate(direction);
        }
    }

    private void AttackTimer() // Puse los cooldowns acá 
    {
        if (_attackCooldown > 0)
        {
            _attackCooldown -= Time.deltaTime;
        }
    }
    private void DashTimer()
    {
        if (_dashCooldown > 0)
        {
            _dashCooldown -= Time.deltaTime;
        }
    }
}
