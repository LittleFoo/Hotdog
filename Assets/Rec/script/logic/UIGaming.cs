using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGaming : MonoBehaviour {

	public Text lifeTxt;
	public Text proceedTxt;
	public GameObject PauseBtn;
	public GameObject RestartBtn;

	public void init(int life)
	{
		setLife(life);
		setPercent(0);
		RestartBtn.SetActive(false);
	}

	public  void setLife(int life)
	{
		lifeTxt.text = "life:"+ life.ToString();
	}

	public void setPercent(float per)
	{
		proceedTxt.text = "proceed:"+((int)(per*100)).ToString()+"%";
	}

	public void showPauseBtn()
	{
		PauseBtn.SetActive(true);
	}

	public void hidePauseBtn()
	{
		PauseBtn.SetActive(false);
	}

	public void showContinueBtn()
	{
		RestartBtn.SetActive(true);
	}

	public void hideContinueBtn()
	{
		RestartBtn.SetActive(false);
	}

	public void onPauseClick()
	{
		

	}

	public void onContinueClick()
	{
		hideContinueBtn();
		GameController.instance.onContinue();

	}

	public void showFailView()
	{
		
	}

	public void hideFailView()
	{
	}
}
