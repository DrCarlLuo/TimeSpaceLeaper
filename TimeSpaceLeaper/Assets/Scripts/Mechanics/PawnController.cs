using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LAB.ManagedTime;
using LAB.Engine;

namespace LAB.Mechanics
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class PawnController : ManagedMonoBehaviour, IVelocityRelay
    {
        Rigidbody2D rb;
        BoxCollider2D mainCollider;
        Vector2 curMovement;

        public float MaximumVerticalSpeed = 26.4f;
        public float gravity = 50f;
        public float jumpSpeed = 22f;
        public float jumpAbortDeaccelaration = 100f;
        public bool jumpAborted;

        //Death hit effect
        public float deathHitVeloX = 2f;
        public float deathHitVeloY = 10f;
        public float deathHitAcce = 2f;
        float curHitBackVelo = 0f;

        public LayerMask groundedLayerMaks;
        public float groundedRaycastDistance = 0.1f;
        ContactFilter2D contactFilter2D;
        RaycastHit2D[] hitBuffer = new RaycastHit2D[5];
        RaycastHit2D[] foundHits = new RaycastHit2D[3];
        Collider2D[] groundColliders = new Collider2D[3];
        Vector2[] raycastPositions = new Vector2[3];

        public bool IsGrounded { get; protected set; } = true;
        public Vector2 RelativeVelocity { get; protected set; }
        public bool Alive { get; set; }
        public UnityEvent DeathAction;

        SFXHandle walkSoundHandle = null;
        float MaxFallingSpeedWhenAirBorne = 0f;

        public ParticleSystem Dust;

        #region Vitality
        public void Kill(Transform trans=null)
        {
            if (Alive)
            {
                Alive = false;
                DeathAction.Invoke();
                if(trans != null)
                {
                    HitBack(trans);
                }
            }
        }
        #endregion

        #region Motion Interface
        /// <summary>
        /// Apply a velocity to the Pawn. Should only be called in fixedupdate
        /// </summary>
        /// <param name="movement">The added velocity in global coordinates relative to the rigidbody2D's position.</param>
        public void Move(Vector2 movement)
        {
            curMovement += movement;
        }

        public void BeginJump()
        {
            if (rb.velocity.y >= jumpSpeed) return;
            var newVelo = rb.velocity;
            newVelo.y = jumpSpeed;// Set jump velocity instead addition
            rb.velocity = newVelo;
            //rb.velocity += new Vector2(0f, jumpSpeed);
            jumpAborted = false;
            IsGrounded = false;
            SoundManager.Instance.PlaySFX(SoundManager.Instance.JumpSound,1f);
        }

        public void EndJump()
        {
            jumpAborted = true;
        }

        public Vector2 GetRelayedVelocity() => rb.velocity;

        public void HitBack(Transform trans)
        {
            curHitBackVelo = trans.position.x > transform.position.x ? -1f : 1f;
            curHitBackVelo *= deathHitVeloX;
            var newVelo = rb.velocity;
            newVelo.y += deathHitVeloY;
            rb.velocity = newVelo;
        }
        #endregion

        #region Ground Check
        public void CheckBottomCollisions()
        {
            Vector2 raycastStart = rb.position + mainCollider.offset;
            float raycastDistance = mainCollider.size.x * 0.5f + groundedRaycastDistance * 2f;
            Vector2 raycastDirection = Vector2.down;
            Vector2 raycastStartBottomCentre = raycastStart + Vector2.down * (mainCollider.size.y * 0.5f - mainCollider.size.x * 0.5f);

            raycastPositions[0] = raycastStartBottomCentre + Vector2.left * mainCollider.size.x * 0.5f;
            raycastPositions[1] = raycastStartBottomCentre;
            raycastPositions[2] = raycastStartBottomCentre + Vector2.right * mainCollider.size.x * 0.5f;

            for (int i = 0; i < raycastPositions.Length; ++i)
            {
                int count = Physics2D.Raycast(raycastPositions[i], raycastDirection, contactFilter2D, hitBuffer, raycastDistance);
                foundHits[i] = count > 0 ? hitBuffer[0] : new RaycastHit2D();
                groundColliders[i] = foundHits[i].collider;
            }

            //Calculate ground Normal
            Vector2 groundNormal = Vector2.zero;
            for (int i = 0; i < foundHits.Length; i++)
            {
                if (foundHits[i].collider != null)
                    groundNormal += foundHits[i].normal;
            }
            groundNormal.Normalize();

            //Calculate Relative Velocity
            //Process velocity Relay
            Vector2 relativeVelocity = rb.velocity;
            relativeVelocity.x = curMovement.x;
            for (int i = 0; i < groundColliders.Length; ++i)
            {
                if (groundColliders[i] == null)
                    continue;
                //TODO: this cost a lot in performance, use a cache instead
                var comp = groundColliders[i].gameObject.GetComponent<IVelocityRelay>();
                if (comp != null)
                {
                    curMovement.x += comp.GetRelayedVelocity().x;
                    relativeVelocity.y -= comp.GetRelayedVelocity().y;
                    break;
                }
            }
            RelativeVelocity = relativeVelocity;

            bool orgIsGrounded = IsGrounded;

            //Check if landed
            //TODO: use a range
            if (Mathf.Approximately(groundNormal.x, 0f) && Mathf.Approximately(groundNormal.y, 0f))
            {
                IsGrounded = false;
            }
            else
            {
                IsGrounded = RelativeVelocity.y <= 0f;
                if (mainCollider != null)
                {
                    if (groundColliders[1] != null)
                    {
                        float capsuleBottomHeight = rb.position.y + mainCollider.offset.y - mainCollider.size.y * 0.5f;
                        float middleHitHeight = foundHits[1].point.y;
                        IsGrounded &= middleHitHeight < capsuleBottomHeight + groundedRaycastDistance;
                    }
                }
            }

            //Landing Sound & FX
            if (!orgIsGrounded && IsGrounded)
            {
                SoundManager.Instance.PlaySFX(SoundManager.Instance.LandSound, 0.3f * Mathf.Clamp01(MaxFallingSpeedWhenAirBorne / (-33)));
                CreateDust();
            }
            if (IsGrounded)
                MaxFallingSpeedWhenAirBorne = 0f;
            else if(RelativeVelocity.y < 0f)
                MaxFallingSpeedWhenAirBorne = Mathf.Min(MaxFallingSpeedWhenAirBorne, RelativeVelocity.y);

            for (int i = 0; i < hitBuffer.Length; i++)
                hitBuffer[i] = new RaycastHit2D();
        }
        #endregion

        #region monobehaviour
        protected void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            mainCollider = GetComponent<BoxCollider2D>();
            RelativeVelocity = Vector2.zero;

            contactFilter2D.layerMask = groundedLayerMaks;
            contactFilter2D.useLayerMask = true;
            contactFilter2D.useTriggers = false;
            Physics2D.queriesStartInColliders = false;

            Alive = true;
            DeathAction = new UnityEvent();
        }

        void FixedUpdate()
        {
            GravityProcessor();
            CheckBottomCollisions();
            HitBackVeloProcessor();
            curMovement.y = rb.velocity.y;
            rb.velocity = curMovement;
            curMovement.x = 0;
        }

        private void Update()
        {
            //Play Walk Sound
            if(IsGrounded && Mathf.Abs(RelativeVelocity.x) > float.Epsilon)
            {
                if(walkSoundHandle == null)
                    walkSoundHandle = SoundManager.Instance.PlaySFX(SoundManager.Instance.WalkSound,0.8f, true);
            }
            else
            {
                if(walkSoundHandle!=null)
                {
                    walkSoundHandle.StopRightNow();
                    walkSoundHandle = null;
                }
            }

        }

        void GravityProcessor()
        {
            if (!IsGrounded && jumpAborted && rb.velocity.y > 0)
                rb.velocity += new Vector2(0, -jumpAbortDeaccelaration) * Time.deltaTime;
            rb.velocity += new Vector2(0, -gravity) * Time.deltaTime;
        }

        void HitBackVeloProcessor()
        {
            if(Mathf.Abs(curHitBackVelo) > float.Epsilon)
            {
                Move(new Vector2(curHitBackVelo, 0f));
                curHitBackVelo = Mathf.MoveTowards(curHitBackVelo, 0f, deathHitAcce * Time.deltaTime);
            }
        }
        #endregion

        #region Time Interfaces
        public override void ExposeTimeData()
        {
            base.ExposeTimeData();
            rb.position = TimeScribe.Look_Property(rb.position);
            rb.velocity = TimeScribe.Look_Property(rb.velocity);
            TimeScribe.Look_Value(ref curMovement);
            TimeScribe.Look_Value(ref jumpAborted);
            IsGrounded = TimeScribe.Look_Property(IsGrounded);
            RelativeVelocity = TimeScribe.Look_Property(RelativeVelocity);
            Alive = TimeScribe.Look_Property(Alive);
            TimeScribe.Look_Value(ref curHitBackVelo);
        }
        #endregion

        void CreateDust()
        {
            Dust.Play();
        }
    }
}
