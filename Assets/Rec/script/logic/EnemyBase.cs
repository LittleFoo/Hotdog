using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour {

	protected float _speed;
	public float speed{set{_speed = value; }}


	void OnTriggerEnter2D(Collider2D coll)
	{

		if(coll.gameObject.tag == "Head" || coll.gameObject.tag == "Tail")
		{
			GameController.instance.dead();
			return;
		}
	}

	public virtual void pause()
	{
		
	}

	public virtual void resume()
	{
	}

	public virtual void clear()
	{
	}
}
