using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LAB.ManagedTime;
using LAB.Mechanics;
using LAB.Engine;
using LAB.VFX;

namespace LAB.UI
{
    public class TimeSelectionHelper : MonoBehaviour
    {
        public TimeIndicatorUI TimeUIGO;
        public GameObject TimeCastFXGO;

        public float ChangeSpeed = 0.02f;
        public int MaxGhostNum = 5;
        bool TimeFreezeStarted = false;
        int TimeStampDiff = 0;
        float PreviousTimeScale;
        float remainChangeTime;
        public float waitForEffectFinish;

        //For tutorial
        public bool AllowLeaving = true;

        //Sound Handle
        SFXHandle FreezeInHandle = null;
        SFXHandle FreezeOutHandle = null;

        public bool GetTimeFreezeStarted => TimeFreezeStarted;
        public int GetTimeStampDiff => TimeStampDiff;

        private void EnterFreeze()
        {
            TimeFreezeStarted = true;
            TimeStampDiff = 0;
            PreviousTimeScale = Time.timeScale;
            Time.timeScale = 0f;

            //Be careful with the following order!
            TimeStateManager.Instance.StashCurrentTime();
            PlayerController.Instance.DetachTimeRegister();
            PlayerGhostManager.Instance.CreateNewGhost();
            remainChangeTime = ChangeSpeed;

            TimeStateManager.Instance.Rewind(0, true);//This will make sure the ghost is in the current State
            PlayerGhostManager.Instance.CurrentGhost.SetActive(false);
        }

        private void LeaveFreeze()
        {
            if (!TimeFreezeStarted) return;
            TimeFreezeStarted = false;
            Time.timeScale = PreviousTimeScale;
            if (TimeStampDiff > 0 && PlayerGhostManager.Instance.EnabledGhostCnt <= MaxGhostNum)
            {
                TimeStateManager.Instance.Rewind(TimeStampDiff, false);
                PlayerGhostManager.Instance.VerifyAndConfirmCurrentGhost();
                PlayerController.Instance.ResetAllTimeStat();
                PlayerController.Instance.RebuildTimeRegister();

                GameObject timeCastFXInstance = Instantiate(TimeCastFXGO, PlayerController.Instance.transform.position, Quaternion.identity);
                timeCastFXInstance.GetComponent<LightCastFX>().PostSpawn();
                ManagedTimeUtilities.RegisterEntireGO(timeCastFXInstance);
            }
            else
            {
                PlayerGhostManager.Instance.DestroyCurrentGhost();
                PlayerController.Instance.RebuildTimeRegister();
                TimeStateManager.Instance.BackToNow();
            }
        }

        private void Awake()
        {
            TimeFreezeStarted = false;
        }

        float leavingCoolDoawn = 0f;

        private void Update()
        {
            if (GameManager.Instance.GameStateNow != GameState.Running) return;
            if (GameManager.Instance.PlayerGO == null || !PlayerController.Instance.pawnController.Alive)
                return;
            if (leavingCoolDoawn > 0f)
                leavingCoolDoawn -= Time.unscaledDeltaTime;

            if (AllowLeaving && TimeFreezeStarted && InputHandler.FreezeTime.KeyUp && leavingCoolDoawn <= 0f)
            {
                leavingCoolDoawn = 1f;
                LeaveFreeze();
                TheWorldEffectHelper.Instance.ExitEffect(0.5f);
                if (FreezeOutHandle != null)
                    FreezeOutHandle.StopRightNow();
                FreezeOutHandle = SoundManager.Instance.PlaySFXIgnoreTimeFreeze(SoundManager.Instance.FreezeOutSound, 0.4f);
                if(FreezeInHandle != null)
                    SoundManager.Instance.CrossFade(FreezeOutHandle, FreezeInHandle, 0.5f);
                SoundManager.Instance.SFXEnvironment = SoundEnvironmentEnum.Normal;
            }
            if (!TimeFreezeStarted && InputHandler.FreezeTime.KeyDown && leavingCoolDoawn <= 0f)
            {
                TheWorldEffectHelper.Instance.ZaWaLuDo(1f);
                waitForEffectFinish = 1f;
                EnterFreeze();
                SoundManager.Instance.ClearSFXForTimeFreeze();
                SoundManager.Instance.SFXEnvironment = SoundEnvironmentEnum.TimeFreezed;
                if(FreezeInHandle != null)
                    FreezeInHandle.StopRightNow();
                FreezeInHandle = SoundManager.Instance.PlaySFXIgnoreTimeFreeze(SoundManager.Instance.FreezeInSound, 0.4f);
                if (FreezeOutHandle != null)
                    SoundManager.Instance.CrossFade(FreezeInHandle, FreezeOutHandle, 0.5f);
            }
            if(TimeFreezeStarted)
            {
                remainChangeTime -= Time.unscaledDeltaTime;
                if(remainChangeTime <= 0f)
                {
                    remainChangeTime = ChangeSpeed;
                    bool changed = false;
                    if (waitForEffectFinish > 0)
                        waitForEffectFinish -= Time.unscaledDeltaTime;
                    else
                    {
                        if (InputHandler.TimeBackward.KeyHold)
                        {
                            ++TimeStampDiff;
                            changed = true;
                        }
                        else if (InputHandler.TimeAfterward.KeyHold)
                        {
                            --TimeStampDiff;
                            changed = true;
                        }
                    }
                    if (changed)
                    {
                        TimeStampDiff = Math.Max(TimeStampDiff, 0);
                        TimeStampDiff = Math.Min(TimeStampDiff, TimeStateManager.Instance.MaxRewindableTimeStamp);
                        TimeStateManager.Instance.Rewind(TimeStampDiff, true);
                        if (TimeStampDiff == 0 && PlayerGhostManager.Instance.CurrentGhost != null)
                            PlayerGhostManager.Instance.CurrentGhost.SetActive(false);
                    }
                }
            }

            if (TimeFreezeStarted && waitForEffectFinish <= 0f)
            {
                TimeUIGO.gameObject.SetActive(true);
                String timeStr = (TimeStateManager.RecordInterval * TimeStampDiff).ToString("F2") + " S";
                TimeUIGO.SetTimeValue(TimeStampDiff, 0, TimeStateManager.Instance.CurrentTimeStamp - TimeStateManager.Instance.EarliestExistingTimeState + 1, timeStr);
            }
            else
                TimeUIGO.gameObject.SetActive(false);
        }

        /*
        private Rect windowRect = new Rect(20, 20, 60, 50);

        private void OnGUI()
        {
            if(TimeFreezeStarted)
            {
                void Func(int windowID) { };
                windowRect = GUI.Window(0, windowRect, Func, (-TimeStateManager.RecordInterval * TimeStampDiff).ToString());
            }
        }
        */
    }
}
