using System;
using System.Collections.Generic;
using UnityEngine;
using LAB.ManagedTime;

namespace LAB.Mechanics
{
    class SimpleBulletEmiter : ManagedMonoBehaviour
    {
        public Transform BulletSpawnPoint;
        public bool ShootLeft;
        public float BulletVelocity = 3f;
        public float ShootInterval = 5f;
        public GameObject BulletPrefab;

        float remainNxtTime = 0f;

        private void Update()
        {
            remainNxtTime -= Time.deltaTime;
            if(remainNxtTime <= 0f)
            {
                GameObject go = Instantiate(BulletPrefab, BulletSpawnPoint.position, Quaternion.identity);
                ManagedTimeUtilities.RegisterEntireGO(go);
                go.GetComponent<Rigidbody2D>().velocity = new Vector2(ShootLeft ? -BulletVelocity : BulletVelocity, 0f);
                remainNxtTime = ShootInterval;
            }
        }

        public override void ExposeTimeData()
        {
            base.ExposeTimeData();
            TimeScribe.Look_Value(ref remainNxtTime);
        }
    }
}
