using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] spawnPoints = null;
    [SerializeField] private GameObject[] enemies = null;
    [SerializeField] private GameObject boss = null;

    public void SpawnEnemy()
    {
        int spawnPicker = Random.Range(0, spawnPoints.Length);
        int enemyPicker = Random.Range(0, enemies.Length);

        Instantiate(enemies[enemyPicker], spawnPoints[spawnPicker].transform.position, quaternion.identity);
        Debug.Log("instantiate enemy ");
    }
    public void SpawnBoss()
    {
        int spawnPicker = Random.Range(0, spawnPoints.Length);
        Instantiate(boss, spawnPoints[spawnPicker].transform.position, quaternion.identity);

    }
}
