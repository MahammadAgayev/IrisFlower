using System;
using System.Collections.Generic;

namespace IrisFlower
{
    public interface IKmeans
    {
        List<IrisData> Predict(int trainingCount);

        event EventHandler<IrisDataEventArgs> NewClustersSelected;
    }
}
