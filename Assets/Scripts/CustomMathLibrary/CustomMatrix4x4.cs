using System;
using UnityEngine;

/*
 * CustomMatrix4x4.cs
 * -------------------
 * This struct defines a custom 4x4 matrix structure for use in manual 3D transformations 
 * such as translation, rotation, and scaling.
 * It simulates Unity's Matrix4x4 but simplified for only needed functionality.
 *
 * Tasks:
 *  - Implements matrix addition, multiplication (both matrix-matrix and matrix-vector).
 *  - Provides methods for constructing translation, rotation (Euler and axis-angle), and scaling matrices.
 *  - Converts to Unity's native Matrix4x4 when needed for rendering or debug.
 * 
 * Extras:
 *  - Matrix operations follow standard linear algebra rules.
 *  - Rotation matrices are constructed using Euler angles for axis-based rotation.
 *  - Homogeneous coordinates are assumed (w = 1) for correct transformation behaviour in 3D space.
 *  - Struct was used as it is more memory efficient for constant math implementations.
 *  - Methods are static to be accessible by all scripts.
 */

public struct CustomMatrix4x4
{
    public float[,] m; // 4x4 matrix array

    // Constructor: Initialises a 4x4 matrix but optionally can make an identity matrix.
    public CustomMatrix4x4(bool identity = false)
    {
        m = new float[4, 4];
        if (identity)
        {
            // Fill diagonal with 1's = identity matrix
            for (int i = 0; i < 4; i++)
                m[i, i] = 1f;
        }
    }

