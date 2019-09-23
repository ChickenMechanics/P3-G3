using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Configuration;
using UnityEngine;
using Random = UnityEngine.Random;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class WaveSpawner : MonoBehaviour
{
    public enum SpawnState { SPAWNING, WAITING, COUNTING }

    [System.Serializable]
    public class Wave
    {
        public string name;
        public Set[] sets;
    }

    [System.Serializable]
    public class Set
    {
        public string name;
        public Transform enemy;
        public int count;
        public float rate;
    }

    private Transform player;

    public Wave[] waves;
    private int nextWave = 0;

    public Set[] sets;

    public Transform[] spawnPoints;
    public float safeSpawnDistance;

    public float timeBetweenWaves;
    private float waveCountdown;

    private float searchCountdown = 1f;

    private SpawnState state = SpawnState.COUNTING;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0);
        waveCountdown = timeBetweenWaves;
    }

    void Update()
    {
        if (state == SpawnState.WAITING)
        {
            if (EnemyIsAlive())
                return;

            WaveCompleted();
        }

        if (waveCountdown <= 0 && state != SpawnState.SPAWNING)
            StartCoroutine(SpawnWave(waves[nextWave]));
        else
            waveCountdown -= Time.deltaTime;
    }

    void WaveCompleted()
    {
        Debug.Log("Wave Completed");

        state = SpawnState.COUNTING;
        waveCountdown = timeBetweenWaves;

        if (nextWave >= waves.Length - 1)
        {
            nextWave = 0;
            Debug.Log("ALL WAVES COMPLETE! Looping...");
        }
        else
            nextWave++;
    }

    bool EnemyIsAlive()
    {
        searchCountdown -= Time.deltaTime;
        
        if (searchCountdown > 0f) return true;

        searchCountdown = 1f;

        return GameObject.FindGameObjectWithTag("Enemy") != null;
    }

    IEnumerator SpawnWave(Wave wave)
    {
        Debug.Log("Spawning Wave: " + wave.name);

        state = SpawnState.SPAWNING;

        foreach (var set in wave.sets)
        {
            Debug.Log("Spawning Set: " + set.name);

            for (var j = 0; j < set.count; ++j)
            {
                SpawnEnemy(set.enemy);
                yield return new WaitForSeconds(1f / set.rate);
            }
        }

        state = SpawnState.WAITING;
    }

    void SpawnEnemy(Transform enemy)
    {
        Debug.Log("Spawning Enemy: " + enemy.name);

        if (spawnPoints.Length == 0)
            Debug.LogError("No spawn points referenced");

        var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        var playerPos = player.position;

        var distanceToPlayer = playerPos.magnitude - spawnPoint.position.magnitude;

        while (Math.Abs(distanceToPlayer) < safeSpawnDistance)
        {
            spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            playerPos = player.position;
            distanceToPlayer = playerPos.magnitude - spawnPoint.position.magnitude;
        }

        Instantiate(enemy, spawnPoint.position, spawnPoint.rotation);
    }

}
