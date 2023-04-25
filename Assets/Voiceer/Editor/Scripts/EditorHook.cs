using System.Reflection;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;

namespace Voiceer
{
    public class EditorHook
    {
        static bool logDebug = false;

        private const string yuiSourcePath = "Packages/com.negipoyoc.voiceer/Voiceer/ScriptableObjects/MusubimeYui.asset";
        private const string voiceSelectorSourcePath = "Packages/com.negipoyoc.voiceer/Voiceer/ScriptableObjects/VoicePresetSelector.asset";

        private const string pluginsPath = "Assets/Plugins/Voiceer";
        private readonly static string voiceSelectorTargetPath = pluginsPath + "/VoicePresetSelector.asset";

        static float timeOfLastError;

        [InitializeOnLoadMethod]
        private static void CreateVoicePresetSelector()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            bool voiceSelectorExists = File.Exists(voiceSelectorTargetPath);

            if (logDebug)
            {
                Debug.Log("Attempting to create a VoicePresetSelector, if necessary\n" +
                    "Current Voice Preset exists? " + SoundPlayer.CurrentVoicePreset + "\n" +
                    "VoicePresetSelector exists? " + voiceSelectorExists + "\n" +
                    "VoicePresetSelector default path: " + voiceSelectorTargetPath + "\n");
            }

            //create a VoicePresetSelector if necessary
            //this will probably only happen right after installing the package
            if (!SoundPlayer.VoiceSelectorExists)
            {
                if (logDebug)
                    Debug.Log("Creating a VoicePresetSelector asset\n");

                //do some wizardry to determine if we're in the Package context or if we're in the Voiceer project
                Assembly assembly = Assembly.GetExecutingAssembly();
                UnityEditor.PackageManager.PackageInfo packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssembly(assembly);
                bool isInPackageFolder = packageInfo != null;

                //if we're in the Package context, we need to do something confusing to get the mutable SOs into the user's Assets folder
                if (isInPackageFolder)
                {
                    //make sure the folders exist
                    if (!Directory.Exists("Assets/Plugins"))
                        Directory.CreateDirectory("Assets/Plugins");

                    if (!Directory.Exists("Assets/Plugins/Voiceer"))
                        Directory.CreateDirectory("Assets/Plugins/Voiceer");

                    if (!Directory.Exists("Assets/Plugins/Voiceer/Voices"))
                        Directory.CreateDirectory("Assets/Plugins/Voiceer/Voices");

                    bool success = AssetDatabase.CopyAsset(voiceSelectorSourcePath, voiceSelectorTargetPath);
                    if (!success)
                    {
                        Debug.LogError("Copy failed\nSource path: " + voiceSelectorSourcePath + "\nTarget path: " + voiceSelectorTargetPath + "\n" +
                            "Source exists? " + File.Exists(voiceSelectorSourcePath));
                    }
                    else
                    {
                        if (logDebug)
                            Debug.Log("Copied from '" + voiceSelectorSourcePath + "'\nTo '" + voiceSelectorTargetPath + "'\n");
                    }

                    //we were originally going to copy the yui asset to the player's Plugins folder, but I guess there's no need
                    //the only asset that really needs to be mutable is the VoicePresetSelector
                }
            }
        }

