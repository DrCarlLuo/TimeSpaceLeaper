using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LAB.ManagedTime;
using LAB.Engine;

namespace LAB.Mechanics
{
    public class PositionExchanger : MonoBehaviour
    {
        public float CD = 5f;
        SpriteRenderer spriteRenderer;
        bool inCD;
        float remainingCD;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            inCD = false;
            remainingCD = 0f;
        }

        private void Update()
        {
            if(inCD)
            {
                remainingCD -= Time.deltaTime;
                if (remainingCD < 0f)
                {
                    inCD = false;
                    spriteRenderer.color = Color.white;
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (inCD) return;
            if(other.gameObject.tag == "Player")
            {
                if(PlayerGhostManager.Instance.TryGetLatestEnabledGhost(out GameObject ghost))
                {
                    Vector3 temp = GameManager.Instance.PlayerGO.transform.position;
                    GameManager.Instance.PlayerGO.transform.position = ghost.transform.position;
                    ghost.transform.position = temp;
                    inCD = true;
                    remainingCD = CD;
                    spriteRenderer.color = Color.black;
                }
            }
        }
    }
}
