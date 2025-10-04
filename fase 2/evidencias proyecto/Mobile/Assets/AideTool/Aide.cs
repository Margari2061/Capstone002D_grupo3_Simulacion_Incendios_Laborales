using AideTool.Geometry;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AideTool
{
    public static class Aide
    {
        private static Dictionary<string, LogWatch> m_watchers;

        #region Log
        public static void Log(params object[] obj)
        {
            string log = FormatLog(obj);
            Debug.Log(log);
        }

        public static void Assert(bool assertion, string ifTrue, string ifFalse)
        {
            if(assertion)
            {
                Log(ifTrue);
                return;
            }
            Log(ifFalse);
        }

        public static void Assert(bool assertion, string ifTrue)
        {
            if(assertion)
                Log(ifTrue);
        }

        public static void LogWarning(object obj)
        {
            object[] args = new object[] { obj };

            if (obj is object[] objs)
                args = objs;

            if (obj is List<string> list)
                args = list.ToArray();

            string log = FormatLog(args);
            Debug.LogWarning(log);
        }

        public static void LogError(object obj)
        {
            object[] args = new object[] { obj };

            if (obj is object[] objs)
                args = objs;

            if (obj is List<string> list)
                args = list.ToArray();

            string log = FormatLog(args);
            Debug.LogError(log);
        }

        public static void LogVerbose<T>(T obj)
        {
            if (LogNull(obj))
                return;

            List<string> log = new() { obj.GetType().ToString() };

            log.AddRange(GetObjectProperties(obj));

            Log(log);
        }

        public static void LogList<T>(IEnumerable<T> list, Func<T, string> predicate = null)
        {
            if (LogNull(list))
                return;

            List<string> log = new() { list.GetType().ToString() };

            int index = 0;

            string handleText(T o)
            {
                if (predicate == null)
                    return $"[{index}]: {o}";
                
                string r = predicate(o);
                return $"[{index}] : {r}";
            }

            foreach (T obj in list)
            {
                log.Add(handleText(obj));
                index++;
            }

            Log(log);
        }

        public static void LogDictionary<TKey, TValue>(Dictionary<TKey, TValue> dictionary, string dictionaryName = "Dictionary Log")
        {
            if (LogNull(dictionary))
                return;

            List<string> log = new() { dictionaryName };

            foreach (KeyValuePair<TKey, TValue> kv in dictionary)
            {
                if (kv.Value != null)
                {
                    log.Add($"[{kv.Key}]: {kv.Value}");
                    continue;
                }
                
                log.Add($"[{kv.Key}]: null");
            }

            Log(log);
        }

        private static string[] GetObjectProperties<T>(T obj)
        {
            List<string> objMsgs = new();

            PropertyInfo[] properties = obj.GetType().GetProperties();

            foreach (PropertyInfo p in properties)
            {
                string value;
                try
                {
                    value = p.GetValue(obj).ToString();
                }
                catch (NullReferenceException)
                {
                    value = "null";
                }
                catch(NotSupportedException)
                {
                    value = "deprecated";
                }
                catch(Exception ex)
                {
                    value = $"{ex.GetType().ToString().ToLower()}";
                }

                string msg = $"\"{p.Name}\": {value}";
                objMsgs.Add(msg);
            }

            return objMsgs.ToArray();
        }

        private static string FormatLog(object[] objs)
        {
            StringBuilder builder = new();
            foreach (object obj in objs)
            {
                if (obj != null)
                {
                    builder.AppendLine(obj.ToString());

                    if(obj is IEnumerable<object> array)
                    {
                        foreach(object obj2 in array)
                            builder.AppendLine(obj2.ToString());
                    }

                    continue;
                }
                builder.AppendLine("null");
            }

            return builder.ToString();
        }

        public static void LogColor(string log, Color color)
        {
            string colorString = color.ColorToHex();
            string[] logs = log.Split("\n");

            StringBuilder builder = new();

            foreach (string line in logs)
                builder.AppendLine($"<color={colorString}>{line}</color>");

            Debug.Log(builder.ToString());
        }

        private static bool LogNull(object obj)
        {
            if(obj == null)
            {
                Log("null");
                return true;
            }
            return false;
        }

        public static void ClearLog(MonoBehaviour sender)
        {
#if UNITY_EDITOR
            Assembly assembly = Assembly.GetAssembly(typeof(Editor));
            Type log = assembly.GetType("UnityEditor.LogEntries");
            MethodInfo method = log.GetMethod("Clear");
            method.Invoke(sender, null);
#endif
        }
        #endregion

        #region Watch
        public static void StartWatch(string operationName, out string watcherId)
        {
#if UNITY_EDITOR
            watcherId = null;
            
            if (m_watchers == null)
                m_watchers = new();

            LogWatch watch = new(operationName);
            m_watchers.Add(watch.WatchId, watch);
            m_watchers[watch.WatchId].Start();
            watcherId = watch.WatchId;
#endif
        }

        public static void StopWatch(string watchId)
        {
#if UNITY_EDITOR
            if(m_watchers.ContainsKey(watchId))
                    m_watchers[watchId].Stop();

                List<string> disposedWatchersList = new();

                foreach(KeyValuePair<string, LogWatch> kv in m_watchers)
                    if(kv.Value.WatchId == null)
                        disposedWatchersList.Add(kv.Key);

                foreach (string id in disposedWatchersList)
                    m_watchers.Remove(id);
#endif
        }
        #endregion

        #region Geometry
        public static void DrawRay(Ray ray, float distance, Color color, bool debugRay = false)
        {
#if UNITY_EDITOR
            Debug.DrawRay(ray.origin, ray.direction * distance, color);
            if (debugRay)
                Log("Debug Ray", $"Origin: {ray.origin}", $"Direction: {ray.direction}", $"Magnitude: {ray.direction.magnitude}", $"Distance: {distance}");
#endif
        }

        public static void DrawRay(Ray ray, float distance, Color color, Vector3 offset, bool debugRay = false)
        {
            Ray newRay = new(ray.origin + offset, ray.direction);
            DrawRay(newRay, distance, color, debugRay);
        }

        public static void DrawRay(Ray ray, float distance)
        {
#if UNITY_EDITOR
            DrawRay(ray, distance, Color.red);
#endif
        }

        public static void DrawLine(Vector3 origin, Vector3 direction, float distance)
        {
#if UNITY_EDITOR
            DrawLine(origin, direction, distance, Color.red);
#endif
        }

        public static void DrawLine(Vector3 origin, Vector3 direction, float distance, Color color)
        {
#if UNITY_EDITOR
            Ray ray = new(origin, direction);
            DrawRay(ray, distance, color);
#endif
        }

        public static void DrawLine(Vector3 origin, Vector3 direction, float distance, Color color, Vector3 offset)
        {
#if UNITY_EDITOR
            Ray ray = new(origin, direction);
            DrawRay(ray, distance, color, offset);
#endif
        }

        public static void DrawBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Color color)
        {
#if UNITY_EDITOR
            Box box = new(origin, halfExtents, orientation);

            DrawBox(box, color);
#endif
        }

        public static void DrawBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation)
        {
#if UNITY_EDITOR
            DrawBox(origin, halfExtents, orientation, Color.red);
#endif
        }

        public static void DrawBox(Box box)
        {
#if UNITY_EDITOR
            DrawBox(box, Color.red);
#endif
        }

        public static void DrawBox(Box box, Color color)
        {
#if UNITY_EDITOR
            Debug.DrawLine(box.FrontTopLeft, box.FrontTopRight, color);
            Debug.DrawLine(box.FrontTopRight, box.FrontBottomRight, color);
            Debug.DrawLine(box.FrontBottomRight, box.FrontBottomLeft, color);
            Debug.DrawLine(box.FrontBottomLeft, box.FrontTopLeft, color);

            Debug.DrawLine(box.BackTopLeft, box.BackTopRight, color);
            Debug.DrawLine(box.BackTopRight, box.BackBottomRight, color);
            Debug.DrawLine(box.BackBottomRight, box.BackBottomLeft, color);
            Debug.DrawLine(box.BackBottomLeft, box.BackTopLeft, color);

            Debug.DrawLine(box.FrontTopLeft, box.BackTopLeft, color);
            Debug.DrawLine(box.FrontTopRight, box.BackTopRight, color);
            Debug.DrawLine(box.FrontBottomRight, box.BackBottomRight, color);
            Debug.DrawLine(box.FrontBottomLeft, box.BackBottomLeft, color);
#endif
        }

        public static void DrawBoxCast(Vector3 origin, Vector3 halfExtents, Vector3 direction, float distance)
        {
#if UNITY_EDITOR

#endif
        }
#endregion

#region Editor
        public static void Pause([CallerFilePath] string filepath = "", [CallerMemberName] string methodName = "", [CallerLineNumber] int lineNumber = 0)
        {
#if UNITY_EDITOR
            StringBuilder builder = new();
            builder.Append("Editor Pause was called from");
            
            int assetsIndex = filepath.LastIndexOf("Assets");
            filepath = filepath[assetsIndex..];
            builder.AppendLine($"...{filepath}");

            builder.AppendLine($"{methodName}()");
            builder.AppendLine($"Line: {lineNumber}");

            Log(builder.ToString());

            UnityEngine.Debug.Break();
#endif
        }

        public static void ExitApplication()
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }
#endregion
    }
}
