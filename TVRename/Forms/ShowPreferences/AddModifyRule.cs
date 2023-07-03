//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TVRename;

/// <inheritdoc />
///  <summary>
///  Summary for AddModifyRule
///  WARNING: If you change the name of this class, you will need to change the
///           'Resource File Name' property for the managed resource compiler tool
///           associated with all .resx files this class depends on.  Otherwise,
///           the designers will not be able to interact properly with localized
///           resources associated with this form.
///  </summary>
public partial class AddModifyRule : Form
{
    private readonly ShowRule mRule;
    private readonly IEnumerable<ProcessedEpisode> eps;
    private readonly ProcessedSeason.SeasonType mOrder;
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public AddModifyRule(ShowRule rule, ShowConfiguration show, IEnumerable<ProcessedEpisode> s)
    {
        mRule = rule;
        eps = s;
        mOrder = show.Order;

        InitializeComponent();

        FillDialog();
    }

    private void rb_Click(object sender, EventArgs e) => EnableDisableAndLabels();

    private void FillDialog()
    {
        switch (mRule.DoWhatNow)
        {
            case RuleAction.kRename:
                rbRename.Checked = true;
                break;

            case RuleAction.kCollapse:
                rbCollapse.Checked = true;
                break;

            case RuleAction.kRemove:
                rbRemove.Checked = true;
                break;

            case RuleAction.kSwap:
                rbSwap.Checked = true;
                break;

            case RuleAction.kMerge:
                rbMerge.Checked = true;
                break;

            case RuleAction.kSplit:
                rbSplit.Checked = true;
                break;

            case RuleAction.kInsert:
                rbInsert.Checked = true;
                break;

            case RuleAction.kIgnoreEp:
                rbIgnore.Checked = true;
                break;

            default:
                throw new InvalidOperationException("Unexpected value mRule.DoWhatNow = " + mRule.DoWhatNow);
        }

        txtUserText.Text = mRule.UserSuppliedText;
        if (mRule.First != -1)
        {
            txtValue1.Text = mRule.First.ToString();
        }

        if (mRule.Second != -1)
        {
            txtValue2.Text = mRule.Second.ToString();
        }

        chkRenumberAfter.Checked = mRule.RenumberAfter;

        EnableDisableAndLabels();
    }

    private void EnableDisableAndLabels()
    {
        if (rbRemove.Checked || rbIgnore.Checked)
        {
            txtLabel1.Text = "&From/Number:";
            txtLabel2.Text = "T&o:";
            txtLeaveBlank.Visible = false;
            txtLabel2.Enabled = true;
            txtValue1.Enabled = true;
            txtValue2.Enabled = true;
            txtUserText.Enabled = false;
            txtWithNameLabel.Enabled = false;
        }
        else if (rbSwap.Checked)
        {
            txtLabel1.Text = "&Number:";
            txtLabel2.Text = "N&umber:";
            txtLeaveBlank.Visible = false;
            txtLabel2.Enabled = true;
            txtValue1.Enabled = true;
            txtValue2.Enabled = true;
            txtUserText.Enabled = false;
            txtWithNameLabel.Enabled = false;
        }
        else if (rbMerge.Checked || rbCollapse.Checked)
        {
            txtLabel1.Text = "&From:";
            txtLabel2.Text = "T&o:";
            txtLeaveBlank.Visible = true;
            txtLabel2.Enabled = true;
            txtValue1.Enabled = true;
            txtValue2.Enabled = true;
            txtUserText.Enabled = true;
            txtWithNameLabel.Enabled = true;
        }
        else if (rbInsert.Checked)
        {
            txtLabel1.Text = "&At:";
            txtLabel2.Text = "N&umber:";
            txtLeaveBlank.Visible = false;
            txtLabel2.Enabled = false;
            txtValue1.Enabled = true;
            txtValue2.Enabled = false;
            txtUserText.Enabled = true;
            txtWithNameLabel.Enabled = true;
        }
        else if (rbRename.Checked)
        {
            txtLabel1.Text = "&Number:";
            txtLabel2.Text = "N&umber:";
            txtLeaveBlank.Visible = false;
            txtLabel2.Enabled = false;
            txtValue1.Enabled = true;
            txtValue2.Enabled = false;
            txtUserText.Enabled = true;
            txtWithNameLabel.Enabled = true;
        }
        else if (rbSplit.Checked)
        {
            txtLabel1.Text = "&Number:";
            txtLabel2.Text = "Int&o:";
            txtLeaveBlank.Visible = false;
            txtLabel2.Enabled = true;
            txtValue1.Enabled = true;
            txtValue2.Enabled = true;
            txtUserText.Enabled = false;
            txtWithNameLabel.Enabled = false;
        }
    }

