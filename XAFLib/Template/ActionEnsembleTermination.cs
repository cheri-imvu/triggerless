using System;

namespace Triggerless.XAFLib
{
    [Serializable]

    public enum ActionEnsembleTermination
    {
        NotUsed = 0,
        ActionEnsembleTerminationAllOneShotEffectsEnd,
        ActionEnsembleTerminationAnyOneShotEffectEnds
    }

}
