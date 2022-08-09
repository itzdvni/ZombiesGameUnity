using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


public class GameManager : MonoBehaviour
{
    [SerializeField] private int[] enemiesSpawnWaves;
    [SerializeField] private EnemySpawner enemySpawner = null;
    private List<Enemy> enemigos = null;
    private int totalEnemies = 0;
    private int totalEnemiesToSpawn = 8;

    [FormerlySerializedAs("Score")] [SerializeField] public int score = 0;
    [SerializeField] private float timer = 5f;

    
    [SerializeField] private TextMeshProUGUI enemiesLeftText = null;
    [SerializeField] private TextMeshProUGUI waveText = null;
    [SerializeField] private TextMeshProUGUI counterTimeText = null;
    [SerializeField] private TextMeshProUGUI scoreText = null;
    [SerializeField] private bool isPaused;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject playerController;// OBJETO O COMPONENTE QUE MUEVE LA CAMARA
    [SerializeField] private GameObject gunController;// OBJETO O COMPONENTE QUE MUEVE LA CAMARA
    [SerializeField] private GameObject gunCanvas;
    [SerializeField] private TextMeshProUGUI diescoreText = null;
    [SerializeField] private GameObject BossObject;
    [SerializeField] public bool gamestarted;


    private bool cycle = false;

    private int currentWave = 0;

    // Update is called once per frame

    private void Awake()
    {
        counterTimeText.enabled = false;
        enemiesLeftText.enabled = true;
        waveText.enabled = true;
        isPaused = false;
        pauseMenu.SetActive(false);
        gamestarted = false;
    }

    void Update()
    {
        countEnemies();
        counterTimeText.text = Mathf.RoundToInt((timer -= Time.deltaTime)).ToString();
        scoreText.text = "Score: " + score.ToString();

        if (totalEnemies <= 0 && cycle && currentWave<6)
        {
            timer = 5f;
            StartCoroutine(StartNewWave(timer));
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                isPaused = true;
                Time.timeScale = 0f;
                pauseMenu.SetActive(true);
                playerController.SetActive(false);
                gunController.SetActive(false);
                gunCanvas.SetActive(false);
            }
            else
            {
                isPaused = false;
                Time.timeScale = 1f;
                pauseMenu.SetActive(false);
                playerController.SetActive(true);
                gunController.SetActive(true);
                gunCanvas.SetActive(true);
            }
        }
    }

    public void setCycleTrue()
    {
        cycle = true;
    }

    public void UpdateScore(int ammount)
    {
        score += ammount;
        
        diescoreText.text = "SCORE: " + score.ToString();

    }
    public void newWave()
    {
        currentWave += 1;
        updateWave(currentWave);
        //totalEnemiesToSpawn += (totalEnemiesToSpawn / 4)+Random.Range(0,(totalEnemiesToSpawn/10));
        //totalEnemiesToSpawn = enemiesSpawnWaves[currentWave];

        bool isBoss = false;
        switch (currentWave)
        {
            case 1:
                totalEnemiesToSpawn = enemiesSpawnWaves[0];
                break;
            case 2:
                totalEnemiesToSpawn = enemiesSpawnWaves[1];
                break;
            case 3:
                totalEnemiesToSpawn = enemiesSpawnWaves[2];
                break;
            case 4:
                totalEnemiesToSpawn = enemiesSpawnWaves[3];
                break;
            case 5:
                //totalEnemiesToSpawn = enemiesSpawnWaves[1];
                //enemySpawner.SpawnBoss();
                BossObject.SetActive(true); 
                isBoss = true;
                break;
        }


        if (!isBoss)
        {
            for (int i = 0; i <= totalEnemiesToSpawn; i++)
            {
                StartCoroutine(SpawnEnemiesWithDelay(Random.Range(0, 6f)));
            }
        }


    }

    IEnumerator StartNewWave(float delay)
    {
        cycle = false;
        timer = 5f;
        counterTimeText.text = Mathf.RoundToInt(timer).ToString();
        counterTimeText.enabled = true;

        yield return new WaitForSeconds(delay);
        newWave();
        counterTimeText.enabled = false;
        gamestarted = true;
    }

    public void countEnemies()
    {

        var gos = GameObject.FindGameObjectsWithTag("Enemy");
        totalEnemies = gos.Length;
        updateEnemies(totalEnemies);
    }

    IEnumerator SpawnEnemiesWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        enemySpawner.SpawnEnemy();
        countEnemies();
        cycle = true;
    }

    void updateEnemies(int ammount)
    {
        enemiesLeftText.text = "Undead Left : " + ammount.ToString();
    }

    void updateWave(int wave)
    {
        waveText.text = "Wave: " + wave.ToString();
    }
}
