using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.IO;
using System.Linq;
using Object = UnityEngine.Object;

namespace DebugExtension {
    public class DebugEx {
        /// <summary>
        ///   <para>Reports whether the development console is visible. The development console cannot be made to appear using:</para>
        /// </summary>
        public static bool developerConsoleVisible {
            get => Debug.developerConsoleVisible;
            set => Debug.developerConsoleVisible = value;
        }

        /// <summary>
        ///   <para>In the Build Settings dialog there is a check box called "Development Build".</para>
        /// </summary>
        public static bool isDebugBuild {
            get => Debug.isDebugBuild;
        }

        /// <summary>
        ///   <para>Get default debug logger.</para>
        /// </summary>
        public static ILogger unityLogger {
            get => Debug.unityLogger;
        }


        private static object ParseIList<T>(IList<T> message, bool newline) {
            string line = string.Empty;
            line += "Type " + ParseDefineTrace(typeof(T)) + "\n";

            for (var i = 0; i < message.Count; i++) {
                object obj = message[i];
                Type objType = obj.GetType();

                if (IsStruct(objType)) {
                    line += $"[{i}]\n";
                    foreach (FieldInfo fieldInfo in objType.GetFields()) {
                        line += " " + fieldInfo.Name + " : " + fieldInfo.GetValue(obj) + (newline ? "\n" : "");
                    }
                }
                else {
                    line += $"[{i}] ";
                    line += obj + ", " + (newline ? "\n" : "");
                }
            }

            line = line.Substring(0, line.Length - 3);
            line += "\n";

            return (object) line;
        }


        private static object ParseIDict<TKey, TValue>(IDictionary<TKey, TValue> message, bool newline) {
            string line = string.Empty;
            Debug.Log(debugExProfile.dictKeyColor);
            Debug.Log(debugExProfile.dictValueColor);
            line += $"Type(<color=#{debugExProfile.dictKeyColor}>Key</color>) " + ParseDefineTrace(typeof(TKey));
            line += $"Type(<color=#{debugExProfile.dictValueColor}>Value</color>) " + ParseDefineTrace(typeof(TValue));
            line += "\n";

            foreach (KeyValuePair<TKey, TValue> keyValue in message) {
                string richerKey = $"<color=#{debugExProfile.dictKeyColor}>Key: {keyValue.Key}</color>";
                string richerValue = $"<color=#{debugExProfile.dictValueColor}>Value: {keyValue.Value}</color>";
                line += $"{richerKey} | {richerValue}, " + (newline ? "\n" : "");
            }

            line = line.Substring(0, line.Length - 2);
            line += "\n";

            return (object) line;
        }


        private static object ParseISet<T>(ISet<T> message, bool newline) {
            return (object) ParseIList(message.ToList(), newline);
        }


        private static string ParseDefineTrace(Type type) {
            IEnumerable<string> typeList = type.ToString().Split('+');

            string specialType = string.Empty;
            if (IsStruct(type))
                specialType = "Struct";
            else if (type.IsEnum)
                specialType = "Enum";

            return $"{specialType} at ({typeList.Aggregate((t1, t2) => t1 + " > " + t2)})\n";
        }

        private static string ParseColor(Color message) =>
            $"#{ColorUtility.ToHtmlStringRGBA(message).Color(message)}  Color({message.r}, {message.g}, {message.b}, {message.a})";


        private static bool IsStruct(Type type) {
            return type.IsValueType && !type.IsEnum && !type.IsPrimitive;
        }

        // For nested list
        // public static string ParseList<T>(IList<T> message, bool newline) {
        //     string line = string.Empty;
        //
        //     for (int i = 0; i < message.Count; i++) {
        //         var obj = message[i];
        //
        //         if (obj is IList<T> _list) {
        //             line += $"List[{i}]" + (newline ? "\n" : "");
        //             line += ParseList(_list, newline);
        //         } else {
        //             line += "- " + obj.ToString() + (newline ? "\n" : "");
        //         }
        //     }
        //
        //     return line;
        // }


        /// <summary>
        ///   <para>Logs a message to the Unity Console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        public static void Log<T>(IList<T> message, bool newline = true) {
            Debug.unityLogger.Log(LogType.Log, ParseIList(message, newline));
        }


