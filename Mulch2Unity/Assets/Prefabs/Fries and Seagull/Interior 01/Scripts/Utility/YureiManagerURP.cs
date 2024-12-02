using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using Fries.Interior_01.Utility.Inspector;
#endif

namespace Fries.Interior_01.Utility {
    public class YureiManagerURP : MonoBehaviour {
        [Tooltip("Post Process effect including glowing will only show to these cameras")]
        public List<Camera> gameCameras = new();
        
        [YureiButton("Initialize")] [IgnoreInInspector]
        public Action initialize;

        private void Reset() {
            initialize = init;
        }

        private void init() {
            if (gameCameras == null || gameCameras.Count == 0) {
                Debug.LogError("Please provide at least 1 valid camera to Game Cameras field.");
                return;
            }

            VolumeProfile vp = Resources.Load<VolumeProfile>("Fries/Interior 01/Scripts/Setup/URP Volume");
            
            foreach (var camera in gameCameras) {
                Volume volume = camera.GetComponent<Volume>();
                if (volume) volume.sharedProfile = vp;
                else {
                    volume = camera.gameObject.AddComponent<Volume>();
                    volume.sharedProfile = vp;
                }
            }
            Debug.Log($"[Yurei] Init post-processor settings for Universal Rendering Pipeline successfully.");
        }

    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(YureiManagerURP))]
    public class YureiManagerURPInspector : YureiInspector { }
    #endif
}