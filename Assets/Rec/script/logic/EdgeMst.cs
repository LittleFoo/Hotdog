using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EdgeMst : EnemyBase {
	private int _dirIdx = 0;
	private Coord _curPos;
	private Transform tf;
	private float halfUnit;
	private Tweener _curTween;
	private float _time = 0.2f;
	private float _curTime;
	public void init()
	{
		tf = transform;
		halfUnit = Config.unit*0.5f;
		int x = Random.Range(0, Config.width-2);
		int y;
		if(x == 0 || x == Config.width-2)
			y = Random.Range(0, Config.height-2);
		else
		{
			if(Random.Range(0, 1) == 0)
				y = 0;
			else
				y = Config.height-1;
		}
		_curTime = _time;
		init(new Coord(x, y));
	}


	public void init(Coord coord)
	{
		int idx = Random.Range(0, Config.directionArray.Length-1);
		_curPos = coord;
		int x,y;
		for(int i = 0; i < Config.directionArray.Length; i++)
		{
			x = Config.directionArray[idx].x + coord.x;
			y = Config.directionArray[idx].y + coord.y;

			if(x < 0 || x >= Config.width || y < 0 || y >= Config.height||GameController.instance.isEmpty(x, y) == Config.FLAG_EMPTY)
			{
				idx = (idx++)%Config.directionArray.Length;
				continue;
			}
			_dirIdx = idx;
			break;
		}
		tf.localPosition = new Vector3(_curPos.x*Config.unit+halfUnit, _curPos.y*Config.unit+halfUnit, 0);
		onMoveComplete();
	}

	private void onMoveComplete()
	{
		Coord next = new Coord(_curPos.x+Config.directionArray[_dirIdx].x, _curPos.y+Config.directionArray[_dirIdx].y);
		Coord dir;
		if(!isAbleToGo(_curPos, next))
		{
			int idx = (_dirIdx+1)%4;
			dir = Config.directionArray[idx];
			next = _curPos+dir;
			if(isAbleToGo(_curPos, next))
			{
				_dirIdx = idx;
				next = _curPos+dir;
			}
			else
			{
				idx = (_dirIdx+3)%4;
				dir = Config.directionArray[idx];
				next = _curPos+dir;
				if(isAbleToGo(_curPos, next))
				{
					_dirIdx = idx;
				}
//				else{
//					idx = (_dirIdx+2)%4;
//					_dirIdx = idx;
//					dir = Config.directionArray[idx];
//					next = _curPos+dir;
//				}
			}
		}

		_curPos = next;
		_curTween = tf.DOLocalMove(new Vector3(next.x*Config.unit+halfUnit, next.y*Config.unit+halfUnit),_curTime).SetEase(Ease.Linear).OnComplete(onMoveComplete);
	}

	//mark tile(0,0)'s top right as (0, 0)
	private bool isAbleToGo(Coord cur, Coord next)
	{
		if(next.x < 0 || next.x >= Config.width-1 || next.y < 0 || next.y >= Config.height-1)
		{
			return false;
		}

		Coord c = new Coord(0, 0);
		if(cur.x == next.x)
		{
			c.y = (cur.y > next.y) ? cur.y : next.y;
			if(GameController.instance.isEmpty(cur.x, c.y) != Config.FLAG_EMPTY && GameController.instance.isEmpty(cur.x+1, c.y)!= Config.FLAG_EMPTY)
				return false;
		}
		else
		{
			c.x = (cur.x > next.x) ? cur.x : next.x;
			if(GameController.instance.isEmpty(c.x, cur.y) != Config.FLAG_EMPTY&& GameController.instance.isEmpty(c.x, cur.y+1)!= Config.FLAG_EMPTY)
				return false;
		}
		if(getEmptyCount(next) == 4)
			return false;
		return true;
	}

	private int getEmptyCount(Coord coord)
	{
		int count = 0;

		if(GameController.instance.isEmpty(coord.x, coord.y) == Config.FLAG_EMPTY)
			count++;

		if(coord.y +1 < Config.height-1 && GameController.instance.isEmpty(coord.x, coord.y+1) == Config.FLAG_EMPTY)
			count++;

		if(coord.x +1 < Config.width-1 &&coord.y +1 < Config.height-1 && GameController.instance.isEmpty(coord.x+1, coord.y+1) == Config.FLAG_EMPTY)
			count++;

		if(coord.x +1 < Config.width-1 && GameController.instance.isEmpty(coord.x+1, coord.y) == Config.FLAG_EMPTY)
			count++;

		return count;
	}

	public override void changeSpeed(float rate)
	{
		_curTime = _time/rate;
	}

	public override void resumeSpeed()
	{
		_curTime = _time;
	}

	public override void pause()
	{
		_curTween.Pause();
	}

	public override void resume()
	{
		_curTween.Restart();
	}

	public override void clear()
	{
		_curTween.Kill();
		_curTween = null;
	}

}
