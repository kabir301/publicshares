using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SMSCore;
using Oracle.DataAccess.Client;
using System.Data;
using SMSDBObjects.ModelObjects;
using System.Collections;
using SMSDBObjects.Views;
using AjaxControlToolkit;
using System.IO;
using SMS.Common;
using System.Web.Services;
using System.Web.Script.Services;

namespace DSESMS.Circular
{
    public partial class CircularInfoNew : System.Web.UI.Page
    {
        
        
        protected void Page_Load ( object sender, EventArgs e )
        {
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Cache.SetNoServerCaching();
            HttpContext.Current.Response.Cache.SetNoStore();
            if ( !IsPostBack )
            {
                #region Menu
                if (Session["newMenuData"] != null)
                {
                    ErpNewmenu.InnerHtml = Session["newMenuData"].ToString();
                    topmenu.InnerHtml = Session["topMenuOwnData"].ToString();
                }
                else
                {
                    Response.Redirect("/Default.aspx");

                }

                // pnlMenu.Controls.Add(MenuClass.PopulateOwnMenu(Session["menuOwnData"]));
                #endregion 

                #region Client Script

               // Page.ClientScript.RegisterClientScriptInclude("script01", @"../Scripts/LeaveApplication.js");
                

                #endregion
                initialData();
                #region Get QueryString Data
                string circularInfoID = string.Empty;
                //ajxCircularfileUpload.ContextKeys = "";
                if (Session["circularMsgID"] != null)
                {

                    circularInfoID = Session["circularMsgID"].ToString();
                }
                 //ViewState["CircularID"] = 0;
                if (!string.IsNullOrEmpty(circularInfoID) && circularInfoID.Length <= 5)//to make all department head selected by default
                {
                    EntityCircular entitycir = new EntityCircular();
                    Int32 circularMessage_ID = 0;
                    circularMessage_ID = Convert.ToInt32(circularInfoID);
                    CIR_MESSAGE_INFO cir_message_info_old = entitycir.CIR_MESSAGE_INFO.Where(i => i.CIR_MESSAGE_INFO_ID.Equals(circularMessage_ID)).Single();
                    chkDeptHeadAll.Checked = true;

                    txtEffectiveFrom.Text = DateTime.Now.ToString("dd-MMM-yyyy");
                    txtExpireDate.Text = DateTime.Now.ToString("dd-MMM-yyyy");
                    txtSubject.Text = cir_message_info_old.MSG_SUBJECT;
                    //txtEditor.Text = cir_message_info_old.MSG_MESSAGE;
                    editor.Content = cir_message_info_old.MSG_MESSAGE;
                   // ViewState["CircularID"] = circularMessage_ID;
                  //  HttpContext.Current.RewritePath("../Circular/CircularInfoNew.aspx");
                    circularInfoID = string.Empty;

                }

            }
            if (!IsPostBack)
            {
                Session["fileName"] = "1";
            }
            var myscript = @"
//                    document.getElementById(""myFile"").addEventListener(""change"", zipUpFile);
//                    document.getElementById(""btnSave"").addEventListener(""click"", function (event) {
//                            Submit();
//                            event.preventDefault();
//                        }); 
                        ";

            var scriptToPopulateFileList = "";
            var fileNameList = (string)System.Web.HttpContext.Current.Session["fileName"];
            //scriptToPopulateFileList += @"listOfFiles.innerHTML += ""<p>"" + """ + "Uploaded Files:" + @""" + ""</p>"";";

            if (fileNameList != null && fileNameList != "1") {





                var fileNames = System.Web.HttpContext.Current.Session["fileName"].ToString().Split(',').ToList();
                foreach (var file in fileNames)
                {
                    if (file.EndsWith(".zip"))
                    {
                        var trueFileName = "";
                        int index = file.IndexOf('-');
                        if (index > 0)
                        {
                            trueFileName = file.Substring(12, index - 12);// + Path.GetExtension(file);
                            scriptToPopulateFileList += @"
                            listOfFiles.innerHTML += `<p id='fname" + trueFileName + @"'><span>" + trueFileName + @"</span> <button type='button' onclick=""deleteFile(this, '" + trueFileName + @"')"">Delete</button></p>`;";
                            //listOfFiles.innerHTML += ""<p>"" + """ + file.Substring(12) + @""" + ""</p>"";";
                        }
                    }
                    else
                    {
                        var trueFileName = "";
                        int index = file.IndexOf('-');
                        if (index > 0)
                        {
                            trueFileName = file.Substring(12, index - 12) + Path.GetExtension(file);
                            scriptToPopulateFileList += @"
                            listOfFiles.innerHTML += `<p id='fname" + trueFileName + @"'><span>" + trueFileName + @"</span> <button type='button' onclick=""deleteFile(this, '" + trueFileName + @"')"">Delete</button></p>`;";
                            //var abu = file.Substring(12, index);
                        }
                    }
                }

//                var anna = fileNameList.Split(',');
//                foreach (string file in anna){
//                    if (file != "1" && file != ""){
//                    scriptToPopulateFileList += @"
//                                                listOfFiles.innerHTML += ""<p>"" + """ + file.Substring(12) + @""" + ""</p>"";";
//                    }
//                }
                scriptToPopulateFileList += @"document.getElementById('myFile').value= null;";
            }
            myscript += scriptToPopulateFileList;
            var javaScript = "<script>" + myscript + "</script>";
            //Page.ClientScript.RegisterStartupScript(GetType(), "test", javaScript);
            ScriptManager.RegisterStartupScript(Page, GetType(), "soopy", javaScript, false);
                #endregion


        }
        //Fill the ddlrequisition type 
    
     
        protected void gvLeavInfo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
           
