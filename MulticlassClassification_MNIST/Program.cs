using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.IO;
using Microsoft.ML.Transforms;
using MulticlassClassification_MNIST.DataStructures;
using System.Linq;
using System.Collections.Generic;

namespace MulticlassClassification_MNIST
{
    class Program
    {
        private static string BaseDatasetsRelativePath = @"../../../Data";
        private static string TrianDataRealtivePath = $"{BaseDatasetsRelativePath}/chars-train.csv";
        private static string TestDataRealtivePath = $"{BaseDatasetsRelativePath}/chars-val.csv";

        private static string TrainDataPath = GetAbsolutePath(TrianDataRealtivePath);
        private static string TestDataPath = GetAbsolutePath(TestDataRealtivePath);

        private static string BaseModelsRelativePath = @"../../../MLModels";
        private static string ModelRelativePath = $@"D:\MachineLearning\Document Layout Analysis\MNIST_Model.zip";

        private static string ModelPath = GetAbsolutePath(ModelRelativePath);

        static void Main(string[] args)
        {
            MLContext mlContext = new MLContext();
            Train(mlContext);
            TestSomePredictions(mlContext);

            Console.WriteLine("Hit any key to finish the app");
            Console.ReadKey();
        }

        public static void Train(MLContext mlContext)
        {
            try
            {
                // STEP 1: Common data loading configuration
                var trainData = mlContext.Data.LoadFromTextFile(path: TrainDataPath,
                        columns: new[]
                        {
                            new TextLoader.Column(nameof(InputData.PixelValues), DataKind.Single, 0, 63),
                            new TextLoader.Column("Number", DataKind.Single, 64)
                        },
                        hasHeader: false,
                        separatorChar: ','
                        );


                var testData = mlContext.Data.LoadFromTextFile(path: TestDataPath,
                        columns: new[]
                        {
                            new TextLoader.Column(nameof(InputData.PixelValues), DataKind.Single, 0, 63),
                            new TextLoader.Column("Number", DataKind.Single, 64)
                        },
                        hasHeader: false,
                        separatorChar: ','
                        );

                // STEP 2: Common data process configuration with pipeline data transformations
                // Use in-memory cache for small/medium datasets to lower training time. Do NOT use it (remove .AppendCacheCheckpoint()) when handling very large datasets.
                var dataProcessPipeline = mlContext.Transforms.Conversion.MapValueToKey("Label", "Number", keyOrdinality: ValueToKeyMappingEstimator.KeyOrdinality.ByValue).
                    Append(mlContext.Transforms.Concatenate("Features", nameof(InputData.PixelValues)).AppendCacheCheckpoint(mlContext));

                // STEP 3: Set the training algorithm, then create and config the modelBuilder
                var trainer = mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy(labelColumnName: "Label", featureColumnName: "Features");
                var trainingPipeline = dataProcessPipeline.Append(trainer).Append(mlContext.Transforms.Conversion.MapKeyToValue("Number", "Label"));

                // STEP 4: Train the model fitting to the DataSet

                Console.WriteLine("=============== Training the model ===============");
                ITransformer trainedModel = trainingPipeline.Fit(trainData);

                Console.WriteLine("===== Evaluating Model's accuracy with Test data =====");
                var predictions = trainedModel.Transform(testData);
                var metrics = mlContext.MulticlassClassification.Evaluate(data: predictions, labelColumnName: "Number", scoreColumnName: "Score");

                //Common.ConsoleHelper.PrintMultiClassClassificationMetrics(trainer.ToString(), metrics);

                mlContext.Model.Save(trainedModel, trainData.Schema, ModelPath);

                Console.WriteLine("The model is saved to {0}", ModelPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                //return null;
            }
        }

        public static string GetAbsolutePath(string relativePath)
        {
            FileInfo _dataRoot = new FileInfo(typeof(Program).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory.FullName;

            string fullPath = Path.Combine(assemblyFolderPath, relativePath);

            return fullPath;
        }

        private static void TestSomePredictions(MLContext mlContext)
        {
            ITransformer trainedModel = mlContext.Model.Load(ModelPath, out var modelInputSchema);

            // Create prediction engine related to the loaded trained model
            var predEngine = mlContext.Model.CreatePredictionEngine<InputData, OutPutData>(trainedModel);

           /* var resultprediction1 = predEngine.Predict(SampleMNISTData.MNIST1);

            Console.WriteLine($"Actual: 1     Predicted probability:       zero:  {resultprediction1.Score[0]:0.####}");
            Console.WriteLine($"                                           One :  {resultprediction1.Score[1]:0.####}");
            Console.WriteLine($"                                           two:   {resultprediction1.Score[2]:0.####}");
            Console.WriteLine($"                                           three: {resultprediction1.Score[3]:0.####}");
            Console.WriteLine($"                                           four:  {resultprediction1.Score[4]:0.####}");
            Console.WriteLine($"                                           five:  {resultprediction1.Score[5]:0.####}");
            Console.WriteLine($"                                           six:   {resultprediction1.Score[6]:0.####}");
            Console.WriteLine($"                                           seven: {resultprediction1.Score[7]:0.####}");
            Console.WriteLine($"                                           eight: {resultprediction1.Score[8]:0.####}");
            Console.WriteLine($"                                           nine:  {resultprediction1.Score[9]:0.####}");
            Console.WriteLine();

            var resultprediction2 = predEngine.Predict(SampleMNISTData.MNIST2);

            Console.WriteLine($"Actual: 7     Predicted probability:       zero:  {resultprediction2.Score[0]:0.####}");
            Console.WriteLine($"                                           One :  {resultprediction2.Score[1]:0.####}");
            Console.WriteLine($"                                           two:   {resultprediction2.Score[2]:0.####}");
            Console.WriteLine($"                                           three: {resultprediction2.Score[3]:0.####}");
            Console.WriteLine($"                                           four:  {resultprediction2.Score[4]:0.####}");
            Console.WriteLine($"                                           five:  {resultprediction2.Score[5]:0.####}");
            Console.WriteLine($"                                           six:   {resultprediction2.Score[6]:0.####}");
            Console.WriteLine($"                                           seven: {resultprediction2.Score[7]:0.####}");
            Console.WriteLine($"                                           eight: {resultprediction2.Score[8]:0.####}");
            Console.WriteLine($"                                           nine:  {resultprediction2.Score[9]:0.####}");*/
            Console.WriteLine();

            var resultprediction1 = predEngine.Predict(SampleMNISTData.MNIST1);
            resultprediction1.Score = resultprediction1.Score.Select(s => (float)Math.Round(s, 3)).ToArray();
            var proba1 = resultprediction1.Score.Max();
            var index1 = resultprediction1.Score.ToList().FindIndex(x => x == proba1);
            Console.WriteLine($"Actual: @     Predicted probability:       " + unicodeChars[index1 + 33] + ":" + proba1 + ")");

            var resultprediction2 = predEngine.Predict(SampleMNISTData.MNIST2);
            resultprediction2.Score = resultprediction2.Score.Select(s => (float)Math.Round(s, 3)).ToArray();
            var proba2 = resultprediction2.Score.Max();
            var index2 = resultprediction2.Score.ToList().FindIndex(x => x == proba2);
            Console.WriteLine("Actual: K     Predicted probability:       " + unicodeChars[index2 + 33] + ":" + proba2 + ")");

            var resultprediction3 = predEngine.Predict(SampleMNISTData.MNIST3);
            resultprediction3.Score = resultprediction3.Score.Select(s => (float)Math.Round(s, 3)).ToArray();
            var proba3 = resultprediction3.Score.Max();
            var index3 = resultprediction3.Score.ToList().FindIndex(x => x == proba3);
            Console.WriteLine($"Actual: z     Predicted probability:       " + unicodeChars[index3 + 33] + ":" + proba3 + ")");

            /*Console.WriteLine($"Actual: 9     Predicted probability:       zero:  {resultprediction3.Score[0]:0.####}");
            Console.WriteLine($"                                           One :  {resultprediction3.Score[1]:0.####}");
            Console.WriteLine($"                                           two:   {resultprediction3.Score[2]:0.####}");
            Console.WriteLine($"                                           three: {resultprediction3.Score[3]:0.####}");
            Console.WriteLine($"                                           four:  {resultprediction3.Score[4]:0.####}");
            Console.WriteLine($"                                           five:  {resultprediction3.Score[5]:0.####}");
            Console.WriteLine($"                                           six:   {resultprediction3.Score[6]:0.####}");
            Console.WriteLine($"                                           seven: {resultprediction3.Score[7]:0.####}");
            Console.WriteLine($"                                           eight: {resultprediction3.Score[8]:0.####}");
            Console.WriteLine($"                                           nine:  {resultprediction3.Score[9]:0.####}");*/
            Console.WriteLine();
        }

        static Dictionary<int, char> unicodeChars = new Dictionary<int, char>()
        {
            { 33, '!' },
            { 34, '"' },
            { 35, '#' },
            { 36, '$' },
            { 37, '%' },
            { 38, '&' },
            { 39, '\'' },
            { 40, '(' },
            { 41, ')' },
            { 42, '*' },
            { 43, '+' },
            { 44, ',' },
            { 45, '-' },
            { 46, '.' },
            { 47, '/' },
            { 48, '0' },
            { 49, '1' },
            { 50, '2' },
            { 51, '3' },
            { 52, '4' },
            { 53, '5' },
            { 54, '6' },
            { 55, '7' },
            { 56, '8' },
            { 57, '9' },
            { 58, ':' },
            { 59, ';' },
            { 60, '<' },
            { 61, '=' },
            { 62, '>' },
            { 63, '?' },
            { 64, '@' },
            { 65, 'A' },
            { 66, 'B' },
            { 67, 'C' },
            { 68, 'D' },
            { 69, 'E' },
            { 70, 'F' },
            { 71, 'G' },
            { 72, 'H' },
            { 73, 'I' },
            { 74, 'J' },
            { 75, 'K' },
            { 76, 'L' },
            { 77, 'M' },
            { 78, 'N' },
            { 79, 'O' },
            { 80, 'P' },
            { 81, 'Q' },
            { 82, 'R' },
            { 83, 'S' },
            { 84, 'T' },
            { 85, 'U' },
            { 86, 'V' },
            { 87, 'W' },
            { 88, 'X' },
            { 89, 'Y' },
            { 90, 'Z' },
            { 91, '[' },
            { 92, '\\' },
            { 93, ']' },
            { 94, '^' },
            { 95, '_' },
            { 96, '`' },
            { 97, 'a' },
            { 98, 'b' },
            { 99, 'c' },
            { 100, 'd' },
            { 101, 'e' },
            { 102, 'f' },
            { 103, 'g' },
            { 104, 'h' },
            { 105, 'i' },
            { 106, 'j' },
            { 107, 'k' },
            { 108, 'l' },
            { 109, 'm' },
            { 110, 'n' },
            { 111, 'o' },
            { 112, 'p' },
            { 113, 'q' },
            { 114, 'r' },
            { 115, 's' },
            { 116, 't' },
            { 117, 'u' },
            { 118, 'v' },
            { 119, 'w' },
            { 120, 'x' },
            { 121, 'y' },
            { 122, 'z' },
            { 123, '{' },
            { 124, '|' },
            { 125, '}' },
            { 126, '~' },
        };
    }
}

