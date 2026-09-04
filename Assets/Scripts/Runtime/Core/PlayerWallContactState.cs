using UnityEngine;

namespace FlowState.Runtime.Core
{
    public readonly struct PlayerWallContactState
    {
        public bool HasLeftWall { get; }

        public Vector3 LeftWallNormal { get; }

        public bool HasRightWall { get; }

        public Vector3 RightWallNormal { get; }

        public bool HasWallContact => HasLeftWall || HasRightWall;

        public PlayerWallContactState(
            bool hasLeftWall,
            Vector3 leftWallNormal,
            bool hasRightWall,
            Vector3 rightWallNormal)
        {
            HasLeftWall = hasLeftWall;
            LeftWallNormal = hasLeftWall ? leftWallNormal.normalized : Vector3.zero;
            HasRightWall = hasRightWall;
            RightWallNormal = hasRightWall ? rightWallNormal.normalized : Vector3.zero;
        }
    }
}
