using Microsoft.ML.Data;

namespace MulticlassClassification_MNIST.DataStructures
{
    class InputData
    {
        [ColumnName("PixelValues")]
        [VectorType(64)]
        public float[] PixelValues;

        [LoadColumn(64)]
        public float Number;
    }
}