    private void bnOK_Click(object sender, EventArgs e)
    {
        mRule.DoWhatNow = Action();
        mRule.UserSuppliedText = txtUserText.Enabled ? txtUserText.Text : string.Empty;
        mRule.First = ParseTextValue(txtValue1);
        mRule.Second = ParseTextValue(txtValue2);
        mRule.RenumberAfter = chkRenumberAfter.Checked;

        try
        {
            //validation Rules

            ValidationCheck(mRule.DoWhatNow != RuleAction.kInsert
                , !ContainsEpisode(mRule.First)
                , "First episode number is not valid for the selected season"
                , txtValue1);

            ValidationCheck(mRule.DoWhatNow == RuleAction.kInsert
                , !(NextEpisodeIs(mRule.First) || ContainsEpisode(mRule.First))
                , "First episode number is not valid for the selected season"
                , txtValue1);

            //these 3 types only have one episode cited
            bool singleEpisodeRule = mRule.DoWhatNow.In(RuleAction.kRename, RuleAction.kInsert, RuleAction.kSplit);

            ValidationCheck(!singleEpisodeRule
                , !ContainsEpisode(mRule.Second)
                , "Second episode number is not valid for the selected season"
                , txtValue2);

            //these 3 types only have one episode cited - others must be in order
            ValidationCheck(!singleEpisodeRule
                , mRule.First > mRule.Second
                , "Second episode number must be after the first episode number"
                , txtValue2);

            //Swap, merge and collapse can't be done on the same episode numbers
            ValidationCheck(mRule.DoWhatNow.In(RuleAction.kSwap, RuleAction.kMerge, RuleAction.kCollapse)
                , txtValue2.Text.Equals(txtValue1.Text)
                , "Episode Numbers must be different"
                , txtValue2);
        }
        catch (ValidationFailedException valException)
        {
            Logger.Warn($"Validation Error: {valException.ErrorText()}: {mRule}");
            DialogResult = DialogResult.None;
        }
    }

    private bool ContainsEpisode(int episodeNumber)
    {
        return eps.Any(ep => ep.GetEpisodeNumber(mOrder) == episodeNumber);
    }

    private bool NextEpisodeIs(int episodeNumber)
    {
        int maxEpNum = eps.MaxOrDefault(ep => ep.GetEpisodeNumber(mOrder), 0);

        return episodeNumber == maxEpNum + 1;
    }

    private static void ValidationCheck(bool ruleCheck, bool check, string message, Control source)
    {
        if (!(ruleCheck && check))
        {
            return;
        }

        MessageBox.Show(message, "Modify Rules", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        source.Focus();
        throw new ValidationFailedException(message);
    }

    private static int ParseTextValue(Control txtValue)
    {
        try
        {
            return txtValue.Enabled ? Convert.ToInt32(txtValue.Text) : -1;
        }
        catch
        {
            return -1;
        }
    }

    private RuleAction Action()
    {
        if (rbIgnore.Checked)
        {
            return RuleAction.kIgnoreEp;
        }
        if (rbSwap.Checked)
        {
            return RuleAction.kSwap;
        }
        if (rbMerge.Checked)
        {
            return RuleAction.kMerge;
        }
        if (rbInsert.Checked)
        {
            return RuleAction.kInsert;
        }
        if (rbRemove.Checked)
        {
            return RuleAction.kRemove;
        }
        if (rbCollapse.Checked)
        {
            return RuleAction.kCollapse;
        }
        if (rbRename.Checked)
        {
            return RuleAction.kRename;
        }
        if (rbSplit.Checked)
        {
            return RuleAction.kSplit;
        }
        throw new ArgumentOutOfRangeException();
    }
}
