using System.Collections;
using DG.Tweening;
using UnityEngine;

public class MusicController : SingletonBehaviour
{
	[Range(0, 1)]
	[SerializeField] private float VolumeLevel;
	[SerializeField] private AudioSource[] musicSource;
	[SerializeField] private AudioClip[] peaceful;
	[SerializeField] private AudioClip panic;
	[SerializeField] private AudioClip sad;
	private int primaryMusicSource = 0;
	private int peacefulIndex = 0;
	private bool isSwitchingClip;

	private void Awake()
	{
		SnakeSpawnSystem.OnSnakeSpawned += SwitchToPanic;
		SnakeSpawnSystem.OnSnakeGone += SetNewPeaceful;
		GameOverSystem.OnGameOver += GameOver;
		GameController.OnGameReset += Reset;
		StartCoroutine(DetectSilence());
	}
	
	private void OnDestroy()
	{
		SnakeSpawnSystem.OnSnakeSpawned -= SwitchToPanic;
		SnakeSpawnSystem.OnSnakeGone -= SetNewPeaceful;
		GameController.OnGameReset -= Reset;
		GameOverSystem.OnGameOver -= GameOver;
	}

	private IEnumerator DetectSilence()
	{
		yield return new WaitForSeconds(1);
		while (true)
		{
			yield return new WaitForSeconds(1);
			if(!isSwitchingClip && !musicSource[primaryMusicSource].isPlaying)
				SetNewPeaceful();
		}
	}
	
	private void SwitchToPanic()
	{
		StartCoroutine(MuteAndPlay(panic));
		musicSource[primaryMusicSource].loop = true;
	}

	private IEnumerator MuteAndPlay(AudioClip newClip)
	{
		isSwitchingClip = true;
		int secondary = primaryMusicSource == 0 ? 1:0;
		musicSource[primaryMusicSource].DOFade(0, 3);
		musicSource[secondary].PlayOneShot(newClip);
		musicSource[secondary].DOFade(VolumeLevel, 5);
		yield return new WaitForSeconds(5);
		musicSource[primaryMusicSource].Stop();
		musicSource[primaryMusicSource].loop = false;
		primaryMusicSource = secondary;
		isSwitchingClip = false;
	}

	private void SetNewPeaceful()
	{
		peacefulIndex++;
		if (peacefulIndex == peaceful.Length)
			peacefulIndex = 0;
		StartCoroutine(MuteAndPlay(peaceful[peacefulIndex]));
	}

	private void GameOver()
	{
		StartCoroutine(MuteAndPlay(sad));
	}
	
	private void Reset()
	{
		StartCoroutine(MuteAndPlay(peaceful[0]));
	}
}
