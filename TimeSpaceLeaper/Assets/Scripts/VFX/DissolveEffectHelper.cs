using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LAB.ManagedTime;

namespace LAB.VFX
{
    class DissolveEffectHelper : ManagedMonoBehaviour
    {
        SpriteRenderer spriteRenderer;
        Animator animator;

        float duration;
        float curTime;
        bool beginDissolving;
        Vector4 uvVec;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            beginDissolving = false;
        }

        private void Start()
        {
            spriteRenderer.material.SetFloat("_Amount", 0f);
        }

        private void Update()
        {
            Sprite sprite = spriteRenderer.sprite;
            uvVec = new Vector4(sprite.textureRect.min.x / sprite.texture.width, sprite.textureRect.min.y / sprite.texture.height, sprite.textureRect.max.x / sprite.texture.width, sprite.textureRect.max.y / sprite.texture.height);
            spriteRenderer.material.SetVector("_Rect", uvVec);
            if(beginDissolving)
            {
                animator.enabled = false;
                curTime += Time.deltaTime;
                float progress = Mathf.Clamp01(curTime / duration);
                spriteRenderer.material.SetFloat("_Amount", progress);
            }
        }

        public void BeginDissolve(float d)
        {
            if (beginDissolving) return;
            duration = d;
            curTime = 0f;
            beginDissolving = true;
        }

        public override void ExposeTimeData()
        {
            base.ExposeTimeData();
            TimeScribe.Look_Value(ref duration);
            TimeScribe.Look_Value(ref curTime);
            TimeScribe.Look_Value(ref beginDissolving);
        }
    }
}
