using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace solar_a
{
    [CreateAssetMenu(fileName = "New AudioList", menuName = "SolarPrject/Audio List")]
    public class DataAudio : ScriptableObject
    {
        [SerializeField]
        private string listName;
        [SerializeField]
        public List<listAudio> listAudios;
    }

    [System.Serializable]
    public class listAudio
    {
        [SerializeField]
        public AudioClip clip;
        [SerializeField]
        public string label;


    }
}
