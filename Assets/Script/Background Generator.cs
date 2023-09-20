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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player.position.y + spawnAheadDist > _lastSpawnY)
        {
            var pos = new Vector2(0, _lastSpawnY + unitLength);
            Instantiate(backgroundPrefab, pos, Quaternion.identity);
            _lastSpawnY += unitLength;
        }
    }
}
