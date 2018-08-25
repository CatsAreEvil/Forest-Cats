﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager_Script : MonoBehaviour {
	private Music_Manager music;
	public List <Edge_Script> exits;
	public int currentScene;
	public string PlayerName;
	public List <string> values;
	public Dictionary<int, string> test = new Dictionary <int, string>();

	public void Start (){
		PlayerName = "Player Name";
		music = GetComponent<Music_Manager>();
		Scene scene = SceneManager.GetActiveScene();
		currentScene = scene.buildIndex;
	}
	
	// Update is called once per frame
	public void Update(){
		Scene scene = SceneManager.GetActiveScene();
		currentScene = scene.buildIndex;
		for (int i = 0; i < exits.Count; i++){
			if (exits[i].startNewLevel == true){
				RunScene(exits[i].newScene);
			}
		}
		if (currentScene == 0){// Main menu
			music.state = Music_Manager.MusicState.mainMenu;
		}
		else if(currentScene == 1){// Camp
			music.state = Music_Manager.MusicState.homeNormal;
		}
	}

	void Awake(){
		DontDestroyOnLoad(this.gameObject);
	}
	public void RunScene (int scene) {
		SceneManager.LoadScene(scene);
	}
}
