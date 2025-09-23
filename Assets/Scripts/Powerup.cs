using UnityEngine;
using static Powerup;

public class Powerup : MonoBehaviour
{
    public enum PowerupType { Increase, Decrease }
    public PowerupType type;
    public float pickupRadius = 0.5f;

    void Update()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null) return;

        float dist = Vector2.Distance(transform.position, playerObj.transform.position);
        if (dist <= pickupRadius)
        {
            Player player = playerObj.GetComponent<Player>();
            if (player != null)
            {
                if (type == PowerupType.Increase)
                    player.IncreaseRocketCount();
                else if (type == PowerupType.Decrease)
                    player.DecreaseRocketCount();

                Destroy(gameObject);
            }
        }
    }
}
