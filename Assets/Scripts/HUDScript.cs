﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDScript : MonoBehaviour
{
    private UnityEngine.UI.Image frontCircle;
    private UnityEngine.UI.Image backCircle;
    private UnityEngine.UI.Image frontSoundBar;
    private UnityEngine.UI.Text countdown;
    private Player playerScript;
    private List<Enemy> enemies;
    private List<key> keys;
    private List<Enemy> enemiesInPursuit;
    private Color defaultBackCircleColor;

    void Start() //Use this for initialization
    {
        //GameManager.Instance

        UnityEngine.UI.Image[] allUIImages = GetComponentsInChildren<UnityEngine.UI.Image>();

        for (int i = 0; i < allUIImages.Length; i++)
        {
            if (allUIImages[i].name == "FillCircle")
            {
                frontCircle = allUIImages[i];
                frontCircle.color = new Color(GameManager.playerColor.r, GameManager.playerColor.g, GameManager.playerColor.b, 1);
            }
            else if (allUIImages[i].name == "StaticCircle")
            {
                backCircle = allUIImages[i];
                backCircle.color = new Color(GameManager.playerColor.r, GameManager.playerColor.g, GameManager.playerColor.b, 100/255f);
                defaultBackCircleColor = backCircle.color;
            }
            else if (allUIImages[i].name == "FillSoundBar")
            {
                frontSoundBar = allUIImages[i];
            }
        }

        countdown = FindObjectOfType<UnityEngine.UI.Text>();

        GameObject[] allGameObjects = FindObjectsOfType<GameObject>();
        enemies = new List<Enemy>();
        keys = new List<key>();

        for (int i = 0; i < allGameObjects.Length; i++)
        {
            if (allGameObjects[i].tag == "Enemy")
            {
                enemies.Add(allGameObjects[i].GetComponent<Enemy>());
                allGameObjects[i].GetComponentInChildren<Renderer>().material.color = GameManager.enemyColor;
            }
            else if (allGameObjects[i].tag == "key")
            {
                keys.Add(allGameObjects[i].GetComponent<key>());
                allGameObjects[i].GetComponent<Renderer>().material.color = GameManager.playerColor;
            }
            else if (allGameObjects[i].tag == "Player")
            {
                playerScript = allGameObjects[i].GetComponent<Player>();
                allGameObjects[i].GetComponentInChildren<Renderer>().material.color = GameManager.playerColor;
            }
        }

        playerScript = FindObjectOfType<Player>();

        enemiesInPursuit = new List<Enemy>();
    }	
	
	void Update() //Update is called once per frame
    {
        PingUI();
        SoundUI();
	}

    void PingUI() //Manage ping cooldown and enemy alert
    {
        if (frontCircle.fillAmount > 0 && !playerScript.RefillPing && !playerScript.HasPing)
        {
            frontCircle.fillAmount -= Time.deltaTime;
        }
        else if (playerScript.RefillPing)
        {
            frontCircle.fillAmount += Time.deltaTime / 2f;
        }

        if (enemiesInPursuit == null && enemiesInPursuit.Count > 0)
        {
            foreach (Enemy e in enemiesInPursuit)
            {
                if (!e.Pursuit)
                {
                    enemiesInPursuit.Remove(e);
                }
                else
                {
                    countdown.text = "!";
                    backCircle.color = Color.Lerp(defaultBackCircleColor, Color.red, Mathf.PingPong(Time.time, 1));
                }
            }
        }
        else
        {
            foreach (Enemy e in enemies)
            {
                if (!e.Pursuit)
                {
                    countdown.text = ((int)(frontCircle.fillAmount * 100)).ToString();
                    backCircle.color = defaultBackCircleColor;
                }
                else
                {
                    enemiesInPursuit.Add(e);
                }
            }
        }
    }

    void SoundUI() //Sound with crouching and such
    {
        frontSoundBar.fillAmount = playerScript.Noise / 425f;
    }
}