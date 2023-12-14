using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    private void Start()
    {
        LoadArenaScene();
    }

    void LoadArenaScene()
    {
        SceneManager.LoadScene("GreyBox", LoadSceneMode.Single);
    }
}
