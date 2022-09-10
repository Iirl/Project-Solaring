using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace solar_a
{
    /// <summary>
    /// 使用音效聲道撥放音效。
    /// </summary>
    public class EffectPlayer : AudioSystem
    {
        protected override void SoundPlayer()
        {
            mds.OneShotEffect(dataAdo.listAudios[playNumber].clip);
        }
        protected override IEnumerator SoundCheck()
        {
            bool runtime = true;
            while (runtime)
            {
                yield return null;
                runtime = mds.EffectIsPlaying;
            }
        }
    }
}
