using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class ItemObjectSpawner : MonoBehaviour
{
    public GameObject ScoreItemPrefab;
    private List<GameObject> _itemPool = null;
    public int PoolSize = 50;

    private float _timer = 0;
    public float SpawnTime = 10;
    public int SpawnCount = 10;
    public int SpawnRange = 20;

    private void Awake()
    {
        _itemPool = new List<GameObject>();
        for (int i = 0; i < PoolSize; i++)
        {
            GameObject item = Instantiate(ScoreItemPrefab);
            item.transform.SetParent(this.transform);
            item.gameObject.SetActive(false);
            _itemPool.Add(item);
        }
    }
    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            _timer += Time.deltaTime;
            if (_timer > SpawnTime)
            {
                ItemSpawn();
                _timer = 0;
            }
        }       
    }

    private GameObject GetAvailableItem()
    {
        foreach (GameObject item in _itemPool)
        {
            if (!item.activeSelf)
            {
                return item;
            }
        }
        return null;
    }

    [PunRPC]
    private void ItemSpawn()
    {
        GameObject availableItem = GetAvailableItem();
        if (availableItem != null)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector3 spawnPosition = transform.position + new Vector3(Random.Range(-SpawnRange, SpawnRange), 0, Random.Range(-SpawnRange, SpawnRange));
                availableItem.transform.position = spawnPosition;
                availableItem.SetActive(true);
            }
        }
        Debug.Log("아이템 생성");
    }
}