using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Platforms
{
    public class PlayerPlatformMove
    {
        public Transform transform { get; set; } 
        public Vector3 velocity { get; set; }
        public bool OnPlatform { get; set; }  // if player is on moving platform
        public bool BeforePlatform { get; set; } // declares if player will move before platform will

        public PlayerPlatformMove(Transform _transform, Vector3 _velocity, bool _OnPlatform, bool _BeforePlatform)
        {
            transform = _transform;
            velocity = _velocity;
            OnPlatform = _OnPlatform;
            BeforePlatform = _BeforePlatform;
        }

    }
}
