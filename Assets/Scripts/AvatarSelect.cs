using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AvatarSelect : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }
    public enum AvatarType
    {
        BALANCED,
        HEAVY,
        FLOATY,
        SWIFT
    }

    public AvatarType SelectedAvatar;

    public void SelectBalanced()
    {
        SelectedAvatar = AvatarType.BALANCED;
        LoadArenaScene();
    }
    public void SelectHeavy()
    {
        SelectedAvatar = AvatarType.HEAVY;
        LoadArenaScene();
    }

    public void SelectFloaty()
    {
        SelectedAvatar = AvatarType.FLOATY;
        LoadArenaScene();
    }

    public void SelectSwift()
    {
        SelectedAvatar = AvatarType.SWIFT;
        LoadArenaScene();
    }

    void LoadArenaScene()
    {
        SceneManager.LoadScene("GreyBox", LoadSceneMode.Single);
    }

}
