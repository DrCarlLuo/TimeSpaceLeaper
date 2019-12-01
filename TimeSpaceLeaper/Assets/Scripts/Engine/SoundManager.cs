using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using LAB.ManagedTime;
using LAB.Mechanics;

namespace LAB.Engine
{
    enum SoundEnvironmentEnum
    {
        Undefined,
        Normal,
        TimeFreezed
    }

    class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; } = null;

        public AudioSource BGMSource;
        public GameObject SFXRoot;
        public bool IsBGMPlaying => BGMSource.isPlaying;
        public SoundEnvironmentEnum SFXEnvironment { get; set; } = SoundEnvironmentEnum.Normal;

        public AudioClip WalkSound;
        public AudioClip JumpSound;
        public AudioClip LandSound;
        public AudioClip FreezeInSound;
        public AudioClip FreezeOutSound;
        public AudioClip SwitchSound;
        public AudioClip SpikeHeadSound;
        public AudioClip DeathSound;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
            {
                Log.Warning("Try to create multiple SoundManager!");
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }

        public void SetBGMPlay(bool active)
        {
            if (active)
                BGMSource.Play();
            else
                BGMSource.Stop();
        }

        public SFXHandle PlaySFX(AudioClip clip, float volume, bool loop = false)
        {
            if (SFXEnvironment == SoundEnvironmentEnum.TimeFreezed)
                return null;
            GameObject go = new GameObject("SFX");
            go.transform.parent = SFXRoot.transform;
            var comp = go.AddComponent<AudioSource>();
            comp.loop = loop;
            comp.volume = volume;
            comp.PlayOneShot(clip);
            return go.AddComponent<SFXHandle>();
        }

        public SFXHandle PlaySFXIgnoreTimeFreeze(AudioClip clip, float volume, bool loop = false)
        {
            GameObject go = new GameObject("SFX");
            go.transform.parent = SFXRoot.transform;
            var comp = go.AddComponent<AudioSource>();
            comp.loop = loop;
            comp.volume = volume;
            comp.PlayOneShot(clip);
            var handle = go.AddComponent<SFXHandle>();
            handle.PlayableInTimeFreeze = true;
            return handle;
        }

        public void ClearAllSFX()
        {
            foreach (Transform child in SFXRoot.transform)
                child.GetComponent<SFXHandle>().StopRightNow();
        }

        public void ClearSFXForTimeFreeze()
        {
            foreach(Transform child in SFXRoot.transform)
            {
                var comp = child.GetComponent<SFXHandle>();
                if(!comp.PlayableInTimeFreeze)
                    child.GetComponent<SFXHandle>().StopRightNow();
            }
        }

        public void CrossFade(SFXHandle inClip, SFXHandle outClip, float fadeTime)
        {
            if(inClip == null || outClip == null)
                return;
            var coroutine = StartCoroutine(FadeCoroutine(inClip, outClip, fadeTime));
            inClip.FadeCoroutine = coroutine;
            outClip.FadeCoroutine = coroutine;
        }

        IEnumerator FadeCoroutine(SFXHandle inClip, SFXHandle outClip, float fadeTime)
        {
            float remainingTime = fadeTime;
            float inClipTarget = inClip.audioSource.volume;
            inClip.audioSource.volume = 0f;
            while (remainingTime >= 0f)
            {
                if (outClip != null)
                    outClip.audioSource.volume = Mathf.Lerp(outClip.audioSource.volume, 0f, Time.unscaledDeltaTime / remainingTime);
                if (inClip != null)
                    inClip.audioSource.volume = Mathf.Lerp(inClip.audioSource.volume, inClipTarget, Time.unscaledDeltaTime / remainingTime);
                remainingTime -= Time.unscaledDeltaTime;
                yield return null;
            }
            if (outClip != null)
                outClip.StopRightNow();
        }
    }
}
