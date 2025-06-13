using System;
using UnityEngine;

/// <summary>
/// Represents a 3D vector with basic math operations.
/// Useful for positions, directions, velocities, etc.
/// </summary>
public struct CustomVector3
{
    public float x, y, z;

    /// <summary>
    /// Constructor: initializes vector with given x, y, z values.
    /// </summary>
    public CustomVector3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    /// <summary>
    /// Adds two vectors (component-wise).
    /// Result.x = a.x + b.x, etc.
    /// </summary>
    public static CustomVector3 operator +(CustomVector3 a, CustomVector3 b)
    {
        return new CustomVector3(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    /// <summary>
    /// Subtracts two vectors (component-wise).
    /// Result.x = a.x - b.x, etc.
    /// </summary>
    public static CustomVector3 operator -(CustomVector3 a, CustomVector3 b)
    {
        return new CustomVector3(a.x - b.x, a.y - b.y, a.z - b.z);
    }

    /// <summary>
    /// Multiplies vector by scalar (scales vector length).
    /// Result.x = v.x * scalar, etc.
    /// </summary>
    public static CustomVector3 operator *(CustomVector3 v, float scalar)
    {
        return new CustomVector3(v.x * scalar, v.y * scalar, v.z * scalar);
    }

    /// <summary>
    /// Computes dot product of two vectors.
    /// Returns a single float: a · b = sum of (a_i * b_i).
    /// Geometric meaning: measures "alignment" of the vectors.
    /// </summary>
    public static float Dot(CustomVector3 a, CustomVector3 b)
    {
        return a.x * b.x + a.y * b.y + a.z * b.z;
    }

    /// <summary>
    /// Computes cross product of two vectors.
    /// Result is a new vector perpendicular to both inputs.
    /// Useful for finding surface normals.
    /// </summary>
    public static CustomVector3 Cross(CustomVector3 a, CustomVector3 b)
    {
        return new CustomVector3(
            a.y * b.z - a.z * b.y,
            a.z * b.x - a.x * b.z,
            a.x * b.y - a.y * b.x
        );
    }

    /// <summary>
    /// Computes the magnitude (length) of the vector.
    /// Formula: sqrt(x² + y² + z²).
    /// </summary>
    public float Magnitude()
    {
        return (float)Math.Sqrt(x * x + y * y + z * z);
    }

    /// <summary>
    /// Returns a normalized copy of the vector.
    /// Result has length 1 but same direction.
    /// If original vector is zero, returns (0,0,0).
    /// </summary>
    public CustomVector3 Normalize()
    {
        float mag = Magnitude();
        return mag == 0 ? new CustomVector3(0, 0, 0) : new CustomVector3(x / mag, y / mag, z / mag);
    }

    /// <summary>
    /// Linearly interpolates between two CustomVector3 vectors.
    /// t = 0 → returns a, t = 1 → returns b.
    /// </summary>
    public static CustomVector3 Lerp(CustomVector3 a, CustomVector3 b, float t)
    {
        // Clamp t between 0 and 1 for safety
        t = Mathf.Clamp01(t);

        return new CustomVector3(
            a.x * (1 - t) + b.x * t,
            a.y * (1 - t) + b.y * t,
            a.z * (1 - t) + b.z * t
        );
    }

    /// <summary>
    /// Converts this CustomVector3 to UnityEngine.Vector3.
    /// </summary>
    public Vector3 ToUnityVector3()
    {
        return new Vector3(x, y, z);
    }


    /// <summary>
    /// Returns a readable string representation of the vector.
    /// Useful for debugging.
    /// </summary>
    public override string ToString()
    {
        return $"({x}, {y}, {z})";
    }
}