            GridViewRow row = e.Row;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
            //    string fromdate=((Label)row.FindControl("lblfromDate")).Text;
         

            }
          
        }
        protected void initialData ( )
        {
            HumanResourceServer hrm = new HumanResourceServer();
            string errorMsg = string.Empty;
            

          

            List<EMPLOYEECATEGORY> empCategoryLst = hrm.GetAllemployeeCategory();
            
            
            chkCategoryAllList.DataSource = empCategoryLst;
            chkCategoryAllList.DataValueField = "EMPLOYEECATID";
            chkCategoryAllList.DataTextField = "CATEGORYNAME";
            chkCategoryAllList.DataBind();
         
            

           

        }


        protected void chkDivision_CheckedChanged(object sender, EventArgs e)
        {

            HumanResourceServer hrServer = new HumanResourceServer();
            string errorMsg = string.Empty;
            if (chkDivision.Checked)
            {
                List<DIVISION> divisionList = hrServer.GetOnlyActiveDivisions(out errorMsg);
                chkDivisionAllList.DataSource = divisionList;
                chkDivisionAllList.DataTextField = "DIVISIONNAME";
                chkDivisionAllList.DataValueField = "DIVISIONID";

                chkDivisionAllList.DataBind();
            }
            else
            {
                chkDivisionAllList.Items.Clear();
                
            }
        }

        protected void chkDept_CheckedChanged(object sender, EventArgs e)
        {

            HumanResourceServer hrServer = new HumanResourceServer();
            string errorMsg = string.Empty;
            if (chkDept.Checked)
            {
                List<DEPARTMENT> deptList = hrServer.GetAllActiveDepartments(out errorMsg);
            

                chkDeptAllList.DataSource = deptList;
                chkDeptAllList.DataTextField = "DEPARTMENTNAME";
                chkDeptAllList.DataValueField = "DEPTID";
                chkDeptAllList.DataBind();
            }
            else
            {
                chkDeptAllList.Items.Clear();
            }
        }



        protected void chkLevelAll_CheckedChanged(object sender, EventArgs e)
        {

            HumanResourceServer hrServer = new HumanResourceServer();
            string errorMsg = string.Empty;
            if (chkLevelAll.Checked)
            {
                List<EMPLEVEL> empLevelLst = hrServer.GetAllEmpLevels(out errorMsg);
                chkLevelAllList.Items.Clear();

                chkLevelAllList.DataSource = empLevelLst;
                chkLevelAllList.DataTextField = "LEVELNAME";
                chkLevelAllList.DataValueField = "EMPLEVELID";
                chkLevelAllList.DataBind();

            }
            else
            {
                chkLevelAllList.Items.Clear();
            }
        }


   
        protected void btnSave_Click ( object sender, EventArgs e )
        {
            string Msg = string.Empty;
            HumanResourceServer hrServer=new HumanResourceServer();
               MessageServer msgServer = new MessageServer();
            List<short> empIDMasterList = new List<short>();
            List<short> empIDInnerList = new List<short>();
            //DateTime effectiveFromDate;
            //DateTime expireDate;
            string passWord = string.Empty;
            string subject = string.Empty;
            string fileName = string.Empty;
            string msgFromEditor = string.Empty;

            try
            {
                //if (string.IsNullOrEmpty(txtPassword.Text.Trim()))
                //{
                //    WebUtil.MessageBox_Show("Please provide your mail Password......",this);
                //    return;
                //}

                //msgFromEditor = txtEditorHidden.Value;

                //WebUtil.MessageBox_Show(msgFromEditor,this);

                if (!WebUtil.IsValidDate(txtEffectiveFrom.Text.Trim()))
                {
                    WebUtil.MessageBox_Show("Please provide valid effective date......", this);
                    return;

                }
                if (!WebUtil.IsValidDate(txtExpireDate.Text.Trim()))
                {
                    WebUtil.MessageBox_Show("Please provide valid expire date......", this);
                    return;

                }
                if (string.IsNullOrEmpty(txtSubject.Text.Trim()))
                {
                    WebUtil.MessageBox_Show("Please provide subject......", this);
                    return;
                }
                if (string.IsNullOrEmpty(editor.Content))
                {
                    WebUtil.MessageBox_Show("Please write  your message......", this);
                    return;
                }
               
                List<short> empIDs = new List<short>();
                if (chkAllEmployee.Checked)
                {
                    empIDMasterList = hrServer.GetOnlyActiveEmployee(out Msg).Select(emp => emp.EMPID).ToList();

                }
                else
                {
                    if (chkDivision.Checked)
                    {
                        foreach (ListItem item in chkDivisionAllList.Items)
                        {
                            if (item.Selected)
                            {
                                empIDInnerList = hrServer.GetEmpInfoByByDivision(Convert.ToInt16(item.Value)).Select(aa => aa.EMPID).ToList();

                                if (empIDInnerList != null && empIDInnerList.Count > 0)
                                {
                                    empIDMasterList.AddRange(empIDInnerList);
                                }
 
                            }
                             empIDInnerList=null;
                        }
                    }

                    if (chkDept.Checked)
                    {
                        foreach (ListItem item in chkDeptAllList.Items)
                        {
                            if (item.Selected)
                            {
                                empIDInnerList = hrServer.GetEmpInfoByDeptWise(Convert.ToInt16(item.Value)).Select(aa => aa.EMPID).ToList();
                                if (empIDInnerList != null && empIDInnerList.Count > 0)
                                {
                                    empIDMasterList.AddRange(empIDInnerList);
                                }
 

                            }
                             empIDInnerList=null;
                        }
                    }

                    if (chkLevelAll.Checked)
                    {
                        foreach (ListItem item in chkLevelAllList.Items)
                        
                           
                            if (item.Selected)
                            {

                                empIDInnerList = hrServer.GetEmpInfoOnlyActiveByLevelID(Convert.ToInt16(item.Value)).Select(aa => aa.EMPID).ToList();
                                if (empIDInnerList != null && empIDInnerList.Count > 0)
                                {
                                    empIDMasterList.AddRange(empIDInnerList);
                                }
 

                            }
                         empIDInnerList=null;
                        }
                    }

                    foreach (ListItem item in chkCategoryAllList.Items)
                    {
                           
                            if (item.Selected)
                            {

                                empIDInnerList = hrServer.GetEmpInfoOnlyActiveByCategoryID(Convert.ToInt16(item.Value)).Select(aa => aa.EMPID).ToList();
                                if (empIDInnerList != null && empIDInnerList.Count > 0)
                                {
                                    empIDMasterList.AddRange(empIDInnerList);
                                }
 

                            }
                         empIDInnerList=null;
                    }

                    if (chkDeptHeadAll.Checked)
                    {
                        empIDInnerList = hrServer.GetActivelDepartmentHeads(out Msg).Select(aa => aa.EMPID).ToList();
                        if (empIDInnerList != null && empIDInnerList.Count > 0)
                        {
                            empIDMasterList.AddRange(empIDInnerList);
                        }
                        empIDInnerList = null;
 
                    }
                    if (chkDivisionHeadAll.Checked)
                    {
                        empIDInnerList = hrServer.GetAllDivisionalHeads(out Msg).Select(aa => aa.EMPID).ToList();
                        if (empIDInnerList != null && empIDInnerList.Count > 0)
                        {
                            empIDMasterList.AddRange(empIDInnerList);
                        }
                        empIDInnerList = null;

                    }
                    if (chkManagementAll.Checked)
                    {
                        empIDInnerList = hrServer.GetManagementInfoOnlyActive().Select(aa => aa.EMPID).ToList();
                        if (empIDInnerList != null && empIDInnerList.Count > 0)
                        {
                            empIDMasterList.AddRange(empIDInnerList);
                        }
                        empIDInnerList = null;

                    }


                    if (!string.IsNullOrEmpty(txtEIN.Text.Trim()))
                    {
                        txtEIN.Text = txtEIN.Text + ";";
                      string[] EIN_List= txtEIN.Text.Split(';');
                      empIDInnerList = new List<short>();
                      short myEin = new short();
                      foreach (string ein in EIN_List)
                      {
                          if(Int16.TryParse(ein,out myEin ))
                          {
                              if (hrServer.isEmployeeActive(myEin))
                              {
                                  empIDInnerList.Add(myEin);
                              }
                              else
                              {
                                  WebUtil.MessageBox_Show("In Valid EIN......", this);
                                  return;
 
                              }
                             
                          }

                      }

                      empIDMasterList.AddRange(empIDInnerList);
                      empIDInnerList = null;
                    }
                    empIDMasterList = empIDMasterList.Distinct().ToList();
                    if (empIDMasterList != null && empIDMasterList.Count <=0)
                    {
                        WebUtil.MessageBox_Show("Message sent to is empty. please tick at least one!!!",this);
                        return;
                    }
                    //HttpUtility.UrlDecode(editor.Content, System.Text.Encoding.Default);

                    if (Session["circularMsgID"] != null)
                    {
                  
                        if (Session["fileName"].ToString().Equals("1"))
                        {

                            DataSet dsCircularAttached = null;
                            dsCircularAttached = msgServer.GetCircularFileInfoByCircularID(Convert.ToInt32(Session["circularMsgID"]));
                            if (dsCircularAttached != null && dsCircularAttached.Tables[0].Rows.Count > 0)
                            {
                                foreach (DataRow row in dsCircularAttached.Tables[0].Rows)
                                {
                                    fileName = fileName + row["CIR_FILE_NAME"].ToString() + ",";
                                }
                                Session["fileName"] = fileName;
                            }

                        }
                    }

                  //  txtEditor.Text = txt.Text;

                    CIR_MESSAGE_INFO circular_info = new CIR_MESSAGE_INFO();

                    circular_info.CIR_MAIL_TYPE_ID = Convert.ToInt16(rdlMail.SelectedValue);
                    circular_info.EFFECTIVE_FROM_DATE = Convert.ToDateTime(txtEffectiveFrom.Text);
                    circular_info.EXPIRE_DATE = Convert.ToDateTime(txtExpireDate.Text);
                    circular_info.MSG_SUBJECT = txtSubject.Text;
                    circular_info.MSG_MESSAGE = editor.Content; //txtEditor.Text;
                    circular_info.MSG_OTHER_RECIPIENTS = txtOtherRecipients.Text;
                    circular_info.ADDEDAT = DateTime.Now;
                    circular_info.ADDEDBY = Convert.ToInt16(Session["loginid"]);
                    circular_info.INSTRUCTION = "";
                    circular_info.STATUS = "New";      // 1. New (message is added) 2. Confirmed 3. Viewed   
                 
                    Msg = msgServer.AddCircularInfo(circular_info,empIDMasterList);
                    if (Msg.Equals("Success"))
                    {
                        Msg = "Circular added successfully......";
                        empIDMasterList = new List<short>();
                        circular_info = null;
                        resetData();
                        
                        Session["fileName"] = 1;
                        if (Session["circularMsgID"] != null)
                        {
                            Session.Remove("circularMsgID");
                            Session["circularMsgID"] = null;
                        }
                       
                        WebUtil.jsCall("window.location='/Circular/CircularInfoDetails.aspx'", this);
                    }

                WebUtil.jsCall("alert('" + Msg + "')", this);
            }
            catch (Exception ex)
            {
                Msg = WebUtil.GetExceptionMessage(ex);

                LogWriter.WriteLogError(ex,"Circular","New Circular");
                WebUtil.jsCall("alert('" + Msg + "')", this);
            }
            //WebUtil.MessageBox_Show ( Msg, this );
        }

        [WebMethod(EnableSession = false)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static object SaveBytes1(object file, string directory, object filename, int loop, string isFinal)
        {
            try{
                var Directory = "~/Documents/";
                if (directory == "Documents")
                {
                    Directory = "~/Documents/";
                }
                else if (directory == "Zips")
                {
                    Directory = "~/Documents_Zips/";
                }
                if (isFinal == "Yes") {
                    var fullFile = File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath(Directory) + filename + "_temp.txt");

                    string ext = Path.GetExtension(filename.ToString());
                    string newfilename = Path.GetFileNameWithoutExtension(filename.ToString()) + "-" + DateTime.Now.ToString("dd-MMM-yyyy_HH:mm:ss").Replace(':', '-') + ext;

                    File.WriteAllBytes(System.Web.HttpContext.Current.Server.MapPath(Directory) + newfilename, Convert.FromBase64String(fullFile));
                    File.Delete(System.Web.HttpContext.Current.Server.MapPath(Directory) + filename + "_temp.txt");

                    return newfilename;
                }
                else if (loop == 0)
                {
                    File.WriteAllText(System.Web.HttpContext.Current.Server.MapPath(Directory + filename + "_temp.txt"), (string)file);
                }
                else
                {
                    File.AppendAllText(System.Web.HttpContext.Current.Server.MapPath(Directory + filename + "_temp.txt"), (string)file);
                }
                return "done!";
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return "Error!";
        }

        [WebMethod(EnableSession = false)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static object notifyDone(string directory, string filename)
        {
            if (directory == "Documents") {
                directory = "~/Documents/";
            }
            if (!System.Web.HttpContext.Current.Session["fileName"].ToString().Equals("1"))
            {
                var fileNameList = (string)System.Web.HttpContext.Current.Session["fileName"];
                fileNameList = fileNameList + directory + filename + ",";
                HttpContext.Current.Session.Add("fileName", fileNameList);
                return filename;
            }
            else {
                HttpContext.Current.Session.Add("fileName", directory + filename + ",");
                return filename;
            }
            return "Error!";
        }
                 
        protected void resetData()
        {
            //txtEditor.Text = "";
            editor.Content = "";          
            WebUtil.ClearFields(this.Controls);
        }
        protected void btnSendeMail_Click(object sender, EventArgs e)
        {
 
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                //txtEditor.Text = string.Empty;
                editor.Content = string.Empty;
                Session["fileName"] = 1;
                WebUtil.ClearFields(this.Controls);
            }
            catch (Exception ee)
            {
                WebUtil.MessageBox_Show(WebUtil.GetExceptionMessage(ee),this);
                
            }
        }

        //protected void ajxCircularfileUpload_UploadCompleteAll(object sender, AjaxFileUploadCompleteAllEventArgs e)
        //{
            
        //}


        ///File Upload
        ///
        protected void File_Upload(object sender, AjaxFileUploadEventArgs e)
        {
            try
            {
                string fileNameList = string.Empty;
                string filename = e.FileName;

                if (!Session["fileName"].ToString().Equals("1"))
                {
                    fileNameList = (string)Session["fileName"]; 
                }
               

                string strDestPath = Server.MapPath("~/Documents/");
                // string myfile = string.Empty;
                //myfile = e.FileId.ToString();

                ////1MB= 1048576 Bytes
                //if (e.FileSize > 1048576 || e.FileSize < 0)
                //{
                //    WebUtil.MessageBox_Show("File is too Large or invalid!!", this);
                //    return;
                //}


                string ext = Path.GetExtension(e.FileName);
              
                filename =  Path.GetFileNameWithoutExtension(e.FileName) + "-" + DateTime.Now.ToString("dd-MMM-yyyy_HH:mm:ss").Replace(':', '-') + ext;
                 

                string saveFileLocation = "~/Documents/" + filename;// "/" + subItemName + "/" + imageName;

                //string myFile = System.IO.Path.Combine(strDestPath, e.FileName);
                //if (File.Exists(myFile))
                //{
                //    WebUtil.MessageBox_Show("File already Exists!!!!!!!!!!!!",this);               
                //    return;
                //}

                
                //ajxCircularfileUpload.SaveAs(Server.MapPath(saveFileLocation));

                fileNameList = fileNameList + saveFileLocation + ","; 
                //System.Diagnostics.Debug.Print("total File:" + x.ToString());
              // Session.Clear();
                Session.Add("fileName", fileNameList);

                System.Diagnostics.Debug.Print("total File:" + fileNameList);


            }
            catch (Exception ex)
            {
                WebUtil.MessageBox_Show("Error:" + WebUtil.GetExceptionMessage(ex), this);
            }
        }

   
    }
}