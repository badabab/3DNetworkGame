using System.Collections.Generic;
using UnityEngine;

public class BattleScene : MonoBehaviour
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
}