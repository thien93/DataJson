using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataJson.Models
{
    public class CustomerViewModel
    {
        public List<SAP> saps { get; set; }
        public List<DataTable> dataTables { get; set; }
        public int MaxLocation { get; set; }
        public int MinLocation { get; set; }
        public string Name { get; set; }
        public List<StatusData> statusDatas { get; set; }
    }

    public class SAP
    {
        public string ObjectID { get; set; }
        public string ETag { get; set; }
        public string RecordNumber { get; set; }
        public string ExternalKey { get; set; }
        public string Ngybngiaothct_KUT { get; set; }
        public string duAnIDName_SDK { get; set; }
        public string khuBlockDuAn_SDK { get; set; }
        public string FloorTngcSnphm_KUT { get; set; }
        public string Description { get; set; }
        public int StatusTrngthiGiaodch_KUT { get; set; }
        public int LocationVtr_KUT { get; set; }
        public int FloorTnghm_KUT { get; set; }
        public int FloorMar { get; set; }
        public List<int> ListFloor { get; set; }
    }
    public class DataTable
    {
        public int Floor { get; set; }
        public List<DataHouse> dataHouses { get; set; }
    }
    public class DataHouse
    {
        public string Detail { get; set; }
        public string Status { get; set; }
        public int MarFloor { get; set; }
        public bool IsMer { get; set; }
    }

    public class StatusData
    {
        public string Name { get; set; }
        public string nameClass { get; set; }
        public int StatusCode { get; set; }
        public int Value { get; set; }
    }
}
