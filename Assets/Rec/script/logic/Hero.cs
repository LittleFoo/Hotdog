using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using common;
using PathologicalGames;
using DG.Tweening;

public class Hero : MonoBehaviour, ITimerEvent {
	[HideInInspector]
	public Transform tf;
	public SpriteAnimation ani;
	public Sprite[] headSprs;
	public Sprite[] headGetSprs;
	public Sprite[] bodyMaskSprs;
	public Sprite[] idleSpr;
	private SpawnPool _pool;
	private PrefabPool pp;
	private int _lastDir;
	private int _nextDir;
	private Coord _curPos;
	private Tweener _curTween;
	private List<Transform> _tailList;
	private bool _isDead = true;
	private Transform _root;
	private List<Coord> tailIdx;
	private int _life;
	public int life
	{
		get{ return _life;}
		set{ _life = value; }
	}

	// Use this for initialization
	void Awake () {
		tf = transform;
		_pool = GlobalController.instance.getCurPool();
		pp = new PrefabPool(GlobalController.instance.prefabSetting.tail);
		_pool.CreatePrefabPool(pp);
		pp.preloadAmount = 5;
		pp.limitInstances = false;

		_tailList = new List<Transform>();
		tailIdx = new List<Coord>();
		_isDead = false;
	}
	
	// Update is called once per frame
	public void onUpdate()
	{
		if(Input.GetKeyDown(KeyCode.UpArrow))
		{
			_nextDir = 0;
			if(_curTween == null)
				onMoveComplete();
		}
		else if(Input.GetKeyDown(KeyCode.DownArrow))
		{
			_nextDir = 2;
			if(_curTween == null)
				onMoveComplete();
		}
		else if(Input.GetKeyDown(KeyCode.LeftArrow))
		{
			_nextDir = 3;
			if(_curTween == null)
				onMoveComplete();
		}
		else if(Input.GetKeyDown(KeyCode.RightArrow))
		{
			_nextDir = 1;
			if(_curTween == null)
				onMoveComplete();
		}

		if(Input.GetKeyUp(KeyCode.UpArrow))
		{
			if(_nextDir == 0)
				_nextDir = -1;
		}
		else if(Input.GetKeyUp(KeyCode.RightArrow))
		{
			if(_nextDir == 1)
				_nextDir = -1;
		}
		else if(Input.GetKeyUp(KeyCode.DownArrow))
		{
			if(_nextDir == 2)
				_nextDir = -1;
		}
		else if(Input.GetKeyUp(KeyCode.LeftArrow))
		{
			if(_nextDir == 3)
				_nextDir = -1;
		}
	}

	public void init(Transform root, int life)
	{
		_life = life;
		_root = root;
		TimerManager.instance.addEventListeners(this);
		_lastDir = 0;
		_nextDir = -1;
		_isDead = false;
		_curPos = new Coord(10, 0);
		_isLastTile = true;
		_curTween = null;
		setPos(_curPos.x, _curPos.y);

		if(_tailList.Count != 0)
		{
			for(int i = 0; i < _tailList.Count; i++)
			{
				_pool.Despawn(_tailList[i]);
			}
			_tailList.Clear();
			tailIdx.Clear();
		}

		Rigidbody2D rid;


		tf.GetComponent<SpriteRenderer>().DOColor(new Color(1, 1, 1, 0), 0.2f).SetLoops(12, LoopType.Yoyo);

	}
		

	private void setPos(int x, int y)
	{
		tf.localPosition = new Vector3(x*Config.unit, y*Config.unit);
	}

	private bool _isLastTile = true;
	private int[,] _map;

	private void onMoveComplete()
	{
		Transform tail;
		_curTween = null;

		int isEmpty = GameController.instance.isEmpty(_curPos.x, _curPos.y);
		switch(isEmpty)
		{
		case Config.FLAG_WALL:
		case Config.FLAG_TILE:
			//move from tile to tile
			_isLastTile = true;
			if(tailIdx.Count == 0)
			{
				if(_nextDir != -1)
				{
					Coord nextDirVal = Config.directionArray[_nextDir];
					int x = nextDirVal.x+_curPos.x;
					int y = nextDirVal.y+_curPos.y;
					if(x >= Config.width || x < 0 || y > Config.height || y < 0)
					{
//						tf.GetComponent<SpriteRenderer>().sprite = idleSpr[0];
						ani.play(-1);
						_nextDir = -1;
						return;
					}

					_curTween = tf.DOLocalMove(new Vector3(tf.localPosition.x+nextDirVal.x*Config.unit, 
						tf.localPosition.y+nextDirVal.y*Config.unit, 0), 0.1f).SetEase(Ease.Linear).OnComplete(onMoveComplete);
					_lastDir = _nextDir;
					_curPos.x = x;
					_curPos.y = y;
				}
			}
			else //move from empty to tile
			{
				for(int i = 0; i < _tailList.Count; i++)
				{
					_pool.Despawn(_tailList[i]);
				}
				_tailList.Clear();
				onTouchTile();
			}
			break;
			//continue move
		case Config.FLAG_EMPTY:
			//FROM TILE TO EMPTY
			if(_isLastTile)
			{
				_map = GameController.instance.getMap();
				_isLastTile = false;
			}
			if(_map[_curPos.x, _curPos.y] == Config.FLAG_TAIL)
			{
				GameController.instance.dead();
				break;
			}
			_map[_curPos.x, _curPos.y] = Config.FLAG_TAIL;
			tailIdx.Add(new Coord(_curPos.x, _curPos.y));
			tail = _pool.Spawn(pp.prefab);
			_tailList.Add(tail);
			tail.SetParent(_root);
			tail.localPosition = new Vector3(_curPos.x*Config.unit, _curPos.y*Config.unit, 0);
		
			if(_nextDir == -1 || Mathf.Abs( _lastDir-_nextDir) == 2)
			{
				Coord lastDirVal = Config.directionArray[_lastDir];
			
				_curTween = tf.DOLocalMove(new Vector3(tf.localPosition.x+lastDirVal.x*Config.unit, 
					tf.localPosition.y+lastDirVal.y*Config.unit, 0), 0.1f).SetEase(Ease.Linear).OnComplete(onMoveComplete);
				
				_curPos.x += lastDirVal.x;
				_curPos.y += lastDirVal.y;
			}
			else //change direction
			{
				_lastDir = _nextDir;
				Coord nextDirVal = Config.directionArray[_nextDir];
				_curTween = tf.DOLocalMove(new Vector3(tf.localPosition.x+nextDirVal.x*Config.unit, 
					tf.localPosition.y+nextDirVal.y*Config.unit, 0), 0.1f).SetEase(Ease.Linear).OnComplete(onMoveComplete);
				_curPos.x += nextDirVal.x;
				_curPos.y += nextDirVal.y;
			}

			break;
			//bite self
		case Config.FLAG_TAIL:
			GameController.instance.dead();
			break;
		}
	}

	private void onTouchTile()
	{
		GameController.instance.onTouchTile(tailIdx);
		tailIdx.Clear();
	}


	//called by controller not by self.
	public void dead()
	{
		if(_isDead)
			return;

		TimerManager.instance.removeEventListeners(this);
		if(_curTween != null)
			_curTween.Kill();
		_isDead = true;
		_life--;
	}

	public void setEnabled()
	{
		tf.GetComponent<SpriteRenderer>().color = Color.white;
	}
}
