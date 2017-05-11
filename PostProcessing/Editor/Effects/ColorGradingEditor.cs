using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PostProcessing;

namespace UnityEditor.Experimental.PostProcessing
{
    [PostProcessEditor(typeof(ColorGrading))]
    public sealed class ColorGradingEditor : PostProcessEffectEditor<ColorGrading>
    {
        SerializedParameterOverride m_GradingMode;

        static GUIContent[] s_Curves =
        {
            new GUIContent("Master"),
            new GUIContent("Red"),
            new GUIContent("Green"),
            new GUIContent("Blue"),
            new GUIContent("Hue VS Hue"),
            new GUIContent("Hue VS Sat"),
            new GUIContent("Sat VS Sat"),
            new GUIContent("Lum VS Sat")
        };

        SerializedParameterOverride m_LdrLut;

        SerializedParameterOverride m_Temperature;
        SerializedParameterOverride m_Tint;

        SerializedParameterOverride m_ColorFilter;
        SerializedParameterOverride m_HueShift;
        SerializedParameterOverride m_Saturation;
        SerializedParameterOverride m_Brightness;
        SerializedParameterOverride m_PostExposure;
        SerializedParameterOverride m_Contrast;

        SerializedParameterOverride m_MixerRedOutRedIn;
        SerializedParameterOverride m_MixerRedOutGreenIn;
        SerializedParameterOverride m_MixerRedOutBlueIn;

        SerializedParameterOverride m_MixerGreenOutRedIn;
        SerializedParameterOverride m_MixerGreenOutGreenIn;
        SerializedParameterOverride m_MixerGreenOutBlueIn;

        SerializedParameterOverride m_MixerBlueOutRedIn;
        SerializedParameterOverride m_MixerBlueOutGreenIn;
        SerializedParameterOverride m_MixerBlueOutBlueIn;

        SerializedParameterOverride m_Lift;
        SerializedParameterOverride m_Gamma;
        SerializedParameterOverride m_Gain;

        SerializedParameterOverride m_MasterCurve;
        SerializedParameterOverride m_RedCurve;
        SerializedParameterOverride m_GreenCurve;
        SerializedParameterOverride m_BlueCurve;

        SerializedParameterOverride m_HueVsHueCurve;
        SerializedParameterOverride m_HueVsSatCurve;
        SerializedParameterOverride m_SatVsSatCurve;
        SerializedParameterOverride m_LumVsSatCurve;

        // Internal references to the actual animation curves
        // Needed for the curve editor
        SerializedProperty m_RawMasterCurve;
        SerializedProperty m_RawRedCurve;
        SerializedProperty m_RawGreenCurve;
        SerializedProperty m_RawBlueCurve;

        SerializedProperty m_RawHueVsHueCurve;
        SerializedProperty m_RawHueVsSatCurve;
        SerializedProperty m_RawSatVsSatCurve;
        SerializedProperty m_RawLumVsSatCurve;

        SerializedProperty m_CurrentMixerChannel;
        SerializedProperty m_CurrentEditingCurve;

        CurveEditor m_CurveEditor;
        Dictionary<SerializedProperty, Color> m_CurveDict;
        
        SerializedParameterOverride m_LogLut;

