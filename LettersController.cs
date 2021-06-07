using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DMVCS.Models;
using DMVCS.BusinessLogic;
using DMVCS.DataWorker;
using System.Configuration;
using MvcReportViewer;
using DMVCS.Attributes;

namespace DMVCS.Controllers
{

    public class LettersController : Controller
    {

        ErrorMessages _er = new ErrorMessages();
        DDLProvider _dlp = new DDLProvider();
        LetterDataAccess _lda = new LetterDataAccess();
        WindowsUserFormatter _userName = new WindowsUserFormatter();
        WindowsUserNameWorker _user = new WindowsUserNameWorker();
        LetterSignatureDataWorker _sig = new LetterSignatureDataWorker();
        DDLDataAccess _ddl = new DDLDataAccess();
        Validation _val = new Validation();
        WindowsUserNameWorker _userWorker = new WindowsUserNameWorker();
        CustomerDataAccess _cda = new CustomerDataAccess();
        DMVCSUserDataAccess _uda = new DMVCSUserDataAccess();

        LetterUtil _lu = new LetterUtil();
        public ActionResult LetterIndex()
        {
            #region UserValidation
            string currentUser = _userName.stripSOM(User.Identity.Name);
            bool validUser = _userWorker.ValidateUser(currentUser);
            if (!validUser)
            {
                return RedirectToAction("../ErrorPage/logInError");
            }
            else
            {

                #endregion

                CreateLetterModel model = new CreateLetterModel();
                model.submitDate = string.IsNullOrWhiteSpace(model.submitDate) ? DateTime.Now.ToShortDateString().ToString() : model.submitDate;
                model = CreateLetterLists(model);
                model.customer = TempData["Customer"] as Customer;

                #region Validate and Create Letter

                if (TempData["ValidateAndCreate"] != null)
                {
                    model = TempData["ValidateAndCreate"] as CreateLetterModel;
                    if (model.addLetterReq != null)
                    {
                        model.addLetterReq = validateDates(model.addLetterReq);
                        if (!model.addLetterReq.validRequirements)
                        {
                            #region validation
                            if (!model.addLetterReq.validCrashDate)
                            {
                                ModelState.AddModelError("CrashDateError", _er.InvalidDate);
                            }
                            if (!model.addLetterReq.validDueBackDate)
                            {
                                ModelState.AddModelError("DueBackByError", _er.InvalidDate);
                            }
                            if (!model.addLetterReq.validPhysStatementDate)
                            {
                                ModelState.AddModelError("PhysDateError", _er.InvalidDate);
                            }
                            if (!model.addLetterReq.validReexamDate)
                            {
                                ModelState.AddModelError("reexamDateError", _er.InvalidDate);
                            }
                            if (!model.addLetterReq.validReexamTime)
                            {
                                ModelState.AddModelError("ReexamTimeError", _er.InvalidTime);
                            }
                            if (!model.addLetterReq.validEffectiveDate)
                            {
                                ModelState.AddModelError("effectiveDateError", _er.InvalidDate);
                            }
                            #endregion
                            model.CityStateZipList = new SelectList(_cda.GetCityStateZip());

                            model.hiddenTabState = "addReqError";
                            return View(model);
                        }
                        else
                        {
                            model.sig = _sig.GetSignature();
                            model.currentUser = _uda.getInitialByUsername(currentUser);
                            model.addLetterReq.newRestrictionsFinal = model.addLetterReq.SelectedNewRestrictions != null ? commaDelimiter(model.addLetterReq.SelectedNewRestrictions) : null;
                            model.addLetterReq.unnecessaryRestrictionsFinal = model.addLetterReq.selectedUnnecessaryRestrictions != null ? commaDelimiter(model.addLetterReq.selectedUnnecessaryRestrictions) : null;
                            _lda.createLetter(model);

                            model.addLetterReq = clearAddRequirements(model.addLetterReq);

                            model.hiddenTabState = "letterQue";
                            TempData["backToIndex"] = model;
                            TempData["ValidateAndCreate"] = null;
                            return RedirectToAction("LetterIndex");
                        }
                    }

                }
                #endregion

                #region After Succesful letter Creation
                if (TempData["backToIndex"] != null)
                {
                    model = TempData["backToIndex"] as CreateLetterModel;
                    model.CustomerLetterHist = _lda.getQueByCustomer(model.customer.CustID, "getHistoryByCustomer");
                    model.CustomerLetterHist = setHistoryPaging(model);
                    model.CustomerLetterQue = _lda.getQueByCustomer(model.customer.CustID, "getHistoryByCustomer");
                    model.CustomerLetterQue = setQuePaging(model);
                    model.CityStateZipList = new SelectList(_cda.GetCityStateZip());
                    TempData["Cust"] = model.customer;
                    TempData["Customer"] = model.customer;
                    return View(model);
                }
                #endregion
                #region Paging
                if (TempData["paging"] != null)
                {
                    model = TempData["paging"] as CreateLetterModel;
                    model = CreateLetterLists(model);
                }
                #endregion
                #region first entry
                if (TempData["toLetters"] != null)
                {


                    CaseDetailsLettersModel cdModel = TempData["toLetters"] as CaseDetailsLettersModel;
                    model.caseDetail = cdModel.caseDetail;
                    model.CityStateZipList = new SelectList(_cda.GetCityStateZip());
                    model.customer = cdModel.customer;
                    TempData["toAddEditCustomer"] = model.customer;
                    model.hideCreateLetterTab = TempData["HideCreateLetterTab"] as string;
                    //model.hiddenTabState = "createLetter";
                }
                if (TempData["DeleteThese"] != null)
                {
                    model.hideCreateLetterTab = "False";
                    model.hiddenTabState = "letterQue";
                }
                if (TempData["backToLetterQue"] != null)
                {
                    model = TempData["backToLetterQue"] as CreateLetterModel;
                    model.hideCreateLetterTab = "False";
                    model.hiddenTabState = "letterQue";
                }
                #endregion

                #region Return Home
                if (TempData["LettersHome"] != null)
                { model = TempData["LettersHome"] as CreateLetterModel; }
                #endregion

                model.CustomerLetterHist = setHistoryPaging(model);
                model.CityStateZipList = new SelectList(_cda.GetCityStateZip());

                model.CustomerLetterQue = setQuePaging(model);
                TempData["Customer"] = model.customer;
                TempData["caseDetail"] = model.caseDetail;
                TempData.Keep("caseDetail");
                if (TempData["EntryTab"] != null)
                {
                    model.hiddenTabState = TempData["EntryTab"] as string;
                }
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult LetterIndex(CreateLetterModel model)
        {
            if (model.addLetterReq != null)
            {
                if (model.addLetterReq.cityStateZipFree != null && model.addLetterReq.cityStateZip == "Select City")
                {
                    model.addLetterReq.cityStateZip = model.addLetterReq.cityStateZipFree;
                }
            }
            model.caseDetail = TempData["caseDetail"] as CaseDetails;
            model.customer = TempData["Customer"] as Customer;
            TempData.Keep("caseDetail");
            model.CityStateZipList = new SelectList(_cda.GetCityStateZip());

            if (model.hiddenIsPreview)
            {
                TempData["ReportInfo"] = model;
                return RedirectToAction("LetterPreviewPage");
            }
            if (TempData["backToLetterQue"] != null)
            {
                model = TempData["backToLetterQue"] as CreateLetterModel;
                model.hiddenSwitchCase = "backToLetters";
                TempData["backToLetterQue"] = null;
            }
            if (TempData["backToLetterHistory"] != null)
            {
                model = TempData["backToLetterHistory"] as CreateLetterModel;
                model.hiddenSwitchCase = "LettersHome";
                // return RedirectToAction("LetterIndex");
            }
            //if(model.hiddenTabState == "caseDetails")
            //{
            //    return RedirectToAction("../Home/CaseLetterDetails");
            //}
            return LetterSwitchboard(model);

        }
        public ActionResult LetterPreviewPage(CreateLetterModel model)
        {
            string currentUser = _userName.stripSOM(User.Identity.Name);
            bool validUser = _userWorker.ValidateUser(currentUser);
            if (!validUser)
            {
                return RedirectToAction("../ErrorPage/logInError");
            }
            else
            {

                if (TempData["ReportInfo"] != null)
                {
                    model = TempData["ReportInfo"] as CreateLetterModel;
                    model.hiddenIsPreview = false;


                }
                else
                {
                    return RedirectToAction("../AddEditCustomer/AddEditIndex");
                    //  return RedirectToAction("../Home/Index");
                }
                TempData["backToLetterHistory"] = model;
                model.hiddenLetterCode = seperateLetterCode(model.hiddenLetterCode);
                return View(model);
            }
        }
        private ActionResult LetterSwitchboard(CreateLetterModel model)
        {
            switch (model.hiddenSwitchCase)
            {
                case "CreateLetter":
                    model = CreateLetterLists(model);
                    TempData["ValidateAndCreate"] = model;
                    return RedirectToAction("LetterIndex");
                case "CaseDetails":
                    TempData["Customer"] = model.customer;
                    return RedirectToAction("../Home2/CaseLetterDetails");
                case "EditCustomer":
                    TempData["EditCustomerDLN"] = model.customer.DLN;
                    return RedirectToAction("../AddEditCustomer/CustomerManagement");
                case "paging":
                case "GridSize":
                    TempData["paging"] = model;
                    return RedirectToAction("LetterIndex");
                case "LettersHome":
                    model = CreateLetterLists(model);
                    TempData["backToLetterHistory"] = null;
                    // model.hiddenTabState = "createLetter";
                    TempData["LettersHome"] = model;
                    return RedirectToAction("LetterIndex");
                case "Delete":
                    _lda.deleteSelectedLetters(model.hiddenLetterList);
                    TempData["DeleteThese"] = model;
                    return RedirectToAction("LetterIndex");
                case "Preview":
                    if (model.hiddenLetterList != null)
                    {
                        model = CreateLetterLists(model);

                        TempData["ToPreview"] = model;

                        return RedirectToAction("PrintPreview");
                    }
                    else
                    {
                        model.hiddenTabState = "letterQue";
                        TempData["paging"] = model;
                        return RedirectToAction("LetterIndex");
                    }
                case "backToLetters":
                    CaseDetailsLettersModel cd = new CaseDetailsLettersModel();
                    cd.caseDetail = model.caseDetail;
                    cd.customer = model.customer;
                    TempData["toLetters"] = cd;
                    return RedirectToAction("LetterIndex");
                default:
                    return RedirectToAction("../AddEditCustomer/AddEditIndex");
            }
        }
        public ActionResult PrintPreview()
        {
            #region UserValidation
            string currentUser = _userName.stripSOM(User.Identity.Name);

            bool validUser = _user.ValidateUser(currentUser);
            if (!validUser)
            {
                return RedirectToAction("../ErrorPage/logInError");
            }
            else
            {
                #endregion
                CreateLetterModel model = new CreateLetterModel();
                if (TempData["ToPreview"] != null)
                {
                    model = TempData["ToPreview"] as CreateLetterModel;
                    TempData["ToPreview"] = null;
                }
                else if (TempData["ToPrint"] != null)
                {
                    model = TempData["ToPrint"] as CreateLetterModel;
                    TempData["ToPrint"] = model;
                }
                string currentBatch = _lda.createBatch(currentUser, Convert.ToInt16(model.SelectedCopies));
                _lda.updateBatchesOnLetters(model.hiddenLetterList, currentBatch);
                _lda.lettersPrinted(model.hiddenLetterList, Convert.ToInt16(model.SelectedCopies), currentUser);
                //byte[] DocAsByte = generateWordDoc(_lu.GetLetterList(model.hiddenLetterList), Convert.ToInt16(model.SelectedCopies));
                //model.fileName = "https://" + ConfigurationManager.AppSettings.Get("SiteName") + "/DMVC_MVC/download/" + _lu.CreateFileName(_userName.stripSOM(User.Identity.Name));

                //string filename = ConfigurationManager.AppSettings.Get("LetterDirectory") + _lu.CreateFileName(_userName.stripSOM(User.Identity.Name));
                //System.IO.File.WriteAllBytes(filename, DocAsByte);

                TempData["backToLetterQue"] = model;
                return RedirectToAction("LetterIndex");
            }
        }

        private byte[] generateWordDoc(List<string> letterList, int copies)
        {
            CombineDocs combine = new CombineDocs();
            var path = ConfigurationManager.AppSettings.Get("ReportPath");
            byte[] returnfile = new byte[0];
            List<byte[]> byteList = new List<byte[]>();
            foreach (string letterID in letterList)
            {
                string myPath = path + _lda.getLetterCode(letterID);
                var myReport = this.Report(ReportFormat.WordOpenXml, myPath,
               new { PrintID = letterID }).FileStream;
                byte[] myFile = new byte[myReport.Length];
                for (int i = 0; i < copies; i++)
                {
                    byteList.Add(myFile);
                }

                var document = myReport.Read(myFile, 0, Convert.ToInt32(myReport.Length));
            }

            returnfile = combine.OpenAndCombine(byteList);
            return returnfile;
        }
        public ActionResult WordIndex()
        {
            var model = new CreateLetterModel();
            model = TempData["ToPrint"] as CreateLetterModel;
            TempData["ToPrint"] = model;
            return View(model);
        }

        [WordDocument]
        public ActionResult previewLetter()
        {

            ViewBag.WordDocumentFilename = "IndexDocument";
            return View("PrintPreview");
        }
        private List<Letter> setHistoryPaging(CreateLetterModel model)
        {
            if (model.customer == null)
            { return null; }
            else
            {
                List<Letter> tempLetterList = tempLetterList = _lda.getQueByCustomer(model.customer.CustID, "getHistoryByCustomer");
                if (tempLetterList.Count > 0)
                {
                    model.CustomerLetterHist = tempLetterList;
                    model.hiddenLetterCount = model.CustomerLetterHist.Count;
                }
                else
                {
                    model.CustomerLetterHist = null;
                    model.hiddenLetterCount = 0;
                }

                int cnt;
                if (!string.IsNullOrWhiteSpace(model.HistorySelectedGridSize) && model.HistorySelectedGridSize.Equals("All"))
                {
                    cnt = model.CustomerLetterHist.Count();
                }
                else
                {
                    cnt = string.IsNullOrWhiteSpace(model.HistoryPaging) ?
                     Convert.ToInt32(ConfigurationManager.AppSettings["Paging"]) : Convert.ToInt32(model.HistoryPaging);
                }
                if (model.CustomerLetterHist != null)
                {
                    model.historyTotal = Convert.ToString(model.CustomerLetterHist.Count);
                    if (String.IsNullOrEmpty(model.historyCount))
                    {
                        model.CustomerLetterHist = tempLetterList.Skip(0).Take(cnt).ToList();
                        model.historyCount = "0";
                    }
                    else
                    {
                        int skip = Convert.ToInt32(model.historyCount);
                        model.CustomerLetterHist = tempLetterList.Skip(skip).Take(cnt).ToList();
                    }
                }
                model.HistoryPaging = Convert.ToString(cnt);
                return model.CustomerLetterHist;
            }

        }
        private string seperateLetterCode(string code)
        {
            char[] splitBy = { ' ' };
            string[] splitUpCode = code.Split(splitBy);

            return splitUpCode[0];
        }
        private List<Letter> setQuePaging(CreateLetterModel model)
        {
            if (model.customer == null)
            {
                return null;
            }
            else
            {
                List<Letter> tempLetterList = tempLetterList = _lda.getQueByCustomer(model.customer.CustID, "getQueByCustomer");
                if (tempLetterList.Count > 0)
                {
                    model.CustomerLetterQue = tempLetterList;
                    model.hiddenLetterCount = model.CustomerLetterQue.Count;
                }
                else
                {
                    model.CustomerLetterQue = null;
                    model.hiddenLetterCount = 0;
                }

                int cnt;
                if (!string.IsNullOrWhiteSpace(model.SelectedGridSize) && model.SelectedGridSize.Equals("All"))
                {
                    cnt = model.CustomerLetterQue.Count();
                }
                else
                {
                    cnt = string.IsNullOrWhiteSpace(model.Paging) ?
                     Convert.ToInt32(ConfigurationManager.AppSettings["Paging"]) : Convert.ToInt32(model.Paging);
                }
                if (model.CustomerLetterQue != null)
                {
                    model.queTotal = Convert.ToString(model.CustomerLetterQue.Count);
                    //if (tempLetterList.Count <= cnt)
                    //{
                    //    ViewBag.Paging = "none";
                    //}
                    if (String.IsNullOrEmpty(model.queCount))
                    {
                        model.CustomerLetterQue = tempLetterList.Skip(0).Take(cnt).ToList();
                        model.queCount = "0";
                    }
                    else
                    {
                        int skip = Convert.ToInt32(model.queCount);
                        model.CustomerLetterQue = tempLetterList.Skip(skip).Take(cnt).ToList();
                        // model.CurrentCount = currentCount;
                    }
                }
                model.Paging = Convert.ToString(cnt);
                return model.CustomerLetterQue;
            }
        }




        public ActionResult CreateLetter(CreateLetterModel model)
        {
            if (TempData["fromCreateLetters"] != null)
            {
                model = TempData["fromCreateLetters"] as CreateLetterModel;
            }
            // LetterSwitchboard(model,"CreateLetter");
            return RedirectToAction("Index");
        }
        public ActionResult LetterQue(string currentCount)
        {
            string currentUser = _userName.stripSOM(User.Identity.Name);
            bool validUser = _user.ValidateUser(currentUser);
            if (!validUser)
            {
                return RedirectToAction("../ErrorPage/logInError");
            }
            else
            {

                if (TempData["toLetterQue"] != null)
                { CreateLetterModel CLmodel = TempData["toLetterQue"] as CreateLetterModel; }
                LetterQueModel model = new Models.LetterQueModel();
                model.hiddenReviewer = string.IsNullOrWhiteSpace(model.hiddenReviewer) ?
                    currentUser : model.hiddenReviewer;
                model.letterQueHeaders = _dlp.LetterQueGridHeaders();

                List<Letter> letters = _lda.getQueByReviewer(_user.getInitialsFromUserName(currentUser));

                int cnt = Convert.ToInt32(ConfigurationManager.AppSettings["Paging"]);
                model.TotalCount = Convert.ToString(letters.Count);
                if (letters.Count <= cnt)
                {
                    ViewBag.Paging = "none";
                }

                if (String.IsNullOrEmpty(currentCount))
                {

                    model.letterList = letters.Skip(0).Take(cnt).ToList();
                    model.CurrentCount = "0";
                }
                else
                {
                    int skip = Convert.ToInt32(currentCount);
                    model.letterList = letters.Skip(skip).Take(cnt).ToList();
                    model.CurrentCount = currentCount;
                }
                model.Paging = Convert.ToString(cnt);
                return View(model);
            }
        }
        private CreateLetterModel CreateLetterLists(CreateLetterModel model)
        {
            model.Copies = new SelectList(_dlp.Copies());
            model.GridSize = new SelectList(_dlp.Display());
            model.HistoryGridSize = new SelectList(_dlp.Display());
            model.letterQueHeaders = _dlp.LetterQueGridHeaders();
            model.CustomerHistoryHeaders = _dlp.CustomerHistoryHeaders();
            model.LetterCodeList = new SelectList(_ddl.getLetterCodes());
            model.ReviewerList = new SelectList(_ddl.getReviewerInit(model.currentUser));
            if (model.addLetterReq == null)
            { model.addLetterReq = new AdditionalLetterRequirements(); }
            model.addLetterReq.UnnecessaryRestrictions = _ddl.getRestrictions();
            model.addLetterReq.NewRestrictions = _ddl.getRestrictions();
            model.addLetterReq.analystLocations = _ddl.getBranchIDs();
            return model;
        }
        private AdditionalLetterRequirements clearAddRequirements(AdditionalLetterRequirements ar)
        {
            ar = new AdditionalLetterRequirements();
            ar.validCrashDate = true;
            ar.validDueBackDate = true;
            ar.validPhysStatementDate = true;
            ar.validReexamDate = true;
            ar.validReexamTime = true;
            ar.validEffectiveDate = true;
            ar.NewRestrictions = _ddl.getRestrictions();
            ar.UnnecessaryRestrictions = _ddl.getRestrictions();
            ar.analystLocations = _ddl.getBranchIDs();
            return ar;

        }
        private AdditionalLetterRequirements validateDates(AdditionalLetterRequirements addLetterReq)
        {
            addLetterReq.validCrashDate = string.IsNullOrWhiteSpace(addLetterReq.crashDate) ? true :
                    _val.validateDate(addLetterReq.crashDate, 1700, 2050);
            addLetterReq.validDueBackDate = string.IsNullOrWhiteSpace(addLetterReq.dueBackBy) ? true :
                _val.validateDate(addLetterReq.dueBackBy, 1990, 2050);
            addLetterReq.validPhysStatementDate = string.IsNullOrWhiteSpace(addLetterReq.physStatementDate) ? true :
                _val.validateDate(addLetterReq.physStatementDate, 1700, 2050);
            addLetterReq.validReexamDate = string.IsNullOrWhiteSpace(addLetterReq.reexamDate) ? true :
                _val.validateDate(addLetterReq.reexamDate, 1700, 2050);
            addLetterReq.validReexamTime = string.IsNullOrWhiteSpace(addLetterReq.reexamTime) ? true :
                _val.validateTime(addLetterReq.reexamTime);
            addLetterReq.validEffectiveDate = string.IsNullOrWhiteSpace(addLetterReq.EffectiveDate) ? true :
                _val.validateDate(addLetterReq.EffectiveDate, 1700, 2050);

            List<bool> isValid = new List<bool>() { addLetterReq.validCrashDate, addLetterReq.validDueBackDate ,
                                                    addLetterReq.validPhysStatementDate, addLetterReq.validReexamDate,
                                                      addLetterReq.validReexamTime,addLetterReq.validEffectiveDate};
            if (isValid.Contains(false))
            { addLetterReq.validRequirements = false; }
            else
            { addLetterReq.validRequirements = true; }
            return addLetterReq;
        }

        private string commaDelimiter(List<string> list)
        {
            string s = string.Empty;
            int x = 0;
            foreach (var item in list)
            {

                s += item;
                x++;
                if (x < list.Count)
                { s += ", "; }
            }
            return s;
        }

    }
}