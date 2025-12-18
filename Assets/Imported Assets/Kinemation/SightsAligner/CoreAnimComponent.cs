using System;
using UnityEngine;

namespace Kinemation.SightsAligner
{
    [Serializable]
    public struct DynamicBone
    {
        public Transform target;
        public Transform hintTarget;
        public GameObject obj;

        public void Retarget()
        {
            // SAFETY: prevent NullReference spam
            if (target == null) return;
            if (obj == null) return;

            obj.transform.position = target.position;
            obj.transform.rotation = target.rotation;
        }
    }

    public class CoreAnimComponent : MonoBehaviour
    {
        [Header("Rig")]
        [Tooltip("Doesn't use Target and Hint")]
        [SerializeField] private DynamicBone masterDynamic;
        [SerializeField] private DynamicBone rightHand;
        [SerializeField] private DynamicBone leftHand;

        [Tooltip("Used for mesh space calculations")]
        [SerializeField] private Transform rootBone;

        [Header("Blending")]
        [Range(0f, 1f)]
        public float aimLayerAlphaLoc;
        [Range(0f, 1f)]
        public float aimLayerAlphaRot;

        [Header("Tools")]
        public GunAimData aimData;                 // <-- THIS IS A STRUCT IN YOUR PROJECT
        [SerializeField] private Transform aimTarget;
        [SerializeField] private Animator animator;
        public bool aiming;

        private float _smoothAim;
        private (Vector3, Quaternion) _smoothAimPoint;

        // Instead of aimData == null (illegal for structs), we validate the *important reference fields*
        private bool HasValidAimSetup()
        {
            if (aimData.pivotPoint == null) return false;
            if (masterDynamic.obj == null) return false;
            if (rootBone == null) return false;
            if (aimTarget == null) return false;
            if (aimData.target == null) return false; // if target is a class reference; if it's a struct, this line may error
            return true;
        }

        private void Retarget()
        {
            // SAFETY: must have a pivotPoint + masterDynamic.obj
            if (aimData.pivotPoint == null) return;
            if (masterDynamic.obj == null) return;

            // Master is retargeted manually as it requires non-character bone
            masterDynamic.obj.transform.position = aimData.pivotPoint.position;
            masterDynamic.obj.transform.rotation = aimData.pivotPoint.rotation;

            rightHand.Retarget();
            leftHand.Retarget();
        }

        private void ApplyProceduralLayer()
        {
            // SAFETY
            if (aimData.pivotPoint == null) return;
            if (aimData.target == null) return;     // if target is a class reference; if it's a struct, remove this line
            if (masterDynamic.obj == null) return;
            if (rootBone == null) return;
            if (aimTarget == null) return;

            // Apply Aiming
            var masterTransform = masterDynamic.obj.transform;
            _smoothAim = AnimToolkitLib.GlerpLayer(_smoothAim, aiming ? 1f : 0f, aimData.aimSpeed);

            Vector3 scopeAimLoc = Vector3.zero;
            Quaternion scopeAimRot = Quaternion.identity;

            if (aimData.aimPoint != null)
            {
                scopeAimRot = Quaternion.Inverse(aimData.pivotPoint.rotation) * aimData.aimPoint.rotation;
                scopeAimLoc = -aimData.pivotPoint.InverseTransformPoint(aimData.aimPoint.position);
            }

            if (!_smoothAimPoint.Item1.Equals(scopeAimLoc))
                _smoothAimPoint.Item1 = AnimToolkitLib.Glerp(_smoothAimPoint.Item1, scopeAimLoc, aimData.aimSpeed);

            if (!_smoothAimPoint.Item2.Equals(scopeAimRot))
                _smoothAimPoint.Item2 = AnimToolkitLib.Glerp(_smoothAimPoint.Item2, scopeAimRot, aimData.aimSpeed);

            Vector3 addAimLoc = aimData.target.aimLoc;
            Quaternion addAimRot = aimData.target.aimRot * _smoothAimPoint.Item2;

            // Base Animation layer
            Vector3 baseLoc = masterTransform.position;
            Quaternion baseRot = masterTransform.rotation;

            AnimToolkitLib.MoveInBoneSpace(masterTransform, masterTransform, addAimLoc);
            masterTransform.rotation *= addAimRot;
            AnimToolkitLib.MoveInBoneSpace(masterTransform, masterTransform, _smoothAimPoint.Item1);

            addAimLoc = masterTransform.position;
            addAimRot = masterTransform.rotation;

            ApplyAiming(_smoothAimPoint.Item1, _smoothAimPoint.Item2);

            // Blend between Absolute and Additive
            masterTransform.position = Vector3.Lerp(masterTransform.position, addAimLoc, aimLayerAlphaLoc);
            masterTransform.rotation = Quaternion.Slerp(masterTransform.rotation, addAimRot, aimLayerAlphaRot);

            // Blend Between Non-Aiming and Aiming
            masterTransform.position = Vector3.Lerp(baseLoc, masterTransform.position, _smoothAim);
            masterTransform.rotation = Quaternion.Slerp(baseRot, masterTransform.rotation, _smoothAim);
        }

        private void ApplyAiming(Vector3 loc, Quaternion rot)
        {
            // SAFETY
            if (masterDynamic.obj == null) return;
            if (aimTarget == null) return;
            if (rootBone == null) return;

            Vector3 offset = -loc;

            //1. Set master IK to the target
            //2. Then rotate
            //3. Finally applied local offset
            masterDynamic.obj.transform.position = aimTarget.position;
            masterDynamic.obj.transform.rotation = rootBone.rotation * rot;
            AnimToolkitLib.MoveInBoneSpace(masterDynamic.obj.transform, masterDynamic.obj.transform, -offset);
        }

