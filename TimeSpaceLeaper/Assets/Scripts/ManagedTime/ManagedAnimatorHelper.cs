using System;
using System.Collections.Generic;
using UnityEngine;

namespace LAB.ManagedTime
{
    public class ManagedAnimatorHelper : ManagedMonoBehaviour
    {
        Animator animator;

        public override void ExposeTimeData()
        {
            base.ExposeTimeData();
            bool AnimEnable = animator.enabled;
            TimeScribe.Look_Value(ref AnimEnable);
            animator.enabled = AnimEnable;

            //State Info
            AnimatorStateInfo[] infos = null;
            if (TimeScribe.Mode == ScribeMode.Record)
            {
                infos = new AnimatorStateInfo[animator.layerCount];
                for (int i = 0; i < animator.layerCount; ++i)
                    infos[i] = animator.GetCurrentAnimatorStateInfo(i);
            }
            TimeScribe.Look_Array(ref infos, animator.layerCount);
            if (TimeScribe.Mode == ScribeMode.Rewind)
            {
                for (int i = 0; i < animator.layerCount; ++i)
                    animator.Play(infos[i]
                        .shortNameHash, i, infos[i].normalizedTime);
            }

            //Parameters
            var rawParms = animator.parameters;
            foreach(var parm in rawParms)
            {
                int nameHash = parm.nameHash;
                TimeScribe.Look_Value(ref nameHash);
                var parmType = parm.type;
                TimeScribe.Look_Value(ref parmType);
                if(parmType == AnimatorControllerParameterType.Bool)
                {
                    bool value = animator.GetBool(nameHash);
                    TimeScribe.Look_Value(ref value);
                    animator.SetBool(nameHash, value);
                }
                else if(parmType == AnimatorControllerParameterType.Int)
                {
                    int value = animator.GetInteger(nameHash);
                    TimeScribe.Look_Value(ref value);
                    animator.SetInteger(nameHash, value);
                }
                else if(parmType == AnimatorControllerParameterType.Float)
                {
                    float value = animator.GetFloat(nameHash);
                    TimeScribe.Look_Value(ref value);
                    animator.SetFloat(nameHash, value);
                }
            }
        }

        protected void Awake()
        {
            animator = GetComponent<Animator>();
        }
    }
}
