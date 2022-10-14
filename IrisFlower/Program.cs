using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace IrisFlower
{
    class IrisVisualizer : Form
    {
        private List<IrisData> _irisDatas;
        private List<IrisData> _centroids;


        private const int _size = 10;

        public IrisVisualizer() : base()
        {
            this.TopMost = true;
            this.DoubleBuffered = true;
            this.ShowInTaskbar = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Normal;
            this.BackColor = Color.Purple;
            this.TransparencyKey = Color.Purple;

            this.BackColor = Color.White;

            this.Size = new Size(1000, 600);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //e.Graphics.DrawRectangle(Pens.Black, 0, 0, 200, 200);

            //e.Graphics.FillRectangle(Brushes.Red, 0, 0, 200, 200);

            //this.Invalidate(); //cause repaint

            if (_irisDatas is null || _irisDatas.Count == 0)
                return;

            if (_centroids is null || _centroids.Count == 0)
                return;

            foreach (var item in _irisDatas)
            {
                if (item.SepalLength == double.NaN || item.SepalWidth == double.NaN)
                    continue;

                int x = (int)(item.SepalLength * 10) * _size;
                int y = (int)(item.SepalWidth * 10) * _size;

                Pen p = null;

                switch (item.ClusterId)
                {
                    case 1:
                        p = Pens.Red;
                        break;
                    case 2:
                        p = Pens.Green;
                        break;
                    case 3:
                        p = Pens.Blue;
                        break;
                }

                e.Graphics.DrawEllipse(p, new Rectangle(x, y, _size, _size));
            }

            foreach (var item in _centroids)
            {
                if (item.SepalLength == double.NaN || item.SepalWidth == double.NaN)
                    continue;

                Brush b = null;

                switch (item.ClusterId)
                {
                    case 1:
                        b = Brushes.Red;
                        break;
                    case 2:
                        b = Brushes.Green;
                        break;
                    case 3:
                        b = Brushes.Blue;
                        break;
                }

                int x = (int)(item.SepalLength * 10) * _size;
                int y = (int)(item.SepalWidth * 10) * _size;
                e.Graphics.FillEllipse(b, new Rectangle(x, y, _size, _size));
            }
        }

        public void RefreshScreen(List<IrisData> irisDatas, List<IrisData> centroids)
        {
            _irisDatas = irisDatas;
            _centroids = centroids;
            this.Invalidate();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            /*
             * Iris flower mamed after the Greek goddess of the rainbow). 
             * 
             * It has three different types. Setosa, Versicolor, Virginica
             * They differ with four basic leaf sizes -> SepalLength, SepalWidth, PetalLength, PetalWidth
             *
             * What we are doing here technically called unsupervised learning. In our daily we are always grouping things. 
             * We never asked ourselves how we are able to differentiate two flower only by looking. 
             * Actually we are collection information with our eyes processing them in our brain. Simple but effcetive. 
             * 
             * Now, what we doing here is to make computer learn by given data to group flowers. Before diving into into code,
             * let's understand what magic we are resolving here. 
             * There is beautifull lady, you want to give her a beautifull flower. You asked for Iris Setosa but all 150 flowers mixed up, and it is hard to to get 50 setosa out of them.
             * We are trying to help this guy with computer. How computer can divide these mixed flowers to 3 gorup. This is where k means alghorithm kicks in.
             * No we got leaf lengths of each flower and created a file named as iris_data.txt,
             * 
             * Let's see if computer will able to group setosas, versicolors and virginicas
             * 
             * The given file iris_data.txt contains below informations
             * SepalLength, SepalWidth, PetalLength, PetlaWidth, TypeName
             */

            //we read data from file to object
            var data = ImportFromFile("iris_data.txt");

            IrisVisualizer visualizer = new IrisVisualizer();

            var th = new Thread((t) =>
            {
                //initiate alghorithm, we give and group count. We have three group which are setosas, versicolors and virginicas.
                IKmeans kmeans = new Kmeans(data, 3);

                kmeans.NewClustersSelected += (o, e) =>
                {
                    Thread.Sleep(2000);
                    visualizer.RefreshScreen(e.IrisDatas, e.Centroids);
                };

                //the 3 is training count for kmeans prediction alghorithm
                var outData = kmeans.Predict(10);

                //write result to file
                ExportToFile(outData, "outfile.csv");
            });

            th.Start();

            Application.Run(visualizer);
        }

        static List<IrisData> ImportFromFile(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                List<IrisData> datas = new List<IrisData>();

                while (reader.EndOfStream == false)
                {
                    string[] parts = reader.ReadLine().Split(',');

                    datas.Add(new IrisData
                    {
                        SepalLength = double.Parse(parts[0]),
                        SepalWidth = double.Parse(parts[1]),
                        PetalLength = double.Parse(parts[2]),
                        PetalWidth = double.Parse(parts[3]),
                        Type = parts[4]
                    });
                }

                return datas;
            }
        }

        static void ExportToFile(List<IrisData> datas, string filename)
        {
            using (var writeStream = new StreamWriter(File.OpenWrite(filename)))
            {
                foreach (var item in datas)
                {
                    writeStream.WriteLine(string.Join(",", new string[]
                    {
                        item.SepalLength.ToString(),
                        item.SepalWidth.ToString(),
                        item.PetalLength.ToString(),
                        item.PetalWidth.ToString(),
                        item.Type,
                        item.ClusterId.ToString()
                    }));
                }
            }
        }
    }
}
