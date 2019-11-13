﻿using UnityEngine;
using System;
using System.Reflection;

namespace SkillEditor {

    public static class Tool {

        private static MethodInfo m_clearConsoleMethod = null;

        public static void ClearConsole() {
            if (m_clearConsoleMethod == null) {
                Type logClass = typeof(UnityEditor.EditorWindow).Assembly.GetType("UnityEditor.LogEntries");
                m_clearConsoleMethod = logClass.GetMethod("Clear");
            }
            m_clearConsoleMethod.Invoke(null, null);
        }

        public static string FileWithExtension(string fileName, string extension) {
            return GetCacheString(string.Format("{0}.{1}", fileName, extension));
        }

        public static string FullPathToProjectPath(string fullPath) {
            int subIndex = fullPath.IndexOf("Assets/");
            return GetCacheString(fullPath.Substring(subIndex));
        }

        public static string ProjectPathToFullPath(string projectPath) {
            return GetCacheString(Config.ProjectPath + projectPath);
        }

        public static string CombinePath(string path1, string path2) {
            if (path1 == null || path1.Length == 0 || path2 == null || path2.Length == 0) {
                Debug.LogError("SkillEditor.Tool::CombinePath argument error");
                return string.Empty;
            }
#if UNITY_EDITOR_WIN
            if (path1.Contains("\\"))
                path1 = path1.Replace('\\', '/');
            if (path2.Contains("\\"))
                path2 = path2.Replace('\\', '/');
#endif            
            string format;
            char flag = '/';
            bool isPath1LastFormat = path1[path1.Length - 1] == flag;
            bool isPath2StartFormat = path2[0] == flag;
            if (isPath1LastFormat && isPath2StartFormat)
                return GetCacheString(path1.Substring(0, path1.Length - 2) + path2);
            else if (!isPath1LastFormat && !isPath2StartFormat)
                format = "{0}/{1}";
            else
                format = "{0}{1}";
            return GetCacheString(string.Format(format, path1, path2));
        }

        public static string CombineFilePath(string path, string fileName, string extension = null) {
            if (extension == null)
                return CombinePath(path, fileName);
            return CombinePath(path, FileWithExtension(fileName, extension));
        }

        public static string GetFileNameFromPath(string path) {
#if UNITY_EDITOR_WIN
            string[] array = path.Replace('\\', '/').Split('/');
#elif UNITY_EDITOR_OSX
            string[] array = path.Split('/');
#else
            string[] array = path.Split('/');
#endif
            return GetCacheString(array[array.Length - 1]);
        }

        public static string GetFileNameWithourExtensionFromPath(string path) {
            string fileName = GetFileNameFromPath(path);
            int subIndex = fileName.IndexOf(".");
            return GetCacheString(fileName.Substring(0, subIndex));
        }

        public static string GetFileNameWithoutPrefix(string fileName, string prefix) {
            return GetCacheString(fileName.Substring(prefix.Length));
        }

        public static string GetPathFromFilePath(string path, string fileName) {
            int subIndex = path.IndexOf(fileName);
            return GetCacheString(path.Substring(0, subIndex));
        }

        internal static string GetCacheString(string text) {
            return string.Intern(text);
        }
    }
}