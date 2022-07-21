using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;

namespace Pigeon.Math
{
    public static class MathUtil
    {
        /// <summary>
        /// Hash an int into a very different int.
        /// <para>Copied from From Squirrel Eiserloh: <see href="https://www.youtube.com/watch?v=LWFzPP8ZbdU"/></para>
        /// </summary>
        public static int Squirrel3Hash(int state)
        {
            const uint BitNoise1 = 0xB5297A4D;
            const uint BitNoise2 = 0x68E31DA4;
            const uint BitNoise3 = 0x1B56C4E9;

            uint mangled = (uint)state;
            mangled *= BitNoise1;
            mangled ^= mangled >> 8;
            mangled += BitNoise2;
            mangled ^= mangled << 8;
            mangled *= BitNoise3;
            mangled ^= mangled >> 8;

            return (int)(mangled * 2654435769u);
        }

        /// <summary>
        /// Hash an int into a very different int, using a seed.
        /// <para>Copied from From Squirrel Eiserloh: <see href="https://www.youtube.com/watch?v=LWFzPP8ZbdU"/></para>
        /// </summary>
        public static int Squirrel3Hash(int state, int seed)
        {
            const uint BitNoise1 = 0xB5297A4D;
            const uint BitNoise2 = 0x68E31DA4;
            const uint BitNoise3 = 0x1B56C4E9;

            uint mangled = (uint)state;
            mangled *= BitNoise1;
            mangled += (uint)seed;
            mangled ^= mangled >> 8;
            mangled += BitNoise2;
            mangled ^= mangled << 8;
            mangled *= BitNoise3;
            mangled ^= mangled >> 8;

            return (int)(mangled * 2654435769u);
        }

        /// <summary>
        /// Hash a Vector2 into a pseudorandom int.
        /// </summary>
        public static int Hash2D(Vector2Int state, int seed)
        {
            return Squirrel3Hash(state.x + (198491317 * state.y), seed);
        }

        /// <summary>
        /// Hash an int into a very different int.
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
        /// Hash a uint into a very different uint.
        /// </summary>
        /// <param name="uState"></param>
        /// <returns></returns>
        public static uint Hash(uint uState)
        {
            uState ^= 2747636419u;
            uState *= 2654435769u;
            uState ^= uState >> 16;
            uState *= 2654435769u;
            uState ^= uState >> 16;
            return uState * 2654435769u;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RandomIntToBool(int randomNumber)
        {
            return (~randomNumber & 1) == 0;
        }

        /// <summary>
        /// Like Mathf.LerpAngle, but unclamped
        /// </summary>
        public static float LerpAngleUnclamped(float a, float b, float t)
        {
            float delta = Mathf.Repeat((b - a), 360);
            if (delta > 180)
                delta -= 360;
            return a + delta * t;
        }

        #region Fast Square Root
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
        struct FloatIntUnion
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public float f;

            [System.Runtime.InteropServices.FieldOffset(0)]
            public int tmp;
        }

        /// <summary>
        /// Very good approximation if we don't need exact decimals
        /// </summary>
        public static float SqrtFast(float z)
        {
            if (z == 0) return 0;
            FloatIntUnion u;
            u.tmp = 0;
            float xhalf = 0.5f * z;
            u.f = z;
            u.tmp = 0x5f375a86 - (u.tmp >> 1);
            u.f = u.f * (1.5f - xhalf * u.f * u.f);
            return u.f * z;
        }

        /// <summary>
        /// Within ~0.1 for low numbers, gets pretty far off for large numbers
        /// </summary>
        public static float SqrtVeryFast(float z)
        {
            if (z == 0) return 0;
            FloatIntUnion u;
            u.tmp = 0;
            u.f = z;
            u.tmp -= 1 << 23; // Subtract 2^m.
            u.tmp >>= 1; // Divide by 2.
            u.tmp += 1 << 29; // Add ((b + 1) / 2) * 2^m.
            return u.f;
        }
        #endregion

        /// <summary>
        /// Very good approximation if we don't need exact decimals
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MagFast(ref Vector3 v)
        {
            return SqrtFast(v.x * v.x + v.y * v.y + v.z * v.z);
        }

        /// <summary>
        /// Within ~0.1 for low numbers, gets pretty far off for large numbers
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MagVeryFast(ref Vector3 v)
        {
            return SqrtVeryFast(v.x * v.x + v.y * v.y + v.z * v.z);
        }

