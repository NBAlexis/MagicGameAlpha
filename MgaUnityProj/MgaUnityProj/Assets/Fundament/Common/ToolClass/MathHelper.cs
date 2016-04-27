using UnityEngine;
using System.Collections;

public class MathHelper
{
    public static Vector3 BoundClamp(Transform pBoundTransform, Vector3 vConstrant, Vector3 vPos, bool bHasYDir = false, Vector3 vOffset = default(Vector3))
    {
        //if (!bHasYDir)
        //{
        //    vConstrant.y = 10000.0f;
        //    vPos.y = pBoundTransform.position.y;
        //}

        //Vector3 vDelta = vPos - pBoundTransform.position;
        vConstrant.x = vConstrant.x < 0.0f ? 0.01f : vConstrant.x;
        vConstrant.y = vConstrant.y < 0.0f ? 0.01f : vConstrant.y;
        vConstrant.z = vConstrant.z < 0.0f ? 0.01f : vConstrant.z;
        
        pBoundTransform.position += vOffset;
        Vector3 vDelta = pBoundTransform.InverseTransformDirection(vPos - pBoundTransform.position);
        vDelta.x = Mathf.Clamp(vDelta.x, -vConstrant.x, vConstrant.x);
        vDelta.y = bHasYDir ? Mathf.Clamp(vDelta.y, -vConstrant.y, vConstrant.y) : 0.0f;
        vDelta.z = Mathf.Clamp(vDelta.z, -vConstrant.z, vConstrant.z);
        vDelta = pBoundTransform.TransformDirection(vDelta) + pBoundTransform.position;
        pBoundTransform.position -= vOffset;
        return vDelta;
    }

    public static bool IsBoundClamp(Transform pBoundTransform, Vector3 vConstrant, Vector3 vPos, Vector3 vOffset = default(Vector3))
    {
        vConstrant.x = vConstrant.x < 0.0f ? 0.01f : vConstrant.x;
        vConstrant.y = vConstrant.y < 0.0f ? 0.01f : vConstrant.y;
        vConstrant.z = vConstrant.z < 0.0f ? 0.01f : vConstrant.z;

        pBoundTransform.position += vOffset;
        Vector3 vDelta = pBoundTransform.InverseTransformDirection(vPos - pBoundTransform.position);
        pBoundTransform.position -= vOffset;
        if (vDelta.x > vConstrant.x || vDelta.x < -vConstrant.x)
        {
            return false;
        }

        if (vDelta.y > vConstrant.y || vDelta.y < -vConstrant.y)
        {
            return false;
        }

        if (vDelta.z > vConstrant.z || vDelta.z < -vConstrant.z)
        {
            return false;
        }

        return true;
    }

    public static Vector3 SimpleRayCast(Ray ray, Transform pPlane)
    {
        float fCos = Vector3.Dot(ray.direction, -pPlane.up);
        if (fCos < 0.01F && fCos > -0.01f)
        {
            return Vector3.zero;
        }

        return ray.GetPoint(Vector3.Dot(pPlane.position - ray.origin, -pPlane.up) / fCos);
    }


}