    /*
     * Overloading (+) operator adds two matrices element-wise.
     * Result.m[row, col] = a.m[row, col] + b.m[row, col]
    */
    public static CustomMatrix4x4 operator +(CustomMatrix4x4 a, CustomMatrix4x4 b)
    {
        CustomMatrix4x4 result = new CustomMatrix4x4(false);
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                result.m[row, col] = a.m[row, col] + b.m[row, col];
            }
        }
        return result;
    }

    /*
     * Overloading (*) operator adds standard row-column dot product (A * B).
     * Result[row, col] = sum over k of (a[row, k] * b[k, col])
    */
    public static CustomMatrix4x4 operator *(CustomMatrix4x4 a, CustomMatrix4x4 b)
    {
        CustomMatrix4x4 result = new CustomMatrix4x4(false);
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                float sum = 0f;
                for (int k = 0; k < 4; k++)
                {
                    sum += a.m[row, k] * b.m[k, col];
                }
                result.m[row, col] = sum;
            }
        }
        return result;
    }

    /*
     * Overloading (*) operator can also multiply a matrix by a vector.
     * Applies transformation to a point (w = 1).
     * Result is a transformed 3D vector.
    */
    public static CustomVector3 operator *(CustomMatrix4x4 matrix, CustomVector3 vector)
    {
        float x = matrix.m[0, 0] * vector.x + matrix.m[0, 1] * vector.y + matrix.m[0, 2] * vector.z + matrix.m[0, 3];
        float y = matrix.m[1, 0] * vector.x + matrix.m[1, 1] * vector.y + matrix.m[1, 2] * vector.z + matrix.m[1, 3];
        float z = matrix.m[2, 0] * vector.x + matrix.m[2, 1] * vector.y + matrix.m[2, 2] * vector.z + matrix.m[2, 3];

        return new CustomVector3(x, y, z);
    }

    /*
     * CreateTranslation() creates a translation matrix.
     * - Used for moving objects by (tx, ty, tz).
     * [ 1  0  0  tx ]
     * [ 0  1  0  ty ]
     * [ 0  0  1  tz ]
     * [ 0  0  0   1 ]
    */
    public static CustomMatrix4x4 CreateTranslation(float tx, float ty, float tz)
    {
        CustomMatrix4x4 result = new CustomMatrix4x4(true); // start with identity
        result.m[0, 3] = tx;
        result.m[1, 3] = ty;
        result.m[2, 3] = tz;
        return result;
    }

    /*
     * CreateScaling() creates a scaling matrix.
     * - Used for non-uniform scaling of objects by (sx, sy, sz).
     * [ sx  0   0   0 ]
     * [ 0   sy  0   0 ]
     * [ 0   0   sz  0 ]
     * [ 0   0   0   1 ]
    */
    public static CustomMatrix4x4 CreateScaling(float sx, float sy, float sz)
    {
        CustomMatrix4x4 result = new CustomMatrix4x4(true); // start with identity
        result.m[0, 0] = sx;
        result.m[1, 1] = sy;
        result.m[2, 2] = sz;
        result.m[3, 3] = 1f; // homogeneous coordinate stays 1
        return result;
    }

    /*
     * CreateRotationZ() creates a rotation matrix around the Z-axis (roll).
     *  [ cosθ  -sinθ  0   0 ]
     *  [ sinθ   cosθ  0   0 ]
     *  [  0      0    1   0 ]
     *  [  0      0    0   1 ]
    */
    public static CustomMatrix4x4 CreateRotationZ(float angleDegrees)
    {
        float rad = angleDegrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);

        CustomMatrix4x4 result = new CustomMatrix4x4(true); // start with identity
        result.m[0, 0] = cos;
        result.m[0, 1] = -sin;
        result.m[1, 0] = sin;
        result.m[1, 1] = cos;
        return result;
    }

    /*
     * CreateRotationY() creates a rotation matrix around the Y-axis (yaw).
     *  [ cosθ   0  sinθ   0 ]
     *  [  0     1   0     0 ]
     *  [−sinθ   0  cosθ   0 ]
     *  [  0     0   0     1 ]
    */
    public static CustomMatrix4x4 CreateRotationY(float angleDegrees)
    {
        float rad = angleDegrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);

        CustomMatrix4x4 result = new CustomMatrix4x4(true); // start with identity
        result.m[0, 0] = cos;
        result.m[0, 2] = sin;
        result.m[2, 0] = -sin;
        result.m[2, 2] = cos;
        return result;
    }

    /*
     * CreateRotationX() creates a rotation matrix around the X-axis (pitch).
     *  [ 1    0       0     0 ]
     *  [ 0  cosθ   -sinθ    0 ]
     *  [ 0  sinθ    cosθ    0 ]
     *  [ 0    0       0     1 ]
    */
    public static CustomMatrix4x4 CreateRotationX(float angleDegrees)
    {
        float rad = angleDegrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);

        CustomMatrix4x4 result = new CustomMatrix4x4(true); // start with identity
        result.m[1, 1] = cos;
        result.m[1, 2] = -sin;
        result.m[2, 1] = sin;
        result.m[2, 2] = cos;
        return result;
    }

    /*
     * CreateRotationXYZ() creates a composite rotation matrix in XYZ order.
     * - Applies X (pitch), then Y (yaw), then Z (roll).
    */
    public static CustomMatrix4x4 CreateRotationXYZ(float pitchDegrees, float yawDegrees, float rollDegrees)
    {
        CustomMatrix4x4 rx = CreateRotationX(pitchDegrees);
        CustomMatrix4x4 ry = CreateRotationY(yawDegrees);
        CustomMatrix4x4 rz = CreateRotationZ(rollDegrees);

        // Note: The order of multiplication matters — this is XYZ (Rz * Ry * Rx)
        return rz * ry * rx;
    }

    /*
     * ToUnityMatrix4x4() converts this CustomMatrix4x4 to UnityEngine.Matrix4x4.
     * - Useful for passing advanced transform effects.
     * - Unity Matrix4x4 is column-major.
    */
    public Matrix4x4 ToUnityMatrix4x4()
    {
        Matrix4x4 unityMatrix = new Matrix4x4();

        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                unityMatrix[row, col] = m[row, col];
            }
        }

        return unityMatrix;
    }

    /*
     * Print() displays the matrix to console.
     * - Useful for debugging.
    */
    public void Print()
    {
        for (int row = 0; row < 4; row++)
        {
            Console.WriteLine($"{m[row, 0]}, {m[row, 1]}, {m[row, 2]}, {m[row, 3]}");
        }
        Console.WriteLine();
    }
}
