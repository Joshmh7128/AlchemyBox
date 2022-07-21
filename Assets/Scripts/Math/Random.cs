using UnityEngine;
using System.Runtime.CompilerServices;

namespace Pigeon.Math
{
    /// <summary>
    /// Fast random.
    /// <para>Use <see cref="Create"/> to create a new instance with a timestamp seed, or the int constructor to pass in a seed.</para>
    /// <para>~13% faster than System.Random and 39% faster than UnityEngine.Random. Period is 2686030364, about 38% smaller than UnityEngine.Random's period.</para>
    /// </summary>
    public struct Random
    {
        /// <summary>
        /// Hash a single int
        /// </summary>
        public static int Hash(int state)
        {
            uint uState = unchecked((uint)state);

            uState ^= 2747636419u;
            uState *= 2654435769u;
            uState ^= uState >> 16;
            uState *= 2654435769u;
            uState ^= uState >> 16;
            return unchecked((int)(uState * 2654435769u));
        }

        /// <summary>
        /// The current state
        /// </summary>
        public int state;

        /// <summary>
        /// Random int (int.MinValue to int.MaxValue)
        /// </summary>
        public int Next()
        {
            uint state = unchecked((uint)this.state);

            state ^= 2747636419u;
            state *= 2654435769u;
            state ^= state >> 16;
            state *= 2654435769u;
            state ^= state >> 16;
            state *= 2654435769u;
            this.state = unchecked((int)state);
            return this.state;
        }

        /// <summary>
        /// Random positive int
        /// </summary>
        public int NextPositive()
        {
            uint state = unchecked((uint)this.state);

            state ^= 2747636419u;
            state *= 2654435769u;
            state ^= state >> 16;
            state *= 2654435769u;
            state ^= state >> 16;
            state *= 2654435769u;
            this.state = unchecked((int)state);
            return this.state > -1 ? this.state : -this.state;
        }

        /// <summary>
        /// Create a new Random instance with its seed set to the current time
        /// </summary>
        public static Random Create()
        {
            return new Random()
            {
                state = (int)System.DateTime.Now.Ticks
            };
        }

        /// <summary>
        /// Create a new Random instance
        /// </summary>
        public Random(int seed)
        {
            state = seed;
        }

        /// <summary>
        /// Create a new Random instance
        /// </summary>
        public Random(Vector2Int seed)
        {
            state = MathUtil.Squirrel3Hash(seed.x + (198491317 * seed.y));
        }

        /// <summary>
        /// Random float between 0 and 1
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float NextFloat()
        {
            return (Next() / 2147483647f + 1f) * 0.5f;
        }

        /// <summary>
        /// Random float between min [inclusive] and max [inclusive]
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float NextFloat(float min, float max)
        {
            return NextFloat() * (max - min) + min;
        }

        /// <summary>
        /// Random float between 0 [inclusive] and max [inclusive]
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float NextFloat(float max)
        {
            return NextFloat() * max;
        }

        /// <summary>
        /// Random byte
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte NextByte()
        {
            return (byte)Next(0, 256);
        }

        /// <summary>
        /// Random int between min [inclusive] and max [exclusive]
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Next(int min, int max)
        {
            return (int)(NextFloat() * (max - min) + min);
        }

        /// <summary>
        /// Random int between 0 [inclusive] and max [exclusive]
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Next(int max)
        {
            return (int)(NextFloat() * max);
        }

        /// <summary>
        /// Random color
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Color NextColor()
        {
            return new Color(NextFloat(), NextFloat(), NextFloat());
        }

        /// <summary>
        /// Random HSV color (better random distribution than a random RGB color)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Color NextColorHSV()
        {
            return Color.HSVToRGB(NextFloat(), NextFloat(), NextFloat());
        }

        /// <summary>
        /// Random color
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Color32 NextColor32()
        {
            return new Color32(NextByte(), NextByte(), NextByte(), 255);
        }

