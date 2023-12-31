using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public BallController m_BallController;
    public Rigidbody2D m_Rigidbody2D;

    public int m_CollisionCount;
    public TrailRenderer m_TrailRenderer;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            m_Rigidbody2D.isKinematic = true;
            m_Rigidbody2D.velocity = Vector2.zero;

            Vector3 nextShootingPosition = m_BallController.RetrieveBall(this.transform.position.x, m_CollisionCount == 0);
            StartCoroutine(Move(nextShootingPosition));
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Block"))
        {
            m_CollisionCount++;
        }
    }

    public IEnumerator Move(Vector3 target)
    {
        Vector2 origin = this.transform.position;
        float duration = Vector2.Distance(origin, target) / 20f;

        for (float time = 0; time < duration; time += Time.deltaTime)
        {
            this.transform.position = Vector2.Lerp(origin, target, time / duration);
            yield return null;
        }

        this.transform.position = target;
    }
}
