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
	public BonusController bonus;
	private float _startCounting;
	private int _life;
	private CsvLevel csvLevel;
	private static GameController _instance;
	private CsvLevelUnit _curLevelData;
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
		csvLevel = DataManager.instance.getCsvData<CsvLevel>(CsvConfig.CSV_LEVEL);
		_curLevelData = csvLevel.getObj(4);
		if(_curLevelData.bonusIds == null)
		{
			_curLevelData.bonusIds = CsvConfig.stringToIntArray<int>(_curLevelData.Bonus);
			string[] ss = _curLevelData.Monsters.Split("|"[0]);
			string[] sss;
			Coord c;
			_curLevelData.monsterDatas = new List<Coord>();
			for(int i = 0; i < ss.Length; i++)
			{
				sss = ss[i].Split(";"[0]);
				c = new Coord();
				c.x = System.Convert.ToInt32(sss[0]);
				c.y = System.Convert.ToInt32(sss[1]);
				_curLevelData.monsterDatas.Add(c);
			}
		}
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
		bonus.init(_curLevelData.bonusIds, _curLevelData.BonusNum, Config.width*Config.height);
		for(int i = 0; i < _curLevelData.monsterDatas.Count; i++)
		{
			enemy.gernerateMst((MstType)_curLevelData.monsterDatas[i].x, _curLevelData.monsterDatas[i].y);
		}
		TimerManager.instance.addEventListeners(this);
	}

	public int isEmpty(int x, int y)
	{
		return map.isEmpty(x, y);
	}

	public void onTouchTile(List<Coord> tails)
	{
		bonus.onNewTileBuild(map.onTouchTile(tails));
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
//		if(_startCounting > 0 || hero.isBackwards)
//			return;
//		hero.dead();
//		enemy.pause();
//
//		if(_life == 0)
//		{
//			UIController.instance.showFailView();
//		}
//		else
//		{
//			
//			UIController.instance.showContinueView();
//		}
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
