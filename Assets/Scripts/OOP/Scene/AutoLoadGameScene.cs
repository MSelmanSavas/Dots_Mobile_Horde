using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoLoadGameScene : MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }
}
