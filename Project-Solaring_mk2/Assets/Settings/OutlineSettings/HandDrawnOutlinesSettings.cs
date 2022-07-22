using UnityEngine;

namespace HandDrawnOutlines
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Data", menuName = "HandDrawnOutlines/HandDrawnOutlinesSettings", order = 1)]
    public class HandDrawnOutlinesSettings : ScriptableObject
    {
        [Header("Contour")]
        [SerializeField, ColorUsage(true, true)] public Color _Color = Color.green;
        [SerializeField, Range(1, 3)] public int NumberOfContours = 1;
        [SerializeField, Range(0, 80)] public float Frequency = 40;
        [SerializeField, Range(-0.01f, 0.01f)] public float Amplitude = 0.003f;

        [SerializeField] public bool UseNoise = false;
        [SerializeField] public Texture2D NoiseTex;
        [SerializeField, Range(0, 0.02f)] public float NoiseAmount = 0;
        [SerializeField] public float NoiseTilling = 1;

        [Header("Contour Time Impact")]
        [SerializeField] public float TimeMultiplier = 1;
        [SerializeField] public bool Synchronize = true;

        [Header("Alpha")]
        [SerializeField] public bool UseNoiseAlpha = false;
        [SerializeField] public Texture2D NoiseAlpha;
        [SerializeField] public float NoiseScaleAlpha = 1;
        [SerializeField, Range(0, 10)] public float MultiplierAlpha = 0.015f;

        [Header("Alpha Time Impact")]
        [SerializeField] public bool UseFlow = false;
        [SerializeField, Range(-20, 20)] public float SpeedXAlpha = 0;
        [SerializeField, Range(-20, 20)] public float SpeedYAlpha = 0;

        [SerializeField] public bool UseRandomization = false;
        [SerializeField] public float TimeMultiplierAlpha = 1;

        [Header("Edges")]
        [SerializeField, Range(0, 5)] public float Width = 2;

        [SerializeField, Range(0.2f, 40f)] public float DepthThreshold = 10;
        [SerializeField, Range(0f, 3f)] public float NormalThreshold = 1;

        [Header("Materials")]
        [HideInInspector] public Material EdgeDetectionMaterial;
        [HideInInspector] public Material OutlineMaterial;

        public void OnValidate()
        {
            if (!EdgeDetectionMaterial) EdgeDetectionMaterial = new Material(Shader.Find(GetEdgeShaderName()));
            if (!OutlineMaterial) OutlineMaterial = new Material(Shader.Find(GetOutlineShaderName()));

            UpdateMaterials();
        }

        public string GetEdgeShaderName()
        {
            return "HandDrawn/EdgeDetecion";
        }

        public void SetupEdgeShaderProperties()
        {
            EdgeDetectionMaterial.SetFloat("_Thickness", Width);
            EdgeDetectionMaterial.SetFloat("_DepthThreshold", DepthThreshold);
            EdgeDetectionMaterial.SetFloat("_NormalThreshold", NormalThreshold);
        }

        public string GetOutlineShaderName()
        {
            return "HandDrawn/PencilContour";
        }

        public void SetupOutlineShaderProperties()
        {
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
            OutlineMaterial.SetFloat("_Synchronize",Synchronize ? 1 : 0);

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

        public void EnableIdsKeywords(int id)
        {
            EdgeDetectionMaterial = new Material(Shader.Find(GetEdgeShaderName()));
            OutlineMaterial = new Material(Shader.Find(GetOutlineShaderName()));

            DisableKeywords(EdgeDetectionMaterial);
            DisableKeywords(OutlineMaterial);

            EdgeDetectionMaterial.EnableKeyword("_Tex" + id.ToString());
            OutlineMaterial.EnableKeyword("_Tex" + id.ToString());

            SetupEdgeShaderProperties();
            SetupOutlineShaderProperties();
        }

        private void DisableKeywords(Material mat)
        {
            foreach(var key in mat.shaderKeywords)
            {
                mat.DisableKeyword(key);
            }
        }

        public void UpdateMaterials()
        {
            SetupEdgeShaderProperties();
            SetupOutlineShaderProperties();
        }
    }
}