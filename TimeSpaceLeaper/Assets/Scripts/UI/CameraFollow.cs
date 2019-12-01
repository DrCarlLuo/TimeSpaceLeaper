using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LAB.UI;
using LAB.Engine;
using LAB.Mechanics;

namespace LAB
{
    public class CameraFollow : MonoBehaviour
    {
        public float smooth=2f;
        Transform target;

        public Transform TopRight;
        public Transform DownLeft;
        float minX, maxX, minY, maxY;
        int formerScreenX, formerScreenY;

        //Calculate the camera position range in the current scene
        public void CalculateSceneCameraRange()
        {
            Vector2 ScreenRightTopPoint = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
            Vector2 ScreenLeftDownPoint = Camera.main.ScreenToWorldPoint(Vector3.zero);
            float screenOffsetX = (ScreenRightTopPoint.x - ScreenLeftDownPoint.x) / 2f;
            float screenOffsetY = (ScreenRightTopPoint.y - ScreenLeftDownPoint.y) / 2f;

            Vector2 WorldRightTopPoint = TopRight.position;
            Vector2 WorldLeftDownPoint = DownLeft.position;

            minX = WorldLeftDownPoint.x - 0.5f + screenOffsetX;
            maxX = WorldRightTopPoint.x + 0.5f - screenOffsetX;
            minY = WorldLeftDownPoint.y - 0.5f + screenOffsetY;
            maxY = WorldRightTopPoint.y + 1.5f - screenOffsetY; //这里要多加一点，因为有些Tile比如墙壁要高一些

            if (maxX <= minX) //场景高/宽小于相机视野时的特殊处理
                maxX = minX = (maxX + minX) / 2;
            if (maxY <= minY)
                maxY = minY = (maxY + minY) / 2;
        }

        private Vector3 ClampInBounds(Vector3 pos)
        {
            return new Vector3(Mathf.Clamp(pos.x, minX, maxX), Mathf.Clamp(pos.y, minY, maxY), pos.z);
        }

        void Start()
        {
            //target = Mechanics.PlayerController.Instance.transform;
            formerScreenX = formerScreenY = 0;
        }

        void LateUpdate ()
        {
            target = Mechanics.PlayerController.Instance.transform;
            if (target == null) return;

            var helper = GameManager.Instance.TimeSelectionHelperInt;
            //Follow the latest ghost when in time freeze
            if (helper != null && helper.GetTimeFreezeStarted && helper.GetTimeStampDiff > 0)
            {
                var newGhost = PlayerGhostManager.Instance.CurrentGhost;
                if (newGhost != null && newGhost.activeSelf)
                    target = newGhost.transform;
                else if (PlayerGhostManager.Instance.TryGetLatestEnabledGhost(out GameObject latestGhost))
                    target = latestGhost.transform;
            }

            Vector3 targetPos = target.position;
            if (TopRight != null & DownLeft != null)
            {
                if (Screen.width != formerScreenX || Screen.height != formerScreenY)
                {
                    formerScreenX = Screen.width;
                    formerScreenY = Screen.height;
                    CalculateSceneCameraRange();
                }
                targetPos = ClampInBounds(targetPos);
            }
            targetPos.z = transform.position.z;
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.unscaledDeltaTime * smooth);
        }
    }
}
