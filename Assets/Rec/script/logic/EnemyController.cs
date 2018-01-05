using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;

public class EnemyController : MonoBehaviour {
	
	private Transform _root;
	private SpawnPool _pool;
	private PrefabPool pp;

	private List<Transform> mstList = new List<Transform>();
	private Vector2[] direction = {
		new Vector2(1, 2), new Vector2(2, 1), 
		new Vector2(1, -2), new Vector2(2, -1), 
		new Vector2(-1, -2), new Vector2(-2, -1),
		new Vector2(-1, 2), new Vector2(-2, 1), };

	private int _darkCount = 0;
	public int darkCount{
		get{ return _darkCount;}
	}
	void Start () {
//		_pool = GlobalController.instance.getCurPool();
//		pp = new PrefabPool(GlobalController.instance.prefabSetting.enemyBase);
//		_pool.CreatePrefabPool(pp);
//		pp.preloadAmount = 10;
//		pp.limitInstances = false;
	}

	public void init(Transform root)
	{
		_root = root;
	}

	public void gernerateMst(MstType mstType, int amount = 1)
	{
		switch(mstType)
		{
		case MstType.randomWalk:
			generateBase(amount, GlobalController.instance.prefabSetting.enemyBase);
			break;

		case MstType.darkRandom:
//			generateEdge(amount);
			break;

		case MstType.darkEdge:
			generateEdge(amount);
			break;

		case MstType.breaker:
			generateBase(1, GlobalController.instance.prefabSetting.breaker);
			break;
		}
	}

	public void generateDark(Coord coord)
	{
		_darkCount++;
		Transform enm;
		enm = GlobalController.instance.getCurPool().Spawn(GlobalController.instance.prefabSetting.randomDark);
		enm.SetParent(_root);
		enm.localPosition = new Vector3(coord.x*Config.unit, coord.y*Config.unit, 0);
		int idx = Random.Range(0, direction.Length-1);
		enm.GetComponent<EnemyBase>().speed = Mathf.Pow(direction[idx].x*58, 2) + Mathf.Pow( direction[idx].y*58, 2);
		enm.GetComponent<Rigidbody2D>().velocity = new Vector2(direction[idx].x*58, direction[idx].y*58);
		mstList.Add(enm);

	}

	public void generateEdge(int amount)
	{
		Transform enm;
		for(int i = 0; i < amount; i++)
		{
			enm = GlobalController.instance.getCurPool().Spawn(GlobalController.instance.prefabSetting.edgeMst);
			enm.SetParent(_root);
			enm.GetComponent<EdgeMst>().init();
			mstList.Add(enm);
		}
	}



	public void generateBase(int amount, Transform prefab)
	{
		int x, y;
		int idx = Random.Range(0, direction.Length-1);
		Transform enm;
		for(int i = 0; i < amount; i++)
		{
			x = Random.Range(1, Config.width-5)*Config.unit;
			y = Random.Range(1, Config.height-8)*Config.unit;

			enm = GlobalController.instance.getCurPool().Spawn(prefab);
			enm.SetParent(_root);
			enm.localPosition = new Vector3(x, y, 0);
			enm.tag = "Enemy";
			enm.GetComponent<EnemyBase>().speed = Mathf.Pow(direction[idx].x*58, 2) + Mathf.Pow( direction[idx].y*58, 2);;
			enm.GetComponent<Rigidbody2D>().velocity = new Vector2(direction[idx].x*58, direction[idx].y*58);
			mstList.Add(enm);

			idx+= 3;
			idx = idx%direction.Length;
		}
	}

	public bool hasMst(int tileFlag, int[,] map)
	{
		Transform e;
		int x, y;
		for(int i = 0; i < mstList.Count; i++)
		{
			e = mstList[i];
			x = (int)(e.localPosition.x/Config.unit);
			y = (int)(e.localPosition.y/Config.unit);
			if(x < 0 || x >= map.Length|| y < 0 || y >= map.GetLength(1))
				continue;
			if(map[x,y] == tileFlag || map[x,y] == Config.FLAG_TAIL)
				return true;
		}

		return false;
	}


	public void pause()
	{
		for(int i = 0; i < mstList.Count; i++)
		{
			mstList[i].GetComponent<EnemyBase>().pause();
		}
	}

	public void resume()
	{
		for(int i = 0; i < mstList.Count; i++)
		{
			mstList[i].GetComponent<EnemyBase>().resume();
		}
	}

	public void clear()
	{
		_pool = GlobalController.instance.getCurPool();
		for(int i = 0; i < mstList.Count; i++)
		{
			mstList[i].GetComponent<EnemyBase>().clear();
			_pool.Despawn(mstList[i]);
		}
		mstList.Clear();
	}
}