        private void ApplyIK()
        {
            // SAFETY
            if (rightHand.target == null || rightHand.obj == null) return;
            if (leftHand.target == null || leftHand.obj == null) return;
            if (rightHand.target.parent == null) return;
            if (leftHand.target.parent == null) return;
            if (rightHand.target.parent.parent == null) return;
            if (leftHand.target.parent.parent == null) return;

            Transform lowerBone = rightHand.target.parent;

            AnimToolkitLib.SolveTwoBoneIK(lowerBone.parent, lowerBone, rightHand.target,
                rightHand.obj.transform, rightHand.hintTarget, 1f, 1f, 1f);

            lowerBone = leftHand.target.parent;

            AnimToolkitLib.SolveTwoBoneIK(lowerBone.parent, lowerBone, leftHand.target,
                leftHand.obj.transform, leftHand.hintTarget, 1f, 1f, 1f);
        }

        private void LateUpdate()
        {
            // SAFETY: only run if the important references exist
            if (aimData.pivotPoint == null) return;
            if (masterDynamic.obj == null) return;

            Retarget();
            ApplyProceduralLayer();
            ApplyIK();
        }

        public void CalculateAimData()
        {
            // SAFETY
            if (aimData.pivotPoint == null) return;
            if (aimTarget == null) return;

            // NOTE: aimData.target might be a struct or class depending on your package.
            // If you get compile errors here, tell me what GunAimData.target type is.
            var stateName =
                (aimData.target.stateName != null && aimData.target.stateName.Length > 0)
                    ? aimData.target.stateName
                    : (aimData.target.staticPose != null ? aimData.target.staticPose.name : "");

            if (animator != null && stateName.Length > 0)
            {
                animator.Play(stateName);
                animator.Update(0f);
            }

            aimData.target.aimLoc = aimData.pivotPoint.InverseTransformPoint(aimTarget.position);
            aimData.target.aimRot = Quaternion.Inverse(aimData.pivotPoint.rotation) * aimTarget.rotation;
        }

        public void SetupBones()
        {
            if (rootBone == null)
            {
                var root = transform.Find("rootBone");

                if (root != null)
                {
                    rootBone = root.transform;
                }
                else
                {
                    var bone = new GameObject("rootBone");
                    bone.transform.parent = transform;
                    rootBone = bone.transform;
                    rootBone.localPosition = Vector3.zero;
                }
            }

            var children = transform.GetComponentsInChildren<Transform>(true);

            bool foundRightHand = false;
            bool foundLeftHand = false;
            bool foundHead = false;

            foreach (var bone in children)
            {
                if (bone.name.ToLower().Contains("ik"))
                    continue;

                bool leftMatch =
                    bone.name.ToLower().Contains("lefthand") || bone.name.ToLower().Contains("hand_l")
                    || bone.name.ToLower().Contains("hand l") || bone.name.ToLower().Contains("l hand")
                    || bone.name.ToLower().Contains("l.hand") || bone.name.ToLower().Contains("hand.l");

                if (!foundLeftHand && leftMatch)
                {
                    leftHand.target = bone;
                    if (leftHand.hintTarget == null) leftHand.hintTarget = bone.parent;
                    foundLeftHand = true;
                    continue;
                }

                bool rightMatch =
                    bone.name.ToLower().Contains("righthand") || bone.name.ToLower().Contains("hand_r")
                    || bone.name.ToLower().Contains("hand r") || bone.name.ToLower().Contains("r hand")
                    || bone.name.ToLower().Contains("r.hand") || bone.name.ToLower().Contains("hand.r");

                if (!foundRightHand && rightMatch)
                {
                    rightHand.target = bone;
                    if (rightHand.hintTarget == null) rightHand.hintTarget = bone.parent;
                    foundRightHand = true;
                }

                if (!foundHead && bone.name.ToLower().Contains("head"))
                {
                    if (masterDynamic.obj == null)
                    {
                        var boneObject = bone.transform.Find("MasterIK");
                        if (boneObject != null)
                        {
                            masterDynamic.obj = boneObject.gameObject;
                        }
                        else
                        {
                            masterDynamic.obj = new GameObject("MasterIK");
                            masterDynamic.obj.transform.parent = bone;
                            masterDynamic.obj.transform.localPosition = Vector3.zero;
                        }
                    }

                    if (rightHand.obj == null)
                    {
                        var boneObject = bone.transform.Find("RightHandIK");
                        rightHand.obj = (boneObject != null) ? boneObject.gameObject : new GameObject("RightHandIK");
                        rightHand.obj.transform.parent = masterDynamic.obj.transform;
                        rightHand.obj.transform.localPosition = Vector3.zero;
                    }

                    if (leftHand.obj == null)
                    {
                        var boneObject = bone.transform.Find("LeftHandIK");
                        leftHand.obj = (boneObject != null) ? boneObject.gameObject : new GameObject("LeftHandIK");
                        leftHand.obj.transform.parent = masterDynamic.obj.transform;
                        leftHand.obj.transform.localPosition = Vector3.zero;
                    }

                    foundHead = true;
                }
            }

            bool bFound = foundRightHand && foundLeftHand && foundHead;
            Debug.Log(bFound ? "All bones are found!" : "Some bones are missing!");
        }

        public void Init(GunAimData data, Transform aimPoint)
        {
            // GunAimData is a struct, so it cannot be null
            aimData = data;
            aimData.aimPoint = aimPoint;

            // SAFETY
            if (aimData.pivotPoint == null) return;
            if (aimData.aimPoint == null) return;

            _smoothAimPoint.Item2 = Quaternion.Inverse(aimData.pivotPoint.rotation) * aimData.aimPoint.rotation;
            _smoothAimPoint.Item1 = -aimData.pivotPoint.InverseTransformPoint(aimData.aimPoint.position);
        }
    }
}
