using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RigidEnemy : EnemyBase {

	protected Rigidbody2D rigid;
	protected bool _isVoChanged = false;
	protected Vector2 _lastVelocity;
	protected Vector2 _vBeforeBonus;
	void Awake()
	{
		rigid = transform.GetComponent<Rigidbody2D>();
	}

	public override void changeSpeed(float rate)
	{
		_vBeforeBonus = rigid.velocity;
		rigid.velocity *= rate;
	}

	public override void resumeSpeed()
	{
		rigid.velocity = _vBeforeBonus;
	}

	public override void pause()
	{
		_lastVelocity = rigid.velocity;
		rigid.velocity = Vector2.zero;
	}

	public override void resume()
	{
		rigid.velocity = _lastVelocity;
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		_isVoChanged = true;
	}

	void FixedUpdate()
	{
		if(_isVoChanged)
		{
			float y = _speed-rigid.velocity.x*rigid.velocity.x;
			if(y < 0)
				y = 0;
			else
				y = Mathf.Sqrt(y);

			if(rigid.velocity.y > 0)
				rigid.velocity = new Vector2(rigid.velocity.x, y);
			else
				rigid.velocity = new Vector2(rigid.velocity.x, -y);
			_isVoChanged = false;
		}

	}

	public override void clear()
	{
		rigid.velocity = Vector2.zero;
		_isVoChanged = false;
	}
}
