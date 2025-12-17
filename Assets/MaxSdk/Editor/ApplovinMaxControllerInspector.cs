using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AppLovinMax.Scripts.IntegrationManager.Editor;
using PS.Utils;
using UnityEditor;
using UnityEngine;

namespace PS.Ad.Inspector
{
    [CustomEditor(typeof(ApplovinMaxController))]
    public class ApplovinMaxControllerInspector : Editor
    {
        private SerializedProperty dontDestroyOnLoad;
        private SerializedProperty apiKey;
        private SerializedProperty bannerAdUnitId;
        private SerializedProperty interstitialAdUnitId;
        private SerializedProperty rewardAdUnitId;
        private SerializedProperty openAdUnitId;
        private SerializedProperty mrecAdUnitId;

        private SerializedProperty bannerBackgroundColor;
        private SerializedProperty useBannerAdaptive;
        private SerializedProperty mrecPosition;
        private SerializedProperty bannerPosition;
        private SerializedProperty isAgeRestrictedUser;
        private SerializedProperty doNotSell;
        private SerializedProperty asyncWithRemote;
        private SerializedProperty bannerPlacement;
        private SerializedProperty mrecPlacement;
        
        private SerializedProperty lstDeviceTest;
        private SerializedProperty debugGeography;
        private SerializedProperty underAgeOfConsent;
        
        private SerializedProperty openAdOnFirstLaunch;
        
        private ApplovinMaxController myScript;

        [SerializeField] private bool customBanner;
        [SerializeField] private bool customMrec;
        [SerializeField] private bool enableApiKey;

        private static bool _hasAnalyticSdk = false;
        private static bool _hasSingularSdk = false;
        private bool _compiledCompleted = false;
        private bool isCheckObfuscatorSymbol;

        private void OnEnable()
        {
            myScript = (ApplovinMaxController) target;
            apiKey = serializedObject.FindProperty("apiKey");
            
            bannerAdUnitId = serializedObject.FindProperty("bannerAdUnitId");
            interstitialAdUnitId = serializedObject.FindProperty("interstitialAdUnitId");
            rewardAdUnitId = serializedObject.FindProperty("rewardAdUnitId");
            openAdUnitId = serializedObject.FindProperty("openAdUnitId");
            mrecAdUnitId = serializedObject.FindProperty("mrecAdUnitId");
            useBannerAdaptive = serializedObject.FindProperty("useBannerAdaptive");
            bannerPosition = serializedObject.FindProperty("bannerPosition");
            bannerBackgroundColor = serializedObject.FindProperty("bannerBackgroundColor");
            
            mrecPosition = serializedObject.FindProperty("mrecPosition");
            
            dontDestroyOnLoad = serializedObject.FindProperty("dontDestroyOnLoad");
            isAgeRestrictedUser = serializedObject.FindProperty("isAgeRestrictedUser");
            doNotSell = serializedObject.FindProperty("doNotSell");
            
            openAdOnFirstLaunch = serializedObject.FindProperty("openAdOnFirstLaunch");
            asyncWithRemote = serializedObject.FindProperty("asyncWithRemote");
            bannerPlacement = serializedObject.FindProperty("bannerPlacement");
            mrecPlacement = serializedObject.FindProperty("mrecPlacement");
            
            lstDeviceTest = serializedObject.FindProperty("lstDeviceTest");
            debugGeography = serializedObject.FindProperty("debugGeography");
            underAgeOfConsent = serializedObject.FindProperty("underAgeOfConsent");

            enableApiKey = EditorPrefs.GetBool("enableApiKey");
            customBanner = EditorPrefs.GetBool("customBanner");
            customMrec = EditorPrefs.GetBool("customMrec");
            
            AppLovinEditorCoroutine.StartCoroutine(IsSingularImported());
            AppLovinEditorCoroutine.StartCoroutine(IsGameAnalyticsImported());
            AppLovinEditorCoroutine.StartCoroutine(CheckObfuscatorAutomation());
            
        }

        private void OnDisable()
        {
            EditorPrefs.SetBool("enableApiKey", enableApiKey);
            EditorPrefs.SetBool("customBanner", customBanner);
            EditorPrefs.SetBool("customMrec", customMrec);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Color guiColor = GUI.color;
            GUI.color = Color.clear;
            float posx = (EditorGUIUtility.currentViewWidth - 100) / 2;
            // EditorGUI.DrawPreviewTexture(new Rect(posx,10, 100, 100), Resources.Load<Texture>("companyLogo"));
            EditorGUI.DrawTextureTransparent(new Rect(posx,10, 100, 100), Resources.Load<Texture>("companyLogo"));
            GUI.color = guiColor;
            EditorGUILayout.Space(120);

            var style = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter};
            EditorGUILayout.LabelField($"SDK Version: {DataHelper.SDK_VERSION}", style, GUILayout.ExpandWidth(true));

