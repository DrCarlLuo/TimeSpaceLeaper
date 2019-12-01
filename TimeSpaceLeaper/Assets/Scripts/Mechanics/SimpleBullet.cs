using System;
using System.Collections.Generic;
using UnityEngine;
using LAB.ManagedTime;

namespace LAB.Mechanics
{
    class SimpleBullet : ManagedMonoBehaviour
    {
        public bool DefaultToLeft = true;
        Rigidbody2D rb;
        Collider2D mainCollider;
        Animator animator;
        SpriteRenderer spriteRenderer;
        bool Exploded = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            mainCollider = GetComponent<CircleCollider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            this.ManagedDestroy(gameObject, 30f);
        }

        private void Update()
        {
            if (rb.velocity.x < 0) spriteRenderer.flipX = !DefaultToLeft;
            if (rb.velocity.x > 0) spriteRenderer.flipX = DefaultToLeft;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.isTrigger && !Exploded)
            {
                animator.SetTrigger("hit");
                this.ManagedDestroy(gameObject, 0.6f);
                //mainCollider.enabled = false;
                rb.velocity = Vector2.zero;
                var pawn = collision.gameObject.GetComponent<PawnController>();
                if (pawn != null)
                    pawn.Kill(this.transform);
                Exploded = true;
            }
        }

        public override void ExposeTimeData()
        {
            base.ExposeTimeData();
            //mainCollider.enabled = TimeScribe.Look_Property(mainCollider.enabled);
            TimeScribe.Look_Value(ref Exploded);
        }
    }
}
