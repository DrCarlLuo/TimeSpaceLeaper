using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LAB.ManagedTime;
using LAB.AI;
using System;

namespace LAB.Mechanics
{
    public class PlayerGhostManager : MonoBehaviour
    {
        #region Singleton
        public static PlayerGhostManager Instance { get; private set; }

        void Awake()
        {
            if (Instance != null)
                Log.Error("Multiple Player Ghost Manager!");
            Instance = this;
            EnabledGhostCnt = 0;
            GhostList = new LinkedList<GameObject>();
        }
        #endregion

        public GameObject GhostPrefab;
        public GameObject CurrentGhost { get; private set; }
        public int EnabledGhostCnt { get; set; }

        private LinkedList<GameObject> GhostList;

        public void CreateNewGhost()
        {
            CurrentGhost = Instantiate(GhostPrefab);
            CloneInHierachy(PlayerController.Instance.transform, CurrentGhost.transform);
            ManagedTimeUtilities.RegisterEntireGO(CurrentGhost);
        }

        private void CloneInHierachy(Transform playerRoot, Transform ghostRoot)
        {
            if (playerRoot.gameObject.tag == "IgnoreCopy") return;
            var playerComps = playerRoot.GetComponents<ITimeManaged>();
            List<Type> compTypes = new List<Type>();
            for (int i = 0; i < playerComps.Length; ++i)
                compTypes.Add(playerComps[i].GetType());
            foreach (var t in compTypes.Distinct())
            {
                var pComps = playerRoot.GetComponents(t);
                var gComps = ghostRoot.GetComponents(t);
                if (pComps.Length != gComps.Length)
                {
                    Log.Error("Player and Ghost don't share the same Comps!");
                    continue;
                }
                for (int i = 0; i < pComps.Length; ++i)
                {
                    var pc = pComps[i] as ITimeManaged;
                    var gc = gComps[i] as ITimeManaged;
                    gc.GetTimeStateContainer.CloneFrom(pc.GetTimeStateContainer);
                }
            }
            for (int i = 0; i < playerRoot.childCount; ++i)
                CloneInHierachy(playerRoot.GetChild(i), ghostRoot.GetChild(i));
        }

        public void DestroyCurrentGhost()
        {
            ManagedTimeUtilities.RealDestroyGO(CurrentGhost);
        }

        public void VerifyAndConfirmCurrentGhost()
        {
            var comp = CurrentGhost.GetComponent<ManagedGameObject>();
            if (comp.GetTimeStateContainer.LatestTimeStamp != TimeStateManager.Instance.CurrentTimeStamp)
            {
                DestroyCurrentGhost();
                return;
            }
            CurrentGhost.GetComponent<PlayerGhostAI>().StartCountDown();
            GhostList.AddFirst(CurrentGhost);
            CurrentGhost = null;
        }

        public bool TryGetLatestEnabledGhost(out GameObject go)
        {
            go = null;
            if (GhostList.Count <= 0) return false;
            foreach(var x in GhostList)
            {
                if(x.activeSelf)
                {
                    go = x;
                    return true;
                }
            }
            return false;
        }

        private void Update()
        {
            while (GhostList.Count > 0 && GhostList.Last.Value == null)
                GhostList.RemoveLast();
        }
    }
}
