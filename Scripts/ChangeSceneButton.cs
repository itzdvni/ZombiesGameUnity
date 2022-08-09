using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSceneButton : MonoBehaviour
{
    [SerializeField] LevelManager levelManager;
    public void ChangeScene(string sceneName)
    {
        levelManager.LoadScene(sceneName);
    }
}
