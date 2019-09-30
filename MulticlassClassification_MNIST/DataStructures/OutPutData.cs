using Microsoft.ML.Data;

namespace MulticlassClassification_MNIST.DataStructures
{
    class OutPutData
    {
        [ColumnName("Score")]
        public float[] Score;
    }
}
