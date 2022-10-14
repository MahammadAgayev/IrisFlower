using System;
using System.Collections.Generic;
using System.Linq;

namespace IrisFlower
{
    public class Kmeans : IKmeans
    {
        private readonly List<IrisData> _dataset;
        private readonly IrisData[] _centroids;

        /*
         * In kmeans we are storing two data
         * 
         * _dataset is original data from text
         * _centroids is a centre of each group which computer will calculate
         * 
         * I suppose explaining the k means alghroithm here generically will be good.
         * It starts with choosing random k data from Dataset. In our case we are choosing three. 
         * 
         * InitializeCentroids() method basically doing this. 
         * Here code a little more complicated, because i adjusted this random selection a little.
         * The reason is this codes belongs to someone important and I couldn't do anyhting but perfect.
         * 
         * Let's name these selsected points as centroids.
         * 
         * After that we going through all data and assinging each data to one of centroinds. We rae assigning data to one of centroids which is closets by ditsance
         * 
         * The computer takes datas which belongs to first centroid. Takes average of each element SepalLength, SepalWidth, PetalLength, PetalWidth
         * The computer takes datas which belongs to second centroid. Takes average of each element SepalLength, SepalWidth, PetalLength, PetalWidth
         * The computer takes datas which belongs to third centroid. Takes average of each element SepalLength, SepalWidth, PetalLength, PetalWidth
         * 
         * Now again we reassinging datas to the closest centroid. After doing this two or three times our we got our best possible centroid grouped values
         * 
         * Let's dive into code
         */
        public Kmeans(List<IrisData> data, int k)
        {
            _dataset = data;
            _centroids = new IrisData[k];
        }

        public event EventHandler<IrisDataEventArgs> NewClustersSelected;

        public List<IrisData> Predict(int trainingCount)
        {
            this.InitializeCentroids();

            for (int i = 0; i < trainingCount; i++)
            {
                NewClustersSelected?.Invoke(this, new IrisDataEventArgs
                {
                    Centroids = _centroids.ToList(),
                    IrisDatas = _dataset
                });

                this.DetermineClusters();
                this.RefreshCentroids();
            }

            return _dataset;
        }


        //Assinging random values to centroid
        private void InitializeCentroids()
        {
            //random initializer
            //for (int i = 0; i < _centroids.Length; i++)
            //{
            //    var randIndex = new Random().Next() % _dataset.Count;
            //    var randIris = _dataset[randIndex];

            //    _centroids[i] = new IrisData
            //    {
            //        SepalLength = randIris.SepalLength,
            //        SepalWidth = randIris.SepalWidth,
            //        PetalLength = randIris.PetalLength,
            //        PetalWidth = randIris.PetalWidth,
            //        Type = randIris.Type,
            //        ClusterId = i + 1,
            //    };

            //    Thread.Sleep(1000);
            //}

            //kpp initializer
            var randIndex = new Random().Next(0, _dataset.Count);
            var randIris = _dataset[randIndex];

            _centroids[0] = new IrisData
            {
                SepalLength = randIris.SepalLength,
                SepalWidth = randIris.SepalWidth,
                PetalLength = randIris.PetalLength,
                PetalWidth = randIris.PetalWidth,
                Type = randIris.Type,
                ClusterId = 1
            };


            for (int i = 1; i < _centroids.Length; i++)
            {
                var further = _dataset[0];
                var maxDistance = this.Distance(further, this.GetNearestCentroid(_dataset[0]));

                for (int j = 1; j < _dataset.Count; j++)
                {
                    var nearest = this.GetNearestCentroid(_dataset[i]);

                    var distance = this.Distance(_dataset[i], nearest);

                    if (distance > maxDistance)
                        further = _dataset[i];

                }

                _centroids[i] = new IrisData
                {
                    SepalLength = further.SepalLength,
                    SepalWidth = further.SepalWidth,
                    PetalLength = further.PetalLength,
                    PetalWidth = further.PetalWidth,
                    Type = further.Type,
                    ClusterId = i + 1
                };
            }
        }

        //This assings closest centroid to each data on one call
        private void DetermineClusters()
        {
            for (int i = 0; i < _dataset.Count; i++)
            {
                var nearest = this.GetNearestCentroid(_dataset[i]);
                _dataset[i].ClusterId = nearest.ClusterId;
            }
        }

        //this method calculates average new data for each centroid 
        private void RefreshCentroids()
        {
            for (int i = 0; i < _centroids.Length; i++)
            {
                var cluster = _dataset.Where(x => x.ClusterId == _centroids[i].ClusterId);

                Console.WriteLine($"ClusterId: {_centroids[i].ClusterId}, Count: {cluster.Count()}");

                _centroids[i].SepalLength = cluster.Sum(x => x.SepalLength) / cluster.Count();
                _centroids[i].SepalWidth = cluster.Sum(x => x.SepalWidth) / cluster.Count();

                _centroids[i].PetalLength = cluster.Sum(x => x.PetalLength) / cluster.Count();
                _centroids[i].PetalWidth = cluster.Sum(x => x.PetalWidth) / cluster.Count();
            }
        }

        //this method calculated distance between each data point
        private double Distance(IrisData a, IrisData b)
        {
            double result = ((a.PetalLength - b.PetalLength) * (a.PetalLength - b.PetalLength))
                + ((a.PetalWidth - b.PetalWidth) * (a.PetalWidth - b.PetalWidth))
                + ((a.SepalLength - b.SepalLength) * (a.SepalLength - b.SepalLength))
                + ((a.SepalWidth - b.SepalWidth) * (a.SepalWidth - b.SepalWidth));

            return Math.Sqrt(result);
        }

        //this method looks for closest centroid for data point
        //alghorithm here is pretty simple if you know find min in array alghorithm
        private IrisData GetNearestCentroid(IrisData point)
        {
            var nearest = _centroids[0];
            var nearestDistance = this.Distance(point, nearest);

            for (int j = 1; j < _centroids.Length; j++)
            {
                if (_centroids[j] == null)
                    continue;

                var distance = this.Distance(point, _centroids[j]);

                if (distance < nearestDistance)
                    nearest = _centroids[j];
            }

            return nearest;
        }
    }
}
