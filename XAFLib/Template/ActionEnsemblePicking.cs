using System;

namespace Triggerless.XAFLib
{
    [Serializable]

    public enum ActionEnsemblePicking {
        ActionEnsemblePickingOncePerIteration,
        ActionEnsemblePickingOnceForAllIterations,
        ActionEnsemblePickingCycle
    }

}
