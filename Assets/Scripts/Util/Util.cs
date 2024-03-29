using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using QT.Core.Map;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace QT.Util
{
    public static class AddressablesDataPath
    {
        public static readonly string[] DoorPaths = {
            "Doors/Normal/NormalUp.prefab",
            "Doors/Normal/NormalDown.prefab",
            "Doors/Normal/NormalLeft.prefab",
            "Doors/Normal/NormalRight.prefab"};
        
        public static readonly string[] StoreDoorPaths = {
            "Doors/Store/StoreUp.prefab",
            "Doors/Store/StoreDown.prefab",
            "Doors/Store/StoreLeft.prefab",
            "Doors/Store/StoreRight.prefab"};        
        
        public static readonly string[] BossDoorPaths = {
            "Doors/Boss/BossUp.prefab",
            "Doors/Boss/BossDown.prefab",
            "Doors/Boss/BossUp.prefab",
            "Doors/Boss/BossUp.prefab"};
        
        
        public static readonly string[] StageMoveDoorPaths = {
            "Doors/StageMove/StageUp.prefab",
            "Doors/StageMove/StageDown.prefab",
            "Doors/StageMove/StageUp.prefab",
            "Doors/StageMove/StageUp.prefab"};
        
        public static string[] GetDoorPath(RoomType doorType)
        {
            switch (doorType)
            {
                case RoomType.Normal:
                    return DoorPaths;
                case RoomType.HpShop:
                case RoomType.GoldShop:
                    return StoreDoorPaths;
                
                case RoomType.Stairs:
                    return StageMoveDoorPaths;
                case RoomType.Boss:
                    return BossDoorPaths;
                
                default:
                    return DoorPaths;
            }
        }
    }
    
    public static class UnityUtil
    {
        public static readonly Vector2Int[] PathDirections = new Vector2Int[4] {Vector2Int.up, Vector2Int.down, Vector2Int.right, Vector2Int.left};

        public static IEnumerator WaitForFunc(Action func, float delay)
        {
            yield return new WaitForSeconds(delay);
            func.Invoke();
        }
        
        public static IEnumerator WaitForRealTimeFunc(Action func, float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            func.Invoke();
        }

        public static void ProgramExit()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        
        public static IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration,Action func = null)
        {
            float startTime = Time.time;
            while (Time.time - startTime < duration)
            {
                cg.alpha = Mathf.Lerp(start, end, (Time.time - startTime) / duration);
                yield return null;
            }
            cg.alpha = end;
            func?.Invoke();
        }

        public static void ResetLocalTransform(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }
        
        public static TweenerCore<string, string, StringOptions> DOTMP(this TextMeshProUGUI target, string endValue, float duration, bool richTextEnabled = true, ScrambleMode scrambleMode = ScrambleMode.None, string scrambleChars = null)
        {
            if (endValue == null) {
                if (Debugger.logPriority > 0) Debugger.LogWarning("You can't pass a NULL string to DOText: an empty string will be used instead to avoid errors");
                endValue = "";
            }
            TweenerCore<string, string, StringOptions> t = DOTween.To(() => target.text, x => target.text = x, endValue, duration);
            t.SetOptions(richTextEnabled, scrambleMode, scrambleChars)
                .SetTarget(target);
            return t;
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
        
        public static Vector2 Rotate90Degree(Vector2 dir, bool isClockwise = true)
        {
            return  isClockwise ? new Vector2(-dir.y, dir.x) : new Vector2(dir.y, -dir.x);
        }
        
        public static float Vector2Cross(Vector2 value1, Vector2 value2)
        {
            return value1.x * value2.y - value1.y * value2.x;
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
        
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            int count = n;
            
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, count);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
        
    }
    
     public static class ScreenMath
    {
        public static Vector3 GetScreenPosition(Camera mainCamera, Vector3 targetPosition)
        {
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(targetPosition);
            return screenPosition;
        }
        
        public static bool IsTargetVisible(Vector3 screenPosition)
        {
            bool isTargetVisible = screenPosition.x > 0 && screenPosition.x < Screen.width && screenPosition.y > 0 && screenPosition.y < Screen.height;
            return isTargetVisible;
        }
        
        public static void GetIndicatorPositionAndAngle(ref Vector3 screenPosition, ref float angle, Vector3 screenCentre, Vector3 screenBounds)
        {
            screenPosition -= screenCentre;

            angle = Mathf.Atan2(screenPosition.y, screenPosition.x);
            float slope = Mathf.Tan(angle);
            if(screenPosition.x > 0)
            {
                screenPosition = new Vector3(screenBounds.x, screenBounds.x * slope, 0);
            }
            else
            {
                screenPosition = new Vector3(-screenBounds.x, -screenBounds.x * slope, 0);
            }
            if(screenPosition.y > screenBounds.y)
            {
                screenPosition = new Vector3(screenBounds.y / slope, screenBounds.y, 0);
            }
            else if(screenPosition.y < -screenBounds.y)
            {
                screenPosition = new Vector3(-screenBounds.y / slope, -screenBounds.y, 0);
            }
            screenPosition += screenCentre;
        }
    }

}