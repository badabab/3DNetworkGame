using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class ItemObjectFactory : MonoBehaviourPun
{
    public static ItemObjectFactory Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public ItemObject MasterCreate(ItemType type, Vector3 position)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("[MasterCreate] 마스터 클라이언트만 호출할 수 있습니다.");
            return null;
        }
        return Create(type, position);
    }

    public void RequestCreate(ItemType type, Vector3 position)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Create(type, position);
        }
        else
        {
            photonView.RPC(nameof(Create), RpcTarget.MasterClient, type, position);
        }
    }

    [PunRPC]
    private ItemObject Create(ItemType type, Vector3 position)
    {
        Vector3 dropPos = position + new Vector3(0, 0.5f, 0) + UnityEngine.Random.insideUnitSphere;
        GameObject gameObject = PhotonNetwork.InstantiateRoomObject(type.ToString(), dropPos, Quaternion.identity);
        return gameObject.GetComponent<ItemObject>();
    }

    public void RequestDelete(int viewID)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Delete(viewID);
        }
        else
        {
            photonView.RPC(nameof(Delete), RpcTarget.MasterClient, viewID);
        }
    }

    [PunRPC]
    private void Delete(int viewID)
    {
        GameObject objectToDelete = PhotonView.Find(viewID)?.gameObject;
        if (objectToDelete != null)
        {
            PhotonNetwork.Destroy(objectToDelete);
        }
    }
}