using AlJamaliUnited.Web.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using static AlJamaliUnited.Web.Models.ApiResponse;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using AlJamaliUnited.Web;
using System.Web.Http.Results;
using Focus.Common.DataStructs;
using HttpGetAttribute = System.Web.Mvc.HttpGetAttribute;

namespace AlJamaliUnited.Web.Controllers
{
    public class MaterialRequestController : Controller
    {
        public string thisConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public string Session_Id = string.Empty;
        public string ServerIp = ConfigurationManager.AppSettings["serverIp"].ToString();
        private int Convertdate(DateTime Dt)
        {
            return ((Convert.ToDateTime(Dt).Year) * 65536) + (Convert.ToDateTime(Dt).Month) * 256 + (Convert.ToDateTime(Dt).Day);
        }
        private int ConvertTime(DateTime Dt)
        {
            return (Dt.Hour * 65536) + (Dt.Minute * 256) + Dt.Second;
        }
        public static bool IsNumeric(char o)
        {
            double result;
            //return o != null && Double.TryParse(o.ToString(), out result);
            return Double.TryParse(o.ToString(), out result);
        }
        private static int getCompCodeVal(char cCode)
        {
            int iRet = 0;
            char[] sLetters = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            for (int i = 0; i < sLetters.Length; i++)
            {
                if (sLetters[i] == cCode)
                {
                    iRet = i;
                    break;
                }
            }
            return iRet + 10;
        }
        public static int GetCompID(string CompCode)
        {
            int iRet = 0;
            string sCompCode = CompCode;
            if (IsNumeric(sCompCode[0]))
            {
                iRet = (36 * 36) * int.Parse(sCompCode[0].ToString());
            }
            else
            {
                iRet = (36 * 36) * getCompCodeVal(sCompCode[0]);
            }
            if (IsNumeric(sCompCode[1]))
            {
                iRet += (36) * int.Parse(sCompCode[1].ToString());
            }
            else
            {
                iRet += (36) * getCompCodeVal(sCompCode[1]);
            }

            if (IsNumeric(sCompCode[2]))
            {
                iRet += (36 * 0) * int.Parse(sCompCode[2].ToString());
            }
            else
            {
                iRet += (36 * 0) * getCompCodeVal(sCompCode[2]);
            }
            return iRet;

        }
        // GET: MaterialRequest
        public ActionResult Index()
        {
            ViewBag.WarehouseItems=GetDropdownItems();
            ViewBag.UnitsItems=GetDropDownUnits();

            return View();
        }
        private int ConvertDate(DateTime dt)
        {
            return ((Convert.ToDateTime(dt).Year) * 65536) + (Convert.ToDateTime(dt).Month * 256) + (Convert.ToDateTime(dt).Day);
        }
        [HttpPost]
        public ActionResult LoadData(DateTime? toDate, int? warehouseId)
        {
            WriteLog.writeLog("Received LoadData Method.");
            // Log ViewBag data
            WriteLog.writeLog("WarehouseItems count: " + ((List<Warehouse>)ViewBag.WarehouseItems)?.Count);
            WriteLog.writeLog("UnitsItems count: " + ((List<Units>)ViewBag.UnitsItems)?.Count);

            //warehouseId = 2;                            // 2 For shop fixed else 1 for Warehouse
            try
            {
                var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
                //int fromInt = 132579585;        //Default Date {1/1/2023 12:00:00 AM}
                int toInt = ConvertDate(toDate.GetValueOrDefault());

                List<MaterialData> materialReuqest = new List<MaterialData>();
                // DataTables parameters
                int draw = Convert.ToInt32(Request["draw"]);
                int start = Convert.ToInt32(Request["start"]);
                int length;

                using (SqlConnection conn = new SqlConnection(thisConnectionString))
                using (SqlCommand cmd = new SqlCommand("spMaterialRequest", conn))
                {
                    try
                    {
                        WriteLog.writeLog("Executing procedure");
                        // Add parameters
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@ToDate",SqlDbType.Int)).Value = toInt;
                        cmd.Parameters.Add(new SqlParameter("@Warehouse", SqlDbType.Int)).Value = warehouseId;
                        //cmd.Parameters.Add(new SqlParameter("@PageSize", SqlDbType.Int)).Value = length;
                        conn.Open();
                        WriteLog.writeLog("Database connection successfull.");
                        cmd.CommandTimeout = 300;
                        int recordCount = 0;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            
                            while (reader.Read())
                            {
                                int masterId = reader.GetInt32(reader.GetOrdinal("MasterId"));
                                //int unitid = reader.GetInt32(reader.GetOrdinal("BaseUnitId"));
                                //int unitid = reader["BaseUnitId"] == DBNull.Value ? 1 : Convert.ToInt32(reader["BaseUnitId"]);
                                int unitid = GetBaseUnit(masterId);
                                // Create a new MaterialData instance
                                MaterialData materialData = new MaterialData()
                                {
                                    Id = masterId,
                                    ItemCode = reader["ItemCode"].ToString(),
                                    Description = reader["ProductName"].ToString(),
                                    QtySold = reader["SoldQuantity"] == DBNull.Value ? 0 : Convert.ToDouble(reader["SoldQuantity"]),
                                    StockQty = reader["StockQuantity"] == DBNull.Value ? 0 : Convert.ToDouble(reader["StockQuantity"]),
                                    BaseUnit = reader["BaseUnit"].ToString(),
                                    //Packing = reader["Packing"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Packing"]),
                                    CRT = reader["CRT"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CRT"]),
                                    PCSRoll = reader["PCSRoll"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PCSRoll"]),
                                    Quantity = reader["Quantity"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Quantity"])
                                };

                                //if (masterId== 200176)
                                //{
                                //    Console.WriteLine("Please Pause.");
                                //}

                                // Assuming "Unit" is a nullable column in the database
                                int unitOrdinal = reader.GetOrdinal("Unit");
                                string existUnit = reader.IsDBNull(unitOrdinal) ? null : reader.GetString(unitOrdinal);

                                if (!string.IsNullOrEmpty(existUnit))
                                {
                                    // Get the list of unit & packing values
                                    List<(int iMasterId, string sName, int xFactor)> unitValues = GetUnitConverstion(masterId, unitid);

                                    // Create a custom order list based on existUnit value
                                    var customOrder = new List<string> { existUnit };
                                    customOrder.AddRange(unitValues.Select(u => u.sName).Distinct().Except(customOrder));

                                    // Populate the Unit property with all key-value pairs
                                    var allUnitData = unitValues
                                        .Select(u => new UnitData { iMasterId = u.iMasterId, sName = u.sName, xFactor = u.xFactor })
                                        .ToList();

                                    // Arrange materialData.Unit based on custom order
                                    materialData.Unit = allUnitData
                                        .OrderBy(u => customOrder.IndexOf(u.sName))
                                        .ToList();
                                }
                                else
                                {
                                    // If existUnit is empty or null, include all unit values without any custom order
                                    List<(int iMasterId, string sName, int xFactor)> unitValues = GetUnitConverstion(masterId, unitid);
                                    materialData.Unit = unitValues
                                        .Select(u => new UnitData { iMasterId = u.iMasterId, sName = u.sName, xFactor = u.xFactor })
                                        .ToList();
                                }



                                //// Get the list of unit & packing values
                                //List<(int iMasterId, string sName, int xFactor)> unitValues = GetUnitValues(masterId);
                                //// Populate the Unit property with key-value pairs
                                //materialData.Unit = unitValues.Select(u => new UnitData { iMasterId = u.iMasterId, sName = u.sName, xFactor = u.xFactor }).ToList();

                                //materialData.Unit = unitValues.Select(u => u.sName).ToList();
                                materialData.Packing = materialData.Unit[0].xFactor;
                                // Add the materialData to the list
                                materialReuqest.Add(materialData);
                                // Increment the record count
                                recordCount++;
                            }
                            WriteLog.writeLog("Number of Records retrieved: " + materialReuqest.Count);
                            //length = materialReuqest.Count;

                             length = materialReuqest.Count;
                        }
                        Console.WriteLine("Number of Records retrieved: " + recordCount);
                    }
                    catch (SqlException ex)
                    {

                        // Handle SQL Server specific exceptions
                        WriteLog.writeLog("SQL Exception: " + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        // Handle other exceptions
                        WriteLog.writeLog("An error occurred: " + ex.Message);
                    }
                    finally
                    {
                        // Ensure the connection is closed, even if an exception occurs
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                }

                var result = new
                {
                    draw = draw,
                    //draw = 1,
                    recordsTotal = materialReuqest.Count,
                    recordsFiltered = materialReuqest.Count,
                    //data = filteredData
                    data = materialReuqest
                };
                return Json(result, JsonRequestBehavior.AllowGet);
               

            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return Json(new { Message = "Error loading data", Error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult PostData(List<MaterialData> dataTableData, bool isLastBatch, DateTime? toDate, int? warehouseId, int Compid, string ssnId)
        {
            WriteLog.writeLog("Request Received on Controller: CompId: " + Compid + " SessionId :" + ssnId);
            //warehouseId = 2; // Store
            DateTime dt = new DateTime(Convert.ToDateTime(toDate).Year, Convert.ToDateTime(toDate).Month, Convert.ToDateTime(toDate).Day);
            int vDate = Convertdate(Convert.ToDateTime(dt));
            int vWarehouseid = Convert.ToInt32(warehouseId);
            //string vCompId= compids.ToString();
            //string Comp = Convert.ToString(GetCompID(CompanyCode));
            
            string VoucherNo = "4528";
            List<PostVoucherResponse> getData = new List<PostVoucherResponse>();
            try
            {
                if (accumulatedData == null)
                {
                    accumulatedData = new List<MaterialData>();
                }

                accumulatedData.AddRange(dataTableData);

                if (isLastBatch)
                {
                    //string SessionId = GetSessionId(Comp, Username, Password);
                    string SessionId = ssnId;
                    HashData objHashRequest = new HashData();
                    Hashtable objHeader = new Hashtable();
                    List<Hashtable> lstBody = new List<Hashtable>();
                    objHeader.Add("DocNo", "4528");
                    objHeader.Add("Date", vDate);
                    //objHeader.Add("Warehouse__Id", vWarehouseid);
                    objHeader.Add("Outlet__Id",vWarehouseid);
                    objHeader.Add("sNarration", "Test...");

                    if (accumulatedData != null && accumulatedData.Count > 0)
                    {
                        foreach (var dataItem in accumulatedData)
                        {
                            // Assuming dataItem is an instance of MaterialData or similar
                            Hashtable objBody = new Hashtable();
                            objBody.Add("MasterId", dataItem.Id);
                            objBody.Add("Item__Code", dataItem.ItemCode);
                            objBody.Add("Description", dataItem.Description);
                            //objBody.Add("Unit__Name", dataItem.BaseUnit);
                            objBody.Add("Unit__Name", dataItem.Unit[0].sName);
                            objBody.Add("Unit2", dataItem.Unit[0].sName);
                            //objBody.Add("Packing", dataItem.Packing);
                            // Accessing the first element of the array
                            objBody.Add("Packing", dataItem.Packing);

                            objBody.Add("CRT", dataItem.CRT);
                            objBody.Add("PCS/Roll", dataItem.PCSRoll);
                            objBody.Add("Quantity", dataItem.Quantity);
                            lstBody.Add(objBody);
                        }
                    }

                    Hashtable objHash = new Hashtable();
                    objHash.Add("Header", objHeader);
                    objHash.Add("Body", lstBody);

                    List<Hashtable> lstHash = new List<Hashtable>();
                    lstHash.Add(objHash);
                    objHashRequest.data = lstHash;
                    objHashRequest.url = "http://" + ServerIp + "/Focus8API/Transactions/7936/" + VoucherNo + "/";          // Material Requisition
                    objHashRequest.result = 1;
                    string sContent = JsonConvert.SerializeObject(objHashRequest);
                    using (var client = new WebClient())
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3
                                        | SecurityProtocolType.Tls12
                                        | SecurityProtocolType.Tls11
                                        | SecurityProtocolType.Tls;
                        client.Encoding = System.Text.Encoding.UTF8;
                        client.Headers.Add("fSessionId", SessionId);
                        client.Headers.Add("Content-Type", "application/json");
                        WriteLog.writeLog("Request API: " + sContent.ToString());
                        var response = client.UploadString("http://" + ServerIp + "/Focus8API/Transactions/7936", sContent);
                        WriteLog.writeLog("Response API: "+ response.ToString());

                        if (response != null)
                        {
                            var responseData = JsonConvert.DeserializeObject<ApiResponse.PostResponse>(response);
                            if (responseData.result == 1)
                            {
                                getData.Add(new ApiResponse.PostVoucherResponse { VoucherNo = Convert.ToString(responseData.data[0]["VoucherNo"]), result = responseData.result, message = "Voucher Posted Successfully" });

                               
                                try
                                {
                                    using (SqlConnection connection = new SqlConnection(thisConnectionString))
                                    {
                                        connection.Open();

                                        using (SqlTransaction transaction = connection.BeginTransaction())
                                        {
                                            // Retrieve VoucherNo from responseData
                                            
                                            string voucherNo = responseData.data[0]["VoucherNo"].ToString();
                                            WriteLog.writeLog("Retrieve VoucherNo from responseData: " + voucherNo.ToString() + "");
                                            // Convert DateTime to string for comparison
                                            //string vDate = Convert.ToDateTime(objHeader["Date"]).ToString("yyyy-MM-dd");

                                            try
                                            {
                                                // Check if rows with the same TransactionDate exist
                                                string checkExistingRowsQuery = "SELECT COUNT(*) FROM tblMaterialRequestForm WHERE TransactionDate = @Date;";
                                                WriteLog.writeLog("checkExistingRowsQuery: " + checkExistingRowsQuery.ToString() + "");
                                                using (SqlCommand checkCmd = new SqlCommand(checkExistingRowsQuery, connection, transaction))
                                                {
                                                    checkCmd.Parameters.AddWithValue("@Date", vDate);

                                                    int rowCount = (int)checkCmd.ExecuteScalar();

                                                    // If rows exist, delete them
                                                    //if (rowCount > 0)
                                                    //{
                                                    //    string deleteExistingRowsQuery = "DELETE FROM tblMaterialRequestForm WHERE TransactionDate = @Date;";
                                                    //    WriteLog.writeLog("If Row Exist Delete: " + deleteExistingRowsQuery.ToString() + "");
                                                    //    using (SqlCommand deleteCmd = new SqlCommand(deleteExistingRowsQuery, connection, transaction))
                                                    //    {
                                                    //        deleteCmd.Parameters.AddWithValue("@Date", vDate);
                                                    //        deleteCmd.ExecuteNonQuery();
                                                    //    }
                                                    //}
                                                }

                                                // Insert Body data into tblMaterialRequestForm
                                                WriteLog.writeLog("Insert Body data into tblMaterialRequestForm");
                                                foreach (Hashtable bodyItem in lstBody)
                                                {
                                                    // Check if the row exists based on MasterId, ItemCode, and Date
                                                    string checkExistenceQuery = @"
        SELECT COUNT(*) 
        FROM tblMaterialRequestForm 
        WHERE MasterId = @MasterId AND ItemCode = @ItemCode AND TransactionDate = @Date;
    ";

                                                    using (SqlCommand checkExistenceCmd = new SqlCommand(checkExistenceQuery, connection, transaction))
                                                    {
                                                        checkExistenceCmd.Parameters.AddWithValue("@MasterId", bodyItem["MasterId"]);
                                                        checkExistenceCmd.Parameters.AddWithValue("@ItemCode", bodyItem["Item__Code"]);
                                                        checkExistenceCmd.Parameters.AddWithValue("@Date", vDate);

                                                        int rowCount = (int)checkExistenceCmd.ExecuteScalar();

                                                        if (rowCount > 0)
                                                        {
                                                            // If the row exists, update the Quantity
                                                            string updateQuery = @"
                UPDATE tblMaterialRequestForm 
                SET Quantity = @Quantity 
                WHERE MasterId = @MasterId AND ItemCode = @ItemCode AND TransactionDate = @Date;
            ";

                                                            using (SqlCommand updateCmd = new SqlCommand(updateQuery, connection, transaction))
                                                            {
                                                                updateCmd.Parameters.AddWithValue("@MasterId", bodyItem["MasterId"]);
                                                                updateCmd.Parameters.AddWithValue("@ItemCode", bodyItem["Item__Code"]);
                                                                updateCmd.Parameters.AddWithValue("@Date", vDate);
                                                                updateCmd.Parameters.AddWithValue("@Quantity", bodyItem["Quantity"]);

                                                                updateCmd.ExecuteNonQuery();
                                                            }
                                                        }
                                                        else
                                                        {
                                                            // If the row does not exist, insert a new row
                                                            string insertQuery = @"
                INSERT INTO tblMaterialRequestForm 
                (MasterId, DocNo, TransactionDate, ItemCode, Description, BaseUnit, Unit, Packing, CRT, PCSRoll, Quantity) 
                VALUES (@MasterId, @DocNo, @Date, @ItemCode, @Description, @BaseUnit, @Unit, @Packing, @CRT, @PCSRoll, @Quantity);
            ";

                                                            using (SqlCommand insertCmd = new SqlCommand(insertQuery, connection, transaction))
                                                            {
                                                                insertCmd.Parameters.AddWithValue("@MasterId", bodyItem["MasterId"]);
                                                                insertCmd.Parameters.AddWithValue("@DocNo", voucherNo);
                                                                insertCmd.Parameters.AddWithValue("@Date", vDate);
                                                                insertCmd.Parameters.AddWithValue("@ItemCode", bodyItem["Item__Code"]);
                                                                insertCmd.Parameters.AddWithValue("@Description", bodyItem["Description"]);
                                                                insertCmd.Parameters.AddWithValue("@BaseUnit", bodyItem["Unit__Name"]);
                                                                insertCmd.Parameters.AddWithValue("@Unit", bodyItem["Unit2"] ?? DBNull.Value);
                                                                insertCmd.Parameters.AddWithValue("@Packing", bodyItem["Packing"]);
                                                                insertCmd.Parameters.AddWithValue("@CRT", bodyItem["CRT"]);
                                                                insertCmd.Parameters.AddWithValue("@PCSRoll", bodyItem["PCS/Roll"]);
                                                                insertCmd.Parameters.AddWithValue("@Quantity", bodyItem["Quantity"]);

                                                                insertCmd.ExecuteNonQuery();
                                                            }
                                                        }
                                                    }
                                                }

                                                //foreach (Hashtable bodyItem in lstBody)
                                                //{
                                                //    string insertBodyQuery = "INSERT INTO tblMaterialRequestForm (MasterId,DocNo,TransactionDate, ItemCode, Description, BaseUnit, Unit, Packing, CRT, PCSRoll, Quantity) " +
                                                //                             "VALUES (@MasterId,@DocNo,@Date, @ItemCode, @Description, @BaseUnit, @Unit, @Packing, @CRT, @PCSRoll, @Quantity);";

                                                //    using (SqlCommand cmd = new SqlCommand(insertBodyQuery, connection, transaction))
                                                //    {
                                                //        cmd.Parameters.AddWithValue("@MasterId", bodyItem["MasterId"]);
                                                //        cmd.Parameters.AddWithValue("@DocNo", voucherNo);
                                                //        cmd.Parameters.AddWithValue("@Date", vDate);
                                                //        cmd.Parameters.AddWithValue("@ItemCode", bodyItem["Item__Code"]);
                                                //        cmd.Parameters.AddWithValue("@Description", bodyItem["Description"]);
                                                //        cmd.Parameters.AddWithValue("@BaseUnit", bodyItem["Unit__Name"]);
                                                //        //cmd.Parameters.AddWithValue("@Unit2", bodyItem["Unit2"]);
                                                //        // Check if "Unit2" is null and add DBNull.Value if it is
                                                //        cmd.Parameters.AddWithValue("@Unit", bodyItem["Unit2"] ?? DBNull.Value);
                                                //        cmd.Parameters.AddWithValue("@Packing", bodyItem["Packing"]);
                                                //        cmd.Parameters.AddWithValue("@CRT", bodyItem["CRT"]);
                                                //        cmd.Parameters.AddWithValue("@PCSRoll", bodyItem["PCS/Roll"]);
                                                //        cmd.Parameters.AddWithValue("@Quantity", bodyItem["Quantity"]);

                                                //        cmd.ExecuteNonQuery();
                                                //    }
                                                //}

                                                transaction.Commit();
                                            }
                                            catch (Exception)
                                            {
                                                transaction.Rollback();
                                                throw; // Re-throw the exception after rollback
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    // Handle database connection error
                                    WriteLog.writeLog("Error connecting to the database: " + ex.Message);
                                    return Json(new { success = false, message = "An error occurred while processing the data.", error = ex.Message });
                                }



                            }
                            if (responseData.result == -1)
                            {
                                WriteLog.writeLog(Convert.ToString(responseData.data[0]["VoucherNo"]));
                                getData.Add(new ApiResponse.PostVoucherResponse { VoucherNo = Convert.ToString(responseData.data[0]["VoucherNo"]), result = 0, message = "Voucher Not Posted : " + Convert.ToString(responseData.message) + "" });
                            }
                        }
                    }
                    accumulatedData.Clear();
                }
                return Json(new { success = true, message = "SuccessfullyPosted" });
                
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return Json(new { success = false, message = "An error occurred while processing the data.", error = ex.Message });
            }
           
        }
        
        private List<Warehouse> GetDropdownItems()
        {
            List<Warehouse> items = new List<Warehouse>();

            using (SqlConnection conn = new SqlConnection(thisConnectionString))
            using (SqlCommand cmd = new SqlCommand("spGetWarehouse", conn)) // Replace "spGetWarehouses" with the actual stored procedure name
            {
                try
                {
                    WriteLog.writeLog("Executing warehouse retrieval procedure");
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();
                    WriteLog.writeLog("Database connection successful.");

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Assuming your warehouse table has columns "WarehouseId" and "WarehouseName"
                            int id = Convert.ToInt32(reader["iMasterId"].ToString());
                            string name = reader["sName"].ToString();

                            items.Add(new Warehouse { Id = id, Name = name });
                        }
                    }
                }
                catch (SqlException ex)
                {
                    // Handle SQL Server specific exceptions
                    WriteLog.writeLog("SQL Exception: " + ex.Message);
                }
                catch (Exception ex)
                {
                    // Handle other exceptions
                    WriteLog.writeLog("An error occurred: " + ex.Message);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }
            // Define the custom order
            Dictionary<int, int> customOrder = new Dictionary<int, int>
            {
                //{2, 1}, // Shop: Sequence 1
                //{4, 2}, // Showroom: Sequence 2
                //{1, 3}  // Warehouse: Sequence 3
                {2,1 },     // Shop
                {3,2},      // Warehouse
                {4,3},      // Panda
            };
            // Sort the items list based on the custom order
            items.Sort((x, y) => customOrder[x.Id].CompareTo(customOrder[y.Id]));
            return items;
        }
        private List<Units> GetDropDownUnits()
        {
            List <Units> units = new List<Units>();
            using (SqlConnection conn = new SqlConnection(thisConnectionString))
            using (SqlCommand cmd = new SqlCommand("spGetUnits",conn))
            {
                try
                {
                    WriteLog.writeLog("Executing spGetUnits retrieval procedure");
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();
                    WriteLog.writeLog("Database connection successful.");

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Assuming your warehouse table has columns "UnitID" and "UnitName"
                            int id = Convert.ToInt32(reader["iMasterId"].ToString());
                            string name = reader["sName"].ToString();

                            units.Add(new Units { Id = id, Name = name });
                        }
                    }

                }
                catch (SqlException ex)
                {
                    // Handle SQL Server specific exceptions
                    WriteLog.writeLog("SQL Exception: " + ex.Message);
                    throw;
                }
                catch (Exception ex)
                {
                    // Handle other exceptions
                    WriteLog.writeLog("An error occurred: " + ex.Message);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }
            return units;
        }
        // Assuming accumulatedData is a class-level variable
        private static List<MaterialData> accumulatedData;
        
        //[HttpPost]
        //public IHttpActionResult PostMaterialRequest([FromBody] MaterialRequest mtr)
        //{
            
        //    string clientIp = System.Web.HttpContext.Current.Request.UserHostAddress;
        //    WriteLog.writeLog("Incoming Client Request from IP: " + clientIp + "" + Environment.NewLine + "Action:JVOmorfia/PostJVOmorfia");
        //    List<PostVoucherResponse> getData = new List<PostVoucherResponse>();
        //    try
        //    {
        //        string Comp = Convert.ToString(GetCompID(CompanyCode));
        //        string SessionId = GetSessionId(Comp, Username, Password);
        //        string voucherno = mtr.Header.DocNo;
        //        string MyVNo = voucherno;
        //        DateTime dt = new DateTime(Convert.ToDateTime(mtr.Header.Date).Year, Convert.ToDateTime(mtr.Header.Date).Month, Convert.ToDateTime(mtr.Header.Date).Day);
        //        int vDate = Convertdate(Convert.ToDateTime(dt));
        //        HashData objHashRequest = new HashData();
        //        Hashtable objHeader = new Hashtable();
        //        objHeader.Add("Doc", MyVNo);
        //        objHeader.Add("Date", vDate);
        //        objHeader.Add("Currency__Id", 110);
        //        objHeader.Add("ExchangeRate", "1.0000000000");
        //        objHeader.Add("Entity__Id", 10);
        //        objHeader.Add("sNarration", mtr.Header.Narration);

        //        List<Dictionary<string, object>> lstBody = new List<Dictionary<string, object>>();

        //        for (int i = 0; i < mtr.Body.Count; i++)
        //        {
        //            Dictionary<string, object> objBody = new Dictionary<string, object>();

        //            objBody.Add("Business Unit__Id", 16);                           // COR-SC

        //            //if (mtr.Body[i].PaymentType == "Cash")       // CASH TRANSACTION
        //            //{
        //            //    objBody.Add("DrAccount__Id", 230);              // CASH
        //            //    objBody.Add("CrAccount__Id", 353);              // Sales
        //            //}
        //            //else if (mtr.Body[i].PaymentType == "KNet")  // K-NET TRANSACTION
        //            //{
        //            //    objBody.Add("DrAccount__Id", 728);          // Knet         Gulf Bank - OML-37144764
        //            //    objBody.Add("CrAccount__Id", 353);          // Sales        SALES - SERVICE
        //            //}
        //            //else if (mtr.Body[i].PaymentType == "Insurance")  //INSURANCE TRANSACTION
        //            //{
        //            //    objBody.Add("DrAccount__Id", 730);          // Insurance    WAPMED
        //            //    objBody.Add("CrAccount__Id", 353);          // Sales        SALES - SERVICE
        //            //}
        //            //else if (mtr.Body[i].PaymentType == "COGS")  // COGS-SERVICES TRANSACTION
        //            //{
        //            //    objBody.Add("DrAccount__Id", 365);          // COGS-SERVICES
        //            //    objBody.Add("CrAccount__Id", 264);          // INVENTORY
        //            //}
        //            //objBody.Add("Amount", mtr.Body[i].Amount);
        //            //objBody.Add("Reference", mtr.Body[i].Reference);
        //            //objBody.Add("sRemarks", mtr.Body[i].Remark);

        //            lstBody.Add(objBody);
        //        }

        //        Hashtable objHash = new Hashtable();
        //        objHash.Add("Header", objHeader);
        //        objHash.Add("Body", lstBody);

        //        List<Hashtable> lstHash = new List<Hashtable>();
        //        lstHash.Add(objHash);
        //        objHashRequest.data = lstHash;
        //        objHashRequest.url = "http://" + ServerIp + "/Focus8API/Transactions/8706/" + MyVNo + "/";
        //        objHashRequest.result = 1;
        //        string sContent = JsonConvert.SerializeObject(objHashRequest);
        //        using (var client = new WebClient())
        //        {
        //            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3
        //                            | SecurityProtocolType.Tls12
        //                            | SecurityProtocolType.Tls11
        //                            | SecurityProtocolType.Tls;
        //            client.Encoding = System.Text.Encoding.UTF8;
        //            client.Headers.Add("fSessionId", SessionId);
        //            client.Headers.Add("Content-Type", "application/json");
        //            WriteLog.writeLog(sContent.ToString());
        //            var response = client.UploadString("http://" + ServerIp + "/Focus8API/Transactions/8706", sContent);
        //            WriteLog.writeLog(response.ToString());
        //            if (response != null)
        //            {
        //                var responseData = JsonConvert.DeserializeObject<ApiResponse.PostResponse>(response);
        //                if (responseData.result == 1)
        //                {
        //                    getData.Add(new ApiResponse.PostVoucherResponse { VoucherNo = Convert.ToString(responseData.data[0]["VoucherNo"]), result = responseData.result, message = "Voucher Posted Successfully" });
        //                }
        //                if (responseData.result == -1)
        //                {
        //                    WriteLog.writeLog(Convert.ToString(responseData.data[0]["VoucherNo"]));
        //                    getData.Add(new ApiResponse.PostVoucherResponse { VoucherNo = Convert.ToString(responseData.data[0]["VoucherNo"]), result = 0, message = "Voucher Not Posted : " + Convert.ToString(responseData.message) + "" });
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        getData.Add(new PostVoucherResponse { result = 0, message = "Voucher Not Posted {" + ex.Message + "}" });
        //    }
        //    return Ok(getData);

        //}

        //private List<(int iMasterId, string sName, int xFactor)> GetUnitValuesOld(int productId)
        //{
        //    List<(int iMasterId, string sName, int xFactor)> unitValues = new List<(int iMasterId, string sName, int xFactor)>();

        //    using (SqlConnection conn = new SqlConnection(thisConnectionString))
        //    //using (SqlCommand cmd = new SqlCommand("SELECT u.iMasterId, u.sName, uc.fXFactor " +
        //    //                                      "FROM mCore_Units u " +
        //    //                                      "JOIN mCore_UnitConversion uc ON u.iMasterId = uc.iUnitId " +
        //    //                                      "WHERE uc.iProductId = @ProductId", conn))

        //    using (SqlCommand cmd = new SqlCommand("SELECT mCore_Units.iMasterId, " +
        //                                  "mCore_UnitsLanguage.sName, " +
        //                                  "CAST(fXFactor + fAdditionalQty AS INT) AS fXFactor " +
        //                                  "FROM mCore_UnitConversion " +
        //                                  "INNER JOIN mCore_Units ON mCore_Units.iMasterId = mCore_UnitConversion.iUnitId " +
        //                                  "JOIN mCore_UnitsLanguage ON mCore_Units.iMasterId = mCore_UnitsLanguage.iMasterId AND iLanguageId = 0 " +
        //                                  "WHERE iBaseUnitId = 1 AND (iProductId = @ProductId OR iProductId = 0) AND bIsDeleted = 0 " +
        //                                  "UNION ALL " +
        //                                  "SELECT iBaseUnitId, " +
        //                                  "mCore_UnitsLanguage.sName, " +
        //                                  "CAST(CASE WHEN fXFactor + fAdditionalQty = 0 THEN 1 ELSE 1 / (fXFactor + fAdditionalQty) END AS INT) AS fXFactor " +
        //                                  "FROM mCore_UnitConversion " +
        //                                  "INNER JOIN mCore_Units ON mCore_Units.iMasterId = mCore_UnitConversion.iBaseUnitId " +
        //                                  "JOIN mCore_UnitsLanguage ON mCore_Units.iMasterId = mCore_UnitsLanguage.iMasterId AND iLanguageId = 0 " +
        //                                  "WHERE mCore_UnitConversion.iUnitId = 1 AND (iProductId = @ProductId OR iProductId = 0) AND bIsDeleted = 0 " +
        //                                  "UNION ALL " +
        //                                  "SELECT mCore_Units.iMasterId, " +
        //                                  "mCore_UnitsLanguage.sName, " +
        //                                  "CAST(1 AS INT) AS fXFactor " +
        //                                  "FROM mCore_Units " +
        //                                  "JOIN mCore_UnitsLanguage ON mCore_Units.iMasterId = mCore_UnitsLanguage.iMasterId AND iLanguageId = 0 " +
        //                                  "WHERE mCore_Units.iMasterId = 1) AS UnitData " +  // Added alias for the result set
        //                                  "ORDER BY sName, iMasterId DESC", conn))  // Adjusted ORDER BY clause


        //        {

        //        conn.Open();
        //        cmd.Parameters.Add(new SqlParameter("@ProductId", SqlDbType.Int)).Value = productId;

        //        using (SqlDataReader reader = cmd.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                int iMasterId = reader.GetInt32(reader.GetOrdinal("iMasterId"));
        //                string sName = reader.GetString(reader.GetOrdinal("sName"));
        //                //int xFactor = reader.GetInt32(reader.GetOrdinal("fXFactor"));
        //                int xFactor = Convert.ToInt32(reader.GetDecimal(reader.GetOrdinal("fXFactor")));


        //                unitValues.Add((iMasterId, sName,xFactor));
        //            }
        //        }
        //    }

        //    return unitValues;
        //}

        [HttpGet]
        public List<(int iMasterId, string sName, int xFactor)> GetUnitConverstion(int productId, int baseUnitId)
        {
            List<(int iMasterId, string sName, int xFactor)> unitValues = new List<(int iMasterId, string sName, int xFactor)>();

            using (SqlConnection conn = new SqlConnection(thisConnectionString))
            using (SqlCommand cmd = new SqlCommand("spUnitConversion", conn)) // Use the stored procedure name
            {
                cmd.CommandType = CommandType.StoredProcedure;

                conn.Open();
                cmd.Parameters.Add(new SqlParameter("@ProductId", SqlDbType.Int)).Value = productId;
                cmd.Parameters.Add(new SqlParameter("@BaseUnitId", SqlDbType.Int)).Value = baseUnitId;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int iMasterId = reader.GetInt32(reader.GetOrdinal("iMasterId"));
                        string sName = reader.GetString(reader.GetOrdinal("sName"));
                        int xFactor = reader.GetInt32(reader.GetOrdinal("fXFactor"));

                        unitValues.Add((iMasterId, sName, xFactor));
                    }
                }
            }

            return unitValues;
        }
        private int GetBaseUnit(int masterId)
        {
            using (SqlConnection connection = new SqlConnection(thisConnectionString))
            using (SqlCommand command = new SqlCommand("spGetBaseUnit", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ProductId", masterId);

                try
                {
                    connection.Open();
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value && Convert.ToInt32(result) != 0)
                    {
                        return Convert.ToInt32(result);
                    }
                    else
                    {
                        // If result is null or 0, query from the provided SQL
                        string sqlQuery = @"
                    SELECT TOP 1 u.iMasterId
                    FROM vrCore_Product p
                    JOIN vrCore_Units u ON u.sName = p.iDefaultBaseUnit
                    WHERE p.iMasterId = @ProductId;
                ";

                        using (SqlCommand sqlCmd = new SqlCommand(sqlQuery, connection))
                        {
                            sqlCmd.Parameters.AddWithValue("@ProductId", masterId);
                            object sqlResult = sqlCmd.ExecuteScalar();

                            return sqlResult != null && sqlResult != DBNull.Value ? Convert.ToInt32(sqlResult) : 1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred in GetUnitValue: {ex.Message}");
                    return 1; // Return a default value
                }
            }
        }

        [HttpPost]
        public ActionResult UpdateUnit(string identifier, string newValue)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(thisConnectionString))
                using (SqlCommand cmd = new SqlCommand("UpdateMaterialUnit", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ItemCode", SqlDbType.NVarChar, 255)).Value = identifier;
                    cmd.Parameters.Add(new SqlParameter("@NewUnit", SqlDbType.NVarChar, 255)).Value = newValue;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                return Json(new { success = true, message = "Update successful" });
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return Json(new { success = false, message = "Error updating unit", error = ex.Message });
            }
        }
        [HttpPost]
        public ActionResult CheckPreviousQuantity(int masterId, DateTime? toDate)
        {
            int toInt = ConvertDate(toDate.GetValueOrDefault());
            try
            {
                using (SqlConnection connection = new SqlConnection(thisConnectionString))
                using (SqlCommand command = new SqlCommand("spCheckPreviousQuantity", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TransactionDate", toInt);
                    command.Parameters.AddWithValue("@MasterId", masterId);

                    connection.Open();
                    var result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        var quantityChanged = Convert.ToInt32(result);
                        Console.WriteLine($"CheckPreviousQuantity Success: QuantityChanged = {quantityChanged}");
                        return Json(new { QuantityChanged = quantityChanged });
                    }
                    else
                    {
                        Console.WriteLine("CheckPreviousQuantity Success: QuantityChanged = 0");
                        return Json(new { QuantityChanged = 0 });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred in CheckPreviousQuantity: {ex.Message}");
                return Json(new { QuantityChanged = 0 });
            }
        }

    }

}