        public override void OnEnable()
        {
            m_GradingMode = FindParameterOverride(x => x.gradingMode);

            m_LdrLut = FindParameterOverride(x => x.ldrLut);

            m_Temperature = FindParameterOverride(x => x.temperature);
            m_Tint = FindParameterOverride(x => x.tint);

            m_ColorFilter = FindParameterOverride(x => x.colorFilter);
            m_HueShift = FindParameterOverride(x => x.hueShift);
            m_Saturation = FindParameterOverride(x => x.saturation);
            m_Brightness = FindParameterOverride(x => x.brightness);
            m_PostExposure = FindParameterOverride(x => x.postExposure);
            m_Contrast = FindParameterOverride(x => x.contrast);

            m_MixerRedOutRedIn = FindParameterOverride(x => x.mixerRedOutRedIn);
            m_MixerRedOutGreenIn = FindParameterOverride(x => x.mixerRedOutGreenIn);
            m_MixerRedOutBlueIn = FindParameterOverride(x => x.mixerRedOutBlueIn);

            m_MixerGreenOutRedIn = FindParameterOverride(x => x.mixerGreenOutRedIn);
            m_MixerGreenOutGreenIn = FindParameterOverride(x => x.mixerGreenOutGreenIn);
            m_MixerGreenOutBlueIn = FindParameterOverride(x => x.mixerGreenOutBlueIn);

            m_MixerBlueOutRedIn = FindParameterOverride(x => x.mixerBlueOutRedIn);
            m_MixerBlueOutGreenIn = FindParameterOverride(x => x.mixerBlueOutGreenIn);
            m_MixerBlueOutBlueIn = FindParameterOverride(x => x.mixerBlueOutBlueIn);

            m_Lift = FindParameterOverride(x => x.lift);
            m_Gamma = FindParameterOverride(x => x.gamma);
            m_Gain = FindParameterOverride(x => x.gain);

            m_MasterCurve = FindParameterOverride(x => x.masterCurve);
            m_RedCurve = FindParameterOverride(x => x.redCurve);
            m_GreenCurve = FindParameterOverride(x => x.greenCurve);
            m_BlueCurve = FindParameterOverride(x => x.blueCurve);

            m_HueVsHueCurve = FindParameterOverride(x => x.hueVsHueCurve);
            m_HueVsSatCurve = FindParameterOverride(x => x.hueVsSatCurve);
            m_SatVsSatCurve = FindParameterOverride(x => x.satVsSatCurve);
            m_LumVsSatCurve = FindParameterOverride(x => x.lumVsSatCurve);

            m_RawMasterCurve = FindProperty(x => x.masterCurve.value.curve);
            m_RawRedCurve = FindProperty(x => x.redCurve.value.curve);
            m_RawGreenCurve = FindProperty(x => x.greenCurve.value.curve);
            m_RawBlueCurve = FindProperty(x => x.blueCurve.value.curve);

            m_RawHueVsHueCurve = FindProperty(x => x.hueVsHueCurve.value.curve);
            m_RawHueVsSatCurve = FindProperty(x => x.hueVsSatCurve.value.curve);
            m_RawSatVsSatCurve = FindProperty(x => x.satVsSatCurve.value.curve);
            m_RawLumVsSatCurve = FindProperty(x => x.lumVsSatCurve.value.curve);
            
            m_CurrentMixerChannel = serializedObject.FindProperty("m_CurrentMixerChannel");
            m_CurrentEditingCurve = serializedObject.FindProperty("m_CurrentEditingCurve");

            m_CurveEditor = new CurveEditor();
            m_CurveDict = new Dictionary<SerializedProperty, Color>();

            // Prepare the curve editor
            SetupCurve(m_RawMasterCurve, new Color(1f, 1f, 1f), 2, false);
            SetupCurve(m_RawRedCurve, new Color(1f, 0f, 0f), 2, false);
            SetupCurve(m_RawGreenCurve, new Color(0f, 1f, 0f), 2, false);
            SetupCurve(m_RawBlueCurve, new Color(0f, 0.5f, 1f), 2, false);
            SetupCurve(m_RawHueVsHueCurve, new Color(1f, 1f, 1f), 0, true);
            SetupCurve(m_RawHueVsSatCurve, new Color(1f, 1f, 1f), 0, true);
            SetupCurve(m_RawSatVsSatCurve, new Color(1f, 1f, 1f), 0, false);
            SetupCurve(m_RawLumVsSatCurve, new Color(1f, 1f, 1f), 0, false);

            m_LogLut = FindParameterOverride(x => x.logLut);
        }

        public override void OnInspectorGUI()
        {
            PropertyField(m_GradingMode);

            var gradingMode = (GradingMode)m_GradingMode.value.intValue;
            if (gradingMode == GradingMode.LowDefinitionRange)
                DoStandardModeGUI(false);
            else if (gradingMode == GradingMode.HighDefinitionRange)
                DoStandardModeGUI(true);
            else
                DoCustomHdrGUI();
            
            EditorGUILayout.Space();
        }

        void SetupCurve(SerializedProperty prop, Color color, uint minPointCount, bool loop)
        {
            var state = CurveEditor.CurveState.defaultState;
            state.color = color;
            state.visible = false;
            state.minPointCount = minPointCount;
            state.onlyShowHandlesOnSelection = true;
            state.zeroKeyConstantValue = 0.5f;
            state.loopInBounds = loop;
            m_CurveEditor.Add(prop, state);
            m_CurveDict.Add(prop, color);
        }

