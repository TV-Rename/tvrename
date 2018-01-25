using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Humanizer;
using TVRename.Core.Metadata;
using TVRename.Core.Metadata.Identifiers;
using TVRename.Windows.Configuration;
using TVRename.Windows.Utilities;

namespace TVRename.Windows.Forms
{
    public partial class MediaCenter : Form
    {
        private readonly List<Identifier> identifiers;

        public MediaCenter()
        {
            InitializeComponent();

            Helpers.DoubleBuffer(this.listView);

            // Image list
            this.imageList.Images.Add(Properties.Resources.Document);
            this.imageList.Images.Add(Properties.Resources.Image);

            this.identifiers = new List<Identifier>(Settings.Instance.Identifiers);
        }

        private void MediaCenter_Load(object sender, EventArgs e)
        {
            this.Render();
        }

        private void Render()
        {
            this.listView.BeginUpdate();

            this.listView.Items.Clear();

            foreach (Identifier identifier in this.identifiers)
            {
                switch (identifier)
                {

                    case TextIdentifier textIdentifier:
                        this.listView.Items.Add(new ListViewItem(new[]
                        {
                            $"{textIdentifier.Target.Humanize()} Metadata",
                            textIdentifier.TextFormat,
                            identifier.Location,
                            identifier.FileName,
                        })
                        {
                            Tag = identifier,
                            ImageIndex = 0,
                            Group = this.listView.Groups[textIdentifier.Target.Humanize().ToLowerInvariant()]
                        });

                        break;

                    case ImageIdentifier imageIdentifier:
                        this.listView.Items.Add(new ListViewItem(new[]
                        {
                            imageIdentifier.ImageType.Humanize(),
                            imageIdentifier.ImageFormat.ToString().ToUpperInvariant(),
                            identifier.Location,
                            identifier.FileName,
                        })
                        {
                            Tag = identifier,
                            ImageIndex = 1,
                            Group = this.listView.Groups[imageIdentifier.Target.Humanize().ToLowerInvariant()]
                        });

                        break;
                }
            }

            this.listView.EndUpdate();
        }

        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.buttonEdit.Enabled = this.listView.SelectedItems.Count == 1;
            this.buttonRemove.Enabled = this.listView.SelectedItems.Count > 0;
        }

        private void listView_DoubleClick(object sender, EventArgs e)
        {
            this.buttonEdit_Click(sender, e);
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            this.toolStripMenuItemEdit.Enabled = this.listView.SelectedItems.Count == 1;
        }

        private void toolStripMenuItemEdit_Click(object sender, EventArgs e)
        {
            this.buttonEdit_Click(sender, e);
        }

        private void toolStripMenuItemRemove_Click(object sender, EventArgs e)
        {
            this.buttonRemove_Click(sender, e);
        }

        private void buttonPresets_Click(object sender, EventArgs e)
        {
            this.contextMenuStripPresets.Show(this.buttonPresets, new Point(0, this.buttonPresets.Height), ToolStripDropDownDirection.Default);
        }

        private void toolStripMenuItemPresetKodi_Click(object sender, EventArgs e)
        {
            this.identifiers.Add(new KodiIdentifier
            {
                Target = Target.Show,
                Location = "{{show.location}}",
                FileName = "tvshow.nfo"
            });
            this.identifiers.Add(new TVDBImageIdentifier
            {
                ImageType = ImageType.ShowPoster,
                ImageFormat = ImageFormat.Jpeg,
                Location = "{{show.location}}",
                FileName = "poster.jpg"
            });
            this.identifiers.Add(new TVDBImageIdentifier
            {
                ImageType = ImageType.ShowBanner,
                ImageFormat = ImageFormat.Jpeg,
                Location = "{{show.location}}",
                FileName = "banner.jpg"
            });
            this.identifiers.Add(new TVDBImageIdentifier
            {
                ImageType = ImageType.ShowFanart,
                ImageFormat = ImageFormat.Jpeg,
                Location = "{{show.location}}",
                FileName = "fanart.jpg"
            });

            this.identifiers.Add(new TVDBImageIdentifier
            {
                ImageType = ImageType.SeasonPoster,
                ImageFormat = ImageFormat.Jpeg,
                Location = "{{show.location}}",
                FileName = "season{{season.number | pad}}-poster.jpg"
            });
            this.identifiers.Add(new TVDBImageIdentifier
            {
                ImageType = ImageType.SeasonBanner,
                ImageFormat = ImageFormat.Jpeg,
                Location = "{{show.location}}",
                FileName = "season{{season.number | pad}}-banner.jpg"
            });

            this.identifiers.Add(new KodiIdentifier
            {
                Target = Target.Episode,
                Location = "{{episode.location}}",
                FileName = "{{episode.filename}}.nfo"
            });
            this.identifiers.Add(new TVDBImageIdentifier
            {
                ImageType = ImageType.EpisodeThumbnail,
                ImageFormat = ImageFormat.Jpeg,
                Location = "{{episode.location}}",
                FileName = "{{episode.filename}}-thumb.jpg"
            });

            this.Render();
        }

