using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using LAB.ManagedTime;
using LAB.Mechanics;
using LAB.Engine;

namespace LAB.AI
{
    enum SpikeHeadStateEnum
    {
        Idle,
        Charging,
        Staging,
        Returning,
        Preparing
    }

    class SpikeHeadAI : ManagedMonoBehaviour
    {
        public DirectionEnum Direction = DirectionEnum.Horizontal;
        public Transform PointA;
        public Transform PointB;
        public float Accelaration = 10f;

        public bool GoBackAfterCharged = true;
        public float ReturnSpeed = 2f;
        public bool DetectAfterReturned = true;
        public float ChargeCoolDown = 1f;
        public float DetectionInterval = 0.5f;
        public float StageTime = 1f;
        public float PrepareTime = 1f;
        public LayerMask DetectTargetMask;

        public GameObject ExclamationGO;

        SpikeHeadStateEnum curState;
        float curVelo;
        float remainingDetectTime = 0f;
        float remainingStageTime = 0f;
        float remainingChargeTime = 0f;
        float remainingPrepareTime = 0f;
        ContactFilter2D filter;
        Vector3 orgPosition;
        Vector2 ChargeTarget;
        float RayOffset;

        Rigidbody2D rb;
        RaycastHit2D[] hitBuffer = new RaycastHit2D[10];

        Animator animator;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            filter = new ContactFilter2D();
            filter.layerMask = DetectTargetMask;
            filter.useLayerMask = true;
            filter.useTriggers = false;
            animator = GetComponent<Animator>();
            RayOffset = GetComponent<BoxCollider2D>().size.y;
        }

        private void Start()
        {
            curState = SpikeHeadStateEnum.Idle;
            orgPosition = transform.position;
            ExclamationGO.SetActive(false);
        }

        Vector2 GetDirectionVec(Transform target)
        {
            Vector2 dif = target.position - transform.position;
            if (Direction == DirectionEnum.Horizontal)
                dif.y = 0f;
            else
                dif.x = 0f;
            return dif.normalized;
        }

        float GetDistance(Transform target)
        {
            Vector2 dif = target.position - transform.position;
            if (Direction == DirectionEnum.Horizontal)
                return Mathf.Abs(dif.x);
            else
                return Mathf.Abs(dif.y);
        }

        Vector2 GetChargeTarget(Transform target)
        {
            Vector2 res = target.position;
            if (Direction == DirectionEnum.Horizontal)
                res.y = transform.position.y;
            else
                res.x = transform.position.x;
            return res;
        }

        bool HasAlivePlayer(int hitCnt)
        {
            if (hitCnt <= 0) return false;
            for(int i =0; i< Math.Min(hitCnt, 10);++i)
            {
                var comp = hitBuffer[i].collider.gameObject.GetComponent<PawnController>();
                if (comp != null)
                {
                    if (comp.Alive)
                        return true;
                }
            }
            return false;
        }

        void DetectPlayer()
        {
            if (remainingChargeTime > float.Epsilon)
                return;

            Vector2 origin1 = transform.position;
            Vector2 origin2 = transform.position;
            origin1.y += RayOffset;
            origin2.y -= RayOffset;

            //Check A
            if(HasAlivePlayer(Physics2D.Raycast(origin1, GetDirectionVec(PointA), filter, hitBuffer, GetDistance(PointA))) || 
                    HasAlivePlayer(Physics2D.Raycast(origin2, GetDirectionVec(PointA), filter, hitBuffer, GetDistance(PointA))))
            {
                curState = SpikeHeadStateEnum.Preparing;
                ChargeTarget = GetChargeTarget(PointA);
                remainingChargeTime = ChargeCoolDown;
                remainingPrepareTime = PrepareTime;
                animator.SetTrigger("Prepare");
                ExclamationGO.SetActive(true);
                animator.speed = PrepareTime == 0f? 1f : 1f / PrepareTime;
                return;
            }
            //Check B
            if(HasAlivePlayer(Physics2D.Raycast(origin1, GetDirectionVec(PointB), filter, hitBuffer, GetDistance(PointB)))||
                    HasAlivePlayer(Physics2D.Raycast(origin2, GetDirectionVec(PointB), filter, hitBuffer, GetDistance(PointB))))
            {
                curState = SpikeHeadStateEnum.Preparing;
                ChargeTarget = GetChargeTarget(PointB);
                remainingChargeTime = ChargeCoolDown;
                remainingPrepareTime = PrepareTime;
                animator.SetTrigger("Prepare");
                ExclamationGO.SetActive(true);
                animator.speed = PrepareTime == 0f ? 1f : 1f / PrepareTime;
                return;
            }
        }