        /// <summary>
        /// Random bool
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool NextBool()
        {
            return (~Next() & 1) == 0;
        }

        /// <summary>
        /// Random quaternion
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Quaternion NextQuaternion()
        {
            float u = NextFloat();
            float v = NextFloat();
            float w = NextFloat();

            float oneMinusUSqrt = Mathf.Sqrt(1f - u);
            float uSqrt = Mathf.Sqrt(u);

            const float PI2 = Mathf.PI * 2f;

            return new Quaternion(oneMinusUSqrt * Mathf.Sin(PI2 * v), oneMinusUSqrt * Mathf.Cos(PI2 * v), uSqrt * Mathf.Sin(PI2 * w), uSqrt * Mathf.Cos(PI2 * w));
        }

        /// <summary>
        /// Random quaternion using approximate sqrts
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Quaternion NextQuaternionFast()
        {
            float u = NextFloat();
            float v = NextFloat();
            float w = NextFloat();

            float oneMinusUSqrt = MathUtil.SqrtVeryFast(1f - u);
            float uSqrt = MathUtil.SqrtVeryFast(u);

            const float PI2 = Mathf.PI * 2f;

            return new Quaternion(oneMinusUSqrt * Mathf.Sin(PI2 * v), oneMinusUSqrt * Mathf.Cos(PI2 * v), uSqrt * Mathf.Sin(PI2 * w), uSqrt * Mathf.Cos(PI2 * w));
        }

        /// <summary>
        /// Random point on unit sphere
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 OnUnitSphere()
        {
            var theta = NextFloat() * 2 * Mathf.PI;
            var phi = Mathf.Acos((2 * NextFloat()) - 1);

            // Convert from spherical coordinates to Cartesian
            var sinPhi = Mathf.Sin(phi);

            var x = sinPhi * Mathf.Cos(theta);
            var y = sinPhi * Mathf.Sin(theta);
            var z = Mathf.Cos(phi);

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Random point inside unit sphere
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 InsideUnitSphere()
        {
            float d, x, y, z;
            do
            {
                x = NextFloat(-1f, 1f);
                y = NextFloat(-1f, 1f);
                z = NextFloat(-1f, 1f);
                d = x * x + y * y + z * z;
            } while (d > 1.0);

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Random point on unit circle
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 OnUnitCircle()
        {
            float theta = Mathf.PI * 2f * NextFloat();

            return new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));
        }

        /// <summary>
        /// Random point inside unit circle
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 InsideUnitCircle()
        {
            float r = Mathf.Sqrt(NextFloat());
            float theta = Mathf.PI * 2f * NextFloat();

            return new Vector2(r * Mathf.Cos(theta), r * Mathf.Sin(theta));
        }

        /// <summary>
        /// Random point inside unit circle using approximate sqrt
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 InsideUnitCircleFast()
        {
            float r = MathUtil.SqrtVeryFast(NextFloat());
            float theta = Mathf.PI * 2f * NextFloat();

            return new Vector2(r * Mathf.Cos(theta), r * Mathf.Sin(theta));
        }

        /// <summary>
        /// Random point inside triangle
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 InsideTriangle(Vector3 vertexA, Vector3 vertexB, Vector3 vertexC)
        {
            float r1Sqrt = Mathf.Sqrt(NextFloat());
            float r2 = NextFloat();
            return (1f - r1Sqrt) * vertexA + (r1Sqrt * (1f - r2)) * vertexB + (r2 * r1Sqrt) * vertexC;
        }

        /// <summary>
        /// Random point inside triangle using approximate sqrt
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 InsideTriangleFast(Vector3 vertexA, Vector3 vertexB, Vector3 vertexC)
        {
            float r1Sqrt = MathUtil.SqrtVeryFast(NextFloat());
            float r2 = NextFloat();
            return (1f - r1Sqrt) * vertexA + (r1Sqrt * (1f - r2)) * vertexB + (r2 * r1Sqrt) * vertexC;
        }
    }
}