        /// <summary>
        /// Very good approximation if we don't need exact decimals
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NormalizeFast(ref Vector3 v)
        {
            float mag = SqrtFast(v.x * v.x + v.y * v.y + v.z * v.z);
            if (mag == 0f)
            {
                v.x = 0f;
                v.y = 0f;
                v.z = 0f;
                return;
            }

            v.x /= mag;
            v.y /= mag;
            v.z /= mag;
        }

        /// <summary>
        /// Within ~0.1 for low numbers, gets pretty far off for large numbers
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NormalizeVeryFast(ref Vector3 v)
        {
            float mag = SqrtVeryFast(v.x * v.x + v.y * v.y + v.z * v.z);
            if (mag == 0f)
            {
                v.x = 0f;
                v.y = 0f;
                v.z = 0f;
                return;
            }

            v.x /= mag;
            v.y /= mag;
            v.z /= mag;
        }

        /// <summary>
        /// Very good approximation if we don't need exact decimals
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MagFast(ref Vector2 v)
        {
            return SqrtFast(v.x * v.x + v.y * v.y);
        }

        /// <summary>
        /// Within ~0.1 for low numbers, gets pretty far off for large numbers
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MagVeryFast(ref Vector2 v)
        {
            return SqrtVeryFast(v.x * v.x + v.y * v.y);
        }

        /// <summary>
        /// Very good approximation if we don't need exact decimals
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NormalizeFast(ref Vector2 v)
        {
            float mag = SqrtFast(v.x * v.x + v.y * v.y);
            if (mag == 0f)
            {
                v.x = 0f;
                v.y = 0f;
                return;
            }

            v.x /= mag;
            v.y /= mag;
        }

        /// <summary>
        /// Within ~0.1 for low numbers, gets pretty far off for large numbers
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NormalizeVeryFast(ref Vector2 v)
        {
            float mag = SqrtVeryFast(v.x * v.x + v.y * v.y);
            if (mag == 0f)
            {
                v.x = 0f;
                v.y = 0f;
                return;
            }

            v.x /= mag;
            v.y /= mag;
        }
    }

    public static class Vector3Extension
    {
        /// <summary>
        /// Very good approximation if we don't need exact decimals
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MagFast(this Vector3 v)
        {
            return MathUtil.SqrtFast(v.x * v.x + v.y * v.y + v.z * v.z);
        }

        /// <summary>
        /// Within ~0.1 for low numbers, gets pretty far off for large numbers
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MagVeryFast(this Vector3 v)
        {
            return MathUtil.SqrtVeryFast(v.x * v.x + v.y * v.y + v.z * v.z);
        }

        /// <summary>
        /// Very good approximation if we don't need exact decimals
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 NormalizedFast(this Vector3 v)
        {
            float mag = MathUtil.SqrtFast(v.x * v.x + v.y * v.y + v.z * v.z);
            return mag == 0f ? Vector3.zero : v / mag;
        }

        /// <summary>
        /// Within ~0.1 for low numbers, gets pretty far off for large numbers
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 NormalizedVeryFast(this Vector3 v)
        {
            float mag = MathUtil.SqrtVeryFast(v.x * v.x + v.y * v.y + v.z * v.z);
            return mag == 0f ? Vector3.zero : v / mag;
        }

        /// <summary>
        /// Very good approximation if we don't need exact decimals
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MagFast(this Vector2 v)
        {
            return MathUtil.SqrtFast(v.x * v.x + v.y * v.y);
        }

        /// <summary>
        /// Within ~0.1 for low numbers, gets pretty far off for large numbers
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MagVeryFast(this Vector2 v)
        {
            return MathUtil.SqrtVeryFast(v.x * v.x + v.y * v.y);
        }

        /// <summary>
        /// Very good approximation if we don't need exact decimals
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 NormalizedFast(this Vector2 v)
        {
            float mag = MathUtil.SqrtFast(v.x * v.x + v.y * v.y);
            return mag == 0f ? Vector2.zero : v / mag;
        }

        /// <summary>
        /// Within ~0.1 for low numbers, gets pretty far off for large numbers
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 NormalizedVeryFast(this Vector2 v)
        {
            float mag = MathUtil.SqrtVeryFast(v.x * v.x + v.y * v.y);
            return mag == 0f ? Vector2.zero : v / mag;
        }
    }
}