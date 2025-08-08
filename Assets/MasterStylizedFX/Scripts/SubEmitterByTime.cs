using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MasterStylizedExplosions
{
    [ExecuteInEditMode]
    public class SubEmitterByTime : MonoBehaviour
    {
        public ParticleSystem Particle;
        public float TriggerInterval = 0.2f;
        float LastTime = 0;
        public float Timer;
        private void OnWillRenderObject()
        {
            Timer += Time.time - LastTime;
            if (Particle == null)
            {
                Particle = GetComponent<ParticleSystem>();
            }
            if (Particle != null && Timer >= TriggerInterval)
            {
                if (Particle.isPlaying)
                {
                    Timer = 0;
                    Particle.TriggerSubEmitter(0);
                }
            }
            LastTime = Time.time;
        }
    }
}
