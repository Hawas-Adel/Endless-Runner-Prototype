using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	[SerializeField] [Scene] private string StartSceneREF = default;

	[SerializeField] private RectTransform MainMenuREF = default;
	[SerializeField] private RectTransform SettingsMenuREF = default;

	public void GoToScene() => SceneManager.LoadScene(StartSceneREF);
	public void MainMenuToSettings()
	{
		MainMenuREF.gameObject.SetActive(false);
		SettingsMenuREF.gameObject.SetActive(true);
	}
	public void SettingsToMainMenu()
	{
		SettingsMenuREF.gameObject.SetActive(false);
		MainMenuREF.gameObject.SetActive(true);
	}
	public void Exit()
	{
		Application.Quit();
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif
	}
}
