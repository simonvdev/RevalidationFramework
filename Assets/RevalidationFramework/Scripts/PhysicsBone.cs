using UnityEngine;

namespace RevalidationFramework
{
    public class PhysicsBone : MonoBehaviour
    {
        ConfigurableJoint joint;
        [HideInInspector] public Rigidbody rb;

        public Transform Target;
        public bool FollowTargetPosition = false;
        public bool FollowTargetRotation = false;

        private Vector3 xAxis, zAxis, yAxis;
        private Quaternion frameOfRotation;

        private Quaternion initialGlobalRotation;
        private Quaternion initialLocalRotation;
        private Quaternion resultRotation;

        void Start()
        {
            if (!joint)
                joint = GetComponent<ConfigurableJoint>();
            if (!rb)
                rb = GetComponent<Rigidbody>();

            var xAxisJoint = joint.axis;
            
            xAxis = xAxisJoint;
            zAxis = Vector3.Cross(xAxisJoint, joint.secondaryAxis).normalized;
            yAxis = Vector3.Cross(zAxis, xAxis).normalized;

            // Joint frame rotation
            frameOfRotation = Quaternion.LookRotation(zAxis, yAxis);

            if (joint.connectedBody != null && !joint.configuredInWorldSpace)
            {
                initialLocalRotation = joint.connectedBody.transform.localRotation;
            }
            else
            {
                initialGlobalRotation = joint.transform.rotation;
            }
        }

        void FixedUpdate()
        {
            if (FollowTargetPosition && Target)
            {
                // Local
                if (joint.connectedBody != null && !joint.configuredInWorldSpace)
                {
                    // Phalanges don't follow position
                }
                // World
                else
                {

                    // We should be able to move connectedAnchor in space
                    if (joint.autoConfigureConnectedAnchor)
                        joint.autoConfigureConnectedAnchor = false;

                    // Joint will move the object towards connectedAnchor
                    if (joint.targetPosition != Vector3.zero)
                        joint.targetPosition = Vector3.zero;

                    // Update the center of the joint frame
                    joint.connectedAnchor = Target.position;
                }

            }

            if (!FollowTargetRotation || !Target) return;
            // Local
            if (joint.connectedBody != null && !joint.configuredInWorldSpace)
            {
                // Target local rotation relative to the joint frame + initial offset (local)
                resultRotation = Quaternion.Inverse(frameOfRotation);
                resultRotation *= Quaternion.Inverse(Target.localRotation);
                resultRotation *= initialLocalRotation;
                resultRotation *= frameOfRotation;

                joint.targetRotation = resultRotation;
            }
            // World
            else
            {
                // Target world rotation relative to joint frame + initial offset (global)
                resultRotation = Quaternion.Inverse(frameOfRotation);
                resultRotation *= initialGlobalRotation;
                resultRotation *= Quaternion.Inverse(Target.rotation);
                resultRotation *= frameOfRotation;

                joint.targetRotation = resultRotation;
            }
        }
    }

}