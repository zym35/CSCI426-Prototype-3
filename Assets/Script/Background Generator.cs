using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BackgroundGenerator : MonoBehaviour
{
    public GameObject backgroundPrefab;
    public Transform player;
    public float unitLength = 20;
    public float spawnAheadDist;
    float _lastSpawnY = 0;

    void Update()
    {
        if (player.position.y + spawnAheadDist > _lastSpawnY)
        {
            var pos = new Vector3(0, _lastSpawnY + unitLength, .5f);
            Instantiate(backgroundPrefab, pos, Quaternion.identity);
            _lastSpawnY += unitLength;
        }
    }
}
