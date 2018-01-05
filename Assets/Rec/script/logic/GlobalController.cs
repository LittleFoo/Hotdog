﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;
public class GlobalController : MonoBehaviour {
	public PrefabSetting prefabSetting;
//	public Setting setting;
//	public SceneSetting curScene;
	private SpawnPool _pool;

	private static GlobalController _instance;
	public static GlobalController instance
	{
		get{
			if(_instance == null)
			{
				_instance = FindObjectOfType<GlobalController>();
				_instance.init(); 
			}
			return _instance;
		}
	}


	void Awake () {
		if(_instance != null) 
			return;
		_instance = this;
		init();
	}

	void init()
	{
		Application.targetFrameRate = 60;

		UIModule.instance.init();
//		setting.init();
	}

	// Update is called once per frame
	void Update () {
		common.TimerManager.instance.Update();
	}

	public void pause()
	{
		common.TimerManager.instance.paused = true;
		Time.timeScale = 0;
	}

	public void resume()
	{
		common.TimerManager.instance.paused = false;
		Time.timeScale = 1;
	}

	public PathologicalGames.SpawnPool getCurPool()
	{
		if(_pool != null)
			return _pool;

		_pool = gameObject.AddComponent<SpawnPool>();
		return _pool;
	}

	void OnDestroy()
	{
		if(_instance == this)
			_instance = null;
	}



}