        /// <summary>
        ///   <para>Logs a message to the Unity Console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        public static void Log<TKey, TValue>(IDictionary<TKey, TValue> message, bool newline = true) {
            Debug.unityLogger.Log(LogType.Log, ParseIDict(message, newline));
        }


        /// <summary>
        ///   <para>Logs a message to the Unity Console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        public static void Log<T>(ISet<T> message, bool newline = true) {
            Debug.unityLogger.Log(LogType.Log, ParseISet(message, newline));
        }

        /// <summary>
        ///   <para>Logs a message to the Unity Console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        public static void Log(Color message) {
            Debug.unityLogger.Log(LogType.Log, ParseColor(message));
        }


        /// <summary>
        ///   <para>Logs a message to the Unity Console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        public static void Log(object message) {
            Debug.unityLogger.Log(LogType.Log, message);
        }


        /// <summary>
        ///   <para>Logs a message to the Unity Console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        public static void Log<T>(IList<T> message, Object context, bool newline = true) {
            Debug.unityLogger.Log(LogType.Log, ParseIList(message, newline), context);
        }


        /// <summary>
        ///   <para>Logs a message to the Unity Console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        public static void Log<TKey, TValue>(IDictionary<TKey, TValue> message, Object context, bool newline = true) {
            Debug.unityLogger.Log(LogType.Log, ParseIDict(message, newline), context);
        }


        /// <summary>
        ///   <para>Logs a message to the Unity Console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        public static void Log<T>(ISet<T> message, Object context, bool newline = true) {
            Debug.unityLogger.Log(LogType.Log, ParseISet(message, newline), context);
        }

        /// <summary>
        ///   <para>Logs a message to the Unity Console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        public static void Log(Color message, Object context) {
            Debug.unityLogger.Log(LogType.Log, ParseColor(message));
        }


        /// <summary>
        ///   <para>Logs a message to the Unity Console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        public static void Log(object message, Object context) {
            Debug.unityLogger.Log(LogType.Log, message, context);
        }


        /// <summary>
        ///   <para>Logs a formatted message to the Unity Console.</para>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        public static void LogFormat(string format, params object[] args) {
            Debug.unityLogger.LogFormat(LogType.Log, format, args);
        }


        /// <summary>
        ///   <para>Logs a formatted message to the Unity Console.</para>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        /// <param name="context">Object to which the message applies.</param>
        public static void LogFormat(Object context, string format, params object[] args) {
            Debug.unityLogger.LogFormat(LogType.Log, context, format, args);
        }


        /// <summary>
        ///   <para>Logs a formatted message to the Unity Console.</para>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="logType">Type of message e.g. warn or error etc.</param>
        /// <param name="logOptions">Option flags to treat the log message special.</param>
        public static void LogFormat(LogType logType,
            LogOption logOptions,
            Object context,
            string format,
            params object[] args) {
            Debug.LogFormat(logType, logOptions, context, format, args);
        }


        /// <summary>
        ///   <para>A variant of Debug.Log that logs an assertion message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        public static void LogAssertion<T>(IList<T> message, bool newline = true) {
            Debug.unityLogger.Log(LogType.Log, ParseIList(message, newline));
        }


        /// <summary>
        ///   <para>A variant of Debug.Log that logs an assertion message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        public static void LogAssertion<TKey, TValue>(IDictionary<TKey, TValue> message, bool newline = true) {
            Debug.unityLogger.Log(LogType.Log, ParseIDict(message, newline));
        }


        /// <summary>
        ///   <para>A variant of Debug.Log that logs an assertion message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        public static void LogAssertion<T>(ISet<T> message, bool newline = true) {
            Debug.unityLogger.Log(LogType.Log, ParseISet(message, newline));
        }

        /// <summary>
        ///   <para>Logs a message to the Unity Console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        public static void LogAssertion(Color message) {
            Debug.unityLogger.Log(LogType.Assert, ParseColor(message));
        }


        /// <summary>
        ///   <para>A variant of Debug.Log that logs an assertion message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        public static void LogAssertion(object message) {
            Debug.unityLogger.Log(LogType.Assert, message);
        }


        /// <summary>
        ///   <para>A variant of Debug.Log that logs an assertion message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        public static void LogAssertion<T>(IList<T> message, Object context, bool newline = true) {
            Debug.unityLogger.Log(LogType.Log, ParseIList(message, newline), context);
        }


        /// <summary>
        ///   <para>A variant of Debug.Log that logs an assertion message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        public static void LogAssertion<TKey, TValue>(IDictionary<TKey, TValue> message,
            Object context,
            bool newline = true) {
            Debug.unityLogger.Log(LogType.Log, ParseIDict(message, newline), context);
        }


        /// <summary>
        ///   <para>A variant of Debug.Log that logs an assertion message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        public static void LogAssertion<T>(ISet<T> message, Object context, bool newline = true) {
            Debug.unityLogger.Log(LogType.Log, ParseISet(message, newline), context);
        }

        /// <summary>
        ///   <para>A variant of Debug.Log that logs an assertion message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        public static void LogAssertion(Color message, Object context) {
            Debug.unityLogger.Log(LogType.Assert, (object) ParseColor(message), context);
        }

        /// <summary>
        ///   <para>A variant of Debug.Log that logs an assertion message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        public static void LogAssertion(object message, Object context) {
            Debug.unityLogger.Log(LogType.Assert, message, context);
        }


        /// <summary>
        ///   <para>Logs a formatted assertion message to the Unity console.</para>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        public static void LogAssertionFormat(string format, params object[] args) {
            Debug.unityLogger.LogFormat(LogType.Assert, format, args);
        }


        /// <summary>
        ///   <para>Logs a formatted assertion message to the Unity console.</para>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        /// <param name="context">Object to which the message applies.</param>
        public static void LogAssertionFormat(Object context, string format, params object[] args) {
            Debug.unityLogger.LogFormat(LogType.Assert, context, format, args);
        }


        /// <summary>
        ///   <para>A variant of Debug.Log that logs a warning message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        public static void LogWarning<T>(IList<T> message, bool newline = true) {
            Debug.unityLogger.Log(LogType.Warning, ParseIList(message, newline));
        }


        /// <summary>
        ///   <para>A variant of Debug.Log that logs a warning message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        public static void LogWarning<TKey, TValue>(IDictionary<TKey, TValue> message, bool newline = true) {
            Debug.unityLogger.Log(LogType.Warning, ParseIDict(message, newline));
        }


        /// <summary>
        ///   <para>A variant of Debug.Log that logs a warning message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        public static void LogWarning<T>(ISet<T> message, bool newline = true) {
            Debug.unityLogger.Log(LogType.Warning, ParseISet(message, newline));
        }

        /// <summary>
        ///   <para>A variant of Debug.Log that logs a warning message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        public static void LogWarning(Color message) {
            Debug.unityLogger.Log(LogType.Warning, (object) ParseColor(message));
        }


        /// <summary>
        ///   <para>A variant of Debug.Log that logs a warning message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        public static void LogWarning(object message) {
            Debug.unityLogger.Log(LogType.Warning, message);
        }


        /// <summary>
        ///   <para>A variant of Debug.Log that logs a warning message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        public static void LogWarning<T>(IList<T> message, Object context, bool newline = true) {
            Debug.unityLogger.Log(LogType.Warning, ParseIList(message, newline), context);
        }


        /// <summary>
        ///   <para>A variant of Debug.Log that logs a warning message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        public static void LogWarning<TKey, TValue>(IDictionary<TKey, TValue> message,
            Object context,
            bool newline = true) {
            Debug.unityLogger.Log(LogType.Warning, ParseIDict(message, newline), context);
        }


        /// <summary>
        ///   <para>A variant of Debug.Log that logs a warning message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        public static void LogWarning<T>(ISet<T> message, Object context, bool newline = true) {
            Debug.unityLogger.Log(LogType.Warning, ParseISet(message, newline), context);
        }

        /// <summary>
        ///   <para>A variant of Debug.Log that logs a warning message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        public static void LogWarning(Color message, Object context) {
            Debug.unityLogger.Log(LogType.Warning, (object) ParseColor(message), context);
        }

        /// <summary>
        ///   <para>A variant of Debug.Log that logs a warning message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        public static void LogWarning(object message, Object context) {
            Debug.unityLogger.Log(LogType.Warning, message, context);
        }


        /// <summary>
        ///   <para>Logs a formatted warning message to the Unity Console.</para>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        public static void LogWarningFormat(string format, params object[] args) {
            Debug.unityLogger.LogFormat(LogType.Warning, format, args);
        }


        /// <summary>
        ///   <para>Logs a formatted warning message to the Unity Console.</para>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        /// <param name="context">Object to which the message applies.</param>
        public static void LogWarningFormat(Object context, string format, params object[] args) {
            Debug.unityLogger.LogFormat(LogType.Warning, context, format, args);
        }


        /// <summary>
        ///   <para>A variant of Debug.Log that logs an error message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        public static void LogError(object message) {
            Debug.unityLogger.Log(LogType.Error, message);
        }


        /// <summary>
        ///   <para>A variant of Debug.Log that logs an error message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        public static void LogError<T>(IList<T> message, bool newline = true) {
            Debug.unityLogger.Log(LogType.Error, ParseIList(message, newline));
        }


        /// <summary>
        ///   <para>A variant of Debug.Log that logs an error message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        public static void LogError<TKey, TValue>(IDictionary<TKey, TValue> message, bool newline = true) {
            Debug.unityLogger.Log(LogType.Error, ParseIDict(message, newline));
        }

        /// <summary>
        ///   <para>A variant of Debug.Log that logs an error message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        public static void LogError(Color message) {
            Debug.unityLogger.Log(LogType.Error, (object) ParseColor(message));
        }


        /// <summary>
        ///   <para>A variant of Debug.Log that logs an error message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        public static void LogError<T>(ISet<T> message, bool newline = true) {
            Debug.unityLogger.Log(LogType.Error, ParseISet(message, newline));
        }


        /// <summary>
        ///   <para>A variant of Debug.Log that logs an error message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        public static void LogError<T>(IList<T> message, Object context, bool newline = true) {
            Debug.unityLogger.Log(LogType.Error, ParseIList(message, newline), context);
        }


        /// <summary>
        ///   <para>A variant of Debug.Log that logs an error message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        public static void LogError<TKey, TValue>(IDictionary<TKey, TValue> message,
            Object context,
            bool newline = true) {
            Debug.unityLogger.Log(LogType.Error, ParseIDict(message, newline), context);
        }


        /// <summary>
        ///   <para>A variant of Debug.Log that logs an error message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        public static void LogError<T>(ISet<T> message, Object context, bool newline = true) {
            Debug.unityLogger.Log(LogType.Error, ParseISet(message, newline), context);
        }

        /// <summary>
        ///   <para>A variant of Debug.Log that logs an error message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        public static void LogError(Color message, Object context) {
            Debug.unityLogger.Log(LogType.Error, (object) ParseColor(message), context);
        }


        /// <summary>
        ///   <para>A variant of Debug.Log that logs an error message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        public static void LogError(object message, Object context) {
            Debug.unityLogger.Log(LogType.Error, message, context);
        }


        /// <summary>
        ///   <para>Logs a formatted error message to the Unity console.</para>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        public static void LogErrorFormat(string format, params object[] args) {
            Debug.unityLogger.LogFormat(LogType.Error, format, args);
        }


        /// <summary>
        ///   <para>Logs a formatted error message to the Unity console.</para>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        /// <param name="context">Object to which the message applies.</param>
        public static void LogErrorFormat(Object context, string format, params object[] args) {
            Debug.unityLogger.LogFormat(LogType.Error, context, format, args);
        }


        /// <summary>
        ///   <para>A variant of Debug.Log that logs an error message to the console.</para>
        /// </summary>
        /// <param name="exception">Runtime Exception.</param>
        public static void LogException(Exception exception) {
            Debug.unityLogger.LogException(exception, (Object) null);
        }


        /// <summary>
        ///   <para>A variant of Debug.Log that logs an error message to the console.</para>
        /// </summary>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="exception">Runtime Exception.</param>
        public static void LogException(Exception exception, Object context) {
            Debug.unityLogger.LogException(exception, context);
        }


        /// <summary>
        ///   <para>Draws a line between specified start and end points.</para>
        /// </summary>
        /// <param name="start">Point in world space where the line should start.</param>
        /// <param name="end">Point in world space where the line should end.</param>
        /// <param name="color">Color of the line.</param>
        /// <param name="duration">How long the line should be visible for.</param>
        public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration) {
            Debug.DrawLine(start, end, color, duration, true);
        }


        /// <summary>
        ///   <para>Draws a line between specified start and end points.</para>
        /// </summary>
        /// <param name="start">Point in world space where the line should start.</param>
        /// <param name="end">Point in world space where the line should end.</param>
        /// <param name="color">Color of the line.</param>
        public static void DrawLine(Vector3 start, Vector3 end, Color color) {
            Debug.DrawLine(start, end, color, 0.0f, true);
        }


        /// <summary>
        ///   <para>Draws a line between specified start and end points.</para>
        /// </summary>
        /// <param name="start">Point in world space where the line should start.</param>
        /// <param name="end">Point in world space where the line should end.</param>
        public static void DrawLine(Vector3 start, Vector3 end) {
            Debug.DrawLine(start, end, Color.white, 0.0f, true);
        }


        /// <summary>
        ///   <para>Draws a line from start to start + dir in world coordinates.</para>
        /// </summary>
        /// <param name="start">Point in world space where the ray should start.</param>
        /// <param name="dir">Direction and length of the ray.</param>
        /// <param name="color">Color of the drawn line.</param>
        /// <param name="duration">How long the line will be visible for (in seconds).</param>
        public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration) {
            Debug.DrawRay(start, dir, color, duration, true);
        }


        /// <summary>
        ///   <para>Draws a line from start to start + dir in world coordinates.</para>
        /// </summary>
        /// <param name="start">Point in world space where the ray should start.</param>
        /// <param name="dir">Direction and length of the ray.</param>
        /// <param name="color">Color of the drawn line.</param>
        public static void DrawRay(Vector3 start, Vector3 dir, Color color) {
            Debug.DrawRay(start, dir, color, 0.0f, true);
        }


        /// <summary>
        ///   <para>Draws a line from start to start + dir in world coordinates.</para>
        /// </summary>
        /// <param name="start">Point in world space where the ray should start.</param>
        /// <param name="dir">Direction and length of the ray.</param>
        public static void DrawRay(Vector3 start, Vector3 dir) {
            Debug.DrawRay(start, dir, Color.white, 0.0f, true);
        }


        /// <summary>
        ///   <para>Assert a condition and logs an error message to the Unity console on failure.</para>
        /// </summary>
        /// <param name="condition">Condition you expect to be true.</param>
        public static void Assert(bool condition) {
            if (condition) return;

            Debug.unityLogger.Log(LogType.Assert, (object) "Assertion failed");
        }


        /// <summary>
        ///   <para>Assert a condition and logs an error message to the Unity console on failure.</para>
        /// </summary>
        /// <param name="condition">Condition you expect to be true.</param>
        public static void Assert(bool condition, Object context) {
            if (condition) return;

            Debug.unityLogger.Log(LogType.Assert, (object) "Assertion failed", context);
        }


        /// <summary>
        ///   <para>Assert a condition and logs an error message to the Unity console on failure.</para>
        /// </summary>
        /// <param name="condition">Condition you expect to be true.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        public static void Assert(bool condition, object message) {
            if (condition) return;

            Debug.unityLogger.Log(LogType.Assert, message);
        }


        /// <summary>
        ///   <para>Assert a condition and logs an error message to the Unity console on failure.</para>
        /// </summary>
        /// <param name="condition">Condition you expect to be true.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        public static void Assert<T>(bool condition, IList<T> message, bool newline = true) {
            if (condition) return;

            Debug.unityLogger.Log(LogType.Assert, ParseIList(message, newline));
        }


        /// <summary>
        ///   <para>Assert a condition and logs an error message to the Unity console on failure.</para>
        /// </summary>
        /// <param name="condition">Condition you expect to be true.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        public static void Assert<TKey, TValue>(bool condition,
            IDictionary<TKey, TValue> message,
            bool newline = true) {
            if (condition) return;

            Debug.unityLogger.Log(LogType.Assert, ParseIDict(message, newline));
        }


        /// <summary>
        ///   <para>Assert a condition and logs an error message to the Unity console on failure.</para>
        /// </summary>
        /// <param name="condition">Condition you expect to be true.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        public static void Assert<T>(bool condition, ISet<T> message, bool newline = true) {
            if (condition) return;

            Debug.unityLogger.Log(LogType.Assert, ParseISet(message, newline));
        }

        /// <summary>
        ///   <para>Assert a condition and logs an error message to the Unity console on failure.</para>
        /// </summary>
        /// <param name="condition">Condition you expect to be true.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        public static void Assert(bool condition, Color message) {
            if (condition) return;

            Debug.unityLogger.Log(LogType.Assert, (object) ParseColor(message));
        }


        /// <summary>
        ///   <para>Assert a condition and logs an error message to the Unity console on failure.</para>
        /// </summary>
        /// <param name="condition">Condition you expect to be true.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        public static void Assert(bool condition, string message) {
            if (condition) return;

            Debug.unityLogger.Log(LogType.Assert, (object) message);
        }


        /// <summary>
        ///   <para>Assert a condition and logs an error message to the Unity console on failure.</para>
        /// </summary>
        /// <param name="condition">Condition you expect to be true.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        public static void Assert<T>(bool condition, IList<T> message, Object context, bool newline = true) {
            if (condition) return;

            Debug.unityLogger.Log(LogType.Assert, ParseIList(message, newline), context);
        }


        /// <summary>
        ///   <para>Assert a condition and logs an error message to the Unity console on failure.</para>
        /// </summary>
        /// <param name="condition">Condition you expect to be true.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        public static void Assert<TKey, TValue>(bool condition,
            IDictionary<TKey, TValue> message,
            Object context,
            bool newline = true) {
            if (condition) return;

            Debug.unityLogger.Log(LogType.Assert, ParseIDict(message, newline), context);
        }


        /// <summary>
        ///   <para>Assert a condition and logs an error message to the Unity console on failure.</para>
        /// </summary>
        /// <param name="condition">Condition you expect to be true.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        public static void Assert<T>(bool condition, ISet<T> message, Object context, bool newline = true) {
            if (condition) return;

            Debug.unityLogger.Log(LogType.Assert, ParseISet(message, newline), context);
        }


        /// <summary>
        ///   <para>Assert a condition and logs an error message to the Unity console on failure.</para>
        /// </summary>
        /// <param name="condition">Condition you expect to be true.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        public static void Assert(bool condition, object message, Object context) {
            if (condition) return;

            Debug.unityLogger.Log(LogType.Assert, message, context);
        }

        /// <summary>
        ///   <para>Assert a condition and logs an error message to the Unity console on failure.</para>
        /// </summary>
        /// <param name="condition">Condition you expect to be true.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        public static void Assert(bool condition, Color message, Object context) {
            if (condition) return;

            Debug.unityLogger.Log(LogType.Assert, (object) ParseColor(message), context);
        }


        /// <summary>
        ///   <para>Assert a condition and logs an error message to the Unity console on failure.</para>
        /// </summary>
        /// <param name="condition">Condition you expect to be true.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        public static void Assert(bool condition, string message, Object context) {
            if (condition) return;

            Debug.unityLogger.Log(LogType.Assert, (object) message, context);
        }


        /// <summary>
        ///   <para>Assert a condition and logs a formatted error message to the Unity console on failure.</para>
        /// </summary>
        /// <param name="condition">Condition you expect to be true.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        public static void AssertFormat(bool condition, string format, params object[] args) {
            if (condition) return;

            Debug.unityLogger.LogFormat(LogType.Assert, format, args);
        }


        /// <summary>
        ///   <para>Assert a condition and logs a formatted error message to the Unity console on failure.</para>
        /// </summary>
        /// <param name="condition">Condition you expect to be true.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        /// <param name="context">Object to which the message applies.</param>
        public static void AssertFormat(bool condition, Object context, string format, params object[] args) {
            if (condition) return;

            Debug.unityLogger.LogFormat(LogType.Assert, context, format, args);
        }


        private static string saveText = string.Empty;
        public static DebugExProfile debugExProfile = null;
        private static bool recordEnabled = false;


        public static void RecordStart() {
            if (debugExProfile == null) {
                LogError("DebugEx: 設定ファイルがありません！");
                return;
            }

            if (recordEnabled) return;

            Application.logMessageReceived += HandleLog;
            Application.quitting += ForceQuit;

            recordEnabled = true;
        }


        public static void RecordStop() {
            if (!recordEnabled) recordEnabled = false;

            Application.logMessageReceived -= HandleLog;
            DateTime dt = DateTime.Now;
            string saveTitle =
                $"{debugExProfile.logSavePath}/{dt.ToString("yyyyMMdd")}_{dt.ToString("hhmm")}_editor";
            if (!Directory.Exists($"{Application.dataPath}/{debugExProfile.logSavePath}")) {
                LogError("DebugEx: logSavePathで指定されたディレクトリが存在しません！");
            }

            CreateTextFile(saveText, saveTitle);
        }


        private static void ForceQuit() {
            Application.logMessageReceived -= HandleLog;
            Application.quitting -= ForceQuit;
        }


        private static void HandleLog(string condition, string stackTrace, LogType type) {
            saveText += $"[{type}]\n" + condition;
            saveText += "\n\n";
            saveText += debugExProfile.saveStackTrace ? stackTrace + "\n" : "";
        }


        private static void CreateTextFile(string content, string fileName) {
            string path = Application.dataPath + "/" + fileName + ".txt";
            StreamWriter sw = File.CreateText(path);
            sw.Write(content.RemoveRichText());
            sw.Close();
        }


        /// <summary>
        ///   <para>Clear all the logs in Unity Editor.</para>
        /// </summary>
        public static void ClearConsoleAll() {
            Assembly assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            Type type = assembly
#if UNITY_2017_1_OR_NEWER
                .GetType("UnityEditor.LogEntries");
#else
           .GetType( "UnityEditorInternal.LogEntries" );
#endif
            MethodInfo method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
        }


        /// <summary>
        ///   <para>Clear the error logs in Unity Editor.</para>
        /// </summary>
        public static void ClearConsoleError() {
            Debug.ClearDeveloperConsole();
        }


        /// <summary>
        ///   <para>Pauses the editor.</para>
        /// </summary>
        public static void Break() {
            Debug.Break();
        }
    }


    public static class RichText {
        public static string Color(this string str, Color color) =>
            $"<color={ColorUtility.ToHtmlStringRGBA(color)}>{str}</color>";


        public static string Bold(this string str) => $"<b>{str}</b>";


        public static string Italic(this string str) => $"<i>{str}</b>";


        public static string Size(this string str, int size) => $"<size={size}>{str}</size>";

        public static string RemoveRichText(this string input) {
            input = RemoveRichTextDynamicTag(input, "color");

            input = RemoveRichTextTag(input, "b");
            input = RemoveRichTextTag(input, "i");

            return input;
        }


        private static string RemoveRichTextDynamicTag(string input, string tag) {
            int index = -1;
            while (true) {
                index = input.IndexOf($"<{tag}=", StringComparison.Ordinal);
                if (index != -1) {
                    int endIndex = input.Substring(index, input.Length - index).IndexOf('>');
                    if (endIndex > 0)
                        input = input.Remove(index, endIndex + 1);
                    continue;
                }

                input = RemoveRichTextTag(input, tag, false);
                return input;
            }
        }

        private static string RemoveRichTextTag(string input, string tag, bool isStart = true) {
            while (true) {
                int index = input.IndexOf(isStart ? $"<{tag}>" : $"</{tag}>", StringComparison.Ordinal);
                if (index != -1) {
                    input = input.Remove(index, 2 + tag.Length + (!isStart).GetHashCode());
                    continue;
                }

                if (isStart)
                    input = RemoveRichTextTag(input, tag, false);
                return input;
            }
        }
    }
}