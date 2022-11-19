using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ProjectileController : MonoBehaviourPun
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
        if (!photonView.IsMine) return;
        transform.position += transform.right * _speed * Time.deltaTime;
        //transform.localScale += transform.localScale * 1.1f * Time.deltaTime;

        time += Time.deltaTime;

        if (time > 5f)
        {

            Debug.LogWarning("Extra Projectile Destroyed.");
            _isDestroy = true;
            PhotonNetwork.Destroy(this.gameObject);
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

        PlayerModel playerModel = collision.gameObject.GetComponent<PlayerModel>();
        if (playerModel != null && playerModel != _owner)
        {
            if (playerModel != null)
            {
                if (playerModel.IsAlive)
                {
                    _isDestroy = true;
                    Vector2 recoil = transform.right * _force;
                    playerModel.photonView.RPC("Recoil", playerModel.photonView.Owner, recoil);
                    PhotonNetwork.Destroy(this.gameObject);
                }
            }
        }

        // Return if not Master.
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
            PhotonNetwork.Destroy(this.gameObject);
            _isDestroy = true;
        }
    }


}
