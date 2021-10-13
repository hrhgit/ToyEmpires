using System;
using UnityEditor;
using UnityEditor.Rendering.Universal.ShaderGUI;
using UnityEngine;


public class CustomSpriteShaderGUI : BaseShaderGUI
    {
        private LitGUI.LitProperties litProperties;
        private LitDetailGUI.LitProperties litDetailProperties;
        private SavedBool m_DetailInputsFoldout;
        
        private bool             m_AddtionalFoldout  = true;
        private MaterialProperty sssColorProp        = null;
        private MaterialProperty outlineColorProp    = null;
        private MaterialProperty outlineWidthProp    = null;
        private MaterialProperty hProp               = null;
        private MaterialProperty sProp               = null;
        private MaterialProperty vProp               = null;
        private MaterialProperty lightIntensityProp = null;

        public override void OnOpenGUI(Material material, MaterialEditor materialEditor)
        {
            base.OnOpenGUI(material, materialEditor);
            m_DetailInputsFoldout = new SavedBool($"{headerStateKey}.DetailInputsFoldout", true);
        }

        public override void DrawAdditionalFoldouts(Material material)
        {
            // m_DetailInputsFoldout.value = EditorGUILayout.BeginFoldoutHeaderGroup(m_DetailInputsFoldout.value, LitDetailGUI.Styles.detailInputs);
            // if (m_DetailInputsFoldout.value)
            // {
            //     LitDetailGUI.DoDetailArea(litDetailProperties, materialEditor);
            //     EditorGUILayout.Space();
            // }
            //
            // EditorGUILayout.EndFoldoutHeaderGroup();
            // if needed
            EditorGUI.BeginChangeCheck();

            m_AddtionalFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(m_AddtionalFoldout, "Addtional");

            if (m_AddtionalFoldout)
            {
                materialEditor.ColorProperty(sssColorProp,     "SSS Color");
                materialEditor.ColorProperty(outlineColorProp, "Outline Color");
                materialEditor.RangeProperty(outlineWidthProp, "Outline Width");
                materialEditor.RangeProperty(hProp, "H");
                materialEditor.RangeProperty(sProp, "S");
                materialEditor.RangeProperty(vProp, "V");
                materialEditor.RangeProperty(lightIntensityProp, "Light Intensity");

                // materialEditor.TextureProperty(customTextureValueProp, "Custom Texture Value");
            }
            
            EditorGUILayout.EndFoldoutHeaderGroup();

            // if needed
            if (EditorGUI.EndChangeCheck())
            {
            }

            base.DrawAdditionalFoldouts(material);

        }

        // collect properties from the material properties
        public override void FindProperties(MaterialProperty[] properties)
        {
            base.FindProperties(properties);
            baseMapProp         = FindProperty("_MainTex",  properties, false);
            sssColorProp        = FindProperty("_SSSColor", properties);
            outlineWidthProp    = FindProperty("_OutlineWidth", properties);
            outlineColorProp    = FindProperty("_OutlineColor", properties);
            hProp    = FindProperty("_H", properties);
            sProp    = FindProperty("_S", properties);
            vProp    = FindProperty("_V", properties);
            lightIntensityProp   = FindProperty("_LightIntensity", properties);
            litProperties       = new LitGUI.LitProperties(properties);
            litDetailProperties = new LitDetailGUI.LitProperties(properties);
        }

        // material changed check
        public override void MaterialChanged(Material material)
        {
            if (material == null)
                throw new ArgumentNullException("material");

            SetMaterialKeywords(material, LitGUI.SetMaterialKeywords, LitDetailGUI.SetMaterialKeywords);
        }

        // material main surface options
        public override void DrawSurfaceOptions(Material material)
        {
            if (material == null)
                throw new ArgumentNullException("material");

            // Use default labelWidth
            EditorGUIUtility.labelWidth = 0f;

            // Detect any changes to the material
            EditorGUI.BeginChangeCheck();
            if (litProperties.workflowMode != null)
            {
                DoPopup(LitGUI.Styles.workflowModeText, litProperties.workflowMode, Enum.GetNames(typeof(LitGUI.WorkflowMode)));
            }
            if (EditorGUI.EndChangeCheck())
            {
                foreach (var obj in blendModeProp.targets)
                    MaterialChanged((Material)obj);
            }
            base.DrawSurfaceOptions(material);
        }

        // material main surface inputs
        public override void DrawSurfaceInputs(Material material)
        {
            base.DrawSurfaceInputs(material);
            LitGUI.Inputs(litProperties, materialEditor, material);
            DrawEmissionProperties(material, true);
            DrawTileOffset(materialEditor, baseMapProp);
        }

        // material main advanced options
        public override void DrawAdvancedOptions(Material material)
        {
            if (litProperties.reflections != null && litProperties.highlights != null)
            {
                EditorGUI.BeginChangeCheck();
                materialEditor.ShaderProperty(litProperties.highlights, LitGUI.Styles.highlightsText);
                materialEditor.ShaderProperty(litProperties.reflections, LitGUI.Styles.reflectionsText);
                if(EditorGUI.EndChangeCheck())
                {
                    MaterialChanged(material);
                }
            }

            base.DrawAdvancedOptions(material);
        }
        
        

        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            if (material == null)
                throw new ArgumentNullException("material");

            // _Emission property is lost after assigning Standard shader to the material
            // thus transfer it before assigning the new shader
            if (material.HasProperty("_Emission"))
            {
                material.SetColor("_EmissionColor", material.GetColor("_Emission"));
            }

            base.AssignNewShaderToMaterial(material, oldShader, newShader);

            if (oldShader == null || !oldShader.name.Contains("Legacy Shaders/"))
            {
                SetupMaterialBlendMode(material);
                return;
            }

            SurfaceType surfaceType = SurfaceType.Opaque;
            BlendMode blendMode = BlendMode.Alpha;
            if (oldShader.name.Contains("/Transparent/Cutout/"))
            {
                surfaceType = SurfaceType.Opaque;
                material.SetFloat("_AlphaClip", 1);
            }
            else if (oldShader.name.Contains("/Transparent/"))
            {
                // NOTE: legacy shaders did not provide physically based transparency
                // therefore Fade mode
                surfaceType = SurfaceType.Transparent;
                blendMode = BlendMode.Alpha;
            }
            material.SetFloat("_Surface", (float)surfaceType);
            material.SetFloat("_Blend", (float)blendMode);

            if (oldShader.name.Equals("Standard (Specular setup)"))
            {
                material.SetFloat("_WorkflowMode", (float)LitGUI.WorkflowMode.Specular);
                Texture texture = material.GetTexture("_SpecGlossMap");
                if (texture != null)
                    material.SetTexture("_MetallicSpecGlossMap", texture);
            }
            else
            {
                material.SetFloat("_WorkflowMode", (float)LitGUI.WorkflowMode.Metallic);
                Texture texture = material.GetTexture("_MetallicGlossMap");
                if (texture != null)
                    material.SetTexture("_MetallicSpecGlossMap", texture);
            }

            MaterialChanged(material);
        }
    }