        [InitializeOnLoadMethod]
        private static void InitializeEditorHookMethods()
        {
            Application.logMessageReceived += ReceiveLogMessage;

            //PlayModeが変わった時
            //シーン再生を開始した時
            //シーン再生を止めた時
            EditorApplication.playModeStateChanged += (mode) =>
            {
                //再生ボタンを押した時であること
                if (!EditorApplication.isPlayingOrWillChangePlaymode
                    && EditorApplication.isPlaying)
                    return;

                //SceneView が存在すること
                if (SceneView.sceneViews.Count == 0)
                    return;

                switch (mode)
                {
                    //Playモードに入れた時
                    case PlayModeStateChange.EnteredPlayMode:
                        SoundPlayer.PlaySound(Hook.OnEnteredPlayMode);
                        break;
                    //Playモードを終了した時
                    case PlayModeStateChange.EnteredEditMode:
                        SoundPlayer.PlaySound(Hook.OnExitingPlayMode);
                        break;
                }

                //エラーがあるのにPlayしようとした。
                //register event for if you enter play mode while an error is present
                EditorApplication.delayCall += () =>
                {
                    var content = typeof(EditorWindow)
                        .GetField("m_Notification", BindingFlags.NonPublic | BindingFlags.Instance)
                        ?.GetValue(SceneView.sceneViews[0]) as GUIContent;

                    if (content != null && !string.IsNullOrEmpty(content.text))
                    {
                        SoundPlayer.PlaySound(Hook.OnPlayButHasError);
                    }
                };
            };

            ///シーンを保存する時
            EditorSceneManager.sceneSaved += (scene) => { SoundPlayer.PlaySound(Hook.OnSave); };
        }

        /// <summary>
        /// コンパイル終了時<br></br>
        /// InitializeOnLoad means that this class will be initialized when Unity loads AND when you recompile.
        /// </summary>
        [InitializeOnLoad]
        public class CompileFinishHook
        {
            static CompileFinishHook()
            {
                if (logDebug)
                    Debug.Log("On recompile or reload\nisPlayingOrWillChangePlaymode? " + EditorApplication.isPlayingOrWillChangePlaymode + "\n");

                if (EditorApplication.isPlayingOrWillChangePlaymode)
                    return;

                if (logDebug)
                    Debug.Log("Registered CompileFinishHook" + "\n");

                //I think this is sort of like yield return null
                //the event will be executed only once, and it will happen after the editor refreshes
                //EditorApplication.delayCall += () => { SoundPlayer.PlaySound(Hook.OnCompileEnd); };

                //only play this on "true" recompile, not on the initial editor load
                if (SessionState.GetBool("playedWelcomeSound", false))
                {
                    SoundPlayer.PlaySound(Hook.OnCompileEnd);
                }
            }
        }

        /// <summary>
        /// ビルド前・後
        /// </summary>
        public class ProcessBuildHook : IPreprocessBuildWithReport, IPostprocessBuildWithReport
        {
            public int callbackOrder
            {
                get { return 0; }
            }

            /// <summary>
            /// ビルド前
            /// </summary>
            /// <param name="report"></param>
            public void OnPreprocessBuild(BuildReport report)
            {
                SoundPlayer.PlaySound(Hook.OnPreProcessBuild);
            }

            /// <summary>
            /// ビルド後
            /// </summary>
            /// <param name="report"></param>
            public void OnPostprocessBuild(BuildReport report)
            {
                if (report.summary.result == BuildResult.Failed || report.summary.result == BuildResult.Failed)
                {
                    SoundPlayer.PlaySound(Hook.OnPostProcessBuildFailed);
                }
                else
                {
                    SoundPlayer.PlaySound(Hook.OnPostProcessBuildSuccess);
                }
            }
        }

        /// <summary>
        /// ビルドターゲット変更時
        /// </summary>
        public class BuildTargetChangeHook : IActiveBuildTargetChanged
        {
            public int callbackOrder => int.MaxValue;

            public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
            {
                SoundPlayer.PlaySound(Hook.OnBuildTargetChanged);
            }
        }

        static void ReceiveLogMessage(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
            {
                if (!Mathf.Approximately(timeOfLastError, Time.realtimeSinceStartup))
                {
                    SoundPlayer.PlaySound(Hook.OnError);
                }

                timeOfLastError = Time.realtimeSinceStartup;
            }
        }

        /// <summary>
        /// Play a sound when you boot up Unity
        /// </summary>
        [InitializeOnLoadMethod]
        static void PlayWelcomeSound()
        {
            if (SessionState.GetBool("playedWelcomeSound", false)) return;

            SessionState.SetBool("playedWelcomeSound", true);

            SoundPlayer.PlaySound(Hook.OnLaunchEditor);
        }
    }
}