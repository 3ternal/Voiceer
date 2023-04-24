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
        static bool logDebug = true;

        private const string pluginsPath = "Assets/Plugins/Voiceer";
        private readonly static string voiceSelectorFullPath = pluginsPath + "/VoicePresetSelector.asset";
        private readonly static string musubimeYuiFullPath = pluginsPath + "/Voices/MusubimeYui.asset";

        static float timeOfLastError;

        [InitializeOnLoadMethod]
        private static void CreateVoicePresetSelector()
        {
            bool voiceSelectorExists = File.Exists(voiceSelectorFullPath);

            if (logDebug)
            {
                Debug.Log("Current Voice Preset exists? " + SoundPlayer.CurrentVoicePreset + "\n" +
                    "VoicePresetSelector exists? " + voiceSelectorExists + "\n" +
                    "VoicePresetSelector default path: " + voiceSelectorFullPath + "\n");
            }

            //make sure the folders exist
            if (!Directory.Exists("Assets/Plugins"))
            {
                Directory.CreateDirectory("Assets/Plugins");
            }
            if (!Directory.Exists("Assets/Plugins/Voiceer"))
            {
                Directory.CreateDirectory("Assets/Plugins/Voiceer");
            }
            if (!Directory.Exists("Assets/Plugins/Voiceer/Voices"))
            {
                Directory.CreateDirectory("Assets/Plugins/Voiceer/Voices");
            }

            //create a VoicePresetSelector if necessary
            //this will probably only happen right after installing the package
            if (!SoundPlayer.CurrentVoicePreset && !voiceSelectorExists)
            {
                if (logDebug)
                    Debug.Log("Creating a VoicePresetSelector asset\n");

                VoicePresetSelector presetSelectorAsset = ScriptableObject.CreateInstance<VoicePresetSelector>();
                AssetDatabase.CreateAsset(presetSelectorAsset, voiceSelectorFullPath);

                //do some wizardry to determine if we're in the Package context or if we're in the Voiceer project
                Assembly assembly = Assembly.GetExecutingAssembly();
                UnityEditor.PackageManager.PackageInfo packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssembly(assembly);
                bool isInPackageFolder = packageInfo != null;

                //if we're in the Package context, we need to do something confusing to get the mutable SOs into the user's Assets folder
                if (isInPackageFolder)
                {
                    string yuiSourcePath = "Packages/com.negipoyoc.voiceer/Plugins/Voiceer/Voices/MusubimeYui.asset";

                    //hang on, do we even have to copy the Yui asset? the user can copy it themselves if they want to
                    //the only asset that needs to be mutable is the VoicePresetSelector
                    //and then we can just assign the regular Yui asset from the Packages folder to it

                    //copy the template yui from the Packages folder into the user's Assets folder
                    //bool success = AssetDatabase.CopyAsset(yuiSourcePath, musubimeYuiFullPath);
                    //if (!success)
                    //{
                    //    Debug.LogError("Copy failed");
                    //}
                    //else
                    //{
                    //    if (logDebug)
                    //        Debug.Log("Copied from '" + yuiSourcePath + "'\nTo '" + musubimeYuiFullPath + "'\n");
                    //}

                    //remember to refresh the asset database after creating the copy
                    //AssetDatabase.SaveAssets();
                    //AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                    //AssetDatabase.ImportAsset(musubimeYuiFullPath);

                    //load the voice preset
                    VoicePreset yuiVoice = AssetDatabase.LoadAssetAtPath(yuiSourcePath/*musubimeYuiFullPath*/, typeof(VoicePreset)) as VoicePreset;
                    if (yuiVoice == null)
                        Debug.LogError("Couldn't find default voice");

                    //assign MusubimeYui as the default voice
                    if (logDebug)
                        Debug.Log("Assigning " + yuiVoice + " as our current VoicePreset\n");

                    presetSelectorAsset.CurrentVoicePreset = yuiVoice;
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
        /// コンパイル終了時
        /// </summary>
        [InitializeOnLoad]
        public class CompileFinishHook
        {
            static CompileFinishHook()
            {
                if (EditorApplication.isPlayingOrWillChangePlaymode)
                    return;

                EditorApplication.delayCall += () => { SoundPlayer.PlaySound(Hook.OnCompileEnd); };
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
    }
}