            if (EditorApplication.isCompiling)
            {
                style.fontStyle = FontStyle.Bold;
                EditorGUILayout.LabelField($"Compiling... Please! Wait!", style, GUILayout.ExpandWidth(true));
                _compiledCompleted = false;
                
                if (!isCheckObfuscatorSymbol)
                {
                    isCheckObfuscatorSymbol = true;
                    AppLovinEditorCoroutine.StartCoroutine(CheckObfuscatorAutomation());
                }
                
                return;
            }

            if (!_compiledCompleted)
            {
                _compiledCompleted = true;
                AppLovinEditorCoroutine.StartCoroutine(IsSingularImported());
                AppLovinEditorCoroutine.StartCoroutine(IsGameAnalyticsImported());
            }

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Singleton:");
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(dontDestroyOnLoad, new GUIContent("Persist all scene"));
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(3);

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.Space(10);

            EditorGUI.indentLevel++;
            enableApiKey = EditorGUILayout.Foldout(enableApiKey, "API KeyUnlockBox");

            if (enableApiKey)
            {
                var apiKeyStyle = new GUIStyle(EditorStyles.textArea); 
                apiKeyStyle.wordWrap = true;
                apiKeyStyle.normal.textColor = Color.gray;
                apiKeyStyle.hover.textColor = Color.green;
                apiKeyStyle.focused.textColor = Color.green;
                //EditorGUILayout.PropertyField(apiKey);
                apiKey.stringValue = EditorGUILayout.TextArea(apiKey.stringValue, apiKeyStyle, GUILayout.Height(100));
            }
            EditorGUI.indentLevel--;
            
            EditorGUILayout.Space(3);

            EditorGUILayout.PropertyField(isAgeRestrictedUser, new GUIContent("Is Age Restricted:"));
            EditorGUILayout.PropertyField(doNotSell, new GUIContent("Do Not Sell (California, USA):"));
            
            EditorGUILayout.Space(3);
            
            EditorGUILayout.PropertyField(bannerAdUnitId, new GUIContent("Banner Id:"));
            EditorGUILayout.PropertyField(interstitialAdUnitId, new GUIContent("Interstitial Id:"));
            EditorGUILayout.PropertyField(rewardAdUnitId, new GUIContent("Reward Id:"));
            EditorGUILayout.PropertyField(openAdUnitId, new GUIContent("Open Ad Id:"));
            EditorGUILayout.PropertyField(mrecAdUnitId, new GUIContent("MREC Id:"));
            EditorGUILayout.Space(10);
            
            EditorGUILayout.PropertyField(asyncWithRemote, new GUIContent("Async with remote config:"));
            
            customBanner = EditorGUILayout.Toggle("Show Banner Option", customBanner);

            if (customBanner)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(useBannerAdaptive, new GUIContent("Use Banner Adaptive"));
                EditorGUILayout.PropertyField(bannerPosition, new GUIContent("Position"));
                EditorGUILayout.PropertyField(bannerBackgroundColor, new GUIContent("BG Color:"));
                EditorGUILayout.PropertyField(bannerPlacement, new GUIContent("Placement Name:"));
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Space(3);
            
            customMrec = EditorGUILayout.Toggle("Show MREC Option", customMrec);

            if (customMrec)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(mrecPosition, new GUIContent("Position"));
                EditorGUILayout.PropertyField(mrecPlacement, new GUIContent("Placement Name:"));
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Space(10);

            var lableStyle = new GUIStyle();
            lableStyle.normal.textColor = Color.red;
            lableStyle.fontStyle = FontStyle.Bold;
            
            EditorGUILayout.LabelField("DEBUG ONLY:", lableStyle);
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginVertical(GUI.skin.window);
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(lstDeviceTest, new GUIContent("List Test Devices:"));
            EditorGUILayout.Space(10);
            EditorGUILayout.PropertyField(debugGeography, new GUIContent("Geography:"));
            EditorGUILayout.PropertyField(underAgeOfConsent, new GUIContent("Is Under Age Of Consent:"));
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;
            
            EditorGUILayout.Space(5);
            
            EditorGUILayout.LabelField("Tracking:");

