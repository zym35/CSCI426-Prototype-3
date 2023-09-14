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
    public List<Transform> piranhas;
    public Transform player;
    public float spawnAheadDist, destroyBehindDist;
    public Transform walls;

    private float _lastSpawnY;

    private void Start()
    {
        foreach (var p in piranhas)
        {
            if (p.position.y > _lastSpawnY)
                _lastSpawnY = p.position.y;
        }
    }

    private void Update()
    {
        walls.position = new Vector3(0, player.position.y, 0);
        
        if (player.position.y + spawnAheadDist > _lastSpawnY)
        {
            var pos = new Vector2(Random.Range(-piranhaSpawnRange, piranhaSpawnRange), _lastSpawnY + unitLength);
            var p = Instantiate(piranhaPrefab, pos, Quaternion.identity);
            piranhas.Add(p.transform);
            _lastSpawnY += unitLength;
        }

        foreach (var unit in piranhas)
        {
            if (unit == null)
                piranhas.Remove(unit);
            if (player.position.y - destroyBehindDist > unit.position.y)
            {
                Destroy(unit.gameObject);  
            }
        }
    }
}
