using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;

public class Map : MonoBehaviour {
	public int amount;
	private Transform _root;
//	private SpawnPool _pool;
	private PrefabPool pp;
	private int width, height, unit;
	private Transform[,] tileArray= new Transform[Config.width,Config.height];
	private int[,] _isEmpty = new int[Config.width,Config.height];
	private int[,] map = new int[Config.width,Config.height];
	private Queue<Transform> tilePool;
	public CompositeCollider2D col;
	public List<Coord> tilelist = new List<Coord>();
	// Use this for initialization
	void Start () {
		_root = transform;
		width = Config.width;
		height = Config.height;
		unit = Config.unit;
//		_pool = GlobalController.instance.getCurPool();
//		pp = new PrefabPool(GlobalController.instance.prefabSetting.tile);
//
//		pp.preloadAmount = width*height;
//		pp.limitInstances = false;
//		_pool.CreatePrefabPool(pp);
		amount = (Config.width-2)*(Config.height-2);
		tilePool = new Queue<Transform>();
		int num = width*height;
		Transform p = GlobalController.instance.prefabSetting.tile;
		Transform ins;
		for(int i = 0; i < num; i++)
		{
			ins = Transform.Instantiate(p);
			ins.SetParent(_root);
			ins.gameObject.SetActive(false);
			tilePool.Enqueue(ins);
		}
	}
	
	public void init()
	{
		
		for(int i = 0; i < width; i++)
		{
			setTileStatus(i, 0, Config.FLAG_WALL);
			setTileStatus(i,height-1,Config.FLAG_WALL);
		}

		for(int j = 0; j < height; j++)
		{
			setTileStatus(0, j, Config.FLAG_WALL);
			setTileStatus(width-1, j, Config.FLAG_WALL);
		}
		StartCoroutine(generateGeometry());
	}

	private void setTileStatus(int x, int y, int status)
	{
		if(_isEmpty[x,y] != Config.FLAG_EMPTY)
		{
			Despawn(tileArray[x,y]);
		}

		_isEmpty[x,y] = status;
		Transform tf;
		switch(status)
		{
		case Config.FLAG_EMPTY:
			tileArray[x,y] = null;
			break;

		case Config.FLAG_WALL:
			tf = spawn();
			tf.tag = Config.TAG_WALL;
			tileArray[x,y] = tf;
			tf.SetParent(_root);
			tf.localPosition = new Vector3(x*unit, y*unit, 0);
			break;

		case Config.FLAG_TILE:
			tf =spawn();
			tf.tag = Config.TAG_TILE;
			tileArray[x,y] = tf;
			tf.SetParent(_root);
			tf.localPosition = new Vector3(x*unit, y*unit, 0);
			break;
		}
	}

	public void breakTile(float x, float y)
	{
		int ix = (int)(x/Config.unit);
		int iy = (int)(y/Config.unit);

		if(_isEmpty[ix, iy] == Config.FLAG_WALL)
		{
			return;
		}

		int dx = 0, dy = 0;
		float minDis = int.MaxValue, curDis;
		if(_isEmpty[ix, iy] == Config.FLAG_EMPTY)
		{
			Vector2 touchPoint = new Vector2(x, y);

			if(ix-1 > 0 && _isEmpty[ix-1, iy] == Config.FLAG_TILE)
			{
				minDis = Vector2.Distance(touchPoint, new Vector2((ix-1)*Config.unit, iy*Config.unit));
				dx = ix-1;
				dy = iy;
			}
		
			if(ix+1 < Config.width-1 && _isEmpty[ix+1, iy] == Config.FLAG_TILE)
			{
				curDis = Vector2.Distance(touchPoint, new Vector2((ix+1)*Config.unit, iy*Config.unit));
				if(curDis < minDis)
				{
					dx = ix+1;
					dy = iy;
				}
			}

			if(iy-1 > 0 && _isEmpty[ix, iy-1] == Config.FLAG_TILE)
			{
				curDis = Vector2.Distance(touchPoint, new Vector2(ix*Config.unit, (iy-1)*Config.unit));
				if(curDis < minDis)
				{
					dx = ix;
					dy = iy-1;
				}
			}

			if(iy+1 < Config.height-1 && _isEmpty[ix, iy+1] == Config.FLAG_TILE)
			{
				curDis = Vector2.Distance(touchPoint, new Vector2(ix*Config.unit, (iy+1)*Config.unit));
				if(curDis < minDis)
				{
					dx = ix;
					dy = iy+1;
				}
			}

			if(dx == 0 && dy == 0)
				return;
			
			ix = dx;
			iy = dy;

		}


		_isEmpty[ix, iy] = Config.FLAG_EMPTY;
		Despawn(tileArray[ix, iy]);
		tileArray[ix, iy] = null;
		StartCoroutine(generateGeometry());
	}

