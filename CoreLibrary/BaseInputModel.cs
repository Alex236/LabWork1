using Newtonsoft.Json;


namespace CoreLibrary
{
    public class BaseInputModel
    {
        public BaseInputModel() { }
        
        public BaseInputModel(int rowBegin, int rowEnd, int columnBegin, int columnEndEnd, string matrixLeftLeft, string matrixRight)
        {
            RowBegin = rowBegin;
            RowEnd = rowEnd;
            ColumnBegin = columnBegin;
            ColumnEnd = columnEndEnd;
            MatrixLeftName = matrixLeftLeft;
            MatrixRightName = matrixRight;
        }
        
        [JsonProperty("rowBegin")]
        public int RowBegin { get; set; }
        [JsonProperty("rowEnd")]
        public int RowEnd { get; set; }
        [JsonProperty("columnBegin")]
        public int ColumnBegin { get; set; }
        [JsonProperty("columnEnd")]
        public int ColumnEnd { get; set; }
        [JsonProperty("matrixLeft")]
        public string MatrixLeftName { get; set; }
        [JsonProperty("matrixRight")]
        public string MatrixRightName { get; set; }
    }
}