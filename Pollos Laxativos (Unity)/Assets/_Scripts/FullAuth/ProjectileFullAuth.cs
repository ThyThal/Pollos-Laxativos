using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ProjectileFullAuth : MonoBehaviourPun
{
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _force = 10f;
    [SerializeField] private PlayerModel _owner;
    bool _isDestroy;

    float time = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //Destroy(this.gameObject, 3f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.right * _speed * Time.deltaTime;
        transform.localScale += transform.localScale * 1.1f * Time.deltaTime;

        time += Time.deltaTime;

        if (time > 5f)
        {
            if (photonView.IsMine)
            {
                Debug.LogWarning("Extra Projectile Destroyed.");
                PhotonNetwork.Destroy(this.gameObject);
            }
        }
    }

    public void Initialize(PlayerModel owner)
    {
        SetOwner(owner);
    }

    public void SetOwner(PlayerModel owner)
    {
        _owner = owner;
    }

    /// <summary>
    /// Checks Collision with Player.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!photonView.IsMine || _isDestroy || _owner == null) return;

        PhotonView player = collision.gameObject?.GetComponent<PhotonView>();
        if (player != null && player != _owner)
        {
            PlayerModel playerModel = player.gameObject?.GetComponent<PlayerModel>();

            if (playerModel != null)
            {
                if (playerModel.IsAlive)
                {
                    _isDestroy = true;
                    Vector2 recoil = transform.right * _force;
                    playerModel.photonView.RPC("Recoil", RpcTarget.OthersBuffered, recoil);
                    PhotonNetwork.Destroy(this.gameObject);
                }
            }
        }
    }

    /// <summary>
    /// Checks Collision with Arena.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!photonView.IsMine || _isDestroy || _owner == null) return;

        if (collision.gameObject.CompareTag("Arena"))
        {
            if (photonView.AmOwner)
            {
                PhotonNetwork.Destroy(this.gameObject);
                _isDestroy = true;
            }
        }
    }


}
