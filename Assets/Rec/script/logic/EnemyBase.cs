using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour {

	protected float _speed;
	public float speed{set{_speed = value; }}
	protected bool _isReverse = false;

	void OnTriggerEnter2D(Collider2D coll)
	{
		if(!_isReverse && coll.gameObject.tag == "Head" || coll.gameObject.tag == "Tail")
		{
			GameController.instance.dead();
			return;
		}
	}

	public virtual void reverse()
	{
		_isReverse = true;
		Color c = transform.GetComponent<SpriteRenderer>().color;
		transform.GetComponent<SpriteRenderer>().color = new Color(c.r, c.g, c.b, 0.3f);
	}

	public virtual void stopReverse()
	{
		_isReverse = false;
		Color c = transform.GetComponent<SpriteRenderer>().color;
		transform.GetComponent<SpriteRenderer>().color = new Color(c.r, c.g, c.b, 1);
	}

	public virtual void changeSpeed(float rate)
	{

	}

	public virtual void resumeSpeed()
	{

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
