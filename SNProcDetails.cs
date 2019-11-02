using Cerner.Foundations.Data;
using SNCommon;
using System;
using System.Collections;

namespace Cerner.SurginetReusable
{
    public class CProcDetails
    : IEnumerable
    {
        //Force explicit variable declartion. //Set default array subscripts to 1. //Set the string comparison method to Text.

        //Internationalization Constant
        private const string msMODULE_NAME = "CProcDetails";

        private int mlKey = 0;
        private const int mnNbrOfKeys = 3;
        private const int mnKey = 1;
        private const int mnCatalogDispKey = 2;
        private const int mnCatalogCdKey = 3;
        private int mnLastKey = 0;

        private CProcDetail moLastProcDetail = null;

        private Cerner.Foundations.Collections.ICernCollection _moProcDetails = null;
        private Cerner.Foundations.Collections.ICernCollection moProcDetails
        {
            get
            {
                if (_moProcDetails == null)
                {
                    _moProcDetails = new Cerner.Foundations.Collections.CernCollection<CProcDetail>();
                }
                return _moProcDetails;
            }
            set
            {
                _moProcDetails = value;
            }
        }
        //Gets the specific Procedure Detail based on an index
        public CProcDetail this[int lIndexKey]
        {
            get
            {
                CProcDetail result = null;
                if (moProcDetails != null)
                {
                    if (moProcDetails.Count > 0)
                    {
                        result = (CProcDetail)moProcDetails.Item(lIndexKey, null);
                        mnLastKey = 0;
                        moLastProcDetail = null;
                    }
                }
                return result;
            }
        }
        public int Count
        {
            get
            {
                return moProcDetails.Count;
            }
        }

        //Returns the number of Procedure Details in the collection which are not deleted
        public int ActiveCount
        {
            get
            {
                int result = 0;

                for (int lIndex = 1; lIndex <= Count; lIndex++)
                {
                    if (this[lIndex].nState != Application.SNStateConstants.snStateDeleted)
                    {
                        result++;
                    }
                }
                return result;
            }
        }

        //Returns the number of Procedure Details in the collection that have an error
        public int ErrorCount
        {
            get
            {
                int result = 0;

                for (int lIndex = 1; lIndex <= Count; lIndex++)
                {
                    if (this[lIndex].bHasInvalidData())
                    {
                        result++;
                    }
                }
                return result;
            }
        }


        //Marks all Procedure Details as a Duplicate within the collection
        public bool MarkAsDuplicates
        {
            set
            {
                CProcDetail oProcDtl = null;

                for (int lIndex = 1; lIndex <= Count; lIndex++)
                {
                    oProcDtl = this[lIndex];
                    if (value)
                    {
                        oProcDtl.oCatalog.AddError(CCdDispError.SnFieldError.DUPLICATE);
                    }
                    else
                    {
                        oProcDtl.oCatalog.RemoveError(CCdDispError.SnFieldError.DUPLICATE);
                    }
                }
            }
        }

        public bool DeleteByKey(string sKey)
        {

            CProcDetail oProcDetail = get_ItemByKey(sKey);

            if (oProcDetail == null)
            {
                return false;
            }

            if (oProcDetail.nState == Application.SNStateConstants.snStateAdded)
            {
                RemoveByKey(sKey);
            }
            else
            {
                get_ItemByKey(sKey).SetToDeletedState();
            }
            return false;
        }

        //Creates a new ProcDetail with default attributes
        public bool bCreateDefaultProcDetail(CProcedure oProcedure, CSurgAreas oAreas)
        {
            bool result = false;
            CProcDetail oProcDetail = new CProcDetail();

            if (oProcedure != null)
            {
                oProcDetail.dCatalogCd = oProcedure.dCatalogCd;
                oProcDetail.sCatalogDisp = oProcedure.sProcedureName;
                oProcDetail.SetToAddedState();
                oProcDetail.dSurgProcDetailID = oAreas.lGetNewID();
                oProcDetail.dCaseLevelCd = 0;
                oProcDetail.dSurgSpecialtyId = 0;
                oProcDetail.dWoundClassCd = 0;
                oProcDetail.dAnesthesiaTypeCd = 0;
                oProcDetail.oProcDurations.NewDefaultProcDuration(oProcDetail.dSurgProcDetailID);

                Add(oProcDetail);
                result = true;
            }
            return result;
        }

        //Creates a copy of a Procedure Detail
        public CProcDetail oCreateCopiedProcDetail(CSurgAreas oAreas, CProcDetail oProcDetail)
        {
            CProcDetail oNewProcDetail = new CProcDetail();
            CProcDuration oProcDur = null;

            oNewProcDetail.Copy(oProcDetail, false, true);
            oNewProcDetail.SetToAddedState();
            oNewProcDetail.dSurgProcDetailID = oAreas.lGetNewID();

            for (int lDurIndex = 1, iteratorTest = oNewProcDetail.oProcDurations.Count; lDurIndex <= iteratorTest; lDurIndex++)
            {
                oProcDur = oNewProcDetail.oProcDurations[lDurIndex];
                oProcDur.dSurgProcDetailID = oNewProcDetail.dSurgProcDetailID;
            }

            Add(oNewProcDetail);
            return oNewProcDetail;
        }

        //Creates a new Procedure Detail which is blank
        public CProcDetail CreateBlankProcDetail(CSurgAreas oAreas)
        {
            CProcDetail oProcDetail = new CProcDetail();

            oProcDetail.dCatalogCd = -1;
            oProcDetail.sCatalogDisp = System.String.Empty;
            oProcDetail.SetToAddedState();
            oProcDetail.dSurgProcDetailID = oAreas.lGetNewID();
            oProcDetail.dCaseLevelCd = -1;
            oProcDetail.dSurgSpecialtyId = -1;
            oProcDetail.dWoundClassCd = -1;
            oProcDetail.dAnesthesiaTypeCd = -1;

            Add(oProcDetail);
            return oProcDetail;
        }

        //Updates the Keys for the Cerner Collection when a ProcDetail is changed
        public bool bUpdateKey(CProcDetail oProcDetail)
        {
            bool temp = moProcDetails.ChangeKey(String.Format("{0:0000000000}", oProcDetail.lKey), (short)mnCatalogDispKey, oProcDetail.sCatalogDisp.ToUpper());
            temp = moProcDetails.ChangeKey(String.Format("{0:0000000000}", oProcDetail.lKey), (short)mnCatalogCdKey, oProcDetail.dCatalogCd.ToString());
            return false;
        }

        public IEnumerator GetEnumerator()
        {
            return moProcDetails.GetEnumerator();
        }

        //Gets the specific Procedure Detail based on its lKey
        public CProcDetail get_ItemByKey(string sIndexKey)
        {
            CProcDetail result = null;
            result = (CProcDetail)moProcDetails.Item(String.Format("{0:0000000000}", Convert.ToInt32(sIndexKey)), mnKey);
            mnLastKey = mnKey;
            moLastProcDetail = result;
            return result;
        }

        //Gets the specific Procedure Detail based on its Procedure Catalog Code
        public CProcDetail get_ItemByCatalogCd(double dCatalogCd)
        {
            CProcDetail result = null;
            result = (CProcDetail)moProcDetails.Item(dCatalogCd.ToString(), mnCatalogCdKey);
            mnLastKey = mnCatalogCdKey;
            moLastProcDetail = result;
            if (result == null)
            {
                return result;
            }
            if (result.dCatalogCd != dCatalogCd)
            {
                result = null;
                moLastProcDetail = null;
            }
            return result;
        }


        //Adds a Procedure Detail to the collection
        public void Add(CProcDetail oNewProcDetail)
        {
            if (oNewProcDetail != null)
            {
                
                mlKey++;
                oNewProcDetail.lKey = mlKey;
                moProcDetails.Add(oNewProcDetail, String.Format("{0:0000000000}", oNewProcDetail.lKey), oNewProcDetail.sCatalogDisp.ToUpper(), oNewProcDetail.dCatalogCd.ToString(), null, null, null, null, null, null, null);
            }
        }

        public void AddCopyDetails(CProcDetail oNewProcDetail)
        {
            if (oNewProcDetail != null)
            {

                mlKey++;
                oNewProcDetail.lKey = mlKey;
                moProcDetails.Add(oNewProcDetail, String.Format("{0:0000000000}", oNewProcDetail.lKey), oNewProcDetail.sCatalogDisp.ToUpper(), oNewProcDetail.dCatalogCd.ToString(), null, null, null, null, null, null, null);
            }
        }

        //Adds a Copy of a Procedure Detail to the Collection by not updating its key
        public void AddCopy(CProcDetail oProcDetail)
        {
            moProcDetails.Add(oProcDetail, String.Format("{0:0000000000}", oProcDetail.lKey), oProcDetail.sCatalogDisp.ToUpper(), oProcDetail.dCatalogCd.ToString(), null, null, null, null, null, null, null);
        }

        //Adds two Collections together
        public void AddDetails(CProcDetails oNewProcDetails, CSurgAreas oAreas)
        {
            CProcDetail oNewProcDetail = null;

            for (int lIndex = 1, iteratorTest = oNewProcDetails.Count; lIndex <= iteratorTest; lIndex++)
            {
                oNewProcDetail = new CProcDetail();
                oNewProcDetail.Copy(oNewProcDetails[lIndex]);
                oNewProcDetail.dSurgProcDetailID = oAreas.lGetNewID();
                oNewProcDetail.oProcDurations.oDefaultProcDuration().dSurgProcDetailID = oNewProcDetail.dSurgProcDetailID;
                Add(oNewProcDetail);
            }
        }

        //Remove the old ProcDetail if necessary and replaces it with the Procedure Detail in the parameters
        public void Replace(CProcDetail oNewProcDetail, ref  string sKey)
        {
            CProcDetail oProcDetail = new CProcDetail();

            if (!string.IsNullOrEmpty(sKey))
            {
                DeleteByKey(sKey);
            }
            oProcDetail.Copy(oNewProcDetail);
            oProcDetail.CopySegments(oNewProcDetail.obFileSegments);
            AddCopyDetails(oProcDetail);
            sKey = oProcDetail.lKey.ToString();
        }

        //Removes a specific Procedure Detail based on its lKey
        private void RemoveByKey(string sIndexKey)
        {
           moProcDetails.Remove(String.Format("{0:0000000000}", Convert.ToInt32(sIndexKey)), mnKey);
           
        }

        public CProcDetails()
        {
            moProcDetails.NbrKeys = (short)mnNbrOfKeys;
        }

        internal void SrvGet(SNCommon.CRequest oRequest, Instance hArea)
        {
            Instance hProcs = null;
            CProcDetail oNewProcDetail = null;
            Index surg_proc_detail_index = new Index("SURG_PROC_DETAIL");
            if (oRequest.ReplyInstance.ToString().Contains(surg_proc_detail_index.Name))
            {
                if (hArea.GetListCount(surg_proc_detail_index) > 0)
                {
                    int lReplyCnt = hArea.GetListCount(surg_proc_detail_index);// oRequest.SNSrvGetItemCount(hArea, "surg_proc_detail") - 1;

                    for (int lIndex = 0; lIndex < lReplyCnt; lIndex++)
                    {
                        hProcs = hArea.GetList(surg_proc_detail_index, lIndex);// oRequest.SNSrvGetItem(hArea, "surg_proc_detail", lIndex);
                        oNewProcDetail = new CProcDetail();
                        oNewProcDetail.SrvGet(oRequest, hProcs);
                        Add(oNewProcDetail);
                        oNewProcDetail = null;
                    }
                }
            }
        }

        public void Clear()
        {
            moProcDetails = null;
        }

        //Determines if the Procedure Detail already exists and if it does, marks all of them as a duplicate
        public CProcDetails CheckForDuplicates()
        {
            CProcDetails result = null;
            CProcDetail oProcDtl = null;

            for (int lIndex = 1; lIndex <= Count; lIndex++)
            {
                oProcDtl = this[lIndex];
                if (oProcDtl.nState != Application.SNStateConstants.snStateDeleted)
                {
                    if (!oProcDtl.bIsChecked || oProcDtl.oCatalog.bDuplicateError)
                    {
                        result = oGetDuplicates(oProcDtl);
                        result.MarkAsDuplicates = result.Count > 1;
                        oProcDtl.bIsChecked = true;
                    }
                }
            }
            return result;
        }

        //Gets all Procedure Details that are for the same Procedure within the Collection
        public CProcDetails oGetDuplicates(CProcDetail oProcDtl)
        {

            CProcDetails result = null;
            result = new CProcDetails();
            CProcDetail otempProcDtl = get_ItemByCatalogCd(oProcDtl.dCatalogCd);
            // cerdcc  -- overlook -1 as potential duplicate
            while (otempProcDtl != null)
            {

                if (otempProcDtl.dCatalogCd > 0)
                {
                    result.AddCopy(otempProcDtl);
                }
                otempProcDtl = NextItem();
            }
            return result;
        }

        //Determines whether any of the Procedure Details have any errors
        public int bHasInvalidData()
        {
            CProcDetail oProcDetail = null;

            for (int lIndex = 1; lIndex <= Count; lIndex++)
            {
                oProcDetail = this[lIndex];
                if (oProcDetail.nState != Application.SNStateConstants.snStateDeleted)
                {
                    if (oProcDetail.bHasInvalidData())
                    {
                        return -1;
                    }
                }
            }
            return 0;
        }

        //Returns a Deep Copy of the object
        public void Copy(CProcDetails oprocDetails, ref  bool bCopyDurations, ref  bool bCopyDefault)
        {
            CProcDetail oProcDetail = new CProcDetail();

            for (int lIndex = 1, iteratorTest = oprocDetails.Count; lIndex <= iteratorTest; lIndex++)
            {
                oProcDetail = new CProcDetail();
                oProcDetail.Copy(oprocDetails.get_ItemByKey(lIndex.ToString()), bCopyDurations, bCopyDefault);
                AddCopyDetails(oProcDetail);
            }
        }

        public void Copy(CProcDetails oprocDetails, ref  bool bCopyDurations)
        {
            bool tempRefParam = false;
            Copy(oprocDetails, ref bCopyDurations, ref tempRefParam);
        }

        public void Copy(CProcDetails oprocDetails)
        {
            bool tempRefParam2 = true;
            bool tempRefParam3 = false;
            Copy(oprocDetails, ref tempRefParam2, ref tempRefParam3);
        }
        public void CopyFileSegments(CProcDetails oprocDetails)
        {
            int lIndex = 1;
            foreach (CProcDetail oProcDetail in moProcDetails)
            {
                oProcDetail.CopySegments(oprocDetails.get_ItemByKey(lIndex.ToString()).obFileSegments);
                lIndex++;
            }
        }
        public void CopyAreaSegments(CSurgArea oArea, CProcedures moProcedures)
        {
            CProcedure oProcedure = null;
            byte[] bSegArray = null;

            foreach (CProcDetail oProcDetail in moProcDetails)
            {
                oProcedure = moProcedures.get_ProcedureByCatalogCd(oProcDetail.dCatalogCd);
                if (oProcedure != null)
                {
                    oProcedure.GetSegmentArray(oArea, ref bSegArray);
                    oProcDetail.CopySegments(bSegArray);
                }
            }
        }
        //Called after a save to remove any Procedure Details that were pending deletion
        public object Cleanup()
        {
            string sKey = String.Empty;

            if (moProcDetails.Count > 0)
            {
                for (int lItemKey = moProcDetails.Count; lItemKey >= 1; lItemKey--)
                {
                    if (((CProcDetail)moProcDetails.Item(lItemKey, null)).nState == Application.SNStateConstants.snStateDeleted)
                    {
                        sKey = Convert.ToString(((CProcDetail)moProcDetails.Item(lItemKey, null)).lKey);
                        RemoveByKey(sKey);
                    }
                    else
                    {
                        ((CProcDetail)moProcDetails.Item(lItemKey, null)).Cleanup();
                    }
                }
            }
            return null;
        }

        //collection dump routine.  Class will dump it's own variables then call to dump it's children
        public void PopulateCollectionDump(ref  Cerner.PcsCollectionDumps.CCollectionDumpCtlr oDumps, string sParent)
        {
            CProcDetail oProcDetail = null;
            string sKeyText = String.Empty;

            oDumps.SetDump("mlKey", mlKey.ToString(), sParent + "mlKey", sParent, true);
            oDumps.SetDump("moLastProcDetail", System.String.Empty, sParent + "moLastProcDetail", sParent, true);
            if (moLastProcDetail != null)
            {
                moLastProcDetail.PopulateCollectionDump(ref oDumps, sParent + "moLastProcDetail");
            }

            //now loop through and dump all of it's children (items contains in the collection)
            for (int lIndex = 1, iteratorTest = moProcDetails.Count; lIndex <= iteratorTest; lIndex++)
            {
                oProcDetail = (CProcDetail)moProcDetails.Item(lIndex, null);
                sKeyText = sParent + oProcDetail.lKey.ToString();
                //add a "key" node so our individual items are collapsable
                oDumps.SetDump("ProcDetail", oProcDetail.lKey.ToString(), sKeyText, sParent, false);
                //dump out item under that node
                oProcDetail.PopulateCollectionDump(ref oDumps, sKeyText);
            }
        }

        //Returns the next ProcDetail object in the collection using the current (previously used) key.
        public CProcDetail NextItem()
        {
            CProcDetail oNextItem = (CProcDetail)moProcDetails.Next();

            if (mnLastKey > 0)
            {
                return ValidateNextItem(oNextItem);
            }
            else
            {
                return oNextItem;
            }
        }

        //Will validate if the next item matches the key and return the item if it does, nothing if it does not
        private CProcDetail ValidateNextItem(CProcDetail oNextItem)
        {
            CProcDetail result = null;
            bool bItemQualifys = false;

            //if the item passed in is nothing or the last item is nothing, return nothing
            if (moLastProcDetail == null || oNextItem == null)
            {
                moLastProcDetail = null;
                mnLastKey = 0;
                //exit to return nothing
                return result;
            }
            switch (mnLastKey)
            {
                case mnCatalogCdKey:
                    if (oNextItem.dCatalogCd == moLastProcDetail.dCatalogCd && oNextItem.nState != Application.SNStateConstants.snStateDeleted)
                    {
                        bItemQualifys = true;
                    }
                    break;
                case mnCatalogDispKey:
                    if (oNextItem.sCatalogDisp == moLastProcDetail.sCatalogDisp && oNextItem.nState != Application.SNStateConstants.snStateDeleted)
                    {
                        bItemQualifys = true;
                    } break;
                default:
                    bItemQualifys = false;
                    break;
            }

            //check if item qualified and if so, return it
            if (bItemQualifys)
            {
                result = oNextItem;
            }
            else
            {
                //else reset the variables and exit (returning nothing)
                moLastProcDetail = null;
                mnLastKey = 0;
            }
            return result;
        }
    }
}