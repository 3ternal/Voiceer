using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Voiceer
{
    public static class SoundPlayer
    {
        const bool logDebug = false;

        private static VoicePreset CurrentVoicePreset => VoiceerEditorUtility.GetStorageSelector()?.CurrentVoicePreset;

        public static void PlaySound(Hook hook)
        {
            //VoicePresetがあるか
            if (CurrentVoicePreset == null)
            {
                if (logDebug)
                    Debug.Log("Current Voice Preset was null");

                return;
            }

            //Clipが存在するか
            var clip = CurrentVoicePreset.GetRandomClip(hook);
            if (clip == null)
            {
                if (logDebug)
                    Debug.Log("Couldn't find a voice clip for " + hook);

                return;
            }

            if (logDebug)
                Debug.Log("Attempting to play a sound for " + hook + "\nClip name: " + clip.name + "\n");

            var unityEditorAssembly = typeof(AudioImporter).Assembly;

            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            var method = audioUtilClass.GetMethod//
            (
                "PlayPreviewClip",
                BindingFlags.Static | BindingFlags.Public,
                null,
#if UNITY_2019_2_OR_NEWER
                new System.Type[] { typeof(AudioClip), typeof(int), typeof(bool) },
#else
                new Type[] { typeof(AudioClip) },
#endif
                null
            );

            if (method == null)
            {
                Debug.LogError("Method is null!");
            }

#if UNITY_2019_2_OR_NEWER
            //Debug.Log(clip);
            //Debug.Log(method);
            method.Invoke(null, new object[] { clip, 0, false });
#else
            method.Invoke(null, new object[] {clip});
#endif
            
        }
    }
}