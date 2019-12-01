using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LAB.ManagedTime;
using LAB.Engine;
using LAB.VFX;
using EZCameraShake;

namespace LAB.Mechanics
{
    //TODO: testing
    public class PlayerController : MonoBehaviour
    {
        protected static PlayerController instance;
        public static PlayerController Instance => instance;

        Rigidbody2D rb;
        public PawnController pawnController;
        public GameObject MainTexture;
        SpriteRenderer spriteRenderer;
        Animator animator;

        public Vector2 moveVector;
        public float groundAcceleration = 100f;
        public float groundDeceleration = 100f;
        public float maxSpeed = 10f;

        public float ContinueJumpInterval = 0.4f;
        public float CoyoteTime = 0.3f;
        private float LastSpacePressedTime;
        private float LastGroundedTime;

        public bool DefaultToRight = true;

        void Awake()
        {
            instance = this;
            rb = GetComponent<Rigidbody2D>();
            pawnController = GetComponent<PawnController>();
            spriteRenderer = MainTexture.GetComponent<SpriteRenderer>();
            animator = MainTexture.GetComponent<Animator>();
        }

        void Start()
        {
            moveVector = Vector2.zero;
            LastSpacePressedTime = -10000f;
            LastGroundedTime = -10000f;
            pawnController.DeathAction.AddListener(() =>{
                StartCoroutine(PlayerDeathCoroutine());
            });
        }

        void Update()
        {       
            float velo = 0f;
            if (pawnController.Alive)
            {
                //Jump Key buffer
                if (InputHandler.Jump.KeyDown)
                    LastSpacePressedTime = Time.time;
                //Coyote Time
                if (pawnController.IsGrounded)
                    LastGroundedTime = Time.time;

                if ((Time.time - LastGroundedTime <= CoyoteTime && InputHandler.Jump.KeyDown) ||
                    (pawnController.IsGrounded && Time.time - LastSpacePressedTime <= ContinueJumpInterval))
                {
                    pawnController.BeginJump();
                }

                if (!pawnController.jumpAborted && InputHandler.Jump.KeyUp)
                {
                    LastSpacePressedTime = -10000f;//avoid buffer the original jump if conducting a short jump
                    pawnController.EndJump();
                }
                velo = InputHandler.HorizontalAxis.Value * maxSpeed;
                //Todo: should check negative inputing
            }

            float acce = Mathf.Abs(velo) < float.Epsilon ?  groundDeceleration : groundAcceleration;
            moveVector.x = Mathf.MoveTowards(moveVector.x, velo, acce * Time.deltaTime);
            UpdateFacing(velo);
            UpdateAnimatorParam();

        }

        void FixedUpdate()
        {
            pawnController.Move(moveVector);
            //moveVector = Vector2.zero;
        }

        void UpdateFacing(float velo)
        {
            float axis = velo;
            if (axis > 0) spriteRenderer.flipX = !DefaultToRight;
            else if (axis < 0) spriteRenderer.flipX = DefaultToRight;
        }

        void UpdateAnimatorParam()
        {
            animator.SetFloat("HorizontalSpeed", moveVector.x);
            animator.SetFloat("VerticalSpeed", rb.velocity.y);
            animator.SetBool("IsGrounded", pawnController.IsGrounded);
        }

        public void DetachTimeRegister()
        {
            var tmp = gameObject.GetComponentsInChildren<ITimeManaged>();
            foreach (var c in tmp)
                TimeStateManager.Instance.Unregister(c);
        }

        public void RebuildTimeRegister()
        {
            var tmp = gameObject.GetComponentsInChildren<ITimeManaged>();
            foreach (var c in tmp)
                TimeStateManager.Instance.Register(c); 
        }

        public void ResetAllTimeStat()
        {
            var tmp = gameObject.GetComponentsInChildren<ITimeManaged>();
            foreach (var c in tmp)
                c.GetTimeStateContainer.Init();
        }

        public IEnumerator PlayerDeathCoroutine()
        {
            animator.SetTrigger("Dead");
            CameraShaker.Instance.ShakeOnce(5f, 10f, 0f, 1f, Vector3.one, Vector3.zero);
            SoundManager.Instance.PlaySFXIgnoreTimeFreeze(SoundManager.Instance.DeathSound, 1f);
            yield return new WaitForSeconds(1.5f);
            GameManager.Instance.ReloadCurrentScene();
        }
    }
}