        private void toolStripMenuItemPresetMede8er_Click(object sender, EventArgs e)
        {
            this.identifiers.Add(new Mede8erIdentifier
            {
                Target = Target.Show,
                Location = "{{show.location}}",
                FileName = "series.xml"
            });
            this.identifiers.Add(new Mede8erViewIdentifier
            {
                Target = Target.Show,
                Location = "{{show.location}}",
                FileName = "view.xml"
            });

            this.identifiers.Add(new Mede8erViewIdentifier
            {
                Target = Target.Season,
                Location = "{{season.location}}",
                FileName = "view.xml"
            });

            this.identifiers.Add(new Mede8erIdentifier
            {
                Target = Target.Episode,
                Location = "{{episode.location}}",
                FileName = "{{episode.filename}}.xml"
            });

            this.Render();
        }

        private void toolStripMenuItemPresetPyTivo_Click(object sender, EventArgs e)
        {
            this.identifiers.Add(new PyTivoIdentifier
            {
                Target = Target.Episode,
                Location = "{{episode.location}}",
                FileName = "{{episode.filename}}.txt" // TODO: Must inc video file ext
            });

            this.Render();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            this.contextMenuStripAdd.Show(this.buttonAdd, new Point(0, this.buttonAdd.Height), ToolStripDropDownDirection.Default);
        }

        private void toolStripMenuItemAddText_Click(object sender, EventArgs e)
        {
            using (MediaCenterText form = new MediaCenterText())
            {
                if (form.ShowDialog() != DialogResult.OK) return;

                this.identifiers.Add(form.Identifier);
            }

            this.Render();
        }

        private void toolStripMenuItemAddImage_Click(object sender, EventArgs e)
        {
            using (MediaCenterImage form = new MediaCenterImage())
            {
                if (form.ShowDialog() != DialogResult.OK) return;

                this.identifiers.Add(form.Identifier);
            }

            this.Render();
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            if (this.listView.SelectedItems.Count < 1) return;

            switch (this.listView.SelectedItems[0].Tag)
            {
                case TextIdentifier textIdentifier:
                    using (MediaCenterText form = new MediaCenterText(textIdentifier))
                    {
                        if (form.ShowDialog() != DialogResult.OK) return;

                        this.identifiers[this.identifiers.IndexOf(textIdentifier)] = form.Identifier;

                        this.Render();
                    }
                    
                    break;

                case ImageIdentifier imageIdentifier:
                    using (MediaCenterImage form = new MediaCenterImage(imageIdentifier))
                    {
                        if (form.ShowDialog() != DialogResult.OK) return;
                        
                        this.identifiers[this.identifiers.IndexOf(imageIdentifier)] = form.Identifier;

                        this.Render();
                    }

                    break;
            }
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in this.listView.SelectedItems)
            {
                this.identifiers.Remove((Identifier)lvi.Tag);

                this.listView.Items.Remove(lvi);
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            Settings.Instance.Identifiers = this.identifiers;
            Settings.Instance.Dirty = true;

            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void listView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (!this.listView.FocusedItem.Bounds.Contains(e.Location)) return;

            this.contextMenuStrip.Show(Cursor.Position);
        }
    }
}
