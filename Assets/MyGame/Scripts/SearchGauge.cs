using UnityEngine;
using Unity.Cinemachine;
public class SearchGauge
{
    private RectTransform m_rotationTarget;
    private Transform m_enemy;
    private Camera m_cam;
    private Transform m_player;

    public SearchGauge (RectTransform target)
    {
        m_rotationTarget = target;
    }

    //score‚ğ0`10‚ÌŠÔ‚É•âŠ®.
    public float CalcLeftFillValue(float score)
    {
        return Mathf.Lerp(0f, 10f, Mathf.InverseLerp(0, 40f, score));
    }

    //score‚ğ10`0‚ÌŠÔ‚É•âŠ®.
    public float CalcRightFillValue(float score)
    {
        return Mathf.Lerp(10f, 0f, Mathf.InverseLerp(0, 40f, score));
    }

    public void GetPlayer(Transform player)
    {
        if (m_player == null)
            m_player = player;
    }

    private float CalcAngleBetweenEnemyAndCam()
    {
        Vector2 rectEnemy = RectTransformUtility.WorldToScreenPoint(m_cam, m_enemy.transform.position);
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 dir = rectEnemy - screenCenter;
        float angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;

        //“G‚ªƒJƒƒ‰‚ÌŒã‚ë‚Ìê‡‚Ì•â³.
        Vector3 viewPoint = m_cam.WorldToViewportPoint(m_enemy.position);
        if (viewPoint.z < 0)
        {
            angle += 180f;
        }

        return angle;
    }

    public void TurnToEnemy(Transform enemy, Camera cam)
    {
        m_enemy = enemy;
        m_cam = cam;

        //“G‚ÆƒJƒƒ‰ŠÔ‚ÌŠp“x‚ğŒvZ‚µA”­Œ©ƒQ[ƒW‚ğ‰ñ“]‚³‚¹‚é.
        m_rotationTarget.localRotation = Quaternion.Euler(0, 0, -CalcAngleBetweenEnemyAndCam());
    }
}
