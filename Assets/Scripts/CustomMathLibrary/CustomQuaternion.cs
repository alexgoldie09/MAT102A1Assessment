using System;
using UnityEngine;

/*
 * CustomQuaternion.cs
 * -------------------
 * This struct defines a custom implementation of quaternions for 3D rotation.
 * It simulates Unity's Quaternion but simplified for only needed functionality.
 * 
 * Tasks:
 *  - Represent quaternions as 4D constructs (x, y, z, w) for efficient rotation.
 *  - Contains identity, normalization, and multiplication operations.
 *  - Convert between rotation matrix and quaternion formats.
 *  - Construct quaternions from axis-angle and Euler angles.
 *  - Convert to Unity's built-in Quaternion and rotation matrix representations.
 * 
 * Extras:
 *  - Quaternions avoid gimbal lock and allow smooth interpolation between orientations.
 *  - The core math involves unit normalization, matrix conversion, and axis-angle calculations.
 *  - All rotations are handled in a 3D homogeneous space using quaternion algebra.
 *  - Struct was used as it is more memory efficient for constant math implementations.
 *  - Methods are static to be accessible by all scripts.
 */

public struct CustomQuaternion
{
    public float x, y, z, w;

    // Constructor: Initialises quaternion with given x, y, z, w components.
    public CustomQuaternion(float x, float y, float z, float w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    /*
     * Identity() returns an identity quaternion (no rotation).
    */
    public static CustomQuaternion Identity()
    {
        return new CustomQuaternion(0f, 0f, 0f, 1f);
    }

    /*
     * Normalize() returns a normalized quaternion to ensure valid unit rotation.
    */
    public CustomQuaternion Normalize()
    {
        float mag = Mathf.Sqrt(x * x + y * y + z * z + w * w);
        return mag == 0 ? Identity() : new CustomQuaternion(x / mag, y / mag, z / mag, w / mag);
    }

    /*
     * Overloading (*) multiplies two quaternions (combines rotations).
     * - Quaternion multiplication is not commutative, order matters.
    */
    public static CustomQuaternion operator *(CustomQuaternion a, CustomQuaternion b)
    {
        return new CustomQuaternion(
            a.w * b.x + a.x * b.w + a.y * b.z - a.z * b.y,
            a.w * b.y - a.x * b.z + a.y * b.w + a.z * b.x,
            a.w * b.z + a.x * b.y - a.y * b.x + a.z * b.w,
            a.w * b.w - a.x * b.x - a.y * b.y - a.z * b.z
        );
    }

    /*
     * FromAxisAngles() creates a quaternion from an axis-angle rotation.
     * Returns an axis that must be normalized; angle in degrees.
    */
    public static CustomQuaternion FromAxisAngle(CustomVector3 axis, float angleDegrees)
    {
        float rad = angleDegrees * Mathf.Deg2Rad;
        float halfAngle = rad / 2f;
        float sinHalf = Mathf.Sin(halfAngle);
        float cosHalf = Mathf.Cos(halfAngle);

        CustomVector3 normAxis = axis.Normalize();

        return new CustomQuaternion(
            normAxis.x * sinHalf,
            normAxis.y * sinHalf,
            normAxis.z * sinHalf,
            cosHalf
        ).Normalize();
    }

    /*
     * FromEulerAngles() constructs from Euler angles (pitch, yaw, roll).
     * - Uses YXZ rotation (same as Unity).
    */
    public static CustomQuaternion FromEulerAngles(float pitch, float yaw, float roll)
    {
        var xRot = FromAxisAngle(new CustomVector3(1, 0, 0), pitch);
        var yRot = FromAxisAngle(new CustomVector3(0, 1, 0), yaw);
        var zRot = FromAxisAngle(new CustomVector3(0, 0, 1), roll);
        return (yRot * xRot * zRot).Normalize();
    }

    /*
     * FromMatrix4x4() creates a CustomQuaternion from a rotation matrix (assumes orthogonal 3x3).
     * - Used to extract rotational information from a matrix (especially after combining transforms).
     * - Ensures the resulting quaternion maintains unit length.
     * - Highlights safe computation using trace and dominant diagonal element selection for stability.
     * - Based on standard algorithm from matrix-to-quaternion conversion.
    */
    public static CustomQuaternion FromMatrix4x4(CustomMatrix4x4 m)
    {
        // Extract the trace of the matrix (sum of diagonal elements)
        // Trace = m00 + m11 + m22; used to determine if a fast path is available
        float trace = m.m[0, 0] + m.m[1, 1] + m.m[2, 2];

        // --- Fast path: if trace is positive, use optimized formula
        if (trace > 0f)
        {
            // s = 1 / (4 * qw), computed in a numerically stable way
            float s = 0.5f / Mathf.Sqrt(trace + 1f);
            // Compute quaternion components from matrix using trace shortcut
            float w = 0.25f / s;
            float x = (m.m[2, 1] - m.m[1, 2]) * s;
            float y = (m.m[0, 2] - m.m[2, 0]) * s;
            float z = (m.m[1, 0] - m.m[0, 1]) * s;
            // Return normalized quaternion
            return new CustomQuaternion(x, y, z, w).Normalize();
        }
        // --- Fallback: choose the largest diagonal term to improve numerical stability
        else
        {
            // Case 1: m00 is the largest diagonal value
            if (m.m[0, 0] > m.m[1, 1] && m.m[0, 0] > m.m[2, 2])
            {
                float s = 2f * Mathf.Sqrt(1f + m.m[0, 0] - m.m[1, 1] - m.m[2, 2]);
                float w = (m.m[2, 1] - m.m[1, 2]) / s;
                float x = 0.25f * s;
                float y = (m.m[0, 1] + m.m[1, 0]) / s;
                float z = (m.m[0, 2] + m.m[2, 0]) / s;
                return new CustomQuaternion(x, y, z, w).Normalize();
            }
            // Case 2: m11 is the largest diagonal value
            else if (m.m[1, 1] > m.m[2, 2])
            {
                float s = 2f * Mathf.Sqrt(1f + m.m[1, 1] - m.m[0, 0] - m.m[2, 2]);
                float w = (m.m[0, 2] - m.m[2, 0]) / s;
                float x = (m.m[0, 1] + m.m[1, 0]) / s;
                float y = 0.25f * s;
                float z = (m.m[1, 2] + m.m[2, 1]) / s;
                return new CustomQuaternion(x, y, z, w).Normalize();
            }
            // Case 3: m22 is the largest diagonal value
            else
            {
                float s = 2f * Mathf.Sqrt(1f + m.m[2, 2] - m.m[0, 0] - m.m[1, 1]);
                float w = (m.m[1, 0] - m.m[0, 1]) / s;
                float x = (m.m[0, 2] + m.m[2, 0]) / s;
                float y = (m.m[1, 2] + m.m[2, 1]) / s;
                float z = 0.25f * s;
                return new CustomQuaternion(x, y, z, w).Normalize();
            }
        }
    }

    /*
     * ToMatrix4x4() converts this quaternion to a 4x4 rotation matrix.
     * - Useful when integrating the quaternion rotation into a larger transformation matrix 
     *   (e.g., combined with translation and scaling).
    */
    public CustomMatrix4x4 ToMatrix4x4()
    {
        // Precompute squared and cross terms for optimization
        float xx = x * x, yy = y * y, zz = z * z;
        float xy = x * y, xz = x * z, yz = y * z;
        float wx = w * x, wy = w * y, wz = w * z;

        // Initialise an identity matrix and overwrite its rotation components
        CustomMatrix4x4 m = new CustomMatrix4x4(true);

        // First row
        m.m[0, 0] = 1f - 2f * (yy + zz); // Rotation component affecting X-axis
        m.m[0, 1] = 2f * (xy - wz);      // XY coupling
        m.m[0, 2] = 2f * (xz + wy);      // XZ coupling
        m.m[0, 3] = 0f;                  // No translation
        // Second row
        m.m[1, 0] = 2f * (xy + wz);      // YX coupling
        m.m[1, 1] = 1f - 2f * (xx + zz); // Rotation component affecting Y-axis
        m.m[1, 2] = 2f * (yz - wx);      // YZ coupling 
        m.m[1, 3] = 0f;
        // Third row
        m.m[2, 0] = 2f * (xz - wy);      // ZX coupling
        m.m[2, 1] = 2f * (yz + wx);      // ZY coupling
        m.m[2, 2] = 1f - 2f * (xx + yy); // Rotation component affecting Z-axis
        m.m[2, 3] = 0f;
        // Fourth row for homogeneous transform
        m.m[3, 0] = 0f;
        m.m[3, 1] = 0f;
        m.m[3, 2] = 0f;
        m.m[3, 3] = 1f;

        return m;
    }

    /*
     * ToUnityQuaternion() converts this CustomQuaternion to UnityEngine.Quaternion.
    */
    public Quaternion ToUnityQuaternion()
    {
        return new Quaternion(x, y, z, w);
    }

    /*
     * Overrides ToString() to return a string representation of the quaternion.
     * - Useful for debugging.
    */
    public override string ToString()
    {
        return $"({x}, {y}, {z}, {w})";
    }
}
