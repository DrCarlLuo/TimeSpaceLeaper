using LAB.Mechanics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LAB.ManagedTime;

namespace LAB.AI
{
    public class SimplePatrolAI : ManagedMonoBehaviour
    {
        public SimplePath Path;
        public float Speed = 5f;
        public SpriteRenderer mainRenderer;
        public bool DefaultToRight = true;
        public float BreakingRadius = 0.1f;
        public Animator animator;

        PawnController pawnController;
        Rigidbody2D rb;
        bool started;
        float remainingWaitingTime;
        Vector2 movement;

        protected void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            pawnController = GetComponent<PawnController>();
        }

        void Start()
        {
            started = false;
            if (Path.NodeCnt > 0)
                Init();
        }

        void Init()
        {
            Path.Init();
            movement = Vector2.zero;
            started = true;
            remainingWaitingTime = 0f;
        }

        void Update()
        {
            animator.SetFloat("HorizontalSpeed", movement.x);
        }

        void FixedUpdate()
        {
            movement = Vector2.zero;
            if (Path.NxtPos == -1)
                started = false;
            if (!started)
                return;
            remainingWaitingTime -= Time.deltaTime;
            if (remainingWaitingTime >= 0f)
                return;
            movement = new Vector2(Path.NxtNode.position.x - rb.position.x,0);

            if (Mathf.Abs(movement.x) <= BreakingRadius)
            {
                remainingWaitingTime = Path.CurStageTime;
                Path.MoveNext();
            }
            else
            {
                movement = movement.normalized * Speed;
                pawnController.Move(movement);
                UpdateFacing(movement);
            }
        }

        void UpdateFacing(Vector2 movement)
        {
            if (movement.x > 0) mainRenderer.flipX = !DefaultToRight;
            else if (movement.x < 0) mainRenderer.flipX = DefaultToRight;
        }

        #region Time Interfaces
        public override void ExposeTimeData()
        {
            base.ExposeTimeData();
            TimeScribe.Look_Sub(Path);
            rb.position = TimeScribe.Look_Property(rb.position);
            rb.velocity = TimeScribe.Look_Property(rb.velocity);
            TimeScribe.Look_Value(ref started);
            TimeScribe.Look_Value(ref remainingWaitingTime);
            TimeScribe.Look_Value(ref movement);
            mainRenderer.flipX = TimeScribe.Look_Property(mainRenderer.flipX);
        }
        #endregion
    }

}
