using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    public GameObject piranhaPrefab;
    public float unitLength = 8;
    public float piranhaSpawnRange;
    public Transform startPiranha;
    public Transform player;
    public float spawnAheadDist;
    public Transform walls;

    private float _lastSpawnY;

    private void Start()
    {
        _lastSpawnY = startPiranha.position.y;
    }

    private void Update()
    {
        walls.position = new Vector3(0, player.position.y, 0);
        
        if (player.position.y + spawnAheadDist > _lastSpawnY)
        {
            var pos = new Vector2(Random.Range(-piranhaSpawnRange, piranhaSpawnRange), _lastSpawnY + unitLength);
            Instantiate(piranhaPrefab, pos, Quaternion.identity);
            _lastSpawnY += unitLength;
        }
    }
}