            EditorGUILayout.BeginHorizontal();
            var btnStyle = new GUIStyle(GUI.skin.button);
            btnStyle.hover.textColor = Color.green;

            bool hasSymbol = HasSymbol(DataHelper.PS_UNITY_ANALYTICS_SYMBOL);

            string btnName = hasSymbol ? "Disable Tracking" : "Enable Tracking";

            GUI.enabled = _hasAnalyticSdk;
            if (GUILayout.Button(btnName, btnStyle, GUILayout.Height(50)))
            {
                if (hasSymbol)
                {
                    DisableGameAnalyticTracking();
                }
                else
                {
                    SetScriptDefineSymbol();
                }
            }
            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5);

            if (!_hasAnalyticSdk)
            {
                EditorGUILayout.HelpBox("Make sure Game Analytics has installed before enabled Game Analytic tracking.", MessageType.Warning);
            }

            EditorGUILayout.Space(5);
            
            bool hasSingularSym = HasSymbol(DataHelper.PS_UNITY_SINGULAR_SYMBOL);
            string btnSingName = hasSingularSym ? "Disable Singular Revenue Tracking" : "Enable Singular Revenue Tracking";

            GUI.enabled = _hasSingularSdk;
            btnStyle.hover.textColor = Color.red;
            if (GUILayout.Button(btnSingName, btnStyle, GUILayout.Height(50)))
            {
                if (hasSingularSym)
                {
                    DisableSingularRevenueTracking();
                }
                else
                {
                    EnableSingularRevenueTracking();
                }
            }
            GUI.enabled = true;
            
            if (!_hasSingularSdk)
            {
                EditorGUILayout.HelpBox("Make sure Singular SDK has installed before enabled tracking.", MessageType.Warning);
            }
            
            EditorGUILayout.Space(10);

            if (GUILayout.Button("Add Obfuscator Skip For Applovin Max", GUILayout.Height(50)))
            {
                AppLovinEditorCoroutine.StartCoroutine(AddApplovinMaxSymbols());
            }

            
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        [MenuItem("AppLovin/Enable Game Analytic Tracking")]
        public static void SetScriptDefineSymbol()
        {
            BuildTargetGroup btg;
            
            #if UNITY_IOS
            btg = BuildTargetGroup.iOS;
            #else
            btg = BuildTargetGroup.Android;
            #endif
            
            string[] define;
            PlayerSettings.GetScriptingDefineSymbolsForGroup(btg, out define);
//            PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, out define);

            var listDefine = define.ToList();

            if (!listDefine.Contains(DataHelper.PS_UNITY_ANALYTICS_SYMBOL))
            {
                listDefine.Add(DataHelper.PS_UNITY_ANALYTICS_SYMBOL);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(btg, listDefine.ToArray());
            }

        }

        [MenuItem("AppLovin/Disable Game Analytic Tracking")]
        public static void DisableGameAnalyticTracking()
        {
            BuildTargetGroup btg;
            
#if UNITY_IOS
            btg = BuildTargetGroup.iOS;
#else
            btg = BuildTargetGroup.Android;
#endif
            
            string[] define;
            PlayerSettings.GetScriptingDefineSymbolsForGroup(btg, out define);
//            PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, out define);

            var listDefine = define.ToList();

            if (listDefine.Contains(DataHelper.PS_UNITY_ANALYTICS_SYMBOL))
            {
                listDefine.Remove(DataHelper.PS_UNITY_ANALYTICS_SYMBOL);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(btg, listDefine.ToArray());
            }
        }

        public static void DisableSingularRevenueTracking()
        {
            BuildTargetGroup btg;
            
#if UNITY_IOS
            btg = BuildTargetGroup.iOS;
#else
            btg = BuildTargetGroup.Android;
#endif
            
            string[] define;
            PlayerSettings.GetScriptingDefineSymbolsForGroup(btg, out define);
//            PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, out define);

            var listDefine = define.ToList();

            if (listDefine.Contains(DataHelper.PS_UNITY_SINGULAR_SYMBOL))
            {
                listDefine.Remove(DataHelper.PS_UNITY_SINGULAR_SYMBOL);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(btg, listDefine.ToArray());
            }
        }
        
        public static void EnableSingularRevenueTracking()
        {
            BuildTargetGroup btg;
            
#if UNITY_IOS
            btg = BuildTargetGroup.iOS;
#else
            btg = BuildTargetGroup.Android;
#endif
            
            string[] define;
            PlayerSettings.GetScriptingDefineSymbolsForGroup(btg, out define);
//            PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, out define);

            var listDefine = define.ToList();
            

            if (!listDefine.Contains(DataHelper.PS_UNITY_SINGULAR_SYMBOL))
            {
                listDefine.Add(DataHelper.PS_UNITY_SINGULAR_SYMBOL);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(btg, listDefine.ToArray());
            }
        }

