using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	// Constant to save Player Preferences
	private const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "SoundEffectsVolume";

	/** SINGLETON PATTERN **/
	public static SoundManager Instance { get; private set; }

	// Reference to a Scriptable Object with the audio clips references
	[SerializeField] private AudioClipRefsSO audioClipRefsSO;

	private float volume = 1f;

	private void Awake()
	{
		Instance = this;

		// Get previous Player Preferences
		volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, 1f);
	}

	private void Start()
	{
		// Events subscription
		DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
		DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;
		CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
		Player.Instance.OnPickedSomething += Player_OnPickedSomething;
		BaseCounter.OnAnyObjectPlacedHere += BaseCounter_OnAnyObjectPlacedHere;
		TrashCounter.onAnyObjectTrashed += TrashCounter_OnAnyObjectTrashed;
	}

	private void TrashCounter_OnAnyObjectTrashed(object sender, System.EventArgs e)
	{
		TrashCounter trashCounter = sender as TrashCounter;
		PlaySound(audioClipRefsSO.trash, trashCounter.transform.position);
	}

	private void BaseCounter_OnAnyObjectPlacedHere(object sender, System.EventArgs e)
	{
		BaseCounter baseCounter = sender as BaseCounter;
		PlaySound(audioClipRefsSO.objectDrop, baseCounter.transform.position);
	}

	private void Player_OnPickedSomething(object sender, System.EventArgs e)
	{
		PlaySound(audioClipRefsSO.objectPickup, Player.Instance.transform.position);
	}

	private void CuttingCounter_OnAnyCut(object sender, System.EventArgs e)
	{
		CuttingCounter cuttingCounter = sender as CuttingCounter;
		PlaySound(audioClipRefsSO.chop, cuttingCounter.transform.position);
	}

	private void DeliveryManager_OnRecipeSuccess(object sender, System.EventArgs e)
	{
		// DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
		PlaySound(audioClipRefsSO.deliverySuccess, Vector3.zero);
	}

	private void DeliveryManager_OnRecipeFailed(object sender, System.EventArgs e)
	{
		// DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
		PlaySound(audioClipRefsSO.deliveryFail, Vector3.zero);
	}

	private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volumen = 1f)
	{
		PlaySound(audioClipArray[Random.Range(0, audioClipArray.Length)], position, volumen);

	}
	private void PlaySound(AudioClip audioClip, Vector3 position, float volumeMultiplier = 1f)
	{
		AudioSource.PlayClipAtPoint(audioClip, position, volumeMultiplier * volume);

	}

	public void PlayFootstepsSound(Vector3 position, float volume)
	{
		PlaySound(audioClipRefsSO.footstep[Random.Range(0, audioClipRefsSO.footstep.Length)], position, volume);
	}

	public void PlayCountdownSound()
	{
		PlaySound(audioClipRefsSO.warning, Vector3.zero);
	}

	public void PlayWarningSound(Vector3 position)
	{
		PlaySound(audioClipRefsSO.warning, position);
	}

	public void ChangeVolume()
	{
		volume += .1f;
		if (volume >= 1.1f)
		{
			volume = 0f;
		}

		// Save Player Preferences
		PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, volume);
		PlayerPrefs.Save();
	}

	public float GetVolume()
	{
		return volume;
	}
}