	public int isEmpty(int x, int y)
	{
		if(x >= width || y >= height)
			return -1;

		return _isEmpty[x, y];
	}

	public int[,] copyMap()
	{
		System.Array.Copy(_isEmpty, map, _isEmpty.Length);
		return map;
	}

	public void makePath(List<Coord> tailList)
	{
		Transform tf;

		for(int i = 0; i < tailList.Count; i++)
		{
			tf = spawn();
			tileArray[tailList[i].x, tailList[i].y] = tf;
			_isEmpty[tailList[i].x, tailList[i].y] = Config.FLAG_TILE;
			tf.SetParent(_root);
			tf.localPosition = new Vector3(tailList[i].x*unit, tailList[i].y*unit, 0);
			tilelist.Add(new Coord(tailList[i].x, tailList[i].y));
		}
		StartCoroutine(generateGeometry());
	}

	public void makeBlock(int[,] map, int flag)
	{
		Transform tf;

		int length1 = map.GetLength(0);
		int length2 = map.GetLength(1);
		for(int i = 0; i < length1; i++)
		{
			for(int j = 0; j < length2; j++)
			{
				if(map[i, j] == Config.FLAG_TAIL || map[i, j] == flag)
				{
					tf = spawn();
					tileArray[i, j] = tf;
					tf.SetParent(_root);
					tf.localPosition = new Vector3(i*unit, j*unit, 0);
					_isEmpty[i, j] = Config.FLAG_TILE;
					tilelist.Add(new Coord(i, j));
				}
			}
		}
		StartCoroutine(generateGeometry());
	}

