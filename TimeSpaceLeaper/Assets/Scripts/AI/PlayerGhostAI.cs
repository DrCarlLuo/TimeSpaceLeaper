using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using LAB.Mechanics;
using LAB.ManagedTime;
using LAB.VFX;

namespace LAB.AI
{
    public class PlayerGhostAI : MonoBehaviour
    {
        public Vector2 moveVector;
        public float groundAcceleration = 100f;
        public float groundDeceleration = 100f;
        public bool DefaultToRight = true;
        public float ExistTime = 6f;

        public GameObject MainTexture;
        public GameObject Text;
        PawnController pawnController;
        Animator animator;
        SpriteRenderer mainSpriteRenderer;
        Rigidbody2D rb;
        DissolveEffectHelper dissolveEffectHelper;

        public float LatestDestroyTime = -100;

        private void OnEnable()
        {
            ++PlayerGhostManager.Instance.EnabledGhostCnt;
        }

        private void OnDisable()
        {
            --PlayerGhostManager.Instance.EnabledGhostCnt;
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            pawnController = GetComponent<PawnController>();
            animator = MainTexture.GetComponent<Animator>();
            mainSpriteRenderer = MainTexture.GetComponent<SpriteRenderer>();
            dissolveEffectHelper = MainTexture.GetComponent<DissolveEffectHelper>();
        }

        private void Update()
        {
            moveVector.x = Mathf.MoveTowards(moveVector.x, 0, groundDeceleration * Time.deltaTime);
            UpdateAnimatorParam();
            UpdateFacing();
            if (!TimeStateManager.Instance.IsPreview && LatestDestroyTime > 0)
            {
                if (LatestDestroyTime + ExistTime < TimeStateManager.Instance.FakeTime)
                    LatestDestroyTime = TimeStateManager.Instance.FakeTime - ExistTime;
                //begin dissolving 1 second before
                if (LatestDestroyTime - 2f <= TimeStateManager.Instance.FakeTime)
                {
                    pawnController.Kill();
                    dissolveEffectHelper.BeginDissolve(2f);
                }
                if (LatestDestroyTime <= TimeStateManager.Instance.FakeTime)
                    this.ManagedDestroy(gameObject);
            }
        }

        private void FixedUpdate()
        {
            pawnController.Move(moveVector);
        }

        public void StartCountDown()
        {
            var comp = Text.GetComponent<UI.GhostCountDown>();
            LatestDestroyTime = TimeStateManager.Instance.FakeTime + ExistTime;
            comp.StartCoundDown();
        }

        void UpdateAnimatorParam()
        {
            if (!pawnController.Alive)
                animator.SetTrigger("Dead");
            animator.SetFloat("HorizontalSpeed", moveVector.x);
            animator.SetFloat("VerticalSpeed", rb.velocity.y);
            animator.SetBool("IsGrounded", pawnController.IsGrounded);
        }

        void UpdateFacing()
        {
            if (moveVector.x > 0)
                mainSpriteRenderer.flipX = !DefaultToRight;
            else if (moveVector.x < 0)
                mainSpriteRenderer.flipX = DefaultToRight;
        }
    }
}
