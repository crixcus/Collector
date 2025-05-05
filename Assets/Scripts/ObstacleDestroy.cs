using UnityEngine;

public class ObstacleDestroy : MonoBehaviour
{
    private bool isTriggered = true;

    public float fallSpeed = 2f;

    /*void Update()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;
    }*/

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Handle hit logic here
            StartCoroutine(DestroyAfterDelay());
        }
    }


    private System.Collections.IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(.5f);
        Destroy(gameObject);
    }
}

