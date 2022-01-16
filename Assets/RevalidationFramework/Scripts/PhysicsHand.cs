using System;
using UnityEngine;

namespace RevalidationFramework
{
    [Serializable]
    public class RigMap
    {
        public Transform master;
        public PhysicsBone slave;
    }

    public class PhysicsHand : MonoBehaviour
    {
        public OVRSkeleton dataProvider;
        public Collider palmCollider;
        public RigMap[] rigMaps = new RigMap[24];
        
        public SkinnedMeshRenderer masterHand;
        public SkinnedMeshRenderer slaveHand;

        public bool debugMode = false;

        private Collider[] _colliders;

        private bool _trackingState = false;

        private void Awake()
        {
            foreach (var rigMap in rigMaps)
            {
                // Set slave targets
                if (rigMap.master && rigMap.slave)
                    rigMap.slave.Target = rigMap.master;
            }

            // Ignore finger part-palm collisions
            _colliders = rigMaps[0].slave.GetComponentsInChildren<Collider>();
            foreach (var c in _colliders)
            {
                Physics.IgnoreCollision(palmCollider, c, true);
                c.gameObject.layer = 9;

                // Debug.Log(colliders[c].gameObject.name + " Slave  " + colliders[c].gameObject.layer);
            }


            masterHand.enabled = debugMode;
        }


        private void Update()
        {
            bool trackingLost = dataProvider.IsDataHighConfidence;
            if(trackingLost != _trackingState)
            {
                if (trackingLost)
                {
                    OnTrackingGained();
                }
                else
                {
                    OnTrackingLost();
                }
            }

            _trackingState = trackingLost;

            if (dataProvider.Bones.Count != rigMaps.Length)
                return;

            // Update wrist position and rotation in world space
            rigMaps[0].master.position = dataProvider.Bones[0].Transform.position;
            rigMaps[0].master.rotation = dataProvider.Bones[0].Transform.rotation;

            // Update bone rotations in local space
            for (int b = 1; b < dataProvider.Bones.Count; b++)
            {
                rigMaps[b].master.localRotation = dataProvider.Bones[b].Transform.localRotation;
            }
        }
        private void OnTrackingGained()
        {
            ResetRig();
            
            if (debugMode)
                masterHand.enabled = true;

            slaveHand.enabled = true;

        }
        private void OnTrackingLost()
        {
            ResetRig();
            
            if (debugMode)
                masterHand.enabled = false;

            slaveHand.enabled = false;

        }

        public void ToggleDebug()
        {
            debugMode = !debugMode;

            if(_trackingState)
                masterHand.enabled = debugMode;
        }

        private void ResetRig()
        {
            foreach (var rigMap in rigMaps)
            {
                if (!rigMap.master || !rigMap.slave) continue;

                var slaveTransform = rigMap.slave.transform;
                var masterTransform = rigMap.master.transform;
                
                slaveTransform.position = masterTransform.position;
                slaveTransform.rotation = masterTransform.rotation;
            }
        }

        private void SetHandKinematic(bool state)
        {
            foreach (var rigMap in rigMaps)
            {
                if (rigMap.slave)
                {
                    rigMap.slave.rb.isKinematic = state;
                  
                }
            }
        }
    }
}