using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;
using common;
public class GameController : MonoBehaviour, ITimerEvent
{
//	private SpawnPool _pool;
	public Transform root;
	public Hero hero;
	public Map map;
	public EnemyController enemy;
	private float _startCounting;
	private int _life;
	private static GameController _instance;
	public static GameController instance
	{
		get{
			return _instance;
		}
	}


	void Awake () {
		if(_instance != null) 
			return;
		_instance = this;
	}

	// Use this for initialization
	void Start()
	{
		onGameStart();

	}

	private void onGameStart()
	{
		_startCounting = 3;
		_life = 3;
		UIController.instance.init(_life);
		hero.init(root, _life);
		map.init();
		enemy.init(root);
		enemy.gernerateMst(MstType.randomWalk, 2);
		enemy.gernerateMst(MstType.darkEdge, 1);
		enemy.gernerateMst(MstType.breaker, 1);
		TimerManager.instance.addEventListeners(this);
	}

	public int isEmpty(int x, int y)
	{
		return map.isEmpty(x, y);
	}

	public void onTouchTile(List<Coord> tails)
	{
		map.onTouchTile(tails);
		if(enemy.darkCount < map.tilelist.Count/200)
		{
			int idx = Random.Range(0, tails.Count-1);
			enemy.generateDark(map.tilelist[idx]);
		}
	}

	public int[,] getMap()
	{
		return map.copyMap();
	}

	public bool hasMst(int tileFlag, int[,] map)
	{
		return enemy.hasMst(tileFlag, map);
	}

	public void breakTile(float x, float y)
	{
		map.breakTile(x, y);
	}

	public void dead()
	{
		if(_startCounting > 0)
			return;
		hero.dead();
		enemy.pause();

		if(_life == 0)
		{
			UIController.instance.showFailView();
		}
		else
		{
			
			UIController.instance.showContinueView();
		}
	}

	public void onContinue()
	{
		enemy.resume();
		hero.init(root, 3);
		_startCounting = 3;
		TimerManager.instance.addEventListeners(this);
	}

	public void onClickRestart()
	{
		enemy.clear();
		onGameStart();
		map.clear();

		onGameStart();
	}

	public void onUpdate()
	{
		if(_startCounting == 3)
		{
			_startCounting -= 0.01f;
			return;
		}
		_startCounting -= Time.unscaledDeltaTime;
		if(_startCounting <= 0)
		{
			hero.setEnabled();
			TimerManager.instance.removeEventListeners(this);
		}
	}
}
