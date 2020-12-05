using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DebugExtension {
    [CreateAssetMenu(menuName = "DebugExSetting")]
    public class DebugExProfile : ScriptableObject {
        [Header("Path to save the log file\n")]
        public string logSavePath = "DebugExtensions/Logs";

        [Header("Save with stackTrace\n")]
        public bool saveStackTrace = true;

        [Header("Debug String Colors")]
        [SerializeField]
        private Color dictionaryKey = new Color(0.8f, 0.023f, 0f, 1f);

        [SerializeField]
        private Color dictionaryValue = new Color(0f, 0.119f, 0.69f, 1f);

        [HideInInspector]
        public string dictKeyColor;

        [HideInInspector]
        public string dictValueColor;

        private void OnEnable() {
            dictKeyColor = ColorUtility.ToHtmlStringRGB(dictionaryKey);
            dictValueColor = ColorUtility.ToHtmlStringRGB(dictionaryValue);

            if (DebugEx.debugExProfile == null) {
                DebugEx.debugExProfile = this;
            }
        }
    }
}