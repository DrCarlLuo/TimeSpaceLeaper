using System;
using System.Collections.Generic;
using UnityEngine;
using LAB.ManagedTime;

namespace LAB.Mechanics
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class SimpleMovingPlatform : ManagedMonoBehaviour, IVelocityRelay
    {
        public SimplePath Path;
        public float Speed = 2f;

        public Vector2 Velocity { get; private set; }
        Rigidbody2D rb;
        bool started;
        float remainingWaitingTime;

        private bool Arrived => Mathf.Approximately(rb.position.x, Path.NxtNode.position.x) && Mathf.Approximately(rb.position.y, Path.NxtNode.position.y);

        public Vector2 GetRelayedVelocity() => Velocity;

        protected void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        void Start()
        {
            started = false;
            Velocity = Vector2.zero;
            if (Path.NodeCnt > 0)
                Init();
        }

        void Init()
        {

            Path.Init();
            started = true;
            remainingWaitingTime = 0f;
        }

        void FixedUpdate()
        {
            if (Path.NxtPos == -1)
                started = false;
            if (!started)
            {
                Velocity = Vector2.zero;
                return;
            }
            remainingWaitingTime -= Time.deltaTime;
            if (remainingWaitingTime >= 0f)
                return;
            Vector2 movement = Vector2.MoveTowards(rb.position, Path.NxtNode.position, Speed * Time.deltaTime);
            Velocity = (movement - rb.position) / Time.deltaTime;
            rb.MovePosition(movement);
            if (Arrived)
            {
                remainingWaitingTime = Path.CurStageTime;
                Path.MoveNext();
            }
        }

        public override void ExposeTimeData()
        {
            base.ExposeTimeData();
            TimeScribe.Look_Sub(Path);
            TimeScribe.Look_Value(ref started);
            TimeScribe.Look_Value(ref remainingWaitingTime);
            rb.position = TimeScribe.Look_Property(rb.position);
            Velocity = TimeScribe.Look_Property(Velocity);
        }
    }

}
