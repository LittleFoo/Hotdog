using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour {

	public UIGaming uiGaming;

	private static UIController _instance;
	public static UIController instance
	{
		get{
			return _instance;
		}
	}

	void Awake () {
		if(_instance != null) 
			return;
		_instance = this;
	}

	public void init(int life)
	{
		uiGaming.init(life);
	}


	public void showContinueView()
	{
		uiGaming.showContinueBtn();
	}

	public void showFailView()
	{
		uiGaming.showFailView();
	}
}
