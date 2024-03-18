using UnityEngine;

public class DestroySelfWhenTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
    }
}