        void TryDetectPlayer()
        {
            remainingDetectTime -= Time.deltaTime;
            if (remainingDetectTime <= float.Epsilon)
            {
                DetectPlayer();
                remainingDetectTime = DetectionInterval;
            }
        }

        private void Update()
        {
            if (remainingChargeTime > 0f)
                remainingChargeTime -= Time.deltaTime;

            if(curState == SpikeHeadStateEnum.Idle)
            {
                curVelo = 0f;
                TryDetectPlayer();
            }
            else if(curState == SpikeHeadStateEnum.Staging)
            {
                curVelo = 0f;
                remainingStageTime -= Time.deltaTime;
                if (!DetectAfterReturned)
                    DetectPlayer();
                if(remainingStageTime <= float.Epsilon)
                {
                    if (GoBackAfterCharged)
                        curState = SpikeHeadStateEnum.Returning;
                    else
                        curState = SpikeHeadStateEnum.Idle;
                }
            }
            else if(curState == SpikeHeadStateEnum.Returning)
            {
                if (!DetectAfterReturned)
                    DetectPlayer();
            }
            else if(curState == SpikeHeadStateEnum.Preparing)
            {
                remainingPrepareTime -= Time.deltaTime;
                if (remainingPrepareTime <= 0f)
                {
                    curState = SpikeHeadStateEnum.Charging;
                    animator.speed = 1f;
                    animator.SetBool("Attack", true);
                    ExclamationGO.SetActive(false);
                }
            }
        }

        private void FixedUpdate()
        {
            if(curState == SpikeHeadStateEnum.Charging || curState == SpikeHeadStateEnum.Returning)
            {
                Vector2 targetPos = transform.position;
                if (curState == SpikeHeadStateEnum.Charging)
                {
                    curVelo += Accelaration * Time.deltaTime;
                    targetPos = ChargeTarget;
                }
                else if (curState == SpikeHeadStateEnum.Returning)
                {
                    curVelo = ReturnSpeed;
                    targetPos = orgPosition;
                }

                float distanceRemain = 0f;
                Vector2 newVelo;
                if(Direction == DirectionEnum.Horizontal)
                {
                    distanceRemain = Mathf.Abs(targetPos.x - transform.position.x);
                    newVelo = new Vector2(curVelo, 0f);
                    if (targetPos.x < transform.position.x)
                        newVelo = -newVelo;
                }
                else
                {
                    distanceRemain = Mathf.Abs(targetPos.y - transform.position.y);
                    newVelo = new Vector2(0f, curVelo);
                    if (targetPos.y < transform.position.y)
                        newVelo = -newVelo;
                }

                if(distanceRemain < curVelo * Time.deltaTime)
                {
                    //Arrived
                    rb.MovePosition(targetPos);
                    rb.velocity = Vector2.zero;
                    if (curState == SpikeHeadStateEnum.Charging)
                    {
                        SoundManager.Instance.PlaySFX(SoundManager.Instance.SpikeHeadSound, 1f);
                        curState = SpikeHeadStateEnum.Staging;
                        remainingStageTime = StageTime;
                        if (Direction == DirectionEnum.Horizontal)
                            animator.SetTrigger("HorizontalHit");
                        else
                            animator.SetTrigger("VerticalHit");
                        animator.SetBool("Attack", false);
                    }
                    else if(curState == SpikeHeadStateEnum.Returning)
                    {
                        curState = SpikeHeadStateEnum.Idle;
                    }
                }
                else
                    rb.velocity = newVelo;
            }
        }

        public override void ExposeTimeData()
        {
            base.ExposeTimeData();
            TimeScribe.Look_Value(ref curState);
            TimeScribe.Look_Value(ref curVelo);
            TimeScribe.Look_Value(ref remainingDetectTime);
            TimeScribe.Look_Value(ref remainingStageTime);
            TimeScribe.Look_Value(ref remainingChargeTime);
            TimeScribe.Look_Value(ref ChargeTarget);
            TimeScribe.Look_Value(ref remainingPrepareTime);
        }
    }
}
