using UnityEngine;

public class PlayerCollisionChecker : ICollisionChecker
{
    private readonly Transform _playerTransform;
    private readonly float _minYForHit;
    private readonly System.Func<int> _laneProvider;

    public PlayerCollisionChecker(Transform playerTransform, float minYForHit, System.Func<int> laneProvider)
    {
        _playerTransform = playerTransform;
        _minYForHit = minYForHit;
        _laneProvider = laneProvider;
    }

    public bool CheckCollision(Vector3 obstaclePos, int obstacleLane)
    {
        bool sameLane = (obstacleLane == _laneProvider());
        float dz = Mathf.Abs(obstaclePos.z - _playerTransform.position.z);
        bool inZRange = dz <= 0.5f;
        bool lowEnough = _playerTransform.position.y <= _minYForHit;
        return sameLane && inZRange && lowEnough;
    }
}
