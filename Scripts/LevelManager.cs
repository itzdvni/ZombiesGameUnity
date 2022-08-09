using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    [SerializeField] private GameObject _loaderCanvas;
    [SerializeField] private Image _progressBar;
    private float target;

    private void Awake()
    {
        /*if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }*/
    }

    public async void LoadScene(string sceneName)
    {
        target = 0;
        _progressBar.fillAmount = 0;
        var scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;
        _loaderCanvas.SetActive(true);

        do {
            await Task.Delay(100);
            target = scene.progress;
        } while (scene.progress < 0.9f);

        await Task.Delay(3000);
        scene.allowSceneActivation = true;
        //_loaderCanvas.SetActive(false);

    }

    public async void LoadSceneDie(string sceneName)
    {
        //target = 0;
        //_progressBar.fillAmount = 0;
        var scene = SceneManager.LoadSceneAsync(sceneName);
        //scene.allowSceneActivation = false;
       // _loaderCanvas.SetActive(true);

       /* do {
            await Task.Delay(100);
            target = scene.progress;
        } while (scene.progress < 0.9f);*/

       // await Task.Delay(3000);
        scene.allowSceneActivation = true;
        //_loaderCanvas.SetActive(false);
    }

    public void loadscenenormal()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void Update()
    {
        _progressBar.fillAmount = Mathf.MoveTowards(_progressBar.fillAmount, target,3 * Time.deltaTime);
    }
}