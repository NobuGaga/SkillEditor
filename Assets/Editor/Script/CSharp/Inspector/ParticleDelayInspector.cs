using UnityEditor;
using UnityEngine;
using System;

namespace SkillEditor {

    [CustomEditor(typeof(ParticleDelay))]
    public class ParticleDelayInspector : Editor {
        
        private const string BatchAddDelayTime = "批量增加 StartDelay";
        private const string BatchSubDelayTime = "批量减少 StartDelay";

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(BatchAddDelayTime))
                SetParticlesDelay(AddDelayTime);
            if (GUILayout.Button(BatchSubDelayTime))
                SetParticlesDelay(SubDelayTime);
            EditorGUILayout.EndHorizontal();          
        }

        private void SetParticlesDelay(Action<ParticleSystem, float> action) {
            ParticleDelay delayComponent = target as ParticleDelay;
            if (delayComponent == null)
                return;
            ParticleSystem[] particles = delayComponent.GetComponentsInChildren<ParticleSystem>(true);
            if (particles == null || particles.Length == 0)
                return;
            float delayOffset = delayComponent.DelayTimeOffset;
            for (ushort index = 0; index < particles.Length; index++)
                action(particles[index], delayOffset);
            EditorUtility.SetDirty(delayComponent.gameObject);
            AssetDatabase.SaveAssets();
        }

        private void AddDelayTime(ParticleSystem particle, float delayTime) =>
            particle.startDelay += delayTime;

        private void SubDelayTime(ParticleSystem particle, float delayTime) =>
            particle.startDelay -= delayTime;
    }
}