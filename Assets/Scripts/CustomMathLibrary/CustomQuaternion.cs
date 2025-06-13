using System;
using UnityEngine;

/// <summary>
/// Represents a quaternion for 3D rotation.
/// Quaternions avoid gimbal lock and are very efficient for rotation interpolation.
/// </summary>
public struct CustomQuaternion
{
    public float x, y, z, w;

    /// <summary>
    /// Constructor: initializes quaternion with given x, y, z, w components.
    /// </summary>
    public CustomQuaternion(float x, float y, float z, float w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    /// <summary>
    /// Returns an identity quaternion (no rotation).
    /// Equivalent to (0,0,0,1).
    /// </summary>
    public static CustomQuaternion Identity()
    {
        return new CustomQuaternion(0f, 0f, 0f, 1f);
    }

    /// <summary>
    /// Returns a normalized copy of this quaternion.
    /// Ensures the quaternion represents a valid rotation.
    /// </summary>
    public CustomQuaternion Normalize()
    {
        float mag = (float)Math.Sqrt(x * x + y * y + z * z + w * w);
        return mag == 0 ? Identity() : new CustomQuaternion(x / mag, y / mag, z / mag, w / mag);
    }

    /// <summary>
    /// Multiplies two quaternions (combines rotations).
    /// Note: Quaternion multiplication is not commutative → order matters!
    /// </summary>
    public static CustomQuaternion operator *(CustomQuaternion a, CustomQuaternion b)
    {
        return new CustomQuaternion(
            a.w * b.x + a.x * b.w + a.y * b.z - a.z * b.y,
            a.w * b.y - a.x * b.z + a.y * b.w + a.z * b.x,
            a.w * b.z + a.x * b.y - a.y * b.x + a.z * b.w,
            a.w * b.w - a.x * b.x - a.y * b.y - a.z * b.z
        );
    }

    /// <summary>
    /// Creates a quaternion from an axis-angle rotation.
    /// Axis must be normalized.
    /// </summary>
    public static CustomQuaternion FromAxisAngle(CustomVector3 axis, float angleDegrees)
    {
        float rad = angleDegrees * (float)Math.PI / 180f;
        float halfAngle = rad / 2f;
        float sinHalf = (float)Math.Sin(halfAngle);
        float cosHalf = (float)Math.Cos(halfAngle);

        CustomVector3 normAxis = axis.Normalize();

        return new CustomQuaternion(
            normAxis.x * sinHalf,
            normAxis.y * sinHalf,
            normAxis.z * sinHalf,
            cosHalf
        ).Normalize();
    }

    /// <summary>
    /// Converts this quaternion to a 4x4 rotation matrix.
    /// Can be used with CustomMatrix4x4 * CustomVector3 to apply rotation.
    /// </summary>
    public CustomMatrix4x4 ToMatrix4x4()
    {
        CustomMatrix4x4 m = CustomMatrix4x4.CreateEmpty();

        float xx = x * x;
        float yy = y * y;
        float zz = z * z;
        float xy = x * y;
        float xz = x * z;
        float yz = y * z;
        float wx = w * x;
        float wy = w * y;
        float wz = w * z;

        // 3x3 rotation part of matrix
        m.m[0, 0] = 1f - 2f * (yy + zz);
        m.m[0, 1] = 2f * (xy - wz);
        m.m[0, 2] = 2f * (xz + wy);
        m.m[0, 3] = 0f;

        m.m[1, 0] = 2f * (xy + wz);
        m.m[1, 1] = 1f - 2f * (xx + zz);
        m.m[1, 2] = 2f * (yz - wx);
        m.m[1, 3] = 0f;

        m.m[2, 0] = 2f * (xz - wy);
        m.m[2, 1] = 2f * (yz + wx);
        m.m[2, 2] = 1f - 2f * (xx + yy);
        m.m[2, 3] = 0f;

        // bottom row for homogeneous transform
        m.m[3, 0] = 0f;
        m.m[3, 1] = 0f;
        m.m[3, 2] = 0f;
        m.m[3, 3] = 1f;

        return m;
    }

    /// <summary>
    /// Converts this CustomQuaternion to UnityEngine.Quaternion.
    /// </summary>
    public Quaternion ToUnityQuaternion()
    {
        return new Quaternion(x, y, z, w);
    }

    /// <summary>
    /// Returns a readable string representation of the quaternion.
    /// Useful for debugging.
    /// </summary>
    public override string ToString()
    {
        return $"({x}, {y}, {z}, {w})";
    }
}
