using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitLevel : MonoBehaviour
{
	[SerializeField]
	private GameObject exitMenu;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			exitMenu.SetActive(true);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			exitMenu.SetActive(false);
		}
	}

	public void ExitGame()
	{
		Application.Quit();
	}

	public void GoToSurvey()
	{
		Application.OpenURL("http://www.google.com/");
		Application.Quit();
	}

	public void GoBack()
	{
		exitMenu.SetActive(false);
	}
}
