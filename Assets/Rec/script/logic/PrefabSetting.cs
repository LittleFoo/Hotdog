using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSetting : MonoBehaviour {

	public Transform tile;
	public Transform tail;
	public Transform enemyBase;
	public Transform randomDark;
	public Transform edgeMst;
	public Transform breaker;
	public void Awake()
	{
		GlobalController.instance.prefabSetting = this;
	}


}