        public static bool HasSymbol(string symbol)
        {
            BuildTargetGroup btg;
            
#if UNITY_IOS
            btg = BuildTargetGroup.iOS;
#else
            btg = BuildTargetGroup.Android;
#endif
            
            string[] define;
            PlayerSettings.GetScriptingDefineSymbolsForGroup(btg, out define);
//            PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, out define);

            var listDefine = define.ToList();

            return listDefine.Contains(symbol);
        }

        public static IEnumerator IsGameAnalyticsImported()
        {
            
            foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach(Type type in assembly.GetTypes())
                {
                    if (type.FullName == "PS.Analytic.GameAnalyticController")
                    {
                        _hasAnalyticSdk = true;
                        yield break;
                    }
                        
                }
            }

            _hasAnalyticSdk = false;
            
            DisableGameAnalyticTracking();

            yield return null;

        }

        public static IEnumerator IsSingularImported()
        {
            foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach(Type type in assembly.GetTypes())
                {
                    if (type.Name == "SingularSDK")
                    {
                        _hasSingularSdk = true;
                        yield break;
                    }
                        
                }
            }

            _hasSingularSdk = false;
            DisableSingularRevenueTracking();
            yield return null;
        }
        
        IEnumerator AddApplovinMaxSymbols()
        {
            bool hasObfuscator = IsObfuscatorImported();
            yield return null;
            
            if (!hasObfuscator)
            {
                EditorUtility.DisplayDialog("PS SDK", "Obfuscator is not install.\nPlease install it before use this func.", "Ok");
                yield break;
            }
            
#if USE_OBFUSCATOR

            Beebyte.Obfuscator.Options o = Beebyte.Obfuscator.OptionsManager.LoadOptions();

            if (o != null)
            {
                var skipClasses = o.skipClasses.ToList();

                int count = skipClasses.Count;

                foreach (var sym in ObfuscatorSym.symbols)
                {
                    if (!skipClasses.Contains(sym))
                    {
                        skipClasses.Add(sym);
                    }
                }

                o.skipClasses = skipClasses.ToArray();
                EditorUtility.SetDirty(o);

                if (skipClasses.Count == count)
                {
                    EditorUtility.DisplayDialog("PS SDK", "You have already add it.", "Ok");
                }
                else
                {
                    EditorUtility.DisplayDialog("PS SDK", "Add Obfuscator is successful", "Ok");
                }
            }
#endif
        }
        
        static bool IsObfuscatorImported()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.FullName == "Beebyte.Obfuscator.Options")
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
        IEnumerator CheckObfuscatorAutomation()
        {
            bool hasOb = IsObfuscatorImported();

            yield return null;

            if (hasOb)
            {
                DoAddSymbol(DataHelper.USE_OBFUSCATOR);
            }
            else
            {
                DoRemoveSymbol(DataHelper.USE_OBFUSCATOR);
            }

            isCheckObfuscatorSymbol = false;
        }
        
        public static void DoAddSymbol(string symbol)
        {
            BuildTargetGroup btg;

#if UNITY_IOS
            btg = BuildTargetGroup.iOS;
#else
            btg = BuildTargetGroup.Android;
#endif

            string[] define;
            PlayerSettings.GetScriptingDefineSymbolsForGroup(btg, out define);
//            PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, out define);

            var listDefine = define.ToList();

            if (!listDefine.Contains(symbol))
            {
                listDefine.Add(symbol);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(btg, listDefine.ToArray());
            }
        }
        
        public static void DoRemoveSymbol(string symbol)
        {
            BuildTargetGroup btg;

#if UNITY_IOS
            btg = BuildTargetGroup.iOS;
#else
            btg = BuildTargetGroup.Android;
#endif

            string[] define;
            PlayerSettings.GetScriptingDefineSymbolsForGroup(btg, out define);

            var listDefine = define.ToList();

            if (listDefine.Contains(symbol))
            {
                listDefine.Remove(symbol);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(btg, listDefine.ToArray());
            }
        }
    }

    public struct ObfuscatorSym
    {
        public static List<string> symbols = new List<string>()
        {
            "MaxSdkAndroid",
            "MaxSdkiOS",
        };
    }

}