	public void onTouchTile(List<Coord> tailIdx)
	{
//		System.Array.Copy(_isEmpty, map, _isEmpty.Length);
//		for(int i = 0; i < tailIdx.Count; i++)
//			map[tailIdx[i].x, tailIdx[i].y]=Config.FLAG_TAIL;
		Coord startIdx = null;
		for(int i = 0; i < tailIdx.Count; i++)
		{
			startIdx = tailIdx[i];
			if(startIdx.x > 0 && map[startIdx.x-1, startIdx.y] == 0)
			{
				startIdx = new Coord(startIdx.x-1, startIdx.y);
				break;
			}
			else if(startIdx.x + 1 < Config.width && map[startIdx.x+1, startIdx.y] == 0)
			{
				startIdx = new Coord(startIdx.x+1, startIdx.y);
				break;
			}
			else if(startIdx.y > 0 && map[startIdx.x, startIdx.y - 1] == 0)
			{
				startIdx = new Coord(startIdx.x, startIdx.y-1);
				break;
			}
			else if(startIdx.y+1 < Config.height && map[startIdx.x, startIdx.y+1] == 0)
			{
				startIdx = new Coord(startIdx.x, startIdx.y+1);
				break;
			}
		}

		if(startIdx == null)
		{
			makePath(tailIdx);
			return;
		}

		tileCount1 = mark(startIdx, 2);

		for(int i = 0; i < tailIdx.Count; i++)
		{
			startIdx = tailIdx[i];
			if(startIdx.x > 0 && map[startIdx.x-1, startIdx.y] == 0)
			{
				startIdx = new Coord(startIdx.x-1, startIdx.y);
				break;
			}
			else if(startIdx.x + 1 < Config.width && map[startIdx.x+1, startIdx.y] == 0)
			{
				startIdx = new Coord(startIdx.x+1, startIdx.y);
				break;
			}
			else if(startIdx.y > 0 && map[startIdx.x, startIdx.y - 1] == 0)
			{
				startIdx = new Coord(startIdx.x, startIdx.y-1);
				break;
			}
			else if(startIdx.y+1 < Config.height && map[startIdx.x, startIdx.y+1] == 0)
			{
				startIdx = new Coord(startIdx.x, startIdx.y+1);
				break;
			}
		}

		if(startIdx == null)
		{
			makePath(tailIdx);
			return;
		}

		tileCount2 = mark(startIdx, 3);
		if(tileCount1 == tileCount2)
		{
			bool tile1 = GameController.instance.hasMst(2, map);
			bool tile2 = GameController.instance.hasMst(3, map);
			if(!tile1)
				makeBlock(map, 2);
			else if(!tile2)
				makeBlock(map, 3);
			else
				makePath(tailIdx);

		}
		else if(tileCount1 < tileCount2)
		{
			if(GameController.instance.hasMst(2, map))
				makePath(tailIdx);
			else
				makeBlock(map, 2);
		}
		else
		{
			if(GameController.instance.hasMst(3, map))
				makePath(tailIdx);
			else
				makeBlock(map, 3);
			}


	}

	private int tileCount1 = 0;
	private int tileCount2 = 0;
	private int mark(Coord startIdx, int flag)
	{
		Queue<Coord> tofill = new Queue<Coord>();
		tofill.Enqueue(startIdx);
		int val;
		int count = 0;
		while(tofill.Count > 0)
		{
			startIdx = tofill.Dequeue();
			val = startIdx.x-1;
			if(val >= 0 && GameController.instance.isEmpty(val, startIdx.y) == 0 && map[val, startIdx.y] == 0)
			{
				map[val, startIdx.y] = flag;
				tofill.Enqueue(new Coord(val, startIdx.y));
				count++;
			}

			val = startIdx.x+1;
			if(val < Config.width && GameController.instance.isEmpty(val, startIdx.y) == 0 && map[val, startIdx.y] == 0)
			{
				map[val, startIdx.y] = flag;
				tofill.Enqueue(new Coord(val, startIdx.y));
				count++;
			}

			val = startIdx.y - 1;
			if(val >= 0 && GameController.instance.isEmpty(startIdx.x, val) == 0 && map[startIdx.x, val] == 0)
			{
				map[startIdx.x, val] = flag;
				tofill.Enqueue(new Coord(startIdx.x, val));
				count++;
			}

			val = startIdx.y + 1;
			if(val < Config.height && GameController.instance.isEmpty(startIdx.x, val) == 0 && map[startIdx.x, val] == 0)
			{
				map[startIdx.x, val] = flag;
				tofill.Enqueue(new Coord(startIdx.x, val));
				count++;
			}
		}

		return count;
	}

	public void clear()
	{
		_isEmpty = new int[Config.width,Config.height];
		int lengthX = tileArray.GetLength(0);
		int lengthY = tileArray.GetLength(1);
		for(int i = 0; i < lengthX; i++)
		{
			for(int j = 0; j < lengthY; j++)
			{
				Despawn(tileArray[i, j]);
				tileArray[i, j] = null;
			}
		}
//		col.GenerateGeometry();
	}

	private Transform spawn()
	{
		Transform ins = tilePool.Dequeue();
		ins.gameObject.SetActive(true);
		return ins;
	}

	private void Despawn(Transform ins)
	{
		tilePool.Enqueue(ins);
		ins.gameObject.SetActive(false);
	}

	System.Collections.IEnumerator generateGeometry()
	{
		yield return null;
		col.GenerateGeometry();
	}
}
