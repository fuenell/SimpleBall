using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallController : MonoBehaviour
{
    private WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
    private int m_RetrieveCount = 0;
    private bool m_IsSkipping;

    public Vector2 m_ShootingPosition;

    public GameObject m_BallPrefab;
    public List<Ball> m_BallList;
    public float m_MaxX;
    public Text m_BallCountText;
    public Vector3 m_TextPosOffset = new Vector3(0, -1);
    public float m_BallSpeed = 800f;
    public float m_MinYVelocity = 0.2f;

    void Start()
    {
        SetBallCountText(m_BallList.Count, m_ShootingPosition);
    }

    // 모든 공 발사
    public IEnumerator LaunchAllBall(Vector3 direction, Action callback)
    {
        m_IsSkipping = false;

        float launchTime = Time.time;

        m_RetrieveCount = 0;

        Vector3 initPos = m_ShootingPosition;

        // 공 발사
        for (int i = 0; i < m_BallList.Count; i++)
        {
            if (m_IsSkipping)
            {
                break;
            }

            SetBallCountText(m_BallList.Count - i - 1, initPos);
            m_BallList[i].m_TrailRenderer.enabled = true;
            m_BallList[i].m_CollisionCount = 0;
            m_BallList[i].m_Rigidbody2D.isKinematic = false;
            m_BallList[i].m_Rigidbody2D.AddForce(direction * m_BallSpeed);
            StartCoroutine(CheckHorizontal(m_BallList[i].m_Rigidbody2D));
            yield return StartCoroutine(WaitFixedUpdate(3));

            SetTimeScale(launchTime);
        }

        // 공 수신 대기
        while (m_RetrieveCount < m_BallList.Count)
        {
            if (m_IsSkipping)
            {
                break;
            }

            yield return null;

            SetTimeScale(launchTime);
        }

        while (Time.timeScale == 0)
        {
            yield return null;
        }
        Time.timeScale = 1f;


        if (m_IsSkipping)
        {
            yield return new WaitForSeconds(1);
        }

        SetBallCountText(m_BallList.Count, m_ShootingPosition);
        callback?.Invoke();
    }

    // 시간에 따른 게임 속도 증가
    private void SetTimeScale(float launchTime)
    {
        if (Time.timeScale == 0)
        {
            return;
        }
        // 최저 속도 1, 최고 속도 20, 발사 이후 흐른 시간에 비례해 게임 속도 증가
        Time.timeScale = Mathf.Min(Mathf.Floor(Mathf.Max(1f, Mathf.Sqrt(Time.time - launchTime))), 20f);
    }

    // 공 수평 이동 검사
    private IEnumerator CheckHorizontal(Rigidbody2D rigidbody2D)
    {
        yield return new WaitForSeconds(1f);
        while (rigidbody2D.isKinematic == false)
        {
            if (-m_MinYVelocity < rigidbody2D.velocity.y && rigidbody2D.velocity.y < m_MinYVelocity)
            {
                rigidbody2D.AddForce(Vector2.down * 1000f);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    // 발사한 공 바닥에 도착
    public Vector2 RetrieveBall(float x, bool isMissed)
    {
        if (m_RetrieveCount == 0)   // 처음으로 떨어진 공의 위치 지정
        {
            if (m_MaxX < x)
            {
                m_ShootingPosition.x = m_MaxX;
            }
            else if (x < -m_MaxX)
            {
                m_ShootingPosition.x = -m_MaxX;
            }
            else
            {
                m_ShootingPosition.x = x;
            }
        }

        // 빗나간 공이 돌아옴
        if (isMissed)
        {
            // 더 이상 충돌이 없을 상태인지 검사
            if (CheckMissedShot())
            {
                ForceRetrieveAllBall(m_ShootingPosition);
            }
        }

        m_RetrieveCount++;

        return m_ShootingPosition;
    }

    // 모든 공 강제 회수
    private void ForceRetrieveAllBall(Vector2 nextShootingPosition)
    {
        SetBallCountText(0, nextShootingPosition);

        for (int i = 0; i < m_BallList.Count; i++)
        {
            m_BallList[i].m_TrailRenderer.enabled = false;
            m_BallList[i].m_Rigidbody2D.isKinematic = true;
            m_BallList[i].m_Rigidbody2D.velocity = Vector2.zero;
            StartCoroutine(m_BallList[i].Move(nextShootingPosition));
        }

        m_RetrieveCount = m_BallList.Count;

        m_IsSkipping = true;
    }

    // 더 이상 충돌이 없을 상태인지 검사
    private bool CheckMissedShot()
    {
        for (int i = 0; i < m_BallList.Count; i++)
        {
            if (m_BallList[i].m_Rigidbody2D.isKinematic == false)
            {
                if (m_BallList[i].m_CollisionCount != 0)
                {
                    return false;
                }
            }
        }
        return true;
    }

    // FixedUpdate 한프레임 기다림
    private IEnumerator WaitFixedUpdate(int count)
    {
        for (int i = 0; i < count; i++)
        {
            yield return waitForFixedUpdate;
        }
    }

    // 공 추가
    public void AddBall(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject ball = Instantiate(m_BallPrefab, m_ShootingPosition, Quaternion.identity, this.transform);
            ball.GetComponent<Ball>().m_BallController = this;
            m_BallList.Add(ball.GetComponent<Ball>());
        }
        SetBallCountText(m_BallList.Count, m_ShootingPosition);
    }

    private void SetBallCountText(int count, Vector3 launchPosition)
    {
        if (count <= 0)
        {
            m_BallCountText.text = "";
        }
        else
        {
            m_BallCountText.text = "x" + count;
            m_BallCountText.transform.position = launchPosition + m_TextPosOffset;
        }
    }
}
