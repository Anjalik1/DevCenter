using Cerner.ApplicationFramework.ConversionSupport.Gui;
using Cerner.ApplicationFramework.ConversionSupport.VB6;
using Cerner.Foundations.Measurement;
using Cerner.SurginetReusable;
using Microsoft.VisualBasic;
using System;
using System.Drawing;
using System.Windows.Forms;
using VB6 = Cerner.ApplicationFramework.ConversionSupport.VB6.Support;

namespace Cerner.SNProcedureDetails
{
    internal partial class frmProcedureDetails
         : Cerner.ApplicationFramework.GUI.CernerStyledControls.CernerStyledForm
    {
        private const int glREQ_FIELD_COLOR = 8454143; //yellow as per human factors
        private const string msWARNING = "Warning";

        private CSurgAreas moSurgAreas = null;
        private CSurgAreas moSurgAreasBackup = null;
        private bool mbClearingFields = false;
        private bool mbPopulatingFields = false;

        //surgery defined data
        private int mlSurgDef1CodeSet = 0;
        private int mlSurgDef2CodeSet = 0;
        private int mlSurgDef3CodeSet = 0;
        private int mlSurgDef4CodeSet = 0;
        private int mlSurgDef5CodeSet = 0;
        private string msSurgDef1Prompt = String.Empty;
        private string msSurgDef2Prompt = String.Empty;
        private string msSurgDef3Prompt = String.Empty;
        private string msSurgDef4Prompt = String.Empty;
        private string msSurgDef5Prompt = String.Empty;

        //Change Management
        #region "DmDcmLib related properties"

        public ToolStripMenuItem ChangeManagementParentMenu
        {
            get { return taskToolStripMenuItem; }
        }
        #endregion

        private Collection _moSelectedAreas = null;
        private Collection moSelectedAreas
        {
            get
            {
                if (_moSelectedAreas == null)
                {
                    _moSelectedAreas = new Collection();
                }
                return _moSelectedAreas;
            }
            set
            {
                _moSelectedAreas = value;
            }
        }

        private SNCommon.CCodeValues AnesthesiaTypes
        {
            get
            {
                SNCommon.CCodeSet oCodeSet = modMain.gSNCommon.CodeSets[modProcedureDetails.glANESTH_TYPES_CODE_SET.ToString()];
                if (oCodeSet == null)
                {
                    oCodeSet = modMain.gSNCommon.CodeSets.Add(modProcedureDetails.glANESTH_TYPES_CODE_SET);
                    oCodeSet.bLoad();
                }
                return oCodeSet.CodeValues;
            }
        }


        private SNCommon.CCodeValues WoundClasses
        {
            get
            {
                SNCommon.CCodeSet oCodeSet = modMain.gSNCommon.CodeSets[modProcedureDetails.glWOUND_CLASS_CODE_SET.ToString()];
                if (oCodeSet == null)
                {
                    oCodeSet = modMain.gSNCommon.CodeSets.Add(modProcedureDetails.glWOUND_CLASS_CODE_SET);
                    oCodeSet.bLoad();
                }
                return oCodeSet.CodeValues;
            }
        }


        private SNCommon.CSpecialties Specialties
        {
            get
            {
                SNCommon.CSpecialties oSpecialties = modMain.gSNCommon.Specialties;
                if (oSpecialties.Count == 0)
                {
                    modMain.gSNCommon.Specialties.bLoad();
                }
                return oSpecialties;
            }
        }


        private SNCommon.CCodeValues CaseLevels
        {
            get
            {
                SNCommon.CCodeSet oCodeSet = modMain.gSNCommon.CodeSets[modProcedureDetails.glCASE_LEVEL_CODE_SET.ToString()];
                if (oCodeSet == null)
                {
                    oCodeSet = modMain.gSNCommon.CodeSets.Add(modProcedureDetails.glCASE_LEVEL_CODE_SET);
                    oCodeSet.bLoad();
                }
                return oCodeSet.CodeValues;
            }
        }


        private SNCommon.CCodeValues SurgeryDefined1
        {
            get
            {
                SNCommon.CCodeSet oCodeSet = modMain.gSNCommon.CodeSets[mlSurgDef1CodeSet];
                if (oCodeSet == null)
                {
                    oCodeSet = modMain.gSNCommon.CodeSets.Add(mlSurgDef1CodeSet);
                    oCodeSet.bLoad();
                }
                return oCodeSet.CodeValues;
            }
        }


        private SNCommon.CCodeValues SurgeryDefined2
        {
            get
            {
                SNCommon.CCodeSet oCodeSet = modMain.gSNCommon.CodeSets[mlSurgDef2CodeSet];
                if (oCodeSet == null)
                {
                    oCodeSet = modMain.gSNCommon.CodeSets.Add(mlSurgDef2CodeSet);
                    oCodeSet.bLoad();
                }
                return oCodeSet.CodeValues;
            }
        }


        private SNCommon.CCodeValues SurgeryDefined3
        {
            get
            {
                SNCommon.CCodeSet oCodeSet = modMain.gSNCommon.CodeSets[mlSurgDef3CodeSet];
                if (oCodeSet == null)
                {
                    oCodeSet = modMain.gSNCommon.CodeSets.Add(mlSurgDef3CodeSet);
                    oCodeSet.bLoad();
                }
                return oCodeSet.CodeValues;
            }
        }


        private SNCommon.CCodeValues SurgeryDefined4
        {
            get
            {
                SNCommon.CCodeSet oCodeSet = modMain.gSNCommon.CodeSets[mlSurgDef4CodeSet];
                if (oCodeSet == null)
                {
                    oCodeSet = modMain.gSNCommon.CodeSets.Add(mlSurgDef4CodeSet);
                    oCodeSet.bLoad();
                }
                return oCodeSet.CodeValues;
            }
        }


        private SNCommon.CCodeValues SurgeryDefined5
        {
            get
            {
                SNCommon.CCodeSet oCodeSet = modMain.gSNCommon.CodeSets[mlSurgDef5CodeSet];
                if (oCodeSet == null)
                {
                    oCodeSet = modMain.gSNCommon.CodeSets.Add(mlSurgDef5CodeSet);
                    oCodeSet.bLoad();
                }
                return oCodeSet.CodeValues;
            }
        }

        private double mdCurOrderCatalogCd = 0;
        private string msProcedureName = String.Empty;
        private CProcedure moProcedure = null;

        private const string msFIELD_SPECIALTY = "SPECIALTY";
        private const string msFIELD_CASE_LEVEL = "CASE_LEVEL";
        private const string msFIELD_WOUND_CLASS = "WOUND_CLASS";
        private const string msFIELD_ANES_TYPE = "ANES_TYPE";
        private const string msFIELD_PROC_DUR = "PROC_DUR";
        private const string msFIELD_PROC_COUNT = "PROC_CNT";
        private const string msFIELD_SETUP_TIME = "SETUP_TIME";
        private const string msFIELD_PRE_TIME = "PRE_TIME";
        private const string msFIELD_POST_TIME = "POST_TIME";
        private const string msFIELD_CLEAN_TIME = "CLEAN_TIME";
        private const string msFIELD_SPEC_REQ = "SPEC_REQ";
        private const string msFIELD_FROZEN = "FROZEN";
        private const string msFIELD_BLOOD = "BLOOD";
        private const string msFIELD_XRAY = "XRAY";
        private const string msFIELD_XRAY_TECH = "XRAY_TECH";
        private const string msFIELD_IMPLANT = "IMPLANT";
        private const string msFIELD_SURG_DEF1 = "SURG_DEF1";
        private const string msFIELD_SURG_DEF2 = "SURG_DEF2";
        private const string msFIELD_SURG_DEF3 = "SURG_DEF3";
        private const string msFIELD_SURG_DEF4 = "SURG_DEF4";
        private const string msFIELD_SURG_DEF5 = "SURG_DEF5";

        private bool mbArrowingThroughList = false;

        public bool ShowForm(ref  CSurgAreas oSurgAreas, CProcedure oProcedure, double dOrderCatalogCd, int lSurgDef1CodeSet, int lSurgDef2CodeSet, int lSurgDef3CodeSet, int lSurgDef4CodeSet, int lSurgDef5CodeSet, string sSurgDef1Prompt, string sSurgDef2Prompt, string sSurgDef3Prompt, string sSurgDef4Prompt, string sSurgDef5Prompt, string sProcedureName)
        {

  
            this.Cursor = Cursors.WaitCursor;
            moSurgAreas = oSurgAreas;
            mdCurOrderCatalogCd = dOrderCatalogCd;
            moProcedure = oProcedure;
            mlSurgDef1CodeSet = lSurgDef1CodeSet;
            mlSurgDef2CodeSet = lSurgDef2CodeSet;
            mlSurgDef3CodeSet = lSurgDef3CodeSet;
            mlSurgDef4CodeSet = lSurgDef4CodeSet;
            mlSurgDef5CodeSet = lSurgDef5CodeSet;
            msSurgDef1Prompt = sSurgDef1Prompt;
            msSurgDef2Prompt = sSurgDef2Prompt;
            msSurgDef3Prompt = sSurgDef3Prompt;
            msSurgDef4Prompt = sSurgDef4Prompt;
            msSurgDef5Prompt = sSurgDef5Prompt;

            //create a backup copy
            moSurgAreasBackup = CopySurgAreas(ref moSurgAreas);

            PopulateSurgicalAreas();
            PopulateComboBoxes();
            SizeForm();

            Text = Text + " - " + sProcedureName;

            EnableDisableDefaults(false);

            this.Cursor = Cursors.Default;
            this.ShowDialog();
            oSurgAreas = moSurgAreas;
            return false;
        }

        private void cboAnesType_SelectedIndexChanged(Object eventSender, EventArgs eventArgs)
        {
            if (!mbClearingFields && !mbPopulatingFields)
            {
                UpdateAnesthesiaType();
            }
        }

        private void cboCaseLevel_SelectedIndexChanged(Object eventSender, EventArgs eventArgs)
        {
            if (!mbClearingFields && !mbPopulatingFields)
            {
                UpdateCaseLevel();
            }
        }

        private void cboSpecialty_SelectedIndexChanged(Object eventSender, EventArgs eventArgs)
        {
            if (!mbClearingFields && !mbPopulatingFields)
            {
                UpdateSpecialty();
            }

        }

        private void cboSurgDefined1_SelectedIndexChanged(Object eventSender, EventArgs eventArgs)
        {
            cboSurgDefined1.BackColor = SystemColors.Window;
            if (!mbClearingFields && !mbPopulatingFields)
            {
                UpdateSurgDefined1();
            }

        }

        private void cboSurgDefined2_SelectedIndexChanged(Object eventSender, EventArgs eventArgs)
        {
            cboSurgDefined2.BackColor = SystemColors.Window;
            if (!mbClearingFields && !mbPopulatingFields)
            {
                UpdateSurgDefined2();
            }

        }

        private void cboSurgDefined3_SelectedIndexChanged(Object eventSender, EventArgs eventArgs)
        {
            cboSurgDefined3.BackColor = SystemColors.Window;
            if (!mbClearingFields && !mbPopulatingFields)
            {
                UpdateSurgDefined3();
            }

        }

        private void cboSurgDefined4_SelectedIndexChanged(Object eventSender, EventArgs eventArgs)
        {
            cboSurgDefined4.BackColor = SystemColors.Window;
            if (!mbClearingFields && !mbPopulatingFields)
            {
                UpdateSurgDefined4();
            }

        }

        private void cboSurgDefined5_SelectedIndexChanged(Object eventSender, EventArgs eventArgs)
        {
            cboSurgDefined5.BackColor = SystemColors.Window;
            if (!mbClearingFields && !mbPopulatingFields)
            {
                UpdateSurgDefined5();
            }
        }

        private void cboWoundClass_SelectedIndexChanged(Object eventSender, EventArgs eventArgs)
        {
            if (!mbClearingFields && !mbPopulatingFields)
            {
                UpdateWoundClass();
            }
        }

        private void chkBlood_CheckStateChanged(Object eventSender, EventArgs eventArgs)
        {
            if (!mbClearingFields && !mbPopulatingFields)
            {
                UpdateBlood();
            }
        }

        private void chkFrozenSection_CheckStateChanged(Object eventSender, EventArgs eventArgs)
        {
            if (!mbClearingFields && !mbPopulatingFields)
            {
                UpdateFrozenSection();
            }
        }

        private void chkImplant_CheckStateChanged(Object eventSender, EventArgs eventArgs)
        {
            if (!mbClearingFields && !mbPopulatingFields)
            {
                UpdateImplant();
            }
        }

        private void chkSpecimenRequired_CheckStateChanged(Object eventSender, EventArgs eventArgs)
        {
            if (!mbClearingFields && !mbPopulatingFields)
            {
                UpdateSpecimenRequired();
            }
        }

        private void chkXRay_CheckStateChanged(Object eventSender, EventArgs eventArgs)
        {
            if (!mbClearingFields && !mbPopulatingFields)
            {
                UpdateXray();
            }
        }

        private void chkXRayTech_CheckStateChanged(Object eventSender, EventArgs eventArgs)
        {
            if (!mbClearingFields && !mbPopulatingFields)
            {
                UpdateXRayTech();
            }
        }

        private void cmdAddRemove_Click(Object eventSender, EventArgs eventArgs)
        {
            AddRemoveAreas();
        }

        private void cmdCancel_Click(Object eventSender, EventArgs eventArgs)
        {
            RestoreOriginalValues();
            this.Close();
        }

        private void cmdOk_Click(Object eventSender, EventArgs eventArgs)
        {         
            this.Close();
        }
       

        private void AddRemoveAreas()
        {
            frmAddRemoveAreas oAddRemoveAreasFrm = new frmAddRemoveAreas();

            oAddRemoveAreasFrm.Icon = Icon;
            if (oAddRemoveAreasFrm.ShowForm(moSurgAreas, moProcedure))
            {
                //process areas returned
                PopulateSurgicalAreas();
            }
        }

        private void RestoreOriginalValues()
        {
            //copy the backup back to the byref object
            moSurgAreas = null;
            moSurgAreas = CopySurgAreas(ref moSurgAreasBackup);
        }

        //return a copy of the supplied surgical areas class instance
        private CSurgAreas CopySurgAreas(ref  CSurgAreas oSurgAreasIn)
        {
            CSurgAreas oSurgAreasOut = new CSurgAreas();
            oSurgAreasOut.Copy(oSurgAreasIn);
            return oSurgAreasOut;
        }

        private void EnableDisableDefaults(bool bEnable)
        {

            ClearDefaultFields();

            foreach (Control oControl in this.Controls)
            {
                if (!(oControl.GetType() == typeof(ImageList)))
                {
                    if (oControl.Parent.Name.Equals("fraDefaults"))
                    {
                        if (!(oControl is Cerner.ApplicationFramework.GUI.CernerStyledControls.CernerStyledLabel))
                        {
                            ControlHelper.SetEnabled(oControl, bEnable);
                        }
                        else
                        {
                            if (bEnable)
                            {
                                oControl.ForeColor = SystemColors.WindowText;
                            }
                            else
                            {
                                oControl.ForeColor = SystemColors.GrayText;
                            }
                        }
                    }
                }
            }
            fraDefaults.Enabled = bEnable;
            lvwSurgAreas.HideSelection = !bEnable;
        }

        private void PopulateSurgicalAreas()
        {
            CSurgArea oArea = null;
            CProcDetail oProcDetail = null;
            ListViewItem oListItem = null;
            double dMaxTextWidth = 0;

            lvwSurgAreas.Items.Clear();
            moSelectedAreas = new Collection();
            //add areas with active proc details to list
            lvwSurgAreas.BeginUpdate();
            for (int lAreaIndex = 1, iteratorTest = moSurgAreas.Count; lAreaIndex <= iteratorTest; lAreaIndex++)
            {
                oArea = moSurgAreas[lAreaIndex];
                if (oArea.oProcDetails != null)
                {
                    for (int lProcDetIndex = 1; lProcDetIndex <= oArea.oProcDetails.Count; lProcDetIndex++)
                    {
                        oProcDetail = oArea.oProcDetails[lProcDetIndex];
                        if (oProcDetail.nState != SNCommon.Application.SNStateConstants.snStateDeleted)
                        {
                            //add the area to the list
                            oListItem = (ListViewItem)lvwSurgAreas.Items.Add("A" + oArea.lKey.ToString(), oArea.sSurgAreaDisp, System.String.Empty);
                            oListItem.Selected = false;
                            if (VB6.PixelsToTwipsX(System.Windows.Forms.TextRenderer.MeasureText(oArea.sSurgAreaDisp, Font).Width) > dMaxTextWidth)
                            {
                                dMaxTextWidth = VB6.PixelsToTwipsX(System.Windows.Forms.TextRenderer.MeasureText(oArea.sSurgAreaDisp, Font).Width);
                            }
                            break;
                        }
                    }
                }
            }
            lvwSurgAreas.EndUpdate();
            if (dMaxTextWidth > 0)
            {
                lvwSurgAreas.Columns[0].Width = (int)VB6.TwipsToPixelsX(dMaxTextWidth) +50; //fudge witdth to account for icon
            }
        }

        private void ClearDefaultFields()
        {

            mbClearingFields = true;
            foreach (Control oControl in this.Controls)
            {
                if (oControl is ComboBox)
                {
                    ((ComboBox)oControl).SelectedIndex = -1;
                }
                else if (oControl is TextBox)
                {
                    ((TextBox)oControl).Text = "0";
                }
                else if (oControl is CheckBox)
                {
                    ((CheckBox)oControl).CheckState = CheckState.Unchecked;
                }
            }

            mbClearingFields = false;
        }

        private void PopulateDefaultFields()
        {
            ListViewItem oListItem = null;
            CSurgArea oSurgArea = null;
            CodeTimer SNCrmTimer = new CodeTimer(this.GetType(), "ENG:SRG PopulateDefaultFields", true);
           // modMain.SNCrmTimers_CCrmTimers_definst.SNCrmStartTimeFunction("PopulateDefaultFields", SNCrmTimers.eTeamType.eSRG, SNCrmTimers.eTagType.eENG);
            if (moSelectedAreas.Count > 0)
            {
                EnableDisableDefaults(true);
                for (int lItemIndex = 1, iteratorTest = moSelectedAreas.Count; lItemIndex <= iteratorTest; lItemIndex++)
                {
                    oListItem = (ListViewItem)moSelectedAreas.GetItemFromCollection(lItemIndex);
                    oSurgArea = moSurgAreas.ItemByKey(oListItem.Name.Substring(1));
                    if (oSurgArea != null)
                    {
                        if (lItemIndex == 1)
                        {
                            PopulateAreaInDefaults(oSurgArea, false);
                        }
                        else
                        {
                            PopulateAreaInDefaults(oSurgArea, true);
                        }
                    }
                }
            }
            else
            {
                EnableDisableDefaults(false);
            }
            //modMain.SNCrmTimers_CCrmTimers_definst.SNCrmEndTimeFunction("PopulateDefaultFields");
            SNCrmTimer.Stop();
        }

        private void PopulateAreaInDefaults(CSurgArea oSurgArea, bool bCheckForSimilarFieldValue)
        {
            CProcDetail oProcDetail = null;
            CProcDuration oProcDuration = null;
            int lIndex = 0;
            const string sMULTI_VALUE = "MULTI VALUE";

            mbPopulatingFields = true;
            for (int lProcIndex = 1, iteratorTest = oSurgArea.oProcDetails.Count; lProcIndex <= iteratorTest; lProcIndex++)
            {
                oProcDetail = oSurgArea.oProcDetails[lProcIndex];
                //make sure procedure details are not deleted, is for our current catalog cd, or is new (negative id)
                if (oProcDetail.nState != SNCommon.Application.SNStateConstants.snStateDeleted && (oProcDetail.oCatalog.dCd == mdCurOrderCatalogCd || oProcDetail.oCatalog.dCd < 0))
                {

                    if (bCheckForSimilarFieldValue)
                    {
                        if (Convert.ToString(cboSpecialty.Tag) != sMULTI_VALUE)
                        {
                            lIndex = GetSpecialtyIndexBySpecialtyId(oProcDetail.dSurgSpecialtyId);
                            if (cboSpecialty.SelectedIndex != -1)
                            {
                                if (lIndex !=((StringValuePair<long>)cboSpecialty.Items[cboSpecialty.SelectedIndex]).Value)
                                {
                                    //different values, blank out and grey
                                    cboSpecialty.SelectedIndex = -1;
                                    cboSpecialty.BackColor = SystemColors.GrayText;
                                    cboSpecialty.Tag = sMULTI_VALUE;
                                }
                            }
                            else
                            {
                                if (lIndex > 0)
                                {
                                    //different values, blank out and grey
                                    cboSpecialty.SelectedIndex = -1;
                                    cboSpecialty.BackColor = SystemColors.GrayText;
                                    cboSpecialty.Tag = sMULTI_VALUE;
                                }
                            }
                        }

                        if (Convert.ToString(cboCaseLevel.Tag) != sMULTI_VALUE)
                        {
                            lIndex = GetCodeValueIndexByCodeValue(CaseLevels, oProcDetail.dCaseLevelCd);
                            if (cboCaseLevel.SelectedIndex != -1)
                            {
                                if (lIndex !=((StringValuePair<long>)cboCaseLevel.Items[cboCaseLevel.SelectedIndex]).Value)
                                {
                                    //different values, blank out and grey
                                    cboCaseLevel.SelectedIndex = -1;
                                    cboCaseLevel.BackColor = SystemColors.GrayText;
                                    cboCaseLevel.Tag = sMULTI_VALUE;
                                }
                            }
                            else
                            {
                                if (lIndex > 0)
                                {
                                    //different values, blank out and grey
                                    cboCaseLevel.SelectedIndex = -1;
                                    cboCaseLevel.BackColor = SystemColors.GrayText;
                                    cboCaseLevel.Tag = sMULTI_VALUE;
                                }
                            }
                        }

                        if (Convert.ToString(cboWoundClass.Tag) != sMULTI_VALUE)
                        {
                            lIndex = GetCodeValueIndexByCodeValue(WoundClasses, oProcDetail.dWoundClassCd);
                            if (cboWoundClass.SelectedIndex != -1)
                            {
                                if (lIndex !=((StringValuePair<long>)cboWoundClass.Items[cboWoundClass.SelectedIndex]).Value)
                                {
                                    //different values, blank out and grey
                                    cboWoundClass.SelectedIndex = -1;
                                    cboWoundClass.BackColor = SystemColors.GrayText;
                                    cboWoundClass.Tag = sMULTI_VALUE;
                                }
                            }
                            else
                            {
                                if (lIndex > 0)
                                {
                                    //different values, blank out and grey
                                    cboWoundClass.SelectedIndex = -1;
                                    cboWoundClass.BackColor = SystemColors.GrayText;
                                    cboWoundClass.Tag = sMULTI_VALUE;
                                }
                            }
                        }

                        if (Convert.ToString(cboAnesType.Tag) != sMULTI_VALUE)
                        {
                            lIndex = GetCodeValueIndexByCodeValue(AnesthesiaTypes, oProcDetail.dAnesthesiaTypeCd);
                            if (cboAnesType.SelectedIndex != -1)
                            {
                                if (lIndex !=((StringValuePair<long>)cboAnesType.Items[cboAnesType.SelectedIndex]).Value) 
                                {
                                    //different values, blank out and grey
                                    cboAnesType.SelectedIndex = -1;
                                    cboAnesType.BackColor = SystemColors.GrayText;
                                    cboAnesType.Tag = sMULTI_VALUE;
                                }
                            }
                            else
                            {
                                if (lIndex > 0)
                                {
                                    //different values, blank out and grey
                                    cboAnesType.SelectedIndex = -1;
                                    cboAnesType.BackColor = SystemColors.GrayText;
                                    cboAnesType.Tag = sMULTI_VALUE;
                                }
                            }
                        }

                        if (Convert.ToString(mebProcedureCount.Tag) != sMULTI_VALUE)
                        {
                            if (Convert.ToString(mebProcedureCount.Value).Trim(' ') != oProcDetail.dProcCnt.ToString().Trim(' '))
                            {
                                mebProcedureCount.Text = mebProcedureCount.Minimum.ToString();
                                mebProcedureCount.BackColor = SystemColors.GrayText;
                                mebProcedureCount.Tag = sMULTI_VALUE;
                            }
                        }
                        if (chkSpecimenRequired.CheckState != CheckState.Indeterminate)
                        {
                            if ((oProcDetail.nSpecReqInd == 1 && chkSpecimenRequired.CheckState == CheckState.Unchecked) || (oProcDetail.nSpecReqInd == 0 && chkSpecimenRequired.CheckState == CheckState.Checked))
                            {
                                //different values, blank out and grey
                                chkSpecimenRequired.CheckState = CheckState.Indeterminate;
                            }
                        }

                        if (chkFrozenSection.CheckState != CheckState.Indeterminate)
                        {
                            if ((oProcDetail.nFrozenSectionReqInd == 1 && chkFrozenSection.CheckState == CheckState.Unchecked) || (oProcDetail.nFrozenSectionReqInd == 0 && chkFrozenSection.CheckState == CheckState.Checked))
                            {
                                //different values, blank out and grey
                                chkFrozenSection.CheckState = CheckState.Indeterminate;
                            }
                        }

                        if (chkBlood.CheckState != CheckState.Indeterminate)
                        {
                            if ((oProcDetail.nBloodProductReqInd == 1 && chkBlood.CheckState == CheckState.Unchecked) || (oProcDetail.nBloodProductReqInd == 0 && chkBlood.CheckState == CheckState.Checked))
                            {
                                //different values, blank out and grey
                                chkBlood.CheckState = CheckState.Indeterminate;
                            }
                        }

                        if (chkImplant.CheckState != CheckState.Indeterminate)
                        {
                            if ((oProcDetail.nImplantInd == 1 && chkImplant.CheckState == CheckState.Unchecked) || (oProcDetail.nImplantInd == 0 && chkImplant.CheckState == CheckState.Checked))
                            {
                                //different values, blank out and grey
                                chkImplant.CheckState = CheckState.Indeterminate;
                            }
                        }

                        if (chkXRay.CheckState != CheckState.Indeterminate)
                        {
                            if ((oProcDetail.nXrayInd == 1 && chkXRay.CheckState == CheckState.Unchecked) || (oProcDetail.nXrayInd == 0 && chkXRay.CheckState == CheckState.Checked))
                            {
                                //different values, blank out and grey
                                chkXRay.CheckState = CheckState.Indeterminate;
                            }
                        }

                        if (chkXRayTech.CheckState != CheckState.Indeterminate)
                        {
                            if ((oProcDetail.nXrayTechInd == 1 && chkXRayTech.CheckState == CheckState.Unchecked) || (oProcDetail.nXrayTechInd == 0 && chkXRayTech.CheckState == CheckState.Checked))
                            {
                                //different values, blank out and grey
                                chkXRayTech.CheckState = CheckState.Indeterminate;
                            }
                        }

                        if (mlSurgDef1CodeSet > 0)
                        {
                            if (Convert.ToString(cboSurgDefined1.Tag) != sMULTI_VALUE)
                            {
                                lIndex = GetCodeValueIndexByCodeValue(SurgeryDefined1, oProcDetail.dUD1Cd);
                                if (cboSurgDefined1.SelectedIndex != -1)
                                {
                                    if (lIndex != ((StringValuePair<long>)cboSurgDefined1.Items[cboSurgDefined1.SelectedIndex]).Value) 
                                    {
                                        //different values, blank out and grey
                                        cboSurgDefined1.SelectedIndex = -1;
                                        cboSurgDefined1.BackColor = SystemColors.GrayText;
                                        cboSurgDefined1.Tag = sMULTI_VALUE;
                                    }
                                }
                                else
                                {
                                    if (lIndex > 0)
                                    {
                                        //different values, blank out and grey
                                        cboSurgDefined1.SelectedIndex = -1;
                                        cboSurgDefined1.BackColor = SystemColors.GrayText;
                                        cboSurgDefined1.Tag = sMULTI_VALUE;
                                    }
                                }
                            }
                        }

                        if (mlSurgDef2CodeSet > 0)
                        {
                            if (Convert.ToString(cboSurgDefined2.Tag) != sMULTI_VALUE)
                            {
                                lIndex = GetCodeValueIndexByCodeValue(SurgeryDefined2, oProcDetail.dUD2Cd);
                                if (cboSurgDefined2.SelectedIndex != -1)
                                {
                                    if (lIndex != ((StringValuePair<long>)cboSurgDefined2.Items[cboSurgDefined2.SelectedIndex]).Value) 
                                    {
                                        //different values, blank out and grey
                                        cboSurgDefined2.SelectedIndex = -1;
                                        cboSurgDefined2.BackColor = SystemColors.GrayText;
                                        cboSurgDefined2.Tag = sMULTI_VALUE;
                                    }
                                }
                                else
                                {
                                    if (lIndex > 0)
                                    {
                                        //different values, blank out and grey
                                        cboSurgDefined2.SelectedIndex = -1;
                                        cboSurgDefined2.BackColor = SystemColors.GrayText;
                                        cboSurgDefined2.Tag = sMULTI_VALUE;
                                    }
                                }
                            }
                        }

                        if (mlSurgDef3CodeSet > 0)
                        {
                            if (Convert.ToString(cboSurgDefined3.Tag) != sMULTI_VALUE)
                            {
                                lIndex = GetCodeValueIndexByCodeValue(SurgeryDefined3, oProcDetail.dUD3Cd);
                                if (cboSurgDefined3.SelectedIndex != -1)
                                {
                                    if (lIndex != ((StringValuePair<long>)cboSurgDefined3.Items[cboSurgDefined3.SelectedIndex]).Value) 
                                    {
                                        //different values, blank out and grey
                                        cboSurgDefined3.SelectedIndex = -1;
                                        cboSurgDefined3.BackColor = SystemColors.GrayText;
                                        cboSurgDefined3.Tag = sMULTI_VALUE;
                                    }
                                }
                                else
                                {
                                    if (lIndex > 0)
                                    {
                                        //different values, blank out and grey
                                        cboSurgDefined3.SelectedIndex = -1;
                                        cboSurgDefined3.BackColor = SystemColors.GrayText;
                                        cboSurgDefined3.Tag = sMULTI_VALUE;
                                    }
                                }
                            }
                        }

                        if (mlSurgDef4CodeSet > 0)
                        {
                            if (Convert.ToString(cboSurgDefined4.Tag) != sMULTI_VALUE)
                            {
                                lIndex = GetCodeValueIndexByCodeValue(SurgeryDefined4, oProcDetail.dUD4Cd);
                                if (cboSurgDefined4.SelectedIndex != -1)
                                {
                                    if (lIndex != ((StringValuePair<long>)cboSurgDefined4.Items[cboSurgDefined4.SelectedIndex]).Value) 
                                    {
                                        //different values, blank out and grey
                                        cboSurgDefined4.SelectedIndex = -1;
                                        cboSurgDefined4.BackColor = SystemColors.GrayText;
                                        cboSurgDefined4.Tag = sMULTI_VALUE;
                                    }
                                }
                                else
                                {
                                    if (lIndex > 0)
                                    {
                                        //different values, blank out and grey
                                        cboSurgDefined4.SelectedIndex = -1;
                                        cboSurgDefined4.BackColor = SystemColors.GrayText;
                                        cboSurgDefined4.Tag = sMULTI_VALUE;
                                    }
                                }
                            }
                        }

                        if (mlSurgDef5CodeSet > 0)
                        {
                            if (Convert.ToString(cboSurgDefined5.Tag) != sMULTI_VALUE)
                            {
                                lIndex = GetCodeValueIndexByCodeValue(SurgeryDefined5, oProcDetail.dUD5Cd);
                                if (cboSurgDefined5.SelectedIndex != -1)
                                {
                                    if (lIndex != ((StringValuePair<long>)cboSurgDefined5.Items[cboSurgDefined5.SelectedIndex]).Value) 
                                    {
                                        //different values, blank out and grey
                                        cboSurgDefined5.SelectedIndex = -1;
                                        cboSurgDefined5.BackColor = SystemColors.GrayText;
                                        cboSurgDefined5.Tag = sMULTI_VALUE;
                                    }
                                }
                                else
                                {
                                    if (lIndex > 0)
                                    {
                                        //different values, blank out and grey
                                        cboSurgDefined5.SelectedIndex = -1;
                                        cboSurgDefined5.BackColor = SystemColors.GrayText;
                                        cboSurgDefined5.Tag = sMULTI_VALUE;
                                    }
                                }
                            }
                        }

                        //DURATIONS
                        for (int lDurationIndex = 1; lDurationIndex <= oProcDetail.oProcDurations.Count; lDurationIndex++)
                        {
                            oProcDuration = oProcDetail.oProcDurations[lDurationIndex];
                            if (oProcDuration.nState != SNCommon.Application.SNStateConstants.snStateDeleted && oProcDuration.dPrsnlID == 0)
                            {
                                //use this duration
                                if (Convert.ToString(mebDuration.Tag) != sMULTI_VALUE)
                                {
                                    if (Convert.ToString(mebDuration.Value).Trim(' ') != oProcDuration.dDefProcedureDur.ToString().Trim(' '))
                                    {
                                        mebDuration.Text = mebProcedureCount.Minimum.ToString();
                                        mebDuration.BackColor = SystemColors.GrayText;
                                        mebDuration.Tag = sMULTI_VALUE;
                                    }
                                }

                                if (Convert.ToString(mebSetupTime.Tag) != sMULTI_VALUE)
                                {
                                    if (Convert.ToString(mebSetupTime.Value).Trim(' ') != oProcDuration.dDefSetupDur.ToString().Trim(' '))
                                    {
                                        mebSetupTime.Text = mebSetupTime.Minimum.ToString();
                                        mebSetupTime.BackColor = SystemColors.GrayText;
                                        mebSetupTime.Tag = sMULTI_VALUE;
                                    }
                                }

                                if (Convert.ToString(mebCleanupTime.Tag) != sMULTI_VALUE)
                                {
                                    if (Convert.ToString(mebCleanupTime.Value).Trim(' ') != oProcDuration.dDefCleanupDur.ToString().Trim(' '))
                                    {
                                        mebCleanupTime.Text = mebCleanupTime.Minimum.ToString();
                                        mebCleanupTime.BackColor = SystemColors.GrayText;
                                        mebCleanupTime.Tag = sMULTI_VALUE;
                                    }
                                }

                                if (Convert.ToString(mebPreIncisionTime.Tag) != sMULTI_VALUE)
                                {
                                    if (Convert.ToString(mebPreIncisionTime.Value).Trim(' ') != oProcDuration.dDefPreIncisionDur.ToString().Trim(' '))
                                    {
                                        mebPreIncisionTime.Text = mebPreIncisionTime.Minimum.ToString();
                                        mebPreIncisionTime.BackColor = SystemColors.GrayText;
                                        mebPreIncisionTime.Tag = sMULTI_VALUE;
                                    }
                                }

                                if (Convert.ToString(mebPostClosureTime.Tag) != sMULTI_VALUE)
                                {
                                    if (Convert.ToString(mebPostClosureTime.Value).Trim(' ') != oProcDuration.dDefPostClosureDur.ToString().Trim(' '))
                                    {
                                        mebPostClosureTime.Text = mebPostClosureTime.Minimum.ToString();
                                        mebPostClosureTime.BackColor = SystemColors.GrayText;
                                        mebPostClosureTime.Tag = sMULTI_VALUE;
                                    }
                                }

                                break;
                            }
                        }
                    }
                    else
                    {
                        lIndex = GetSpecialtyIndexBySpecialtyId(oProcDetail.dSurgSpecialtyId);
                        SelectValueInComboBox(cboSpecialty, lIndex);
                        cboSpecialty.Tag = System.String.Empty;
                        if (cboSpecialty.SelectedIndex != -1)
                        {
                            cboSpecialty.BackColor = SystemColors.Window;
                        }
                        else
                        {
                            cboSpecialty.BackColor = ColorTranslator.FromOle(glREQ_FIELD_COLOR);
                        }

                        lIndex = GetCodeValueIndexByCodeValue(CaseLevels, oProcDetail.dCaseLevelCd);
                        SelectValueInComboBox(cboCaseLevel, lIndex);
                        cboCaseLevel.Tag = System.String.Empty;
                        if (cboCaseLevel.SelectedIndex != -1)
                        {
                            cboCaseLevel.BackColor = SystemColors.Window;
                        }
                        else
                        {
                            cboCaseLevel.BackColor = ColorTranslator.FromOle(glREQ_FIELD_COLOR);
                        }

                        lIndex = GetCodeValueIndexByCodeValue(WoundClasses, oProcDetail.dWoundClassCd);
                        SelectValueInComboBox(cboWoundClass, lIndex);
                        cboWoundClass.Tag = System.String.Empty;
                        if (cboWoundClass.SelectedIndex != -1)
                        {
                            cboWoundClass.BackColor = SystemColors.Window;
                        }
                        else
                        {
                            cboWoundClass.BackColor = ColorTranslator.FromOle(glREQ_FIELD_COLOR);
                        }

                        lIndex = GetCodeValueIndexByCodeValue(AnesthesiaTypes, oProcDetail.dAnesthesiaTypeCd);
                        SelectValueInComboBox(cboAnesType, lIndex);
                        cboAnesType.Tag = System.String.Empty;
                        if (cboAnesType.SelectedIndex != -1)
                        {
                            cboAnesType.BackColor = SystemColors.Window;
                        }
                        else
                        {
                            cboAnesType.BackColor = ColorTranslator.FromOle(glREQ_FIELD_COLOR);
                        }

                        if (oProcDetail.nSpecReqInd == 1)
                        {
                            chkSpecimenRequired.CheckState = CheckState.Checked;
                        }
                        else
                        {
                            chkSpecimenRequired.CheckState = CheckState.Unchecked;
                        }

                        if (oProcDetail.nFrozenSectionReqInd == 1)
                        {
                            chkFrozenSection.CheckState = CheckState.Checked;
                        }
                        else
                        {
                            chkFrozenSection.CheckState = CheckState.Unchecked;
                        }

                        if (oProcDetail.nBloodProductReqInd == 1)
                        {
                            chkBlood.CheckState = CheckState.Checked;
                        }
                        else
                        {
                            chkBlood.CheckState = CheckState.Unchecked;
                        }

                        if (oProcDetail.nImplantInd == 1)
                        {
                            chkImplant.CheckState = CheckState.Checked;
                        }
                        else
                        {
                            chkImplant.CheckState = CheckState.Unchecked;
                        }

                        if (oProcDetail.nXrayInd == 1)
                        {
                            chkXRay.CheckState = CheckState.Checked;
                        }
                        else
                        {
                            chkXRay.CheckState = CheckState.Unchecked;
                        }

                        if (oProcDetail.nXrayTechInd == 1)
                        {
                            chkXRayTech.CheckState = CheckState.Checked;
                        }
                        else
                        {
                            chkXRayTech.CheckState = CheckState.Unchecked;
                        }

                        mebProcedureCount.Text = oProcDetail.dProcCnt.ToString();
                        mebProcedureCount.Tag = System.String.Empty;
                        mebProcedureCount.BackColor = SystemColors.Window;

                        if (mlSurgDef1CodeSet > 0)
                        {
                            lIndex = GetCodeValueIndexByCodeValue(SurgeryDefined1, oProcDetail.dUD1Cd);
                            SelectValueInComboBox(cboSurgDefined1, lIndex);
                        }
                        cboSurgDefined1.Tag = System.String.Empty;
                        cboSurgDefined1.BackColor = SystemColors.Window;

                        if (mlSurgDef2CodeSet > 0)
                        {
                            lIndex = GetCodeValueIndexByCodeValue(SurgeryDefined2, oProcDetail.dUD2Cd);
                            SelectValueInComboBox(cboSurgDefined2, lIndex);
                        }
                        cboSurgDefined2.Tag = System.String.Empty;
                        cboSurgDefined2.BackColor = SystemColors.Window;

                        if (mlSurgDef3CodeSet > 0)
                        {
                            lIndex = GetCodeValueIndexByCodeValue(SurgeryDefined3, oProcDetail.dUD3Cd);
                            SelectValueInComboBox(cboSurgDefined3, lIndex);
                        }
                        cboSurgDefined3.Tag = System.String.Empty;
                        cboSurgDefined3.BackColor = SystemColors.Window;

                        if (mlSurgDef4CodeSet > 0)
                        {
                            lIndex = GetCodeValueIndexByCodeValue(SurgeryDefined4, oProcDetail.dUD4Cd);
                            SelectValueInComboBox(cboSurgDefined4, lIndex);
                        }
                        cboSurgDefined4.Tag = System.String.Empty;
                        cboSurgDefined4.BackColor = SystemColors.Window;

                        if (mlSurgDef5CodeSet > 0)
                        {
                            lIndex = GetCodeValueIndexByCodeValue(SurgeryDefined5, oProcDetail.dUD5Cd);
                            SelectValueInComboBox(cboSurgDefined5, lIndex);
                        }
                        cboSurgDefined5.Tag = System.String.Empty;
                        cboSurgDefined5.BackColor = SystemColors.Window;

                        //DURATIONS
                        for (int lDurationIndex = 1; lDurationIndex <= oProcDetail.oProcDurations.Count; lDurationIndex++)
                        {
                            oProcDuration = oProcDetail.oProcDurations[lDurationIndex];
                            if (oProcDuration.nState != SNCommon.Application.SNStateConstants.snStateDeleted && oProcDuration.dPrsnlID == 0)
                            {
                                //use this duration
                                mebDuration.Text = oProcDuration.dDefProcedureDur.ToString();
                                mebSetupTime.Text = oProcDuration.dDefSetupDur.ToString();
                                mebCleanupTime.Text = oProcDuration.dDefCleanupDur.ToString();
                                mebPreIncisionTime.Text = oProcDuration.dDefPreIncisionDur.ToString();
                                mebPostClosureTime.Text = oProcDuration.dDefPostClosureDur.ToString();
                                mebDuration.Tag = System.String.Empty;
                                mebSetupTime.Tag = System.String.Empty;
                                mebCleanupTime.Tag = System.String.Empty;
                                mebPreIncisionTime.Tag = System.String.Empty;
                                mebPostClosureTime.Tag = System.String.Empty;
                                mebDuration.BackColor = SystemColors.Window;
                                mebSetupTime.BackColor = SystemColors.Window;
                                mebCleanupTime.BackColor = SystemColors.Window;
                                mebPreIncisionTime.BackColor = SystemColors.Window;
                                mebPostClosureTime.BackColor = SystemColors.Window;
                                break;
                            }
                        }
                    }
                    break;
                }
            }

            mbPopulatingFields = false;
        }

        private void SizeForm()
        {
            double dLeftSide = (((float)VB6.PixelsToTwipsX(ClientRectangle.Width)) - (3 * modMain.gn4DLUS) - modMain.glFudgeFactor) * 0.35d;
            double dRightSide = (((float)VB6.PixelsToTwipsX(ClientRectangle.Width)) - (3 * modMain.gn4DLUS) - modMain.glFudgeFactor) * 0.65d;
            double dRightSideFieldWidth = (dRightSide - (3 * modMain.gn4DLUS) - modMain.glFudgeFactor) * 0.5d;
            double dLabelHeight = 2 * modMain.gn4DLUS;
            double dBoxHeight = (float)VB6.PixelsToTwipsY(cboSpecialty.Height);
            double dSpinWidth = dBoxHeight;

            lblSurgAreas.SetBounds(Convert.ToInt32((float)VB6.TwipsToPixelsX(modMain.gn4DLUS)), Convert.ToInt32((float)VB6.TwipsToPixelsY(modMain.gn4DLUS)), Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dLeftSide)), Convert.ToInt32((float)VB6.TwipsToPixelsY((float)dLabelHeight)));

