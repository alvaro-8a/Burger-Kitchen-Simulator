using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
	// Constant to save Player Preferences
	private const string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume";

	/** SINGLETON PATTERN **/
	public static MusicManager Instance { get; private set; }

	private AudioSource audioSource;
	private float volume = .3f;

	private void Awake()
	{
		Instance = this;

		audioSource = GetComponent<AudioSource>();

		// Get previous Player Preferences
		volume = PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME, .3f);
		audioSource.volume = volume;
	}

	public void ChangeVolume()
	{
		volume += .1f;
		if (volume >= 1.1f)
		{
			volume = 0f;
		}
		audioSource.volume = volume;

		PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, volume);
		PlayerPrefs.Save();
	}

	public float GetVolume()
	{
		return volume;
	}
}
