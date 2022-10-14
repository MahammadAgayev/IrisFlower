using System;
using System.Collections.Generic;

namespace IrisFlower
{
    public class IrisDataEventArgs : EventArgs
    {
        public List<IrisData> IrisDatas { get; set; }
        public List<IrisData> Centroids { get; set; }
    }
}
