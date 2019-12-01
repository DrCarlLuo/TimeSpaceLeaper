using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LAB.ManagedTime;
using LAB.Engine;
using EZCameraShake;
using UnityEngine.Rendering.PostProcessing;

namespace LAB.VFX
{
    class TheWorldEffectHelper : MonoBehaviour
    {
        public static TheWorldEffectHelper Instance { get; private set; }

        public GameObject Circle = null;
        public GameObject Freeze = null;
        public GameObject PostEffect = null;
        float fadeInDuration;
        float fadeOutDuration;
        Vector3 scaleTarget;
        [Range(0f,1f)]
        public float EnterPortion = 0.25f;
        [Range(0f, 1f)]
        public float LeavePortion = 0.25f;
        [Range(0,360)]
        public int targetHue = 360;
        [Range(0f,2f)]
        public float WarpSpeed = 0.4f;

        Material circleMaterial;
        float progress;
        bool begin = false;
        bool reverse = false;
        bool InWorld = false;
        CameraShakeInstance cameraShake;
        Coroutine fadeOutCoroutine;

        private void Awake()
        {
            Instance = this;
            circleMaterial = Circle.GetComponent<SpriteRenderer>().material;
        }

        private void Start()
        {
            Circle.SetActive(false);
            Freeze.SetActive(false);
            PostEffect.SetActive(false);
        }

        public void ZaWaLuDo(float _duration)
        {
            CalculateScaleVec();
            fadeInDuration = _duration;
            begin = true;
            InWorld = false;
            reverse = false;
            progress = 0f;
            cameraShake = CameraShaker.Instance.StartShake(3f, 4f, 0.25f);
            Circle.transform.position = GameManager.Instance.PlayerGO.transform.position;
            PostEffect.SetActive(true);
            Freeze.SetActive(false);
        }

        private void CalculateScaleVec()
        {
            float radius = Screen.width * Screen.width + Screen.height * Screen.height;
            radius = Mathf.Sqrt(radius);
            var sprite = Circle.GetComponent<SpriteRenderer>().sprite;

            //get world space size (this version handles rotating correctly)
            Vector2 sprite_size = sprite.rect.size;
            Vector2 local_sprite_size = sprite_size / sprite.pixelsPerUnit;
            Vector3 world_size = local_sprite_size;
            //world_size.x *= transform.lossyScale.x;
            //world_size.y *= transform.lossyScale.y;

            //convert to screen space size
            Vector3 screen_size = 0.5f * world_size / Camera.main.orthographicSize;
            screen_size.y *= Camera.main.aspect;

            //size in pixels
            Vector3 in_pixels = new Vector3(screen_size.x * Camera.main.pixelWidth, screen_size.y * Camera.main.pixelHeight, 0) * 0.5f;

            //Debug.Log(string.Format("World size: {0}, Screen size: {1}, Pixel size: {2}", world_size, screen_size, in_pixels));

            float ratio = radius / (in_pixels.x) * 2;
            scaleTarget = new Vector3(ratio, ratio, 1f);
        }

        public void ExitEffect(float fadeOutTime = 1f)
        {
            if(begin)
            {
                begin = false;
                reverse = true;
            }
            if (InWorld)
                fadeOutCoroutine = StartCoroutine(FadeOutCoroutine(fadeOutTime));
        }

        public void Update()
        {
            //Follow Camera
            Vector3 newPos = transform.position;
            newPos.x = Camera.main.transform.position.x;
            newPos.y = Camera.main.transform.position.y;
            transform.position = newPos;

            if(begin || reverse)
            {
                if (begin)
                    progress += Time.unscaledDeltaTime / fadeInDuration;
                else
                    progress -= Time.unscaledDeltaTime / fadeInDuration;
                progress = Mathf.Clamp01(progress);

                if (EnterPortion > float.Epsilon && progress < EnterPortion)
                {
                    Circle.SetActive(true);
                    Circle.transform.localScale = (progress / EnterPortion) * scaleTarget;
                }
                if (begin && progress > EnterPortion)
                {
                    Freeze.SetActive(true);
                }
                if(reverse && progress < EnterPortion)
                {
                    Freeze.SetActive(false);
                }
                if(LeavePortion > float.Epsilon && progress > 1 - LeavePortion)
                    Circle.transform.localScale = ((1f - progress) / LeavePortion) * scaleTarget;

                circleMaterial.SetFloat("_Offset", progress * WarpSpeed);
                circleMaterial.SetFloat("_Hue", progress * targetHue);
                if (begin && progress >= 0.99f)
                {
                    begin = false;
                    reverse = false;
                    InWorld = true;
                    Circle.SetActive(false);
                    cameraShake.StartFadeOut(2f);
                }
                else if(reverse && progress <= 0.01f)
                {
                    begin = false;
                    reverse = false;
                    InWorld = false;
                    Circle.SetActive(false);
                    cameraShake.StartFadeOut(1f);
                    PostEffect.SetActive(false);
                    Freeze.SetActive(false);
                }
            }
        }

        IEnumerator FadeOutCoroutine(float fadeOutTime)
        {
            Circle.SetActive(false);
            Material FreezeMat = Freeze.GetComponent<SpriteRenderer>().material;
            var Motion = PostEffect.GetComponent<PostProcessVolume>();
            Motion.profile.TryGetSettings<ChromaticAberration>(out var effect);
            float remainingTime = fadeOutTime;

            float orgIntensity = effect.intensity;
            float orgSaturation = FreezeMat.GetFloat("_Saturation");
            float orgValue = FreezeMat.GetFloat("_Value");

            while (remainingTime > 0)
            {
                float intensity = effect.intensity;
                float currentSaturation = FreezeMat.GetFloat("_Saturation");
                float currentValue = FreezeMat.GetFloat("_Value");
                currentSaturation = Mathf.Lerp(currentSaturation, 1.0f, Time.unscaledDeltaTime/ remainingTime);
                currentValue = Mathf.Lerp(currentValue, 1.0f, Time.unscaledDeltaTime/ remainingTime);
                FreezeMat.SetFloat("_Saturation", currentSaturation);
                FreezeMat.SetFloat("_Value", currentValue);
                effect.intensity.Interp(intensity, 0f, Time.unscaledDeltaTime / remainingTime);
                remainingTime -= Time.unscaledDeltaTime;
                yield return null;
            }

            FreezeMat.SetFloat("_Saturation", orgSaturation);
            FreezeMat.SetFloat("_Value", orgValue);
            effect.intensity.Override(orgIntensity);
            Freeze.SetActive(false);
            PostEffect.SetActive(false);
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
