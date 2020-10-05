﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject levelSelectMenu;
    public GameObject nextLevelMenu;
    public GameObject optionsMenu;
    public GameObject creditsMenu;

    public GameObject currentMenu;

    public AudioSource clickSoundEffect;

    public Text finalScoreText;

    public void Start()
    {
        //GoToMainMenu();
    }

    public void CloseAllMenus(){
        mainMenu.SetActive(false);
        levelSelectMenu.SetActive(false);
        nextLevelMenu.SetActive(false);
        optionsMenu.SetActive(false);
        creditsMenu.SetActive(false);
    }

    public void GoToMenu(GameObject menu, bool click = true)
    {
        if(currentMenu != null)
            currentMenu.SetActive(false);

        menu.SetActive(true);
        currentMenu = menu;

        if(click) clickSoundEffect.Play();
    }

    public void GoToMainMenu()
    {
        GoToMenu(mainMenu);
    }

    public void GoToLevelSelectMenu()
    {
        GoToMenu(levelSelectMenu);
    }

    public void GoToNextLevelMenu()
    {
        GoToMenu(nextLevelMenu);
    }

    public void GoToOptionsMenu()
    {
        GoToMenu(optionsMenu);
    }

    public void GoToCreditsMenu()
    {
        GoToMenu(creditsMenu);
    }

    public void CloseMenu()
    {
        currentMenu.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void UpdateFinalScore(int score)
    {
        finalScoreText.text = "Final Score\n" + score;
    }

    public static MenuController GetMC()
    {
        return GameObject.Find("MenuController").GetComponent<MenuController>();
    }
}
