using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class BattleScene : MonoBehaviourPunCallbacks
{
    public static BattleScene Instance {  get; private set; }
    public List<Transform> SpawnPoints;

    private void Awake()
    {
        Instance = this;
    }

    public Vector3 GetRandomSpawnPoint()
    {
        int randomIndex = UnityEngine.Random.Range(0,SpawnPoints.Count);
        return SpawnPoints[randomIndex].position;
    }

    public override void OnJoinedRoom()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        GameObject[] points = GameObject.FindGameObjectsWithTag("BearSpawnPoint");
        foreach (GameObject p in points)
        {
            PhotonNetwork.InstantiateRoomObject("Bear", p.transform.position, Quaternion.identity);
        }
    }
}