using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerBaru : MonoBehaviour
{
    public GameObject prefab;
    public float spawnInterval = 1;

    float time = 0;

    void Update()
    {
        time += Time.deltaTime;
        if (time >= spawnInterval) {
            time -= spawnInterval;

            if (prefab != null) {
                Instantiate(prefab, transform.position, transform.rotation);
            }
        }
    }
}