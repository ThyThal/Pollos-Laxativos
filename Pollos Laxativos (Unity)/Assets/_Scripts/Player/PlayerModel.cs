using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerModel : MonoBehaviourPun
{
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private bool _isAlive = true;
    [SerializeField] private float _walkSpeed = 5f;
    [SerializeField] private float _rotateSpeed = 3f;
    [SerializeField] private float _attackCooldown = 0.4f;
    [SerializeField] private float _dashCooldown = 1f;

    private float _originalAttackCooldown;
    private float _originalDashCooldown;

    public Action<Player> loseAction = delegate { };
    public Action<Player> winAction = delegate { };

    public bool IsAlive => _isAlive;
    public bool CanAttack => _attackCooldown < 0;
    public bool CanDash => _dashCooldown < 0;

    private void Awake()
    {
        if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _originalAttackCooldown = _attackCooldown;
        _originalDashCooldown = _dashCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) return;

        DashTimer();
        AttackTimer();
    }

    [PunRPC]
    public void Recoil(Vector2 recoil)
    {
        _rigidbody.AddForce(recoil, ForceMode2D.Impulse);
    }

    /// <summary>
    /// Moves Player with Rigidbody Force.
    /// </summary>
    public void Move(Vector2 direction)
    {
        // Move Rigidbody with Constant Force.
        _rigidbody.AddForce(direction * _walkSpeed, ForceMode2D.Force);
    }

    /// <summary>
    /// Impulses Player Forwards.
    /// </summary>
    public void Dash()
    {
        // Do Dash with Impulse.
        _rigidbody.AddForce(transform.right * _walkSpeed, ForceMode2D.Impulse);

        // Reset Timer
        _dashCooldown = _originalDashCooldown;
    }

    /// <summary>
    /// Rotates Player Transform with Lerp.
    /// </summary>
    public void Rotate(Vector2 dir)
    {
        transform.right = Vector2.Lerp(transform.right, dir, _rotateSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Instantiates a Projectile with its Owner.
    /// </summary>
    public void Attack(PhotonView owner)
    {
        GameManager.Instance.LevelManager.SpawnProjectile(owner);
        _attackCooldown = _originalAttackCooldown;
    }

    [PunRPC]
    public void Die()
    {
        _isAlive = false;
        GameObject username = GetComponent<PlayerView>().Username.gameObject;

        if (photonView.IsMine)
            PhotonNetwork.Destroy(this.gameObject);
    }

    private void AttackTimer()
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
