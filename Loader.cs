using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
	// Game Scenes Type
	public enum Scene
	{
		MainMenuScene,
		GameScene,
		LoadingScene
	}

	// Scene to load after LoadingScene
	private static Scene targetScene;

	public static void Load(Scene targetScene)
	{
		Loader.targetScene = targetScene;
		SceneManager.LoadScene(Scene.LoadingScene.ToString());
	}

	public static void LoaderCallback()
	{
		SceneManager.LoadScene(targetScene.ToString());
	}
}
