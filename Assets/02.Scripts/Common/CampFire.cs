using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CampFire : MonoBehaviour
{
    public int Damage = 20;
    public float Cooltime = 1f;
    private float _timer = 0f;

    private IDamaged _target = null;

    private void OnTriggerEnter(Collider col)
    {
        IDamaged damagedObject = col.GetComponent<IDamaged>();
        if (damagedObject == null)
        {
            return;
        }

        PhotonView photonView = col.GetComponent<PhotonView>();
        if (photonView == null || !photonView.IsMine)
        {
            return;
        }

        _target = damagedObject;
    }

    private void OnTriggerStay(Collider col)
    {
        if (_target == null || !col.CompareTag("Player"))
        {
            return;
        }

        _timer += Time.deltaTime;
        if (_timer >= Cooltime)
        {
            _target.Damaged(Damage, -1);
            _timer = 0f;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        IDamaged damagedObject = col.GetComponent<IDamaged>();
        if (damagedObject == null)
        {
            return;
        }

        if (damagedObject == _target)
        {
            _timer = 0f;
            _target = null;
        }
    }
}