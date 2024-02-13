using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoLoadGameScene : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(DelaySceneLoad());
    }

    IEnumerator DelaySceneLoad()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }
}
