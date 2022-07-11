using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandDrawnOutlines
{
    public class RuntimeHandDrawnOutlineHandler : MonoBehaviour
    {
        [Tooltip("Set HandDrawnOutlinesSettings scriptable object reference")]
        [SerializeField] public HandDrawnOutlinesSettings settings;

        [Header("Contour")]
        [SerializeField, ColorUsage(true, true)] public Color _Color;
        [SerializeField, Range(1, 3)] public int NumberOfContours;
        [SerializeField, Range(0, 80)] public float Frequency;
        [SerializeField, Range(0, 0.01f)] public float Amplitude;

        [SerializeField] public bool UseNoise;
        [SerializeField] public Texture2D NoiseTex;
        [SerializeField, Range(0, 0.02f)] public float NoiseAmount;
        [SerializeField] public float NoiseTilling;

        [Header("Contour Time Impact")]
        [SerializeField] public float TimeMultiplier;
        [SerializeField] public bool Synchronize;

        [Header("Alpha")]
        [SerializeField] public bool UseNoiseAlpha;
        [SerializeField] public Texture2D NoiseAlpha;
        [SerializeField] public float NoiseScaleAlpha;
        [SerializeField, Range(0, 5)] public float MultiplierAlpha ;

        [Header("Alpha Time Impact")]
        [SerializeField] public bool UseFlow;
        [SerializeField, Range(-20, 20)] public float SpeedXAlpha;
        [SerializeField, Range(-20, 20)] public float SpeedYAlpha;

        [SerializeField] public bool UseRandomization;
        [SerializeField] public float TimeMultiplierAlpha;

        [Header("Edges")]
        [SerializeField, Range(0, 5)] public float Width;

        [SerializeField, Range(0.2f, 40f)] public float DepthThreshold;
        [SerializeField, Range(0f, 3f)] public float NormalThreshold;

        [HideInInspector] public Material EdgeDetectionMaterial;
        [HideInInspector] public Material OutlineMaterial;

        /// <summary>
        /// Use this function to update the dynamic material properties of HandDrawnOutlinesSettings with overwritten properties of RuntimeHandDrawnOutlineHandler object.
        /// </summary>
        public void UpdateMaterials()
        {
            if (!settings) return;

            EdgeDetectionMaterial = settings.EdgeDetectionMaterial;
            OutlineMaterial = settings.OutlineMaterial;

            EdgeDetectionMaterial.SetFloat("_Thickness", Width);
            EdgeDetectionMaterial.SetFloat("_DepthThreshold", DepthThreshold);
            EdgeDetectionMaterial.SetFloat("_NormalThreshold", NormalThreshold);

            if (!UseNoise) NoiseAmount = 0;

            OutlineMaterial.DisableKeyword("_Contour1");
            OutlineMaterial.DisableKeyword("_Contour2");
            OutlineMaterial.DisableKeyword("_Contour3");

            OutlineMaterial.EnableKeyword("_Contour" + NumberOfContours.ToString());

            //Contour
            OutlineMaterial.SetColor("_Color", _Color);
            OutlineMaterial.SetFloat("_ErrorPeriod", Frequency);
            OutlineMaterial.SetFloat("_ErrorRange", Amplitude);
            OutlineMaterial.SetFloat("_NoiseAmount", NoiseAmount);
            OutlineMaterial.SetFloat("_NoiseTilling", NoiseTilling);
            OutlineMaterial.SetTexture("_NoiseTex", NoiseTex);

            //Contour time
            OutlineMaterial.SetFloat("_TimeMultiplier", TimeMultiplier);
            OutlineMaterial.SetFloat("_Synchronize", Synchronize ? 1 : 0);

            //Alpha
            OutlineMaterial.SetFloat("_UseNoiseAlpha", UseNoiseAlpha ? 1 : 0);
            OutlineMaterial.SetTexture("_NoiseAlpha", NoiseAlpha);
            OutlineMaterial.SetFloat("_NoiseScaleAlpha", NoiseScaleAlpha);
            OutlineMaterial.SetFloat("_MultiplierAlpha", MultiplierAlpha);

            //Alpha time
            OutlineMaterial.SetFloat("_UseFlow", UseFlow ? 1 : 0);
            OutlineMaterial.SetFloat("_SpeedXAlpha", SpeedXAlpha);
            OutlineMaterial.SetFloat("_SpeedYAlpha", SpeedYAlpha);

            OutlineMaterial.SetFloat("_UseRandomization", UseRandomization ? 1 : 0);
            OutlineMaterial.SetFloat("_TimeMultiplierAlpha", TimeMultiplierAlpha);
        }

        /// <summary>
        /// This function overrides RuntimeHandDrawnOutlineHandler object properties with the current HandDrawnOutlinesSettings scriptable object.
        /// </summary>
        public void OverridePropertiesWithSettingsObject()
        {
            if (!settings) return;

            _Color = settings._Color;
            NumberOfContours = settings.NumberOfContours;
            Frequency = settings.Frequency;
            Amplitude = settings.Amplitude;

            UseNoise = settings.UseNoise;
            NoiseTex = settings.NoiseTex;
            NoiseAmount = settings.NoiseAmount;
            NoiseTilling = settings.NoiseTilling;

            TimeMultiplier = settings.TimeMultiplier;
            Synchronize = settings.Synchronize;

            UseNoiseAlpha = settings.UseNoiseAlpha;
            NoiseAlpha = settings.NoiseAlpha;
            NoiseScaleAlpha = settings.NoiseScaleAlpha;
            MultiplierAlpha = settings.MultiplierAlpha;

            UseFlow = settings.UseFlow;
            SpeedXAlpha = settings.SpeedXAlpha;
            SpeedYAlpha = settings.SpeedYAlpha;

            UseRandomization = settings.UseRandomization;
            TimeMultiplierAlpha = settings.TimeMultiplierAlpha;

            Width = settings.Width;

            DepthThreshold = settings.DepthThreshold;
            NormalThreshold = settings.NormalThreshold;
        }


        public void Start()
        {
            //We are overrding handler properties values with settings object values at start.
            OverridePropertiesWithSettingsObject();
        }

        public void Update()
        {
            //We are calling this function to update settings ( HandDrawnOutlinesSettings scriptable object) material properties.
            UpdateMaterials();
        }
    }
}