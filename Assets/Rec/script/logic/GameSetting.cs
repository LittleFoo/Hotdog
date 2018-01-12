using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetting : MonoBehaviour {
	public float heroBaseTime = 0.3f;
	public float heroBaseBackwardsTime = 0.2f;
	public float mstBaseSpeed = 150;
	public float mstBreakerSpeed = 100;
	public float mstEdgeSpeed = 150;
	public float bonusTime = 4;
	public float bonusSpeed = 20;
	public void Awake()
	{
		GlobalController.instance.setting = this;
	}
}