            lvwSurgAreas.SetBounds(Convert.ToInt32(lblSurgAreas.Left), Convert.ToInt32((float)(lblSurgAreas.Top + lblSurgAreas.Height)), Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dLeftSide)), 0, BoundsSpecified.X | BoundsSpecified.Y | BoundsSpecified.Width);

            fraDefaults.SetBounds(Convert.ToInt32((float)VB6.TwipsToPixelsX(((float)VB6.PixelsToTwipsX(lvwSurgAreas.Left)) + ((float)VB6.PixelsToTwipsX(lvwSurgAreas.Width)) + modMain.gn4DLUS)), Convert.ToInt32((float)(lvwSurgAreas.Top - VB6.TwipsToPixelsY(modMain.glFudgeFactor)))-3, Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dRightSide)), 0, BoundsSpecified.X | BoundsSpecified.Y | BoundsSpecified.Width);

            //LEFT side combos
            lblSpecialty.SetBounds(Convert.ToInt32(lblSpecialty.Left), Convert.ToInt32((float)VB6.TwipsToPixelsY(2 * modMain.gn4DLUS)), Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dRightSideFieldWidth)), Convert.ToInt32((float)VB6.TwipsToPixelsY((float)dLabelHeight)));

            cboSpecialty.SetBounds(Convert.ToInt32(lblSpecialty.Left), Convert.ToInt32((float)(lblSpecialty.Top + lblSpecialty.Height)), Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dRightSideFieldWidth)), 0, BoundsSpecified.X | BoundsSpecified.Y | BoundsSpecified.Width);

            lblWoundClass.SetBounds(Convert.ToInt32(lblSpecialty.Left), Convert.ToInt32((float)VB6.TwipsToPixelsY(((float)VB6.PixelsToTwipsY(cboSpecialty.Top)) + ((float)VB6.PixelsToTwipsY(cboSpecialty.Height)) + modMain.gn4DLUS)), Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dRightSideFieldWidth)), Convert.ToInt32((float)VB6.TwipsToPixelsY((float)dLabelHeight)));

            cboWoundClass.SetBounds(Convert.ToInt32(lblSpecialty.Left), Convert.ToInt32((float)(lblWoundClass.Top + lblWoundClass.Height)), Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dRightSideFieldWidth)), 0, BoundsSpecified.X | BoundsSpecified.Y | BoundsSpecified.Width);



            //RIGHT side combos
            lblCaseLevel.SetBounds(Convert.ToInt32((float)VB6.TwipsToPixelsX(((float)VB6.PixelsToTwipsX(lblSpecialty.Left)) + ((float)VB6.PixelsToTwipsX(lblSpecialty.Width)) + modMain.gn4DLUS)), Convert.ToInt32(lblSpecialty.Top), Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dRightSideFieldWidth)), Convert.ToInt32((float)VB6.TwipsToPixelsY((float)dLabelHeight)));

            cboCaseLevel.SetBounds(Convert.ToInt32(lblCaseLevel.Left), Convert.ToInt32((float)(lblCaseLevel.Top + lblCaseLevel.Height)), Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dRightSideFieldWidth)), 0, BoundsSpecified.X | BoundsSpecified.Y | BoundsSpecified.Width);

            lblAnesType.SetBounds(Convert.ToInt32(lblCaseLevel.Left), Convert.ToInt32((float)VB6.TwipsToPixelsY(((float)VB6.PixelsToTwipsY(cboCaseLevel.Top)) + ((float)VB6.PixelsToTwipsY(cboCaseLevel.Height)) + modMain.gn4DLUS)), Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dRightSideFieldWidth)), Convert.ToInt32((float)VB6.TwipsToPixelsY((float)dLabelHeight)));

            cboAnesType.SetBounds(Convert.ToInt32(lblCaseLevel.Left), Convert.ToInt32((float)(lblAnesType.Top + lblAnesType.Height)), Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dRightSideFieldWidth)), 0, BoundsSpecified.X | BoundsSpecified.Y | BoundsSpecified.Width);


            //LEFT side spin boxes
            lblProcedureCount.SetBounds(Convert.ToInt32(lblSpecialty.Left), Convert.ToInt32((float)VB6.TwipsToPixelsY(((float)VB6.PixelsToTwipsY(cboWoundClass.Top)) + ((float)VB6.PixelsToTwipsY(cboWoundClass.Height)) + modMain.gn4DLUS)), Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dRightSideFieldWidth)), Convert.ToInt32((float)VB6.TwipsToPixelsY((float)dLabelHeight)));

            mebProcedureCount.SetBounds(Convert.ToInt32(lblProcedureCount.Left), Convert.ToInt32((float)(lblProcedureCount.Top + lblProcedureCount.Height)), Convert.ToInt32((float)VB6.TwipsToPixelsX((float)(dRightSideFieldWidth / 2 - dSpinWidth))), Convert.ToInt32((float)VB6.TwipsToPixelsY((float)dBoxHeight)));


            lblPreIncisionTime.SetBounds(Convert.ToInt32(lblSpecialty.Left), Convert.ToInt32((float)VB6.TwipsToPixelsY(((float)VB6.PixelsToTwipsY(mebProcedureCount.Top)) + ((float)VB6.PixelsToTwipsY(mebProcedureCount.Height)) + modMain.gn4DLUS)), Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dRightSideFieldWidth)), Convert.ToInt32((float)VB6.TwipsToPixelsY((float)dLabelHeight)));

            mebPreIncisionTime.SetBounds(Convert.ToInt32(lblPreIncisionTime.Left), Convert.ToInt32((float)(lblPreIncisionTime.Top + lblPreIncisionTime.Height)), Convert.ToInt32((float)VB6.TwipsToPixelsX((float)(dRightSideFieldWidth / 2 - dSpinWidth))), Convert.ToInt32((float)VB6.TwipsToPixelsY((float)dBoxHeight)));


            lblPostClosureTime.SetBounds(Convert.ToInt32(lblSpecialty.Left), Convert.ToInt32((float)VB6.TwipsToPixelsY(((float)VB6.PixelsToTwipsY(mebPreIncisionTime.Top)) + ((float)VB6.PixelsToTwipsY(mebPreIncisionTime.Height)) + modMain.gn4DLUS)), Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dRightSideFieldWidth)), Convert.ToInt32((float)VB6.TwipsToPixelsY((float)dLabelHeight)));

            mebPostClosureTime.SetBounds(Convert.ToInt32(lblPostClosureTime.Left), Convert.ToInt32((float)(lblPostClosureTime.Top + lblPostClosureTime.Height)), Convert.ToInt32((float)VB6.TwipsToPixelsX((float)(dRightSideFieldWidth / 2 - dSpinWidth))), Convert.ToInt32((float)VB6.TwipsToPixelsY((float)dBoxHeight)));


            //RIGHT side spin boxes
            lblSetupTime.SetBounds(Convert.ToInt32(lblCaseLevel.Left), Convert.ToInt32((float)VB6.TwipsToPixelsY(((float)VB6.PixelsToTwipsY(cboAnesType.Top)) + ((float)VB6.PixelsToTwipsY(cboAnesType.Height)) + modMain.gn4DLUS)), Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dRightSideFieldWidth)), Convert.ToInt32((float)VB6.TwipsToPixelsY((float)dLabelHeight)));

            mebSetupTime.SetBounds(Convert.ToInt32(lblSetupTime.Left), Convert.ToInt32((float)(lblSetupTime.Top + lblSetupTime.Height)), Convert.ToInt32((float)VB6.TwipsToPixelsX((float)(dRightSideFieldWidth / 2 - dSpinWidth))), Convert.ToInt32((float)VB6.TwipsToPixelsY((float)dBoxHeight)));


            lblDuration.SetBounds(Convert.ToInt32(lblCaseLevel.Left), Convert.ToInt32((float)VB6.TwipsToPixelsY(((float)VB6.PixelsToTwipsY(mebSetupTime.Top)) + ((float)VB6.PixelsToTwipsY(mebSetupTime.Height)) + modMain.gn4DLUS)), Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dRightSideFieldWidth)), Convert.ToInt32((float)VB6.TwipsToPixelsY((float)dLabelHeight)));

            mebDuration.SetBounds(Convert.ToInt32(lblDuration.Left), Convert.ToInt32((float)(lblDuration.Top + lblDuration.Height)), Convert.ToInt32((float)VB6.TwipsToPixelsX((float)(dRightSideFieldWidth / 2 - dSpinWidth))), Convert.ToInt32((float)VB6.TwipsToPixelsY((float)dBoxHeight)));

            lblCleanupTime.SetBounds(Convert.ToInt32(lblCaseLevel.Left), Convert.ToInt32((float)VB6.TwipsToPixelsY(((float)VB6.PixelsToTwipsY(mebDuration.Top)) + ((float)VB6.PixelsToTwipsY(mebDuration.Height)) + modMain.gn4DLUS)), Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dRightSideFieldWidth)), Convert.ToInt32((float)VB6.TwipsToPixelsY((float)dLabelHeight)));

            mebCleanupTime.SetBounds(Convert.ToInt32(lblCleanupTime.Left), Convert.ToInt32((float)(lblCleanupTime.Top + lblCleanupTime.Height)), Convert.ToInt32((float)VB6.TwipsToPixelsX((float)(dRightSideFieldWidth / 2 - dSpinWidth))), Convert.ToInt32((float)VB6.TwipsToPixelsY((float)dBoxHeight)));



            //LEFT side check boxes
            chkSpecimenRequired.SetBounds(Convert.ToInt32(lblSpecialty.Left), Convert.ToInt32((float)VB6.TwipsToPixelsY(((float)VB6.PixelsToTwipsY(mebPostClosureTime.Top)) + ((float)VB6.PixelsToTwipsY(mebPostClosureTime.Height)) + modMain.gn4DLUS)), Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dRightSideFieldWidth)), Convert.ToInt32((float)VB6.TwipsToPixelsY((float)dBoxHeight)));

            chkBlood.SetBounds(Convert.ToInt32(lblSpecialty.Left), Convert.ToInt32((float)VB6.TwipsToPixelsY(((float)VB6.PixelsToTwipsY(chkSpecimenRequired.Top)) + ((float)VB6.PixelsToTwipsY(chkSpecimenRequired.Height)) + modMain.gn4DLUS)), Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dRightSideFieldWidth)), Convert.ToInt32((float)VB6.TwipsToPixelsY((float)dBoxHeight)));

            chkXRay.SetBounds(Convert.ToInt32(lblSpecialty.Left), Convert.ToInt32((float)VB6.TwipsToPixelsY(((float)VB6.PixelsToTwipsY(chkBlood.Top)) + ((float)VB6.PixelsToTwipsY(chkBlood.Height)) + modMain.gn4DLUS)), Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dRightSideFieldWidth)), Convert.ToInt32((float)VB6.TwipsToPixelsY((float)dBoxHeight)));

            //RIGHT side check boxes
            chkFrozenSection.SetBounds(Convert.ToInt32(lblCaseLevel.Left), Convert.ToInt32((float)VB6.TwipsToPixelsY(((float)VB6.PixelsToTwipsY(mebCleanupTime.Top)) + ((float)VB6.PixelsToTwipsY(mebCleanupTime.Height)) + modMain.gn4DLUS)), Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dRightSideFieldWidth)), Convert.ToInt32((float)VB6.TwipsToPixelsY((float)dBoxHeight)));

            chkImplant.SetBounds(Convert.ToInt32(lblCaseLevel.Left), Convert.ToInt32((float)VB6.TwipsToPixelsY(((float)VB6.PixelsToTwipsY(chkFrozenSection.Top)) + ((float)VB6.PixelsToTwipsY(chkFrozenSection.Height)) + modMain.gn4DLUS)), Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dRightSideFieldWidth)), Convert.ToInt32((float)VB6.TwipsToPixelsY((float)dBoxHeight)));

            chkXRayTech.SetBounds(Convert.ToInt32(lblCaseLevel.Left), Convert.ToInt32((float)VB6.TwipsToPixelsY(((float)VB6.PixelsToTwipsY(chkImplant.Top)) + ((float)VB6.PixelsToTwipsY(chkImplant.Height)) + modMain.gn4DLUS)), Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dRightSideFieldWidth)), Convert.ToInt32((float)VB6.TwipsToPixelsY((float)dBoxHeight)));

            //SURGERY DEFINED FIELDS
            double dNextSDFLeft = (float)VB6.PixelsToTwipsX(lblSpecialty.Left);
            double dNextSDFTop = ((float)VB6.PixelsToTwipsY(chkXRay.Top)) + ((float)VB6.PixelsToTwipsY(chkXRay.Height)) + modMain.gn4DLUS;

            if (mlSurgDef1CodeSet > 0)
            {
                fdlSurgDefined1.Visible = true;
                cboSurgDefined1.Visible = true;
                fdlSurgDefined1.SetBounds(Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dNextSDFLeft)), Convert.ToInt32((float)VB6.TwipsToPixelsY((float)dNextSDFTop)), Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dRightSideFieldWidth)), Convert.ToInt32((float)VB6.TwipsToPixelsY((float)dLabelHeight)));

                cboSurgDefined1.SetBounds(Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dNextSDFLeft)), Convert.ToInt32((float)(fdlSurgDefined1.Top + fdlSurgDefined1.Height)), Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dRightSideFieldWidth)), 0, BoundsSpecified.X | BoundsSpecified.Y | BoundsSpecified.Width);

                //determine if we need to go down or across
                if (dNextSDFLeft == ((float)VB6.PixelsToTwipsX(lblSpecialty.Left)))
                {
                    //need to go to the right
                    dNextSDFLeft = (float)VB6.PixelsToTwipsX(lblCaseLevel.Left);
                }
                else
                {
                    //need to go down and to the left
                    dNextSDFLeft = (float)VB6.PixelsToTwipsX(lblSpecialty.Left);
                    dNextSDFTop = ((float)VB6.PixelsToTwipsY(cboSurgDefined1.Top)) + ((float)VB6.PixelsToTwipsY(cboSurgDefined1.Height)) + modMain.gn4DLUS;
                }
            }
            else
            {
                fdlSurgDefined1.Visible = false;
                cboSurgDefined1.Visible = false;
            }

            if (mlSurgDef2CodeSet > 0)
            {
                fdlSurgDefined2.Visible = true;
                cboSurgDefined2.Visible = true;
                fdlSurgDefined2.SetBounds(Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dNextSDFLeft)), Convert.ToInt32((float)VB6.TwipsToPixelsY((float)dNextSDFTop)), Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dRightSideFieldWidth)), Convert.ToInt32((float)VB6.TwipsToPixelsY((float)dLabelHeight)));

                cboSurgDefined2.SetBounds(Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dNextSDFLeft)), Convert.ToInt32((float)(fdlSurgDefined2.Top + fdlSurgDefined2.Height)), Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dRightSideFieldWidth)), 0, BoundsSpecified.X | BoundsSpecified.Y | BoundsSpecified.Width);

                //determine if we need to go down or across
                if (dNextSDFLeft == ((float)VB6.PixelsToTwipsX(lblSpecialty.Left)))
                {
                    //need to go to the right
                    dNextSDFLeft = (float)VB6.PixelsToTwipsX(lblCaseLevel.Left);
                }
                else
                {
                    //need to go down and to the left
                    dNextSDFLeft = (float)VB6.PixelsToTwipsX(lblSpecialty.Left);
                    dNextSDFTop = ((float)VB6.PixelsToTwipsY(cboSurgDefined2.Top)) + ((float)VB6.PixelsToTwipsY(cboSurgDefined2.Height)) + modMain.gn4DLUS;
                }
            }
            else
            {
                fdlSurgDefined2.Visible = false;
                cboSurgDefined2.Visible = false;
            }

            if (mlSurgDef3CodeSet > 0)
            {
                fdlSurgDefined3.Visible = true;
                cboSurgDefined3.Visible = true;
                fdlSurgDefined3.SetBounds(Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dNextSDFLeft)), Convert.ToInt32((float)VB6.TwipsToPixelsY((float)dNextSDFTop)), Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dRightSideFieldWidth)), Convert.ToInt32((float)VB6.TwipsToPixelsY((float)dLabelHeight)));

                cboSurgDefined3.SetBounds(Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dNextSDFLeft)), Convert.ToInt32((float)(fdlSurgDefined3.Top + fdlSurgDefined3.Height)), Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dRightSideFieldWidth)), 0, BoundsSpecified.X | BoundsSpecified.Y | BoundsSpecified.Width);

                //determine if we need to go down or across
                if (dNextSDFLeft == ((float)VB6.PixelsToTwipsX(lblSpecialty.Left)))
                {
                    //need to go to the right
                    dNextSDFLeft = (float)VB6.PixelsToTwipsX(lblCaseLevel.Left);
                }
                else
                {
                    //need to go down and to the left
                    dNextSDFLeft = (float)VB6.PixelsToTwipsX(lblSpecialty.Left);
                    dNextSDFTop = ((float)VB6.PixelsToTwipsY(cboSurgDefined3.Top)) + ((float)VB6.PixelsToTwipsY(cboSurgDefined3.Height)) + modMain.gn4DLUS;
                }
            }
            else
            {
                fdlSurgDefined3.Visible = false;
                cboSurgDefined3.Visible = false;
            }

            if (mlSurgDef4CodeSet > 0)
            {
                fdlSurgDefined4.Visible = true;
                cboSurgDefined4.Visible = true;
                fdlSurgDefined4.SetBounds(Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dNextSDFLeft)), Convert.ToInt32((float)VB6.TwipsToPixelsY((float)dNextSDFTop)), Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dRightSideFieldWidth)), Convert.ToInt32((float)VB6.TwipsToPixelsY((float)dLabelHeight)));

                cboSurgDefined4.SetBounds(Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dNextSDFLeft)), Convert.ToInt32((float)(fdlSurgDefined4.Top + fdlSurgDefined4.Height)), Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dRightSideFieldWidth)), 0, BoundsSpecified.X | BoundsSpecified.Y | BoundsSpecified.Width);

                //determine if we need to go down or across
                if (dNextSDFLeft == ((float)VB6.PixelsToTwipsX(lblSpecialty.Left)))
                {
                    //need to go to the right
                    dNextSDFLeft = (float)VB6.PixelsToTwipsX(lblCaseLevel.Left);
                }
                else
                {
                    //need to go down and to the left
                    dNextSDFLeft = (float)VB6.PixelsToTwipsX(lblSpecialty.Left);
                    dNextSDFTop = ((float)VB6.PixelsToTwipsY(cboSurgDefined4.Top)) + ((float)VB6.PixelsToTwipsY(cboSurgDefined4.Height)) + modMain.gn4DLUS;
                }
            }
            else
            {
                fdlSurgDefined4.Visible = false;
                cboSurgDefined4.Visible = false;
            }

            if (mlSurgDef5CodeSet > 0)
            {
                fdlSurgDefined5.Visible = true;
                cboSurgDefined5.Visible = true;
                fdlSurgDefined5.SetBounds(Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dNextSDFLeft)), Convert.ToInt32((float)VB6.TwipsToPixelsY((float)dNextSDFTop)), Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dRightSideFieldWidth)), Convert.ToInt32((float)VB6.TwipsToPixelsY((float)dLabelHeight)));

                cboSurgDefined5.SetBounds(Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dNextSDFLeft)), Convert.ToInt32((float)(fdlSurgDefined5.Top + fdlSurgDefined5.Height)), Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dRightSideFieldWidth)), 0, BoundsSpecified.X | BoundsSpecified.Y | BoundsSpecified.Width);
                //determine if we need to go down or across
                if (dNextSDFLeft == ((float)VB6.PixelsToTwipsX(lblSpecialty.Left)))
                {
                    //need to go to the right
                    dNextSDFLeft = (float)VB6.PixelsToTwipsX(lblCaseLevel.Left);
                }
                else
                {
                    //need to go down and to the left
                    dNextSDFLeft = (float)VB6.PixelsToTwipsX(lblSpecialty.Left);
                    dNextSDFTop = ((float)VB6.PixelsToTwipsY(cboSurgDefined4.Top)) + ((float)VB6.PixelsToTwipsY(cboSurgDefined4.Height)) + modMain.gn4DLUS;
                }
            }
            else
            {
                fdlSurgDefined5.Visible = false;
                cboSurgDefined5.Visible = false;
            }

            //Now determine where the frame/listview should size to
            if (dNextSDFLeft == ((float)VB6.PixelsToTwipsX(lblSpecialty.Left)))
            {
                //we've already advanced the top to the next line.  This is the bottom of the frame
                fraDefaults.Height = (int)VB6.TwipsToPixelsY(dNextSDFTop); // + glFudgeFactor
            }
            else
            {
                //need to move down
                dNextSDFTop = dNextSDFTop + dLabelHeight + dBoxHeight + modMain.gn4DLUS; // + glFudgeFactor
                fraDefaults.Height = (int)VB6.TwipsToPixelsY(dNextSDFTop);
            }

            lvwSurgAreas.SetBounds(Convert.ToInt32(lblSurgAreas.Left), Convert.ToInt32((float)(lblSurgAreas.Top + lblSurgAreas.Height)), Convert.ToInt32((float)VB6.TwipsToPixelsX((float)dLeftSide)), Convert.ToInt32((float)(fraDefaults.Height - VB6.TwipsToPixelsY(modMain.gn4DLUS))));


            cmdCancel.SetBounds(Convert.ToInt32((float)VB6.TwipsToPixelsX(((float)VB6.PixelsToTwipsX(ClientRectangle.Width)) - modMain.gn4DLUS - ((float)VB6.PixelsToTwipsX(cmdCancel.Width)) - modMain.glFudgeFactor)), Convert.ToInt32((float)VB6.TwipsToPixelsY(((float)VB6.PixelsToTwipsY(fraDefaults.Top)) + ((float)VB6.PixelsToTwipsY(fraDefaults.Height)) + modMain.gn4DLUS)), Convert.ToInt32(cmdCancel.Width), Convert.ToInt32((float)VB6.TwipsToPixelsY((float)dBoxHeight)));

            cmdOk.SetBounds(Convert.ToInt32((float)(cmdCancel.Left - VB6.TwipsToPixelsX(modMain.gn4DLUS) - cmdOk.Width)), Convert.ToInt32(cmdCancel.Top), Convert.ToInt32(cmdCancel.Width), Convert.ToInt32((float)VB6.TwipsToPixelsY((float)dBoxHeight)));

            cmdAddRemove.SetBounds(Convert.ToInt32(lblSurgAreas.Left), Convert.ToInt32(cmdOk.Top), Convert.ToInt32(cmdAddRemove.Width), Convert.ToInt32((float)VB6.TwipsToPixelsY((float)dBoxHeight)));

            Height = (int)VB6.TwipsToPixelsY(((float)VB6.PixelsToTwipsY(cmdOk.Top)) + ((float)VB6.PixelsToTwipsY(cmdOk.Height)) + modMain.gn4DLUS + (((float)VB6.PixelsToTwipsY(Height)) - ((float)VB6.PixelsToTwipsY(ClientRectangle.Height))));
            Width = (int)VB6.TwipsToPixelsX(((float)VB6.PixelsToTwipsX(fraDefaults.Left)) + ((float)VB6.PixelsToTwipsX(fraDefaults.Width)) + modMain.gn4DLUS + modMain.glFudgeFactor)+12;

        }

        private void PopulateComboBox(ComboBox cboCombo, object oValues, bool bAddBlank)
        {
            if (bAddBlank)
            {
                //add blank
                cboCombo.Items.Add(new StringValuePair<long>(" ", 0));
                cboCombo.SelectedIndex = 0;
            }
            cboCombo.BeginUpdate();
            if (cboCombo == cboSpecialty)
            {
                SNCommon.CSpecialties tmp = (SNCommon.CSpecialties)oValues;
                if (oValues != null)
                {
                    for (int lItemIndex = 1; lItemIndex <= ((SNCommon.CSpecialties)oValues).Count; lItemIndex++)
                    {
                        cboCombo.Items.Add(new StringValuePair<long>(((SNCommon.CSpecialties)oValues)[lItemIndex].sSurgSpecialtyDesc, lItemIndex));
                    }
                }
               
            }
            else
            {
                if (oValues != null)
                {
                    for (int lItemIndex = 1; lItemIndex <= ((SNCommon.CCodeValues)oValues).Count; lItemIndex++)
                    {
                        cboCombo.Items.Add(new StringValuePair<long>(((SNCommon.CCodeValues)oValues)[lItemIndex].sDisplay, lItemIndex));
                    }
                }
            }
            cboCombo.EndUpdate();
        }

        private void PopulateComboBoxes()
        {
            PopulateComboBox(cboSpecialty, Specialties, true);
            PopulateComboBox(cboWoundClass, WoundClasses, true);
            PopulateComboBox(cboAnesType, AnesthesiaTypes, true);
            PopulateComboBox(cboCaseLevel, CaseLevels, true);

            fdlSurgDefined1.Text = msSurgDef1Prompt;
            fdlSurgDefined2.Text = msSurgDef2Prompt;
            fdlSurgDefined3.Text = msSurgDef3Prompt;
            fdlSurgDefined4.Text = msSurgDef4Prompt;
            fdlSurgDefined5.Text = msSurgDef5Prompt;

            PopulateComboBox(cboSurgDefined1, SurgeryDefined1, true);
            PopulateComboBox(cboSurgDefined2, SurgeryDefined2, true);
            PopulateComboBox(cboSurgDefined3, SurgeryDefined3, true);
            PopulateComboBox(cboSurgDefined4, SurgeryDefined4, true);
            PopulateComboBox(cboSurgDefined5, SurgeryDefined5, true);
        }

        private void frmProcedureDetails_MouseDown(Object eventSender, MouseEventArgs eventArgs)
        {
            int Button = (int)eventArgs.Button;
            int Shift = (int)Control.ModifierKeys / 0x10000;
            float x = (float)VB6.PixelsToTwipsX(eventArgs.X);
            float y = (float)VB6.PixelsToTwipsY(eventArgs.Y);
            PcsCollectionDumps.CCollectionDumpCtlr oDumps = null;
            ListViewItem oListItem = null;

            if (Button == ((int)MouseButtons.Right))
            {
                if (Shift == ((int)ShiftConstants.CtrlMask) + ((int)ShiftConstants.AltMask))
                {
                    //Display collection information.
                    //create the object
                    oDumps = new PcsCollectionDumps.CCollectionDumpCtlr();
                    //you can dump out module level variable too for added benefit
                    oDumps.SetDump("Modular Level Variables", System.String.Empty, "MOD_LEV_VAR", String.Empty, false);
                    //add a child to MOD_LEV_VAR
                    oDumps.SetDump("mbClearingFields", mbClearingFields.ToString(), "mbClearingFields", "MOD_LEV_VAR", true);
                    oDumps.SetDump("mbPopulatingFields", mbPopulatingFields.ToString(), "mbPopulatingFields", "MOD_LEV_VAR", true);
                    oDumps.SetDump("mlSurgDef1CodeSet", mlSurgDef1CodeSet.ToString(), "mlSurgDef1CodeSet", "MOD_LEV_VAR", true);
                    oDumps.SetDump("msSurgDef1Prompt", msSurgDef1Prompt, "msSurgDef1Prompt", "MOD_LEV_VAR", true);
                    oDumps.SetDump("mlSurgDef2CodeSet", mlSurgDef2CodeSet.ToString(), "mlSurgDef2CodeSet", "MOD_LEV_VAR", true);
                    oDumps.SetDump("msSurgDef2Prompt", msSurgDef2Prompt, "msSurgDef2Prompt", "MOD_LEV_VAR", true);
                    oDumps.SetDump("mlSurgDef3CodeSet", mlSurgDef3CodeSet.ToString(), "mlSurgDef3CodeSet", "MOD_LEV_VAR", true);
                    oDumps.SetDump("msSurgDef3Prompt", msSurgDef3Prompt, "msSurgDef3Prompt", "MOD_LEV_VAR", true);
                    oDumps.SetDump("mlSurgDef4CodeSet", mlSurgDef4CodeSet.ToString(), "mlSurgDef4CodeSet", "MOD_LEV_VAR", true);
                    oDumps.SetDump("msSurgDef4Prompt", msSurgDef4Prompt, "msSurgDef4Prompt", "MOD_LEV_VAR", true);
                    oDumps.SetDump("mlSurgDef5CodeSet", mlSurgDef5CodeSet.ToString(), "mlSurgDef5CodeSet", "MOD_LEV_VAR", true);
                    oDumps.SetDump("msSurgDef6Prompt", msSurgDef5Prompt, "msSurgDef5Prompt", "MOD_LEV_VAR", true);
                    oDumps.SetDump("mdCurOrderCatalogCd", mdCurOrderCatalogCd.ToString(), "mdCurOrderCatalogCd", "MOD_LEV_VAR", true);
                    oDumps.SetDump("msProcedureName", msProcedureName, "msProcedureName", "MOD_LEV_VAR", true);
                    oDumps.SetDump("mbArrowingThroughList", mbArrowingThroughList.ToString(), "mbArrowingThroughList", "MOD_LEV_VAR", true);

                    oDumps.SetDump("moSelectedAreas", System.String.Empty, "moSelectedAreas", "MOD_LEV_VAR", false);
                    if (moSelectedAreas != null)
                    {
                        for (int lIndex = 1, iteratorTest = moSelectedAreas.Count; lIndex <= iteratorTest; lIndex++)
                        {
                            oListItem = (ListViewItem)moSelectedAreas.GetItemFromCollection(lIndex);
                            oDumps.SetDump("Key", oListItem.Name, "moSelectedAreas" + "Key", "moSelectedAreas", true);
                            oDumps.SetDump("Text", oListItem.Text, "moSelectedAreas" + "Text", "moSelectedAreas", true);
                        }
                    }

                    if (moProcedure != null)
                    {
                        oDumps.SetDump("moProcedure", System.String.Empty, "moProcedure", "MOD_LEV_VAR", false);
                        //give that node to the singular class as a parent
                        moProcedure.PopulateCollectionDump(ref oDumps, "moProcedure");
                    }

                    if (moSurgAreas != null)
                    {
                        //create a parent node for the singlar entity class (who's parent is MOD_LEV_VAR)
                        oDumps.SetDump("moSurgAreas", System.String.Empty, "moSurgAreas", "MOD_LEV_VAR", false);
                        //give that node to the singular class as a parent
                        moSurgAreas.PopulateCollectionDump(ref oDumps, "moSurgAreas");
                    }

                    if (moSurgAreasBackup != null)
                    {
                        //create a parent node for the singlar entity class (who's parent is MOD_LEV_VAR)
                        oDumps.SetDump("moSurgAreasBackup", System.String.Empty, "moSurgAreasBackup", "MOD_LEV_VAR", false);
                        //give that node to the singular class as a parent
                        moSurgAreas.PopulateCollectionDump(ref oDumps, "moSurgAreasBackup");
                    }

                    oDumps.ShowCollectionDump(0, "Procedure Details Collection Dump"); //NOTE: ghApp of function does nothing
                }
            }
        }

        private void lvwSurgAreas_Click(Object eventSender, EventArgs eventArgs)
        {
            FillOutSelectedAreas();
            EnableDisableDefaults(true);
            PopulateDefaultFields();
        }

        private void FillOutSelectedAreas()
        {
            ListViewItem oListItem = null;
            CodeTimer SNCrmTimer = new CodeTimer(this.GetType(), "ENG:SRG FillOutSelectedAreas", true);
            //modMain.SNCrmTimers_CCrmTimers_definst.SNCrmStartTimeFunction("FillOutSelectedAreas", SNCrmTimers.eTeamType.eSRG, SNCrmTimers.eTagType.eENG);
            moSelectedAreas = null;
            moSelectedAreas = new Collection();
            for (int lItemIndex = 1, iteratorTest = lvwSurgAreas.Items.Count; lItemIndex <= iteratorTest; lItemIndex++)
            {
                oListItem = (ListViewItem)lvwSurgAreas.Items[lItemIndex - 1];
                if (oListItem.Selected)
                {
                    moSelectedAreas.AddItemToCollection(oListItem, null);
                }
            }
            //modMain.SNCrmTimers_CCrmTimers_definst.SNCrmEndTimeFunction("FillOutSelectedAreas");
            SNCrmTimer.Stop();
        }

        private void SelectValueInComboBox(ComboBox cboCombo, int lItemData)
        {

            for (int lIdx = 0, iteratorTest = cboCombo.Items.Count - 1; lIdx <= iteratorTest; lIdx++)
            {
                if (((StringValuePair<long>)cboCombo.Items[lIdx]).Value == lItemData)
                {
                    cboCombo.SelectedIndex = lIdx;
                    break;
                }
            }
        }

        private int GetSpecialtyIndexBySpecialtyId(double dSpecialtyId)
        {
            int result = 0;
            CodeTimer SNCrmTimer = new CodeTimer(this.GetType(), "ENG:SRG GetSpecialtyIndexBySpecialtyId", true);
            //modMain.SNCrmTimers_CCrmTimers_definst.SNCrmStartTimeFunction("GetSpecialtyIndexBySpecialtyId", SNCrmTimers.eTeamType.eSRG, SNCrmTimers.eTagType.eENG);
            SNCommon.CSpecialties oSpecialties = Specialties;
            if (oSpecialties != null)
            {
                for (int lIdx = 1; lIdx <= oSpecialties.Count; lIdx++)
                {
                    if (oSpecialties[lIdx].dSurgSpecialtyId == dSpecialtyId)
                    {
                        result = lIdx;
                        break;
                    }




                }
            }
            // modMain.SNCrmTimers_CCrmTimers_definst.SNCrmEndTimeFunction("GetSpecialtyIndexBySpecialtyId");
            SNCrmTimer.Stop();
            return result;
        }

        private int GetCodeValueIndexByCodeValue(SNCommon.CCodeValues oCodeValues, double dCodeValue)
        {
            int result = 0;
            CodeTimer SNCrmTimer = new CodeTimer(this.GetType(), "ENG:SRG " + "GetCodeValueIndexByCodeValue - " + Convert.ToString(oCodeValues.Count), true);
            //modMain.SNCrmTimers_CCrmTimers_definst.SNCrmStartTimeFunction("GetCodeValueIndexByCodeValue - " + Convert.ToString(oCodeValues.Count), SNCrmTimers.eTeamType.eSRG, SNCrmTimers.eTagType.eENG);
            if (oCodeValues != null)
            {

                for (int lIdx = 1; lIdx <= oCodeValues.Count; lIdx++)
                {
                    if (oCodeValues[lIdx].dCodeValue == dCodeValue)
                    {

                        result = lIdx;
                        break;
                    }
                }
            }
            //modMain.SNCrmTimers_CCrmTimers_definst.SNCrmEndTimeFunction("GetCodeValueIndexByCodeValue - " + Convert.ToString(oCodeValues.Count));
            SNCrmTimer.Stop();
            return result;
        }

        private SNCommon.CCodeValue GetSelectedCodeValueFromComboBox(ComboBox cboCombo, SNCommon.CCodeValues oCodeValues)
        {
            if (cboCombo.SelectedIndex != -1)
            {
                //-- Get the exact index value populated in combobox which is in sync with oCodeValues
                int indexObj = Convert.ToInt32(((StringValuePair<long>)cboCombo.Items[cboCombo.SelectedIndex]).Value);
                return oCodeValues[indexObj];
            }
            else
            {
                return null;
            }
        }

        private SNCommon.CSpecialty GetSelectedSpecialtyFromComboBox()
        {
            SNCommon.CSpecialties oSpecialties = null;
            CodeTimer SNCrmTimer = new CodeTimer(this.GetType(), "ENG:SRG GetSelectedSpecialtyFromComboBox", true);
            //modMain.SNCrmTimers_CCrmTimers_definst.SNCrmStartTimeFunction("GetSelectedSpecialtyFromComboBox", SNCrmTimers.eTeamType.eSRG, SNCrmTimers.eTagType.eENG);
            if (cboSpecialty.SelectedIndex != -1)
            {
                oSpecialties = Specialties;
                if (oSpecialties != null)
                {
                    int indexObj = Convert.ToInt32(((StringValuePair<long>)cboSpecialty.Items[cboSpecialty.SelectedIndex]).Value);
                    return oSpecialties[indexObj];
                }
            }
            //modMain.SNCrmTimers_CCrmTimers_definst.SNCrmEndTimeFunction("GetSelectedSpecialtyFromComboBox");
            SNCrmTimer.Stop();
            return null;
        }

        private void UpdateAnesthesiaType()
        {
            if (cboAnesType.SelectedIndex != -1)
            {
                UpdateProcedureDetail(msFIELD_ANES_TYPE);
            }
        }

        private void UpdateSpecialty()
        {
            if (cboSpecialty.SelectedIndex != -1)
            {
                UpdateProcedureDetail(msFIELD_SPECIALTY);
            }
        }

        private void UpdateCaseLevel()
        {
            if (cboCaseLevel.SelectedIndex != -1)
            {
                UpdateProcedureDetail(msFIELD_CASE_LEVEL);
            }
        }

        private void UpdateWoundClass()
        {
            if (cboWoundClass.SelectedIndex != -1)
            {
                UpdateProcedureDetail(msFIELD_WOUND_CLASS);
            }
        }

        private void UpdateSurgDefined1()
        {
            if (cboSurgDefined1.SelectedIndex != -1)
            {
                UpdateProcedureDetail(msFIELD_SURG_DEF1);
            }
        }

        private void UpdateSurgDefined2()
        {
            if (cboSurgDefined2.SelectedIndex != -1)
            {
                UpdateProcedureDetail(msFIELD_SURG_DEF2);
            }
        }

        private void UpdateSurgDefined3()
        {
            if (cboSurgDefined3.SelectedIndex != -1)
            {
                UpdateProcedureDetail(msFIELD_SURG_DEF3);
            }
        }

        private void UpdateSurgDefined4()
        {
            if (cboSurgDefined4.SelectedIndex != -1)
            {
                UpdateProcedureDetail(msFIELD_SURG_DEF4);
            }
        }

        private void UpdateSurgDefined5()
        {
            if (cboSurgDefined5.SelectedIndex != -1)
            {
                UpdateProcedureDetail(msFIELD_SURG_DEF5);
            }
        }

        private void UpdateSpecimenRequired()
        {
            UpdateProcedureDetail(msFIELD_SPEC_REQ);
        }

        private void UpdateFrozenSection()
        {
            UpdateProcedureDetail(msFIELD_FROZEN);
        }

        private void UpdateBlood()
        {
            UpdateProcedureDetail(msFIELD_BLOOD);
        }

        private void UpdateImplant()
        {
            UpdateProcedureDetail(msFIELD_IMPLANT);
        }

        private void UpdateXray()
        {
            UpdateProcedureDetail(msFIELD_XRAY);
        }

        private void UpdateXRayTech()
        {
            UpdateProcedureDetail(msFIELD_XRAY_TECH);
        }

        private void UpdateDuration()
        {
            UpdateProcedureDetail(msFIELD_PROC_DUR);
            CheckAreasForRequiredFieldsComplete(0);
        }

        private void UpdateSetupTime()
        {
            UpdateProcedureDetail(msFIELD_SETUP_TIME);
        }

        private void UpdateProcedureCount()
        {
            UpdateProcedureDetail(msFIELD_PROC_COUNT);
            CheckAreasForRequiredFieldsComplete(0);
        }

        private void UpdateCleanupTime()
        {
            UpdateProcedureDetail(msFIELD_CLEAN_TIME);
        }

        private void UpdatePreIncisionTime()
        {
            UpdateProcedureDetail(msFIELD_PRE_TIME);
        }

        private void UpdatePostClosureTime()
        {
            UpdateProcedureDetail(msFIELD_POST_TIME);
        }

        private void lvwSurgAreas_ItemClick(ListViewItem Item)
        {
            if (mbArrowingThroughList)
            {
                lvwSurgAreas_Click(lvwSurgAreas, new EventArgs());
                mbArrowingThroughList = false;
            }
        }

        private void lvwSurgAreas_KeyDown(Object eventSender, KeyEventArgs eventArgs)
        {
            int KeyCode = (int)eventArgs.KeyCode;
            int Shift = (int)eventArgs.KeyData / 0x10000;
            if (KeyCode == ((int)Keys.Down) || KeyCode == ((int)Keys.Up))
            {
                mbArrowingThroughList = true;
            }
        }

        private void lvwSurgAreas_KeyUp(Object eventSender, KeyEventArgs eventArgs)
        {
            int KeyCode = (int)eventArgs.KeyCode;
            int Shift = (int)eventArgs.KeyData / 0x10000;
            mbArrowingThroughList = false;
        }

        private void mebCleanupTime_TextChanged(Object eventSender, EventArgs eventArgs)
        {
            if (!(Convert.ToString(mebCleanupTime.Value).Trim(' ').Length > 0))
            {
                mebCleanupTime.Text = "0";
            }
            if (!mbClearingFields && !mbPopulatingFields)
            {
                mebCleanupTime.BackColor = SystemColors.Window;
                UpdateCleanupTime();
            }

        }

        private void mebDuration_TextChanged(Object eventSender, EventArgs eventArgs)
        {
            if (!(Convert.ToString(mebDuration.Value).Trim(' ').Length > 0))
            {
                mebDuration.Text = "1";
            }
            if (!mbClearingFields && !mbPopulatingFields)
            {
                mebDuration.BackColor = SystemColors.Window;
                UpdateDuration();
            }
        }

        //sets icon if it finds a required field not answered.
        //returns true if all are ok
        private bool CheckAreasForRequiredFieldsComplete( int lFirstRequiredKey)
        {
            bool result = false;
            int lSurgAreaKey = 0;
            ListViewItem oListItem = null;
            CSurgArea oSurgArea = null;
            CProcDetail oProcDetail = null;
            bool bTurnOnWarning = false;

            lFirstRequiredKey = 0;
            result = true;
            for (int lIndex = 1, iteratorTest = lvwSurgAreas.Items.Count; lIndex <= iteratorTest; lIndex++)
            {
                bTurnOnWarning = false;
                oListItem = (ListViewItem)lvwSurgAreas.Items[lIndex - 1];
                lSurgAreaKey = Convert.ToInt32(Double.Parse(oListItem.Name.Substring(1)));
                oSurgArea = moSurgAreas.ItemByKey(lSurgAreaKey.ToString());
                if (oSurgArea != null)
                {
                    for (int lProcDetailIndex = 1; lProcDetailIndex <= oSurgArea.oProcDetails.Count; lProcDetailIndex++)
                    {
                        oProcDetail = oSurgArea.oProcDetails[lProcDetailIndex];
                        if (oProcDetail.nState != SNCommon.Application.SNStateConstants.snStateDeleted)
                        {
                            if (!CheckProcDetailForRequiredFields(oProcDetail))
                            {
                                //set area icon
                                bTurnOnWarning = false;
                                if (lFirstRequiredKey == 0)
                                {
                                    lFirstRequiredKey = lSurgAreaKey;
                                }
                            }
                            break;
                        }
                    }

                }
                if (bTurnOnWarning)
                {
                    oListItem.ImageKey = msWARNING;
                    result = false;
                }
                else
                {
                    oListItem.ImageIndex = -1;
                }
            }
            System.Windows.Forms.Application.DoEvents();
            lvwSurgAreas.Refresh();

            return result;
        }

        private bool CheckProcDetailForRequiredFields(CProcDetail oProcDetail)
        {
            bool result = false;
            if (!(oProcDetail.dAnesthesiaTypeCd > 0))
            {
                return result;
            }
            if (!(oProcDetail.dSurgSpecialtyId > 0))
            {
                return result;
            }
            if (!(oProcDetail.dCaseLevelCd > 0))
            {
                return result;
            }
            if (!(oProcDetail.dWoundClassCd > 0))
            {
                return result;
            }
            //all fields checked out
            return true;
        }

        private void mebPostClosureTime_TextChanged(Object eventSender, EventArgs eventArgs)
        {
            if (!(Convert.ToString(mebPostClosureTime.Value).Trim(' ').Length > 0))
            {
                mebPostClosureTime.Text = "0";
            }
            if (!mbClearingFields && !mbPopulatingFields)
            {
                mebPostClosureTime.BackColor = SystemColors.Window;
                UpdatePostClosureTime();
            }

        }

        private void mebPreIncisionTime_TextChanged(Object eventSender, EventArgs eventArgs)
        {
            if (!(Convert.ToString(mebPreIncisionTime.Value).Trim(' ').Length > 0))
            {
                mebPreIncisionTime.Text = "0";
            }
            if (!mbClearingFields && !mbPopulatingFields)
            {
                mebPreIncisionTime.BackColor = SystemColors.Window;
                UpdatePreIncisionTime();
            }

        }

        private void mebProcedureCount_TextChanged(Object eventSender, EventArgs eventArgs)
        {
            if (Convert.ToString(mebProcedureCount.Value).Trim(' ').Length > 0)
            {
                mebProcedureCount.BackColor = SystemColors.Window;
            }
            else
            {
                mebProcedureCount.Text = "1";
                mebProcedureCount.BackColor = SystemColors.Window;
            }
            if (!mbClearingFields && !mbPopulatingFields)
            {
                mebProcedureCount.BackColor = SystemColors.Window;
                UpdateProcedureCount();
            }

        }

        private void mebSetupTime_TextChanged(Object eventSender, EventArgs eventArgs)
        {
            if (!(Convert.ToString(mebSetupTime.Value).Trim(' ').Length > 0))
            {
                mebSetupTime.Text = "0";
            }
            if (!mbClearingFields && !mbPopulatingFields)
            {
                mebSetupTime.BackColor = SystemColors.Window;
                UpdateSetupTime();
            }

        }

        private void UpdateProcedureDetail(string sFIELD)
        {
            CSurgArea oSurgArea = null;
            CProcDetail oProcDetail = null;
            SNCommon.CCodeValue oCodeValue = null;
            SNCommon.CSpecialty oSpecialty = null;

            //PRECONDITION: for combo boxes, we are assuming the calling field's list index is NOT -1
            //If calling this procedure, must ensure this to be true

            if (sFIELD == msFIELD_SPECIALTY)
            {
                oSpecialty = GetSelectedSpecialtyFromComboBox();
                for (int lIndex = 1, iteratorTest = moSelectedAreas.Count; lIndex <= iteratorTest; lIndex++)
                {
                    oSurgArea = moSurgAreas.ItemByKey(((ListViewItem)moSelectedAreas.GetItemFromCollection(lIndex)).Name.Substring(1));
                    if (oSurgArea != null)
                    {
                        for (int lProcDetailIndex = 1; lProcDetailIndex <= oSurgArea.oProcDetails.Count; lProcDetailIndex++)
                        {
                            oProcDetail = oSurgArea.oProcDetails[lProcDetailIndex];
                            if (oProcDetail.nState != SNCommon.Application.SNStateConstants.snStateDeleted)
                            {
                                if (oSpecialty != null)
                                {
                                    oProcDetail.dSurgSpecialtyId = oSpecialty.dSurgSpecialtyId;
                                    oProcDetail.sSurgSpecialtyDisp = oSpecialty.sSurgSpecialtyDesc;
                                }
                                else
                                {
                                    oProcDetail.dSurgSpecialtyId = 0;
                                    oProcDetail.sSurgSpecialtyDisp = System.String.Empty;
                                }
                                oProcDetail.SetToModifiedState();
                            }
                        }
                    }
                }

            }
            else
            {
                switch (sFIELD)
                {
                    case msFIELD_ANES_TYPE:
                        oCodeValue = GetSelectedCodeValueFromComboBox(cboAnesType, AnesthesiaTypes);
                        break;
                    case msFIELD_WOUND_CLASS:
                        oCodeValue = GetSelectedCodeValueFromComboBox(cboWoundClass, WoundClasses);
                        break;
                    case msFIELD_CASE_LEVEL:
                        oCodeValue = GetSelectedCodeValueFromComboBox(cboCaseLevel, CaseLevels);
                        break;
                    case msFIELD_SURG_DEF1:
                        oCodeValue = GetSelectedCodeValueFromComboBox(cboSurgDefined1, SurgeryDefined1);
                        break;
                    case msFIELD_SURG_DEF2:
                        oCodeValue = GetSelectedCodeValueFromComboBox(cboSurgDefined2, SurgeryDefined2);
                        break;
                    case msFIELD_SURG_DEF3:
                        oCodeValue = GetSelectedCodeValueFromComboBox(cboSurgDefined3, SurgeryDefined3);
                        break;
                    case msFIELD_SURG_DEF4:
                        oCodeValue = GetSelectedCodeValueFromComboBox(cboSurgDefined4, SurgeryDefined4);
                        break;
                    case msFIELD_SURG_DEF5:
                        oCodeValue = GetSelectedCodeValueFromComboBox(cboSurgDefined5, SurgeryDefined5);
                        break;
                }

                for (int lIndex = 1, iteratorTest = moSelectedAreas.Count; lIndex <= iteratorTest; lIndex++)
                {
                    oSurgArea = moSurgAreas.ItemByKey(((ListViewItem)moSelectedAreas.GetItemFromCollection(lIndex)).Name.Substring(1));
                    if (oSurgArea != null)
                    {
                        for (int lProcDetailIndex = 1; lProcDetailIndex <= oSurgArea.oProcDetails.Count; lProcDetailIndex++)
                        {
                            oProcDetail = oSurgArea.oProcDetails[lProcDetailIndex];
                            if (oProcDetail.nState != SNCommon.Application.SNStateConstants.snStateDeleted)
                            {
                                //update field based on the field meaning
                                switch (sFIELD)
                                {
                                    case msFIELD_ANES_TYPE:
                                        if (oCodeValue == null)
                                        {
                                            oProcDetail.dAnesthesiaTypeCd = 0;
                                            oProcDetail.sAnesthesiaTypeDisp = System.String.Empty;
                                        }
                                        else
                                        {
                                            oProcDetail.dAnesthesiaTypeCd = oCodeValue.dCodeValue;
                                            oProcDetail.sAnesthesiaTypeDisp = oCodeValue.sDisplay;
                                        }

                                        break;
                                    case msFIELD_BLOOD:
                                        if (chkBlood.CheckState == CheckState.Checked)
                                        {
                                            oProcDetail.nBloodProductReqInd = 1;
                                        }
                                        else
                                        {
                                            oProcDetail.nBloodProductReqInd = 0;
                                        }
                                        break;
                                    case msFIELD_CASE_LEVEL:
                                        if (oCodeValue == null)
                                        {
                                            oProcDetail.dCaseLevelCd = 0;
                                            oProcDetail.sCaseLevelDisp = System.String.Empty;
                                        }
                                        else
                                        {
                                            oProcDetail.dCaseLevelCd = oCodeValue.dCodeValue;
                                            oProcDetail.sCaseLevelDisp = oCodeValue.sDisplay;
                                        }
                                        break;
                                    case msFIELD_FROZEN:
                                        if (chkFrozenSection.CheckState == CheckState.Checked)
                                        {
                                            oProcDetail.nFrozenSectionReqInd = 1;
                                        }
                                        else
                                        {
                                            oProcDetail.nFrozenSectionReqInd = 0;
                                        }
                                        break;
                                    case msFIELD_IMPLANT:
                                        if (chkImplant.CheckState == CheckState.Checked)
                                        {
                                            oProcDetail.nImplantInd = 1;
                                        }
                                        else
                                        {
                                            oProcDetail.nImplantInd = 0;
                                        }
                                        break;
                                    case msFIELD_PROC_COUNT:
                                        oProcDetail.dProcCnt = Convert.ToDouble(mebProcedureCount.Value);

                                        break;
                                    case msFIELD_SPEC_REQ:
                                        if (chkSpecimenRequired.CheckState == CheckState.Checked)
                                        {
                                            oProcDetail.nSpecReqInd = 1;
                                        }
                                        else
                                        {
                                            oProcDetail.nSpecReqInd = 0;
                                        }

                                        break;
                                    case msFIELD_SURG_DEF1:
                                        if (oCodeValue != null)
                                        {
                                            oProcDetail.dUD1Cd = oCodeValue.dCodeValue;
                                            oProcDetail.sUD1Disp = oCodeValue.sDisplay;
                                        }
                                        else
                                        {
                                            oProcDetail.dUD1Cd = 0;
                                            oProcDetail.sUD1Disp = System.String.Empty;
                                        }
                                        break;
                                    case msFIELD_SURG_DEF2:
                                        if (oCodeValue != null)
                                        {
                                            oProcDetail.dUD2Cd = oCodeValue.dCodeValue;
                                            oProcDetail.sUD2Disp = oCodeValue.sDisplay;
                                        }
                                        else
                                        {
                                            oProcDetail.dUD2Cd = 0;
                                            oProcDetail.sUD2Disp = System.String.Empty;
                                        }
                                        break;
                                    case msFIELD_SURG_DEF3:
                                        if (oCodeValue != null)
                                        {
                                            oProcDetail.dUD3Cd = oCodeValue.dCodeValue;
                                            oProcDetail.sUD3Disp = oCodeValue.sDisplay;
                                        }
                                        else
                                        {
                                            oProcDetail.dUD3Cd = 0;
                                            oProcDetail.sUD3Disp = System.String.Empty;
                                        }

                                        break;
                                    case msFIELD_SURG_DEF4:
                                        if (oCodeValue != null)
                                        {
                                            oProcDetail.dUD4Cd = oCodeValue.dCodeValue;
                                            oProcDetail.sUD4Disp = oCodeValue.sDisplay;
                                        }
                                        else
                                        {
                                            oProcDetail.dUD4Cd = 0;
                                            oProcDetail.sUD4Disp = System.String.Empty;
                                        }
                                        break;
                                    case msFIELD_SURG_DEF5:
                                        if (oCodeValue != null)
                                        {
                                            oProcDetail.dUD5Cd = oCodeValue.dCodeValue;
                                            oProcDetail.sUD5Disp = oCodeValue.sDisplay;
                                        }
                                        else
                                        {
                                            oProcDetail.dUD5Cd = 0;
                                            oProcDetail.sUD5Disp = System.String.Empty;
                                        }
                                        break;
                                    case msFIELD_WOUND_CLASS:
                                        if (oCodeValue == null)
                                        {
                                            oProcDetail.dWoundClassCd = 0;
                                            oProcDetail.sWoundClassDisp = System.String.Empty;
                                        }
                                        else
                                        {
                                            oProcDetail.dWoundClassCd = oCodeValue.dCodeValue;
                                            oProcDetail.sWoundClassDisp = oCodeValue.sDisplay;
                                        }
                                        break;
                                    case msFIELD_XRAY:
                                        if (chkXRay.CheckState == CheckState.Checked)
                                        {
                                            oProcDetail.nXrayInd = 1;
                                        }
                                        else
                                        {
                                            oProcDetail.nXrayInd = 0;
                                        }
                                        break;
                                    case msFIELD_XRAY_TECH:
                                        if (chkXRayTech.CheckState == CheckState.Checked)
                                        {
                                            oProcDetail.nXrayTechInd = 1;
                                        }
                                        else
                                        {
                                            oProcDetail.nXrayTechInd = 0;
                                        }
                                        break;
                                }
                                if (sFIELD == msFIELD_CLEAN_TIME || sFIELD == msFIELD_POST_TIME || sFIELD == msFIELD_PRE_TIME || sFIELD == msFIELD_PROC_DUR || sFIELD == msFIELD_SETUP_TIME)
                                {
                                    UpdateProcedureDetailDuration(oProcDetail, sFIELD);

                                }
                                else
                                {
                                    oProcDetail.SetToModifiedState();
                                }
                            }
                        }
                    }
                }
            }
        }

        private void UpdateProcedureDetailDuration(CProcDetail oProcDetail, string sFIELD)
        {
            CProcDuration oProcDuration = null;

            for (int lProcDurIndex = 1, iteratorTest = oProcDetail.oProcDurations.Count; lProcDurIndex <= iteratorTest; lProcDurIndex++)
            {
                oProcDuration = oProcDetail.oProcDurations[lProcDurIndex];
                if (oProcDuration.nState != SNCommon.Application.SNStateConstants.snStateDeleted && oProcDuration.dPrsnlID == 0)
                {
                    switch (sFIELD)
                    {
                        case msFIELD_CLEAN_TIME:
                            oProcDuration.dDefCleanupDur = Convert.ToDouble(mebCleanupTime.Value);
                            break;
                        case msFIELD_POST_TIME:
                            oProcDuration.dDefPostClosureDur = Convert.ToDouble(mebPostClosureTime.Value);
                            break;
                        case msFIELD_PRE_TIME:
                            oProcDuration.dDefPreIncisionDur = Convert.ToDouble(mebPreIncisionTime.Value);
                            break;
                        case msFIELD_PROC_DUR:
                            oProcDuration.dDefProcedureDur = Convert.ToDouble(mebDuration.Value);
                            break;
                        case msFIELD_SETUP_TIME:
                            oProcDuration.dDefSetupDur = Convert.ToDouble(mebSetupTime.Value);
                            break;
                    }
                    oProcDuration.SetToModifiedState();
                    break;
                }
            }
        }

        private void frmProcedureDetails_Load(object sender, EventArgs e)
        {
            ListViewItem oListItem = null;
            if (lvwSurgAreas.Items.Count > 0)
            {
                oListItem = (ListViewItem)lvwSurgAreas.Items[0];
                oListItem.Selected = true;
                mbArrowingThroughList = true;
                lvwSurgAreas_ItemClick(oListItem);
            }
        }

        private void mebProcedureCount_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = !(e.KeyValue >= 48 && e.KeyValue <= 57 || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9) || e.KeyValue == 8 || e.KeyValue == 46 || e.KeyValue == 188 || e.KeyValue == 190 || e.KeyValue == 37 || e.KeyValue ==39|| (e.Control && (e.KeyValue == 86 || e.KeyValue == 67 || e.KeyValue == 88 || e.KeyValue == 65)));
           
        }
        private void mebProcedureCount_KeyUp(object sender, KeyEventArgs e)
        {
            if (!e.SuppressKeyPress && string.IsNullOrEmpty(mebProcedureCount.Text))
            {
                mebProcedureCount.Text = mebProcedureCount.Minimum.ToString();
            }
        }
        private void mebPreIncisionTime_KeyUp(object sender, KeyEventArgs e)
        {
            if (!e.SuppressKeyPress && string.IsNullOrEmpty(mebPreIncisionTime.Text))
            {
                mebPreIncisionTime.Text = mebPreIncisionTime.Minimum.ToString();
            }
        }

        private void mebPostClosureTime_KeyUp(object sender, KeyEventArgs e)
        {
            if (!e.SuppressKeyPress && string.IsNullOrEmpty(mebPostClosureTime.Text))
            {
                mebPostClosureTime.Text = mebPostClosureTime.Minimum.ToString();
            }
        }

        private void mebSetupTime_KeyUp(object sender, KeyEventArgs e)
        {
            if (!e.SuppressKeyPress && string.IsNullOrEmpty(mebSetupTime.Text))
            {
                mebSetupTime.Text = mebSetupTime.Minimum.ToString();
            }
        }
        private void mebCleanupTime_KeyUp(object sender, KeyEventArgs e)
        {
            if (!e.SuppressKeyPress && string.IsNullOrEmpty(mebCleanupTime.Text))
            {
                mebCleanupTime.Text = mebCleanupTime.Minimum.ToString();
            }
        }
        private void mebDuration_KeyUp(object sender, KeyEventArgs e)
        {
            if (!e.SuppressKeyPress && string.IsNullOrEmpty(mebDuration.Text))
            {
                mebDuration.Text = mebDuration.Minimum.ToString();
            }
        }

        private void NumericKeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox ntb = GetNumericUpDownTextBox((NumericUpDown)sender);
            ClipBoardText(ntb); 
            if (  ntb != null && e.KeyChar != '\b' && ntb.Text.Length + e.KeyChar.ToString().Length > 3&& ntb.SelectionLength <= 0)
            {
                e.Handled = true;
            }
        }

        private void ClipBoardText(TextBox txtValue)
        {

            string text = string.IsNullOrEmpty(txtValue.Text) ? string.Empty : txtValue.Text;
                String clipboardText = System.Windows.Forms.Clipboard.GetText();
            int length = 0;
            if (clipboardText.Length > 0)
            {
                if (text.Length >= 3)
                {
                    length = 0;
                    Clipboard.Clear();
                }
                else
                {
                    if (txtValue.SelectionLength > 0)
                    {

                    }
                    length = 3 - (text.Length - txtValue.SelectionLength);
                    Clipboard.Clear();
                    string newClipboardText = string.Empty;
                    if (clipboardText.Length > length)
                    {
                        newClipboardText = clipboardText.Substring(0, length);
                    }
                    else if (clipboardText.Length != 0)
                    {
                        newClipboardText = clipboardText.Substring(0, clipboardText.Length);
                    }
                    Clipboard.SetData(DataFormats.Text, (Object)newClipboardText);
                }
            }
        }

        private void NumericMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                TextBox ntb = GetNumericUpDownTextBox((NumericUpDown)sender);
                ClipBoardText(ntb);
            }
        }
        public static TextBox GetNumericUpDownTextBox(NumericUpDown nud)
        {
            TextBox tb = GetPrivateFieldValue(nud, "upDownEdit") as TextBox;
            return (tb);
        }

        public static object GetPrivateFieldValue(object obj, string fieldName)
        {
            Type t = obj.GetType();
            System.Reflection.FieldInfo[] fiArr = t.GetFields(
                        System.Reflection.BindingFlags.NonPublic
                        | System.Reflection.BindingFlags.Instance);
            if (fiArr != null)
            {
                foreach (System.Reflection.FieldInfo fi in fiArr)
                {
                    if (fi.Name == fieldName)
                        return (fi).GetValue(obj);
                }
            }
            return (null);
        } 
    }
}