using System;
using UnityEngine;

/// <summary>
/// Represents a 4x4 matrix with basic math operations.
/// This can be used for transforms (translation, rotation, scaling).
/// </summary>
public struct CustomMatrix4x4
{
    // The matrix data: 4x4 array of floats
    public float[,] m;

    /// <summary>
    /// Constructor: initializes a matrix.
    /// If 'identity' is true → creates identity matrix (diagonal = 1).
    /// </summary>
    public CustomMatrix4x4(bool identity = false)
    {
        m = new float[4, 4];
        if (identity)
        {
            // Fill diagonal with 1's → identity matrix
            for (int i = 0; i < 4; i++)
                m[i, i] = 1f;
        }
    }

    /// <summary>
    /// Creates an empty matrix (m array initialized).
    /// </summary>
    public static CustomMatrix4x4 CreateEmpty()
    {
        return new CustomMatrix4x4(false);
    }

    /// <summary>
    /// Adds two 4x4 matrices.
    /// Result.m[row, col] = a.m[row, col] + b.m[row, col]
    /// </summary>
    public static CustomMatrix4x4 operator +(CustomMatrix4x4 a, CustomMatrix4x4 b)
    {
        CustomMatrix4x4 result = new CustomMatrix4x4();
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                result.m[row, col] = a.m[row, col] + b.m[row, col];
            }
        }
        return result;
    }

    /// <summary>
    /// Multiplies two 4x4 matrices (matrix multiplication).
    /// Standard definition: result[row, col] = sum over k of (a[row, k] * b[k, col])
    /// </summary>
    public static CustomMatrix4x4 operator *(CustomMatrix4x4 a, CustomMatrix4x4 b)
    {
        CustomMatrix4x4 result = new CustomMatrix4x4();
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                float sum = 0f;
                for (int k = 0; k < 4; k++)
                {
                    // Multiply row of 'a' with column of 'b'
                    sum += a.m[row, k] * b.m[k, col];
                }
                result.m[row, col] = sum;
            }
        }
        return result;
    }

    /// <summary>
    /// Multiplies a matrix by a vector (Matrix * Vector).
    /// Assumes vector w=1 for homogeneous coordinate (so translation is applied).
    /// Result is a transformed 3D vector.
    /// </summary>
    public static CustomVector3 operator *(CustomMatrix4x4 matrix, CustomVector3 vector)
    {
        float x = matrix.m[0, 0] * vector.x + matrix.m[0, 1] * vector.y + matrix.m[0, 2] * vector.z + matrix.m[0, 3];
        float y = matrix.m[1, 0] * vector.x + matrix.m[1, 1] * vector.y + matrix.m[1, 2] * vector.z + matrix.m[1, 3];
        float z = matrix.m[2, 0] * vector.x + matrix.m[2, 1] * vector.y + matrix.m[2, 2] * vector.z + matrix.m[2, 3];

        return new CustomVector3(x, y, z);
    }

    /// <summary>
    /// Creates a translation matrix.
    /// Moves objects by (tx, ty, tz).
    /// </summary>
    public static CustomMatrix4x4 CreateTranslation(float tx, float ty, float tz)
    {
        CustomMatrix4x4 result = new CustomMatrix4x4(true); // start with identity
        result.m[0, 3] = tx; // set translation component for x
        result.m[1, 3] = ty; // set translation component for y
        result.m[2, 3] = tz; // set translation component for z
        return result;
    }

    /// <summary>
    /// Creates a scaling matrix.
    /// Scales objects by (sx, sy, sz).
    /// </summary>
    public static CustomMatrix4x4 CreateScaling(float sx, float sy, float sz)
    {
        CustomMatrix4x4 result = new CustomMatrix4x4();
        result.m[0, 0] = sx; // scale X axis
        result.m[1, 1] = sy; // scale Y axis
        result.m[2, 2] = sz; // scale Z axis
        result.m[3, 3] = 1f; // homogeneous coordinate stays 1
        return result;
    }

    /// <summary>
    /// Creates a rotation matrix around the Z-axis.
    /// Rotates by angleDegrees (clockwise).
    /// </summary>
    public static CustomMatrix4x4 CreateRotationZ(float angleDegrees)
    {
        float rad = angleDegrees * (float)Math.PI / 180f;
        float cos = (float)Math.Cos(rad);
        float sin = (float)Math.Sin(rad);

        CustomMatrix4x4 result = new CustomMatrix4x4(true); // start with identity
        result.m[0, 0] = cos;   // cos(theta)
        result.m[0, 1] = -sin;  // -sin(theta)
        result.m[1, 0] = sin;   // sin(theta)
        result.m[1, 1] = cos;   // cos(theta)
        return result;
    }

    /// <summary>
    /// Converts this CustomMatrix4x4 to UnityEngine.Matrix4x4.
    /// Useful for passing to shaders or advanced transform effects.
    /// </summary>
    public Matrix4x4 ToUnityMatrix4x4()
    {
        Matrix4x4 unityMatrix = new Matrix4x4();

        // Unity Matrix4x4 is column-major, but you can assign row/column directly.
        // Our m[row,col] maps to Unity m[row,col].
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                unityMatrix[row, col] = m[row, col];
            }
        }

        return unityMatrix;
    }


    /// <summary>
    /// Helper method: prints the matrix to console (for debugging).
    /// </summary>
    public void Print()
    {
        for (int row = 0; row < 4; row++)
        {
            Console.WriteLine($"{m[row, 0]}, {m[row, 1]}, {m[row, 2]}, {m[row, 3]}");
        }
        Console.WriteLine();
    }
}
