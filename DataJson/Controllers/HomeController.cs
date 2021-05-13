using DataJson.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DataJson.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GetDataJson(string block)
        {
            CustomerViewModel model = new CustomerViewModel();
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add(HttpRequestHeader.Cookie, "SAP_SESSIONID_LQU_047=neOFM3oWTtY6uOKMEBMygv-_PeGz6xHrvooAFj6rN8M%3d");
                string url = "https://my352905.crm.ondemand.com/sap/c4c/odata/cust/v1/productdata_powerbi_api/MaterialCollection?$format=json&$filter=duAnIDName_SDK%20eq%20%27Q7%20BOULEVARD%27&fbclid=IwAR0Ci7FsAHHQKUQcZFZIGU65oMaOUVVuL-NfucAU99l-Iuls0wyZJ_xkHKk&sap-client=047&sap-language=EN";
                string jsonTokenKey = wc.DownloadString(url);
                try
                {
                    dynamic JsontokenObj = JsonConvert.DeserializeObject(jsonTokenKey);
                    List<SAP> sAPs = new List<SAP>();
                    for (int i = 0; i < JsontokenObj.d.results.Count; i++)
                    {
                        SAP sAP = new SAP();
                        sAP.ObjectID = JsontokenObj.d.results[i].ObjectID.ToString();
                        sAP.ETag = JsontokenObj.d.results[i].ETag.ToString();
                        sAP.RecordNumber = JsontokenObj.d.results[i].RecordNumber.ToString();
                        sAP.ExternalKey = JsontokenObj.d.results[i].ExternalKey.ToString();
                        sAP.Ngybngiaothct_KUT = JsontokenObj.d.results[i].Ngybngiaothct_KUT.ToString();
                        sAP.duAnIDName_SDK = JsontokenObj.d.results[i].duAnIDName_SDK.ToString();
                        sAP.khuBlockDuAn_SDK = JsontokenObj.d.results[i].khuBlockDuAn_SDK.ToString();
                        sAP.FloorTngcSnphm_KUT = JsontokenObj.d.results[i].FloorTngcSnphm_KUT.ToString();
                        sAP.Description = JsontokenObj.d.results[i].Description.ToString();

                        sAP.StatusTrngthiGiaodch_KUT = int.Parse(JsontokenObj.d.results[i].StatusTrngthiGiaodch_KUT.ToString());
                        sAP.LocationVtr_KUT = int.Parse(JsontokenObj.d.results[i].LocationVtr_KUT.ToString());
                        var stringFoll = JsontokenObj.d.results[i].FloorTnghm_KUT.ToString().Replace("Tầng ", "").Split("+");
                        sAP.FloorTnghm_KUT = int.Parse(stringFoll[stringFoll.Length - 1]);
                        sAP.FloorMar = stringFoll.Length;
                        sAP.ListFloor = new List<int>();
                        for (int j = 0; j < stringFoll.Length; j++)
                        {
                            int fl = int.Parse(stringFoll[j]);
                            sAP.ListFloor.Add(fl);
                        }
                        sAPs.Add(sAP);
                    }
                    List<SAP> sAPBLOCK = sAPs.Where(x => x.khuBlockDuAn_SDK.Equals(block)).ToList();
                    int max = sAPBLOCK.OrderByDescending(x => x.FloorTnghm_KUT).First().FloorTnghm_KUT;
                    int maxLocation = sAPBLOCK.OrderByDescending(x => x.LocationVtr_KUT).First().LocationVtr_KUT;
                    int minLocation = sAPBLOCK.OrderBy(x => x.LocationVtr_KUT).First().LocationVtr_KUT;

                    model.MaxLocation = maxLocation;
                    model.MinLocation = minLocation;
                    model.saps = sAPBLOCK;
                    model.dataTables = new List<DataTable>();
                    for (int i = max; i >= 0; i--)
                    {
                        DataTable dataTable = new DataTable();
                        dataTable.Floor = i;
                        dataTable.dataHouses = new List<DataHouse>();
                        for (int j = minLocation; j <= maxLocation; j++)
                        {
                            DataHouse dataHouse = new DataHouse();
                            SAP mSap = sAPBLOCK.Where(x => x.FloorTnghm_KUT == i && x.LocationVtr_KUT == j).FirstOrDefault();
                            if (mSap != null)
                            {
                                dataHouse.Detail = mSap.Description;
                                dataHouse.Status = "status_" + mSap.StatusTrngthiGiaodch_KUT;
                                dataHouse.MarFloor = mSap.FloorMar;
                                dataHouse.IsMer = false;
                            }
                            else
                            {
                                List<SAP> mSaps = sAPBLOCK.Where(x => x.ListFloor.Contains(i) && x.LocationVtr_KUT == j).ToList();
                                if (mSaps.Count > 0)
                                {
                                    dataHouse.IsMer = true;
                                }
                                else
                                {
                                    dataHouse.IsMer = false;
                                }
                                dataHouse.Detail = "";
                                dataHouse.Status = "status_";
                                dataHouse.MarFloor = 1;
                            }
                            dataTable.dataHouses.Add(dataHouse);
                        }
                        model.dataTables.Add(dataTable);

                        model.Name = "SO DO BLOCK " + block + " DU AN " + " " + sAPBLOCK[0].duAnIDName_SDK;
                    }

                    model.statusDatas = new List<StatusData>();

                    StatusData statusData101 = new StatusData();
                    statusData101.Name = "Giữ chỗ";
                    statusData101.nameClass = "status_101";
                    statusData101.StatusCode = 101;
                    model.statusDatas.Add(statusData101);
                    StatusData statusData111 = new StatusData();
                    statusData111.Name = "Đặt cọc";
                    statusData111.nameClass = "status_111";
                    statusData111.StatusCode = 111;
                    model.statusDatas.Add(statusData111);

                    StatusData statusData121 = new StatusData();
                    statusData121.Name = "Góp, vay vốn";
                    statusData121.nameClass = "status_121";
                    statusData121.StatusCode = 121;
                    model.statusDatas.Add(statusData121);

                    StatusData statusData131 = new StatusData();
                    statusData131.Name = "Mua bán";
                    statusData131.nameClass = "status_131";
                    statusData131.StatusCode = 131;
                    model.statusDatas.Add(statusData131);

                    StatusData statusData141 = new StatusData();
                    statusData141.Name = "Mở bán";
                    statusData141.nameClass = "status_141";
                    statusData141.StatusCode = 141;
                    model.statusDatas.Add(statusData141);

                    StatusData statusData151 = new StatusData();
                    statusData151.Name = "Bàn giao GCN";
                    statusData151.nameClass = "status_151";
                    statusData151.StatusCode = 151;
                    model.statusDatas.Add(statusData151);

                    StatusData statusData161 = new StatusData();
                    statusData161.Name = "Bàn giao sản phẩm";
                    statusData161.nameClass = "status_161";
                    statusData161.StatusCode = 161;
                    model.statusDatas.Add(statusData161);


                    StatusData statusData171 = new StatusData();
                    statusData171.Name = "Đã xây dựng nhà";
                    statusData171.nameClass = "status_171";
                    statusData171.StatusCode = 171;
                    model.statusDatas.Add(statusData171);

                    StatusData statusData181 = new StatusData();
                    statusData181.Name = "Đã yêu cần thanh lý";
                    statusData181.nameClass = "status_181";
                    statusData181.StatusCode = 181;
                    model.statusDatas.Add(statusData181);

                    StatusData statusData191 = new StatusData();
                    statusData191.Name = "Đã yêu cần chuyển nhượng";
                    statusData191.nameClass = "status_191";
                    statusData191.StatusCode = 191;
                    model.statusDatas.Add(statusData191);

                    StatusData statusData201 = new StatusData();
                    statusData201.Name = "Xả bán";
                    statusData201.nameClass = "status_201";
                    statusData201.StatusCode = 201;
                    model.statusDatas.Add(statusData201);


                    for (int i = 0; i < model.statusDatas.Count; i++)
                    {
                        model.statusDatas[i].Value = model.saps.Where(x => x.StatusTrngthiGiaodch_KUT == model.statusDatas[i].StatusCode).Count();
                    }
                }
                catch
                {

                }



            }
            return View(model);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
