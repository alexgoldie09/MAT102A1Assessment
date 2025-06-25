using System;
using UnityEngine;

/*
 * CustomVector3.cs
 * ----------------
 * This struct defines a 3D vector class for use in custom math operations.
 * It simulates Unity's Vector3 but simplified for only needed functionality.
 *
 * Tasks:
 * - Applies basic vector math operations:
 *   + Addition, subtraction, scalar multiplication.
 * - Applies vector analysis:
 *   + Magnitude, normalization, dot product, cross product.
 * - Other functions include:
 *   + Linear interpolation (Lerp), angle calculation between vectors.
 *   + Conversion to UnityEngine.Vector3 for use in the engine.
 *
 * Extras:
 * - Struct was used as it is more memory efficient for constant math implementations.
 * - Methods are static to be accessible by all scripts.
 */

public struct CustomVector3
{
    public float x, y, z;

    // Constructor: Initialises vector with given x, y, z values.
    public CustomVector3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    /*
     * Overloading (+) operator adds two vectors (component-wise).
     * Result.x = a.x + b.x
    */
    public static CustomVector3 operator +(CustomVector3 a, CustomVector3 b)
    {
        return new CustomVector3(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    /*
     * Overloading (-) operator subtracts two vectors (component-wise).
     * Result.x = a.x - b.x
    */
    public static CustomVector3 operator -(CustomVector3 a, CustomVector3 b)
    {
        return new CustomVector3(a.x - b.x, a.y - b.y, a.z - b.z);
    }

    /*
     * Overloading (*) operator multiplies a vector by scalar value.
     * Result.x = v.x * scalar
    */
    public static CustomVector3 operator *(CustomVector3 v, float scalar)
    {
        return new CustomVector3(v.x * scalar, v.y * scalar, v.z * scalar);
    }

    /*
     * Dot() calculates the dot product of two vectors.
     * Indicates how aligned the two vectors are.
     * Returns a single float: a · b = sum of (a_i * b_i).
    */
    public static float Dot(CustomVector3 a, CustomVector3 b)
    {
        return a.x * b.x + a.y * b.y + a.z * b.z;
    }

    /*
     * Cross() calculates the cross product of two vectors.
     * Returns a vector that is perpendicular to the two input vectors.
     * - Follows right-hand rule for orientation.
    */
    public static CustomVector3 Cross(CustomVector3 a, CustomVector3 b)
    {
        return new CustomVector3(
            a.y * b.z - a.z * b.y,
            a.z * b.x - a.x * b.z,
            a.x * b.y - a.y * b.x
        );
    }

    /*
     * Magnitude() calculates the magnitude (length) of the vector.
     * Represents the Euclidean length of the vector.
     * Returns a single float: sqrt(x² + y² + z²).
    */
    public float Magnitude()
    {
        return Mathf.Sqrt(x * x + y * y + z * z);
    }

    /*
     * Normalize() returns a normalized copy of the vector.
     * Result has length 1 but same direction.
     * - If original vector is zero, returns (0,0,0).
    */
    public CustomVector3 Normalize()
    {
        float mag = Magnitude();
        return mag == 0 ? new CustomVector3(0, 0, 0) : new CustomVector3(x / mag, y / mag, z / mag);
    }

    /*
     * Lerp() linearly interpolates between two vectors based on the interpolant.
     * t = 0 returns a, t = 1 returns b, t = 0.5 returns mid point between a and b.
    */
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

    /*
     * AngleBetween() calculates the angle in degrees between two vectors.
     * - Useful for steering and orientation.
    */
    public static float AngleBetween(CustomVector3 a, CustomVector3 b)
    {
        float dot = Dot(a.Normalize(), b.Normalize());
        return Mathf.Acos(Mathf.Clamp(dot, -1f, 1f)) * Mathf.Rad2Deg;
    }

    /*
     * ToUnityVector3() converts this CustomVector3 to UnityEngine.Vector3.
     * - For compatibility with Unity's transform system.
    */
    public Vector3 ToUnityVector3()
    {
        return new Vector3(x, y, z);
    }

    /*
     * Overrides ToString() to return a string representation of the vector.
     * - Useful for debugging.
    */
    public override string ToString()
    {
        return $"({x}, {y}, {z})";
    }
}
