using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static HandsSimulatorVRMod.VRController;

namespace HandsSimulatorVRMod
{

    class InputSystem {
        public static VRController leftController = null;
        public static VRController rightController = null;
        public static void InitControllers() {
            leftController = new VRController(ControllerType.Left);
            rightController = new VRController(ControllerType.Right);
            ControllerStatus leftInitStatus = leftController.Init();
            ControllerStatus rightInitStatus = rightController.Init();
            if (leftInitStatus == ControllerStatus.FailedInit || rightInitStatus == ControllerStatus.FailedInit) {
                Debug.LogError($"Left or Right controller not Inited: LeftStatus: {leftInitStatus} | RightStatus: {rightInitStatus} - try to restart steam vr");
                HarmonyPatches.RemoveHarmonyPatches();
                return;
            }
            Debug.Log($"Init controller status: LeftStatus: {leftInitStatus} | RightStatus: {rightInitStatus}");
        }
    }
    internal class VRController
    {
        public enum ControllerStatus { 
            FailedInit,
            SuccessInit
        }
        public enum ControllerType { 
            Left = 1,
            Right
        }

        public SteamVR_Controller.Device controller = null;
        public ControllerType controllerType;
        public VRController(ControllerType controllerType) {
            this.controllerType = controllerType;
        }
        public ControllerStatus Init() {
            this.controller = SteamVR_Controller.Input((int)this.controllerType);
            if (this.controller == null) { 
                return ControllerStatus.FailedInit;
            }
            return ControllerStatus.SuccessInit;
        }
        public Vector3 GetLocalPosition() {
            return this.controller.transform.pos;
        }
        public Quaternion GetLocalRotation() { 
            return this.controller.transform.rot; 
        }
    }
}