        void DoStandardModeGUI(bool hdr)
        {
            EditorGUILayout.HelpBox("Only GradingMode.CustomLogLUT works right now.", MessageType.Error);

            if (!hdr)
            {
                PropertyField(m_LdrLut);

                var lut = (target as ColorGrading).ldrLut.value;
                CheckLutImportSettings(lut);
            }

            EditorGUILayout.Space();
            EditorUtilities.DrawHeaderLabel("White Balance");
            
            PropertyField(m_Temperature);
            PropertyField(m_Tint);

            EditorGUILayout.Space();
            EditorUtilities.DrawHeaderLabel("Tone");

            if (hdr)
                PropertyField(m_PostExposure);

            PropertyField(m_ColorFilter);
            PropertyField(m_HueShift);
            PropertyField(m_Saturation);

            if (!hdr)
                PropertyField(m_Brightness);

            PropertyField(m_Contrast);

            EditorGUILayout.Space();
            int currentChannel = m_CurrentMixerChannel.intValue;

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel("Channel Mixer", GUIStyle.none, Styling.labelHeader);

                EditorGUI.BeginChangeCheck();
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        GUILayoutUtility.GetRect(14f, 18f, GUILayout.ExpandWidth(false)); // Dirty hack to do proper right column alignement
                        if (GUILayout.Toggle(currentChannel == 0, EditorUtilities.GetContent("Red|Red output channel."), EditorStyles.miniButtonLeft)) currentChannel = 0;
                        if (GUILayout.Toggle(currentChannel == 1, EditorUtilities.GetContent("Green|Green output channel."), EditorStyles.miniButtonMid)) currentChannel = 1;
                        if (GUILayout.Toggle(currentChannel == 2, EditorUtilities.GetContent("Blue|Blue output channel."), EditorStyles.miniButtonRight)) currentChannel = 2;
                    }
                }
                if (EditorGUI.EndChangeCheck())
                    GUI.FocusControl(null);
            }

            m_CurrentMixerChannel.intValue = currentChannel;

            if (currentChannel == 0)
            {
                PropertyField(m_MixerRedOutRedIn);
                PropertyField(m_MixerRedOutGreenIn);
                PropertyField(m_MixerRedOutBlueIn);
            }
            else if (currentChannel == 1)
            {
                PropertyField(m_MixerGreenOutRedIn);
                PropertyField(m_MixerGreenOutGreenIn);
                PropertyField(m_MixerGreenOutBlueIn);
            }
            else
            {
                PropertyField(m_MixerBlueOutRedIn);
                PropertyField(m_MixerBlueOutGreenIn);
                PropertyField(m_MixerBlueOutBlueIn);
            }

            EditorGUILayout.Space();
            EditorUtilities.DrawHeaderLabel("Trackballs");

            using (new EditorGUILayout.HorizontalScope())
            {
                PropertyField(m_Lift);
                GUILayout.Space(4f);
                PropertyField(m_Gamma);
                GUILayout.Space(4f);
                PropertyField(m_Gain);
            }

            EditorGUILayout.Space();
            EditorUtilities.DrawHeaderLabel("Grading Curves");

            DoCurvesGUI();
        }

        void DoCustomHdrGUI()
        {
            PropertyField(m_LogLut);

            var lut = (target as ColorGrading).logLut.value;
            CheckLutImportSettings(lut);
        }

        void CheckLutImportSettings(Texture lut)
        {
            if (lut != null)
            {
                var importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(lut)) as TextureImporter;

                // Fails when using an internal texture as you can't change import settings on
                // builtin resources, thus the check for null
                if (importer != null)
                {
                    bool valid = importer.anisoLevel == 0
                        && importer.mipmapEnabled == false
                        && importer.sRGBTexture == false
                        && (importer.textureCompression == TextureImporterCompression.Uncompressed)
                        && importer.filterMode == FilterMode.Bilinear;

                    if (!valid)
                        EditorUtilities.DrawFixMeBox("Invalid LUT import settings.", () => SetLutImportSettings(importer));
                }
            }
        }

        void SetLutImportSettings(TextureImporter importer)
        {
            importer.textureType = TextureImporterType.Default;
            importer.filterMode = FilterMode.Bilinear;
            importer.mipmapEnabled = false;
            importer.anisoLevel = 0;
            importer.sRGBTexture = false;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.alphaSource = TextureImporterAlphaSource.None;
            importer.SaveAndReimport();
            AssetDatabase.Refresh();
        }

        void ResetVisibleCurves()
        {
            foreach (var curve in m_CurveDict)
            {
                var state = m_CurveEditor.GetCurveState(curve.Key);
                state.visible = false;
                m_CurveEditor.SetCurveState(curve.Key, state);
            }
        }

        void SetCurveVisible(SerializedProperty rawProp, SerializedProperty overrideProp)
        {
            var state = m_CurveEditor.GetCurveState(rawProp);
            state.visible = true;
            state.editable = overrideProp.boolValue;
            m_CurveEditor.SetCurveState(rawProp, state);
        }

        void CurveOverrideToggle(SerializedProperty overrideProp)
        {
            overrideProp.boolValue = GUILayout.Toggle(overrideProp.boolValue, EditorUtilities.GetContent("Override"), EditorStyles.toolbarButton);
        }

        static Material s_MaterialGrid;

        void DoCurvesGUI()
        {
            EditorGUILayout.Space();
            ResetVisibleCurves();

            using (new EditorGUI.DisabledGroupScope(serializedObject.isEditingMultipleObjects))
            {
                int curveEditingId = 0;
                SerializedProperty currentCurveRawProp = null;

                // Top toolbar
                using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
                {
                    curveEditingId = EditorGUILayout.Popup(m_CurrentEditingCurve.intValue, s_Curves, EditorStyles.toolbarPopup, GUILayout.MaxWidth(150f));
                    curveEditingId = Mathf.Clamp(curveEditingId, 0, 7);
                    EditorGUILayout.Space();

                    switch (curveEditingId)
                    {
                        case 0:
                            CurveOverrideToggle(m_MasterCurve.overrideState);
                            SetCurveVisible(m_RawMasterCurve, m_MasterCurve.overrideState);
                            currentCurveRawProp = m_RawMasterCurve;
                            break;
                        case 1:
                            CurveOverrideToggle(m_RedCurve.overrideState);
                            SetCurveVisible(m_RawRedCurve, m_RedCurve.overrideState);
                            currentCurveRawProp = m_RawRedCurve;
                            break;
                        case 2:
                            CurveOverrideToggle(m_GreenCurve.overrideState);
                            SetCurveVisible(m_RawGreenCurve, m_GreenCurve.overrideState);
                            currentCurveRawProp = m_RawGreenCurve;
                            break;
                        case 3:
                            CurveOverrideToggle(m_BlueCurve.overrideState);
                            SetCurveVisible(m_RawBlueCurve, m_BlueCurve.overrideState);
                            currentCurveRawProp = m_RawBlueCurve;
                            break;
                        case 4:
                            CurveOverrideToggle(m_HueVsHueCurve.overrideState);
                            SetCurveVisible(m_RawHueVsHueCurve, m_HueVsHueCurve.overrideState);
                            currentCurveRawProp = m_RawHueVsHueCurve;
                            break;
                        case 5:
                            CurveOverrideToggle(m_HueVsSatCurve.overrideState);
                            SetCurveVisible(m_RawHueVsSatCurve, m_HueVsSatCurve.overrideState);
                            currentCurveRawProp = m_RawHueVsSatCurve;
                            break;
                        case 6:
                            CurveOverrideToggle(m_SatVsSatCurve.overrideState);
                            SetCurveVisible(m_RawSatVsSatCurve, m_SatVsSatCurve.overrideState);
                            currentCurveRawProp = m_RawSatVsSatCurve;
                            break;
                        case 7:
                            CurveOverrideToggle(m_LumVsSatCurve.overrideState);
                            SetCurveVisible(m_RawLumVsSatCurve, m_LumVsSatCurve.overrideState);
                            currentCurveRawProp = m_RawLumVsSatCurve;
                            break;
                    }

                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Reset", EditorStyles.toolbarButton))
                    {
                        switch (curveEditingId)
                        {
                            case 0: m_RawMasterCurve.animationCurveValue = AnimationCurve.Linear(0f, 0f, 1f, 1f);
                                break;
                            case 1: m_RawRedCurve.animationCurveValue = AnimationCurve.Linear(0f, 0f, 1f, 1f);
                                break;
                            case 2: m_RawGreenCurve.animationCurveValue = AnimationCurve.Linear(0f, 0f, 1f, 1f);
                                break;
                            case 3: m_RawBlueCurve.animationCurveValue = AnimationCurve.Linear(0f, 0f, 1f, 1f);
                                break;
                            case 4: m_RawHueVsHueCurve.animationCurveValue = new AnimationCurve();
                                break;
                            case 5: m_RawHueVsSatCurve.animationCurveValue = new AnimationCurve();
                                break;
                            case 6: m_RawSatVsSatCurve.animationCurveValue = new AnimationCurve();
                                break;
                            case 7: m_RawLumVsSatCurve.animationCurveValue = new AnimationCurve();
                                break;
                        }
                    }

                    m_CurrentEditingCurve.intValue = curveEditingId;
                }

                // Curve area
                var settings = m_CurveEditor.settings;
                var rect = GUILayoutUtility.GetAspectRect(2f);
                var innerRect = settings.padding.Remove(rect);

                if (Event.current.type == EventType.Repaint)
                {
                    // Background
                    EditorGUI.DrawRect(rect, new Color(0.15f, 0.15f, 0.15f, 1f));

                    if (curveEditingId == 4 || curveEditingId == 5)
                        DrawBackgroundTexture(innerRect, 0);
                    else if (curveEditingId == 6 || curveEditingId == 7)
                        DrawBackgroundTexture(innerRect, 1);

                    // Bounds
                    Handles.color = Color.white * (GUI.enabled ? 1f : 0.5f);
                    Handles.DrawSolidRectangleWithOutline(innerRect, Color.clear, new Color(0.8f, 0.8f, 0.8f, 0.5f));

                    // Grid setup
                    Handles.color = new Color(1f, 1f, 1f, 0.05f);
                    int hLines = (int)Mathf.Sqrt(innerRect.width);
                    int vLines = (int)(hLines / (innerRect.width / innerRect.height));

                    // Vertical grid
                    int gridOffset = Mathf.FloorToInt(innerRect.width / hLines);
                    int gridPadding = ((int)(innerRect.width) % hLines) / 2;

                    for (int i = 1; i < hLines; i++)
                    {
                        var offset = i * Vector2.right * gridOffset;
                        offset.x += gridPadding;
                        Handles.DrawLine(innerRect.position + offset, new Vector2(innerRect.x, innerRect.yMax - 1) + offset);
                    }

                    // Horizontal grid
                    gridOffset = Mathf.FloorToInt(innerRect.height / vLines);
                    gridPadding = ((int)(innerRect.height) % vLines) / 2;

                    for (int i = 1; i < vLines; i++)
                    {
                        var offset = i * Vector2.up * gridOffset;
                        offset.y += gridPadding;
                        Handles.DrawLine(innerRect.position + offset, new Vector2(innerRect.xMax - 1, innerRect.y) + offset);
                    }
                }

                // Curve editor
                if (m_CurveEditor.OnGUI(rect))
                {
                    Repaint();
                    GUI.changed = true;
                }

                if (Event.current.type == EventType.Repaint)
                {
                    // Borders
                    Handles.color = Color.black;
                    Handles.DrawLine(new Vector2(rect.x, rect.y - 18f), new Vector2(rect.xMax, rect.y - 18f));
                    Handles.DrawLine(new Vector2(rect.x, rect.y - 19f), new Vector2(rect.x, rect.yMax));
                    Handles.DrawLine(new Vector2(rect.x, rect.yMax), new Vector2(rect.xMax, rect.yMax));
                    Handles.DrawLine(new Vector2(rect.xMax, rect.yMax), new Vector2(rect.xMax, rect.y - 18f));

                    bool editable = m_CurveEditor.GetCurveState(currentCurveRawProp).editable;
                    string editableString = editable ? string.Empty : "(Not Overriding)\n";

                    // Selection info
                    var selection = m_CurveEditor.GetSelection();
                    var infoRect = innerRect;
                    infoRect.x += 5f;
                    infoRect.width = 100f;
                    infoRect.height = 30f;

                    if (selection.curve != null && selection.keyframeIndex > -1)
                    {
                        var key = selection.keyframe.Value;
                        GUI.Label(infoRect, string.Format("{0}\n{1}", key.time.ToString("F3"), key.value.ToString("F3")), Styling.preLabel);
                    }
                    else
                    {
                        GUI.Label(infoRect, editableString, Styling.preLabel);
                    }
                }
            }

            EditorGUILayout.Space();
        }

        void DrawBackgroundTexture(Rect rect, int pass)
        {
            if (s_MaterialGrid == null)
                s_MaterialGrid = new Material(Shader.Find("Hidden/PostProcessing/Editor/CurveGrid")) { hideFlags = HideFlags.HideAndDontSave };

            float scale = EditorGUIUtility.pixelsPerPoint;

            var oldRt = RenderTexture.active;
            var rt = RenderTexture.GetTemporary(Mathf.CeilToInt(rect.width * scale), Mathf.CeilToInt(rect.height * scale), 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            s_MaterialGrid.SetFloat("_DisabledState", GUI.enabled ? 1f : 0.5f);
            s_MaterialGrid.SetFloat("_PixelScaling", EditorGUIUtility.pixelsPerPoint);

            Graphics.Blit(null, rt, s_MaterialGrid, pass);
            RenderTexture.active = oldRt;

            GUI.DrawTexture(rect, rt);
            RenderTexture.ReleaseTemporary(rt);
        }
    }
}