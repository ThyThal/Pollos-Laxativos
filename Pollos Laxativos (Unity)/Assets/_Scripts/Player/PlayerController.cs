using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviourPun
{
    [SerializeField] private KeyCode _dashKey = KeyCode.LeftShift;
    [SerializeField] private PlayerModel _playerModel;
    [SerializeField] private Animator _animator;

    private void Awake()
    {
        // Destroy Component if not Owner.
        if (!photonView.IsMine) Destroy(this);

        // Get Player Model.
        if (_playerModel == null) _playerModel = GetComponent<PlayerModel>();
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        // Don't Update Input if Game is not Started.
        if (!GameManager.Instance.LevelManager.GameStarted) return;

        // Create Movement Vector2
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 direction = new Vector2(h, v).normalized;

        // Call Move and Rotate from Model.
        if (direction != Vector2.zero)
        {
            _playerModel.Move(direction);
            _playerModel.Rotate(direction);
        }


        //_animator.SetFloat("Velocity", _rigidbody.velocity.magnitude);
    }

    private void Update()
    {
        // Don't Update Input if Game is not Started.
        if (!GameManager.Instance.LevelManager.GameStarted) return;

        if (_playerModel.CanAttack) _playerModel.Attack(photonView);
        if (_playerModel.CanDash && Input.GetKeyDown(_dashKey)) _playerModel.Dash();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Arena"))
        {
            _playerModel.loseAction(photonView.Owner);

            if (photonView.IsMine)
            {
                _playerModel.photonView.RPC("Die", RpcTarget.AllBuffered);
            }

            else
            {
                _playerModel.photonView.RPC("Die", photonView.Owner);
            }
        }
    }

    private void OnPlayerDisconnected(Player player)
    {
        Debug.LogError($"Disconnected {player.NickName}");
        _playerModel.photonView.RPC("Die", RpcTarget.AllBuffered);
    }
}
