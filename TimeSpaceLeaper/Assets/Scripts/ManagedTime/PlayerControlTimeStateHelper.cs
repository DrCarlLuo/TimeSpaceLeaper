using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using LAB.Mechanics;
using LAB.AI;

namespace LAB.ManagedTime
{
    enum PlayerTimeDataEnum
    {
        Real,
        Ghost
    }

    class PlayerControlTimeStateHelper : ManagedMonoBehaviour
    {
        public PlayerTimeDataEnum DataSource;
        PlayerController playerController;
        PlayerGhostAI playerGhostAI;

        public GameObject MainTexture;
        SpriteRenderer mainSpriteRenderer;

        protected void Awake()
        {
            mainSpriteRenderer = MainTexture.GetComponent<SpriteRenderer>();
            if (DataSource == PlayerTimeDataEnum.Ghost)
                playerGhostAI = GetComponent<PlayerGhostAI>();
            else
                playerController = GetComponent<PlayerController>();
        }

        public override void ExposeTimeData()
        {
            base.ExposeTimeData();
            Vector2 moveVector;
            if (DataSource == PlayerTimeDataEnum.Ghost)
                moveVector = playerGhostAI.moveVector;
            else
                moveVector = playerController.moveVector;
            TimeScribe.Look_Value(ref moveVector);
            if (TimeScribe.Mode == ScribeMode.Rewind && DataSource == PlayerTimeDataEnum.Ghost)
                playerGhostAI.moveVector = moveVector;
            mainSpriteRenderer.flipX = TimeScribe.Look_Property(mainSpriteRenderer.flipX);
        }
    }
}
