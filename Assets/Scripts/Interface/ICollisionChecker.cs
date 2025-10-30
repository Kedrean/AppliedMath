using UnityEngine;

public interface ICollisionChecker
{
    bool CheckCollision(Vector3 obstaclePos, int obstacleLane);
}