using Photon.Pun;
using UnityEngine;

public class ItemSpawner : MonoBehaviourPun
{
    public GameObject[] ScoreItemPrefabs;
    public float SpawnTime = 5f;
    public int SpawnRange = 10;

    private float _timer = 0;

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            _timer += Time.deltaTime;
            if (_timer >= SpawnTime)
            {
                GameObject randomItem = ScoreItemPrefabs[Random.Range(0, ScoreItemPrefabs.Length)];
                Vector3 spawnPosition = transform.position + new Vector3(Random.Range(-SpawnRange, SpawnRange), 0, Random.Range(-SpawnRange, SpawnRange));
                photonView.RPC(nameof(SpawnItem), RpcTarget.MasterClient, spawnPosition, randomItem.name);
                _timer = 0;
            }
        }
    }

    [PunRPC]
    private void SpawnItem(Vector3 spawnPosition, string prefabName)
    {
        GameObject prefabToSpawn = null;
        foreach (GameObject item in ScoreItemPrefabs)
        {
            if (item.name == prefabName)
            {
                prefabToSpawn = item;
                break;
            }
        }
        if (prefabToSpawn != null)
        {
            Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
            Debug.Log("아이템 생성: " + prefabName);
        }
    }
}