using System;
using System.Collections.Generic;
using System.Text;
using HandPhysicsExtenstions;
using HarmonyLib;
using HutongGames.PlayMaker.Actions;
using RootMotion;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.XR;
using Valve.VR;
using Valve.VR.InteractionSystem;
using static HandsSimulatorVRMod.VRController;
using static RootMotion.FinalIK.HitReactionVRIK;

namespace HandsSimulatorVRMod.Patches {

    [HarmonyPatch(typeof(HandPhysicsStandaloneInput), "Update")]
    internal class HandPhysicsStandaloneInputPatch
    {
        
        public static void UpdateController(VRController vrController, HandPhysicsController __instance)
        {
            if (vrController.controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                //Triger
                __instance.StartBendFinger(FingerType.Index);
            }
            else if (vrController.controller.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                __instance.StopBendFinger(FingerType.Index);
            }
            if (vrController.controller.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
            {
                //B/Y button   
                __instance.StartBendFinger(FingerType.Thumb);
            }
            else if (vrController.controller.GetPressUp(SteamVR_Controller.ButtonMask.ApplicationMenu))
            {
                __instance.StopBendFinger(FingerType.Thumb);
            }
            if (vrController.controller.GetPressDown(EVRButtonId.k_EButton_A))
            {
                //A/X button
                __instance.StartBendFinger(FingerType.Ring);
                __instance.StartBendFinger(FingerType.Pinky);
            }
            else if (vrController.controller.GetPressUp(EVRButtonId.k_EButton_A))
            {
                __instance.StopBendFinger(FingerType.Ring);
                __instance.StopBendFinger(FingerType.Pinky);
            }
            if (vrController.controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
            {
                //grip button
                __instance.StartBendFinger(FingerType.Middle);
            }
            else if (vrController.controller.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
            {
                __instance.StopBendFinger(FingerType.Middle);
            }
            if (vrController.controller.GetPressDown(SteamVR_Controller.ButtonMask.System))
            {
                Debug.Log("Левый контроллер нажал System");
            }
            if (InputSystem.leftController.controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
            {
                //stick button
                OpenVR.System.ResetSeatedZeroPose();
                OpenVR.Compositor.SetTrackingSpace(ETrackingUniverseOrigin.TrackingUniverseSeated);
            }

        }
        private static bool Prefix(HandPhysicsStandaloneInput __instance)
        {

            if (__instance.name == "Hand_Left")
            {
                UpdateController(InputSystem.leftController, __instance.Controller);
            }
            if (__instance.name == "Hand_Right")
            {
                UpdateController(InputSystem.rightController, __instance.Controller);
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(HandPhysicsController), "GetForearmTargetVelocity")]
    internal class GetForearmTargetVelocityPatch
    {

        public static readonly float movementHardness = 1f;
        public static readonly float handSpeed = 8f;
        public static bool Prefix(ref Vector3 __result,HandPhysicsController __instance, Vector3 direction, ref Vector3 ____curForearmDirection, ref Vector3 ____lastForearmPosition) {


            

            Camera vrCamera = Camera.main;
            if (vrCamera != null)
            {
               
                if (__instance.name == "Hand_Left") {
                    __instance.Forearm.MovementHardness = movementHardness;
                    Transform xrRigOrigin = vrCamera.transform.parent;
                    Vector3 leftHandLocalPosition = InputSystem.leftController.GetLocalPosition();      
                    Vector3 leftHandWorldPosition = xrRigOrigin.TransformPoint(leftHandLocalPosition);                  
                    Quaternion leftRotation = InputSystem.leftController.GetLocalRotation();           
                    __instance.Parts.Forearm.Joint.targetRotation = new Quaternion(-leftRotation.z, leftRotation.x, leftRotation.y, leftRotation.w) * Quaternion.Euler(-90, 0, 0);
                    Vector3 rightGlobal = leftHandWorldPosition - __instance.Parts.Forearm.transform.position;
                    ____curForearmDirection = rightGlobal;
                    __result = new Vector3(rightGlobal.x * handSpeed, rightGlobal.y * handSpeed, rightGlobal.z * handSpeed);
                    return false;
                }
                if (__instance.name == "Hand_Right")
                {
                    __instance.Forearm.MovementHardness = movementHardness;
                    Transform xrRigOrigin = vrCamera.transform.parent;
                    Vector3 rightHandLocalPosition = InputSystem.rightController.GetLocalPosition();
                    Vector3 rightHandWorldPosition = xrRigOrigin.TransformPoint(rightHandLocalPosition);
                    Quaternion rightRotation = InputSystem.rightController.GetLocalRotation();
                    __instance.Parts.Forearm.Joint.targetRotation = new Quaternion(-rightRotation.z, -rightRotation.x , -rightRotation.y, rightRotation.w)* Quaternion.Euler(90,0,0);
                    Vector3 globalRightPos = rightHandWorldPosition - __instance.Parts.Forearm.transform.position;
                    ____curForearmDirection = globalRightPos;
                    __result = new Vector3(globalRightPos.x * handSpeed, globalRightPos.y * handSpeed, globalRightPos.z * handSpeed); 
                    return false;
                }

            }
            return false;
        }


    }
    [HarmonyPatch(typeof(HandPhysicsController), "ControlForearm")]
    internal class HandPatches {
        private static void Prefix(HandPhysicsController __instance, ref Vector3 ____curForearmDirection)
        {
            typeof(HandPhysicsController).GetMethod("set_IsForearmMoving", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
               .Invoke(__instance, new object[] { true });

        }
    }
    [HarmonyPatch(typeof(CheckVR), "Update")]
    internal class CheckVRPatch
    {
        private static void Postfix(CheckVR __instance)
        {
            if (InputSystem.leftController.controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
            {
                //stick button
                OpenVR.System.ResetSeatedZeroPose();
                OpenVR.Compositor.SetTrackingSpace(ETrackingUniverseOrigin.TrackingUniverseSeated);
            }
        }
    }
}
