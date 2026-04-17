using UnityEngine;
using System;
using System.Collections.Generic;

public class Grappling
{
    private Camera m_cam;
    private float m_maxDistance;
    private LayerMask m_grappling;

    public void SetCamera(Camera camera)
    {
        m_cam = camera;
    }

    public Collider GetMostNearPoint(List<Collider> colliders)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(m_cam);

        Collider best = null;
        float bestScore = float.MaxValue;

        foreach(var point in colliders)
        {

            //ƒJƒƒ‰‚Ì‹–ì“à‚È‚ç‘±s.
            if (!GeometryUtility.TestPlanesAABB(planes, point.bounds))
                continue;

            Vector3 viewPort = m_cam.WorldToViewportPoint(point.transform.position);

            //”w–Ê‚ÍœŠO.
            if (viewPort.z < 0)
                continue;

            float dx = viewPort.x - 0.5f;
            float dy = viewPort.y - 0.5f;

            //sqrt‰ñ”ğ.
            float score = dx * dx + dy * dy;

            if (score < bestScore)
            {
                bestScore = score;
                best = point;
            }
        }

        return best;
    }
}
