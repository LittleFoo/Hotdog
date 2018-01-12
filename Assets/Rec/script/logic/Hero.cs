using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using common;
using PathologicalGames;
using DG.Tweening;

public class Hero : MonoBehaviour, ITimerEvent {
	[HideInInspector]
	public Transform tf;
	public SpriteRenderer headSprRdr;
	public Transform headMask;
	public Transform tailMask;
	public SpriteAnimation ani;
	public Sprite[] headSprs;
	public Sprite[] headGetSprs;
	public Sprite[] bodyMaskSprs;
	public Sprite[] idleSpr;
	public Sprite[] tailSprs; //straight, turn right , turn left;
	public Sprite[] bodySprs;

	private float _moveTime = 0.3f;
	private SpawnPool _pool;
	private PrefabPool pp;
	private int _lastDir;
	private int _nextDir;
	private int _initDir;
	private Coord _curPos;
	public Coord curPos{get{ return _curPos;}}
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

	private bool _isBackwards = false;
	public bool isBackwards
	{
		get{ return _isBackwards;}
	}

	// Use this for initialization
	void Awake () {
		tf = transform;
		headSprRdr = tf.GetComponent<SpriteRenderer>();

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
			if(_isBackwards)
				return;
			_nextDir = 0;
			if(_curTween == null)
				onMoveComplete();
		}
		else if(Input.GetKeyDown(KeyCode.DownArrow))
		{
			if(_isBackwards)
				return;
			_nextDir = 2;
			if(_curTween == null)
				onMoveComplete();
		}
		else if(Input.GetKeyDown(KeyCode.LeftArrow))
		{
			if(_isBackwards)
				return;
			_nextDir = 3;
			if(_curTween == null)
				onMoveComplete();
		}
		else if(Input.GetKeyDown(KeyCode.RightArrow))
		{
			if(_isBackwards)
				return;
			_nextDir = 1;
			if(_curTween == null)
				onMoveComplete();
		}

	

//		if(Input.GetKeyUp(KeyCode.UpArrow))
//		{
//			if(_nextDir == 0)
//				_nextDir = -1;
//		}
//		else if(Input.GetKeyUp(KeyCode.RightArrow))
//		{
//			if(_nextDir == 1)
//				_nextDir = -1;
//		}
//		else if(Input.GetKeyUp(KeyCode.DownArrow))
//		{
//			if(_nextDir == 2)
//				_nextDir = -1;
//		}
//		else if(Input.GetKeyUp(KeyCode.LeftArrow))
//		{
//			if(_nextDir == 3)
//				_nextDir = -1;
//		}
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

//		tf.GetComponent<SpriteRenderer>().DOColor(new Color(1, 1, 1, 0), 0.2f).SetLoops(12, LoopType.Yoyo);
		headSprRdr.maskInteraction = SpriteMaskInteraction.None;
	}

	public void changeSpeed(float rate)
	{
		_moveTime /= rate;
	}

	public void resumeSpeed()
	{
		_moveTime = 0.3f;
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

					//TODO
					if(GameController.instance.isEmpty(x, y) == Config.FLAG_EMPTY)
					{
						_initDir = _nextDir;
						tf.rotation = Config.RotationArray[_nextDir];
					}
					_curTween = tf.DOLocalMove(new Vector3(tf.localPosition.x+nextDirVal.x*Config.unit, 
						tf.localPosition.y+nextDirVal.y*Config.unit, 0), _moveTime).SetEase(Ease.Linear).OnComplete(onMoveComplete);
					_lastDir = _nextDir;
					_curPos.x = x;
					_curPos.y = y;
					_nextDir = -1;
				}
			}
			else //move from empty to tile
			{
//				for(int i = 0; i < _tailList.Count; i++)
//				{
//					_pool.Despawn(_tailList[i]);
//				}
//				_tailList.Clear();
				headSprRdr.sprite = headGetSprs[(_lastDir-_initDir+4)%4];
				_nextDir = -1;
				onTouchTile();
			}
			break;
			//continue move
		case Config.FLAG_EMPTY:
			//FROM TILE TO EMPTY
			if(_isLastTile)
			{
				_map = GameController.instance.getMap();

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
			tail.rotation = Config.RotationArray[_lastDir];
			SpriteRenderer spr = tail.GetComponent<SpriteRenderer>();
			spr.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
			spr.sortingOrder = 2;
			if(_nextDir == _lastDir|| _nextDir == -1 || Mathf.Abs( _lastDir-_nextDir) == 2)
			{
				if(_isLastTile)
					spr.sprite = tailSprs[0];
				else
					spr.sprite = bodySprs[0];
				Coord lastDirVal = Config.directionArray[_lastDir];
			
				_curTween = tf.DOLocalMove(new Vector3(tf.localPosition.x+lastDirVal.x*Config.unit, 
					tf.localPosition.y+lastDirVal.y*Config.unit, 0), _moveTime).SetEase(Ease.Linear).OnComplete(onMoveComplete);
				
				_curPos.x += lastDirVal.x;
				_curPos.y += lastDirVal.y;
			}
			else //change direction
			{
				tf.rotation = Config.RotationArray[_nextDir];
				headSprRdr.sprite = headSprs[(_nextDir-_initDir+4)%4];
				if(_nextDir-_lastDir == 1)//right
				{
					if(_isLastTile)
						spr.sprite = tailSprs[1];
					else
						spr.sprite = bodySprs[1];
				}
				else//left
				{
					if(_isLastTile)
						spr.sprite = tailSprs[2];
					else
						spr.sprite = bodySprs[2];
				}
			
				_lastDir = _nextDir;
				Coord nextDirVal = Config.directionArray[_nextDir];
				_curTween = tf.DOLocalMove(new Vector3(tf.localPosition.x+nextDirVal.x*Config.unit, 
					tf.localPosition.y+nextDirVal.y*Config.unit, 0), _moveTime).SetEase(Ease.Linear).OnComplete(onMoveComplete);
				_curPos.x += nextDirVal.x;
				_curPos.y += nextDirVal.y;
			}
			if(_isLastTile)
			{
				headSprRdr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
				_curTween.onUpdate = ()=>{
					if(_curTween.ElapsedDirectionalPercentage() >= 0.2f)
					{
						headSprRdr.maskInteraction = SpriteMaskInteraction.None;

						_curTween.onUpdate = null;
					}
				};
				ani.stop();
				headSprRdr.sprite = headSprs[0];
			}
			else
				headSprRdr.maskInteraction = SpriteMaskInteraction.None;
			_isLastTile = false;
			break;
			//bite self
		case Config.FLAG_TAIL:
			GameController.instance.dead();
			break;
		}
	}

	int beforeIdx;
	Transform lastTile;
	SpriteRenderer lastSpr;
	private void onTouchTile()
	{
		GameController.instance.onTouchTile(tailIdx);
		_isBackwards = true;
		lastTile = _tailList[0];
		lastSpr = lastTile.GetComponent<SpriteRenderer>();
		beforeIdx = 1;
		if(beforeIdx < _tailList.Count)
		{
			headMask.SetParent(lastTile);
			headMask.localScale = new Vector3(0.6f, 0.2f, 1);
			headMask.localPosition = new Vector3(0, -Config.unit*0.5f, 0);
			headMask.rotation = lastTile.rotation;

			moveBackward();
		}
		else
			onBackToHeadComplete();
		
	}

	private float backWardsTime = 0.2f;
	private Tweener moveBackTweener;
	private void moveBackward()
	{
		if(beforeIdx >= _tailList.Count)
		{
			onBackToHeadComplete();
			return;
		}

		Transform before = _tailList[beforeIdx];
		beforeIdx++;
		lastSpr.maskInteraction = SpriteMaskInteraction.None;
		lastSpr.sortingOrder = 1;
		lastTile.rotation = before.rotation;
		moveBackTweener = lastTile.DOLocalMove(before.localPosition, backWardsTime).SetEase(Ease.Linear);
		moveBackTweener.OnUpdate(()=>{
			if(moveBackTweener.ElapsedDirectionalPercentage() >= 0.8f)
				{
					lastSpr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
					moveBackTweener.onUpdate = null;
				}});
		moveBackTweener.OnComplete(()=>{
			before.gameObject.SetActive(false);
			moveBackward();});
	}

	private void onBackToHeadComplete()
	{
//		headSprRdr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
		lastSpr.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
		headMask.SetParent(tf);
		headMask.localScale = new Vector3(0.6f, 0.6f, 1);
		headMask.localPosition =  new Vector3(0, -6, 0);
		lastTile.rotation = tf.rotation;
		moveBackTweener = lastTile.DOLocalMove(tf.localPosition, backWardsTime).SetEase(Ease.Linear);
		moveBackTweener.OnUpdate(()=>{
			if(moveBackTweener.ElapsedDirectionalPercentage() >= 0.6f)
			{
				headSprRdr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
				moveBackTweener.onUpdate = null;
			}
		});
		moveBackTweener.OnComplete(()=>{ani.play(1, false, onBackwardComplete);});
	}

	private void onBackwardComplete(GameObject obj)
	{
		headSprRdr.maskInteraction = SpriteMaskInteraction.None;
		for(int i = 0; i < _tailList.Count; i++)
		{
			_pool.Despawn(_tailList[i]);
		}
		_tailList.Clear();
		_isBackwards = false;

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
