using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QT.Util
{
    public static class UnityUtil
    {
        public static readonly Vector2Int[] PathDirections = new Vector2Int[4] {Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right};

        public static IEnumerator WaitForFunc(Action func, float delay)
        {
            yield return new WaitForSeconds(delay);
            func.Invoke();
        }
    }

    public static class Math
    {
        //0~1f 값 비율 반환
        public static float Remap(float value, float max, float min)
        {
            return Unity.Mathematics.math.remap(min, max, 0f, 1f, value);
        }

        /// <summary>
        /// 목표 위치를 바라보는 각도 값 구하기
        /// </summary>
        /// <param name="originalPos">시작 좌표</param>
        /// <param name="targetPos">목표 좌표</param>
        /// <returns></returns>
        public static float GetDegree(Vector2 originalPos, Vector2 targetPos)
        {
            float angleRadian = Mathf.Atan2(targetPos.y - originalPos.y, targetPos.x - originalPos.x);
            return 180 / Mathf.PI * angleRadian;
        }

        public static Vector2 ZAngleToGetDirection(Transform transform)
        {
            float angleInRadians = transform.eulerAngles.z * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
        }
    }

    public static class RandomSeed
    {
        private const string stringSeeds =
            "abcdefghijklmnopqrstuvwxyz0123456789";

        public static void SeedSetting()
        {
            int seed = (int) DateTime.Now.Ticks & 0x0000FFFF;
            UnityEngine.Random.InitState(seed);
            string stringSeed = GenerateStringSeed(8);
            byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes(stringSeed);
            int utfSeed = BitConverter.ToInt32(utf8Bytes);
            UnityEngine.Random.InitState(utfSeed);
        }

        public static string GenerateStringSeed(int length)
        {
            var sb = new System.Text.StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                int pos = UnityEngine.Random.Range(0,stringSeeds.Length);
                char c = stringSeeds[pos];
                sb.Append(c);
            }

            return sb.ToString();
        }
        
        public static List<T> GetRandomIndexes<T>(List<T> list, int maxIndex)
        {
            List<T> randomIndexes = new List<T>();
            List<int> usedIndexes = new List<int>();
            int currentIndex = 0;
    
            if (maxIndex > list.Count)
            {
                maxIndex = list.Count;
            }

            for (int i = 0; i < maxIndex; i++)
            {
                int randomIndex;

                if (usedIndexes.Count == list.Count)
                {
                    randomIndex = UnityEngine.Random.Range(0, list.Count);
                }
                else
                {
                    do
                    {
                        randomIndex = UnityEngine.Random.Range(0, list.Count);
                    } while (usedIndexes.Contains(randomIndex));

                    usedIndexes.Add(randomIndex);
                    currentIndex++;
                }

                randomIndexes.Add(list[randomIndex]);
            }

            if (maxIndex > list.Count)
            {
                for (int i = list.Count; i < maxIndex; i++)
                {
                    int randomIndex = UnityEngine.Random.Range(0, list.Count);
                    randomIndexes.Add(list[randomIndex]);
                }
            }
            
            return randomIndexes;
        }
        
    }

}