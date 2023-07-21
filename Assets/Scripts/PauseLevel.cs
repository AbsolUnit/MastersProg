using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PauseLevel : MonoBehaviour
{
	[SerializeField]
	private GameObject pauseMenu;

	public void ExitGame()
	{
		Application.Quit();
	}

	public void GoBack()
	{
		pauseMenu.SetActive(false);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (!pauseMenu.activeSelf)
			{
				pauseMenu.SetActive(true);
			}
			else
			{
				pauseMenu.SetActive(false);
			}
			
		}

		if (pauseMenu.activeSelf)
		{
			Time.timeScale = 0;
		}
		else
		{
			Time.timeScale = 1;
		}
	}
}
