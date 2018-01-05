using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breaker : RigidEnemy {



	void OnCollisionEnter2D(Collision2D coll) {

		
		Transform tile = coll.transform;

		Vector2 vec = coll.contacts[0].point;
		if(tile.tag == Config.TAG_TILE)
		{
			vec= tile.InverseTransformVector(new Vector3(vec.x, vec.y, 0));
			GameController.instance.breakTile(vec.x +342, vec.y+612);
		}

		_isVoChanged = true;
	}
}
