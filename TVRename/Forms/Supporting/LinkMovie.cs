// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at https://github.com/TV-Rename/tvrename
// 
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
// 

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Humanizer;
using JetBrains.Annotations;
using NLog;
using FileInfo = Alphaleonis.Win32.Filesystem.FileInfo;

namespace TVRename
{
    public partial class LinkMovie : Form
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private FileInfo chosenFile;
        private List<MovieConfiguration> shows;
        public MovieConfiguration ChosenShow;

        public LinkMovie([NotNull] List<MovieConfiguration> matchingShows, [NotNull] FileInfo file)
        {
            InitializeComponent();
            lblSourceFileName.Text = file.FullName;
            chosenFile = file;
            shows = matchingShows;
            DialogResult = DialogResult.Abort;
            foreach (var movie in matchingShows)
            {
                cbShows.Items.Add(movie);
            }
        }

        private void lnkOpenLeftFolder_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Helpers.OpenFolderSelectFile(chosenFile.FullName);
        }


        private void btnUseSelectedMovie_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            ChosenShow = (MovieConfiguration)cbShows.SelectedItem;
            Close();
        }

        private void Ignore_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Abort;
            Close();
        }

        private void btnNewMovie_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
