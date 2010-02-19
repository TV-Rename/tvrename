//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//
//
// Main website for TVRename is http://tvrename.com
//
// Source code available at http://code.google.com/p/tvrename/
//
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//


using System;
using System.ComponentModel;
using System.Collections;
using System.Windows.Forms;
using System.Data;
using System.Drawing;
using System.Collections.Generic;
using SourceGrid;

namespace TVRename
{

	/// <summary>
	/// Summary for ActorsGrid
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public class ActorsGrid : System.Windows.Forms.Form
	{
		private class DataArr
		{
		public class ArrData
				{
					public bool yes;
					public bool act; // false=>guest

					public ArrData(bool @set, bool isact)
					{
						yes = @set;
						act = isact;
					}
					public int Score()
					{
						return yes ? 1 : 0;
					}
				}

				public int AllocR;
				public int AllocC;
				public int DataR;
				public int DataC;
				public System.Collections.Generic.List<string > Rows;
				public System.Collections.Generic.List<string > Cols;
				public ArrData[,] Data;

				public DataArr(int rowCountPreAlloc)
				{
					Rows = new System.Collections.Generic.List<string >();
					Cols = new System.Collections.Generic.List<string >();
					AllocR = rowCountPreAlloc;
					AllocC = rowCountPreAlloc *10;
					Data = new ArrData[AllocR, AllocC];
					DataR = DataC = 0;
				}
				public void SwapCols(int c1, int c2)
				{
					for (int r =0;r<DataR;r++)
					{
						ArrData t = Data[r,c2];
						Data[r,c2] = Data[r,c1];
						Data[r,c1] = t;
					}
					string t2 = Cols[c1];
					Cols[c1] = Cols[c2];
					Cols[c2] = t2;
				}
				public void SwapRows(int r1, int r2)
				{
					for (int c =0;c<DataC;c++)
					{
						ArrData t = Data[r2,c];
						Data[r2,c] = Data[r1,c];
						Data[r1,c] = t;
					}
					string t2 = Rows[r1];
					Rows[r1] = Rows[r2];
					Rows[r2] = t2;
				}
				public int RowScore(int r, bool[] onlyCols)
				{
					int t = 0;
					for (int c =0;c<DataC;c++)
						if ((Data[r,c] != null) && ((onlyCols == null) || onlyCols[c]))
							t += Data[r,c].Score();
					return t;
				}
				public int ColScore(int c)
				{
					int t = 0;
					for (int r =0;r<DataR;r++)
						if (Data[r,c] != null)
							t += Data[r,c].Score();
					return t;
				}
				public void Resize()
				{
					int newr = Rows.Count;
					int newc = Cols.Count;
					if ((newr > AllocR) || (newc > AllocC)) // need to enlarge array
					{
						if (newr > AllocR)
							AllocR = newr * 2;
						if (newc > AllocC)
							AllocC = newc * 2;
						ArrData[,] newarr = new ArrData[AllocR,AllocC];
						for (int r =0;r<DataR;r++)
							for (int c =0;c<DataC;c++)
								if ((r < newr) && (c < newc))
									newarr[r,c] = Data[r,c];
						Data = newarr;
					}
					DataR = newr;
					DataC = newc;
				}
				public int AddRow(string name)
				{
					Rows.Add(name);
					Resize();
					return Rows.Count - 1;
				}
				public int AddCol(string name)
				{
					Cols.Add(name);
					Resize();
					return Cols.Count - 1;
				}
				public void RemoveEmpties()
				{
					bool[] keepR = new bool[DataR];
					bool[] keepC = new bool[DataC];
					// trim by actor 
					int countR = 0;
					int countC = 0;

					for (int c =0;c<DataC;c++)
						if (ColScore(c) >= 2)
						{
							keepC[c] = true;
							countC++;
						}
						for (int r =0;r<DataR;r++)
							if (RowScore(r, keepC) >= 1)
							{
								keepR[r] = true;
								countR++;
							}

							ArrData[,] newarr = new ArrData[countR, countC];
							for (int r =0, newR =0;r<DataR;r++)
							{
								if (keepR[r])
								{
									for (int c =0, newC =0;c<DataC;c++)
										if (keepR[r] && keepC[c])
											newarr[newR,newC++] = Data[r,c];
									newR++;
								}
							}

							for (int r =0, newR =0;r<DataR;r++)
								if (keepR[r])
									Rows[newR++] = Rows[r];
							for (int c =0, newC =0;c<DataC;c++)
								if (keepC[c])
									Cols[newC++] = Cols[c];

							Rows.RemoveRange(countR, Rows.Count - countR);
							Cols.RemoveRange(countC, Cols.Count - countC);

							Data = newarr;
							AllocR = DataR = countR;
							AllocC = DataC = countC;
				}
				public void Set(string row, string col, ArrData d)
				{
					int r = Rows.IndexOf(row);
					int c = Cols.IndexOf(col);
					if (r == -1)
						r = AddRow(row);
					if (c == -1)
						c = AddCol(col);
					Data[r,c] = d;
				}
				public void SortCols(bool score) // false->name
				{
					for (int c2 =0;c2<(DataC-1);c2++)
					{
						int topscore = 0;
						string topword = "";
						int maxat = -1;
						for (int c =c2;c<DataC;c++)
						{
							if (score)
							{
								int sc = ColScore(c);
								if ((maxat == -1) || (sc > topscore))
								{
									maxat = c;
									topscore = sc;
								}
							}
							else
							{
								if ((maxat == -1) || (Cols[c].CompareTo(topword) < 0))
								{
									maxat = c;
									topword = Cols[c];
								}
							}
						}
						if (maxat != c2)
							SwapCols(c2, maxat);
					}
				}
				public void MoveColToTop(string col)
				{
					int n = Cols.IndexOf(col);
					if (n == -1)
						return;

					for (int r =0;r<DataR;r++)
					{
						DataArr.ArrData t = Data[r, n];
						for (int c =n;c>0;c--)
							Data[r, c] = Data[r, c-1];
						Data[r, 0] = t;
					}

					string t2 = Cols[n];
					for (int c =n;c>0;c--)
						Cols[c] = Cols[c-1];
					Cols[0] = t2;
				}
				public void MoveRowToTop(string row)
				{
					int n = Rows.IndexOf(row);
					if (n == -1)
						return;

					for (int c =0;c<DataC;c++)
					{
						DataArr.ArrData t = Data[n, c];
						for (int r =n;r>0;r--)
							Data[r,c] = Data[r-1,c];
						Data[0, c] = t;
					}

					string t2 = Rows[n];
					for (int r =n;r>0;r--)
						Rows[r] = Rows[r-1];
					Rows[0] = t2;
				}
				public void SortRows(bool score) // false->name
				{
					for (int r2 =0;r2<(DataR-1);r2++)
					{
						int topscore = 0;
						int maxat = -1;
						string topword = "";
						for (int r =r2;r<DataR;r++)
						{
							if (score)
							{
								int sc = RowScore(r, null);
								if ((maxat == -1) || (sc > topscore))
								{
									maxat = r;
									topscore = sc;
								}
							}
							else
							{
								if ((maxat == -1) || (Rows[r].CompareTo(topword) < 0))
								{
									maxat = r;
									topword = Rows[r];
								}
							}
						}
						if (maxat != r2)
							SwapRows(r2, maxat);
					}
				}
		}

		private class RotatedText : DevAge.Drawing.VisualElements.TextGDI
		{
			public RotatedText(float angle)
			{
				Angle = angle;
			}

			public float Angle;

			protected override void OnDraw(DevAge.Drawing.GraphicsCache graphics, RectangleF area)
			{
				System.Drawing.Drawing2D.GraphicsState state = graphics.Graphics.Save();
				try
				{
					float width2 = area.Width / 2;
					float height2 = area.Height / 2;

					//For a better drawing use the clear type rendering
					graphics.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

					//Move the origin to the center of the cell (for a more easy rotation)
					graphics.Graphics.TranslateTransform(area.X + width2, area.Y + height2);

					graphics.Graphics.RotateTransform(Angle);
					graphics.Graphics.TranslateTransform(-height2,0); //-(area.Y + height2));

					StringFormat.Alignment = StringAlignment.Near;
					StringFormat.LineAlignment = StringAlignment.Center;
					graphics.Graphics.DrawString(Value, Font, graphics.BrushsCache.GetBrush(ForeColor), 0, 0, StringFormat);
				}
				finally
				{
					graphics.Graphics.Restore(state);
				}
			}

			//TODO Implement Clone and MeasureContent
			//Here I should also implement MeasureContent (think also for a solution to allow rotated text with any kind of alignment)
		} // RotatedText


		private TVDoc mDoc;
		private int Internal;
	private System.Windows.Forms.RadioButton rbName;
	private System.Windows.Forms.RadioButton rbTotals;
	private System.Windows.Forms.RadioButton rbCustom;



	private System.Windows.Forms.Label label1;
			 private DataArr TheData;

		public ActorsGrid(TVDoc doc)
		{
			Internal = 0;

			InitializeComponent();

			mDoc = doc;

			BuildData();
			DoSort();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
	private SourceGrid.Grid grid1;
	private System.Windows.Forms.Button bnClose;
	private System.Windows.Forms.CheckBox cbGuestStars;
	private System.Windows.Forms.Button bnSave;
	private System.Windows.Forms.SaveFileDialog saveFile;
	private System.ComponentModel.IContainer components;


		/// <summary>
		/// Required designer variable.
		/// </summary>


#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = (new System.ComponentModel.ComponentResourceManager(typeof(ActorsGrid)));
			this.grid1 = (new SourceGrid.Grid());
			this.bnClose = (new System.Windows.Forms.Button());
			this.cbGuestStars = (new System.Windows.Forms.CheckBox());
			this.bnSave = (new System.Windows.Forms.Button());
			this.saveFile = (new System.Windows.Forms.SaveFileDialog());
			this.rbName = (new System.Windows.Forms.RadioButton());
			this.rbTotals = (new System.Windows.Forms.RadioButton());
			this.rbCustom = (new System.Windows.Forms.RadioButton());
			this.label1 = (new System.Windows.Forms.Label());
			this.SuspendLayout();
			// 
			// grid1
			// 
			this.grid1.Anchor = (System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
			this.grid1.BackColor = System.Drawing.SystemColors.Window;
			this.grid1.Location = new System.Drawing.Point(12, 12);
			this.grid1.Name = "grid1";
			this.grid1.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
			this.grid1.SelectionMode = SourceGrid.GridSelectionMode.Cell;
			this.grid1.Size = new System.Drawing.Size(907, 522);
			this.grid1.TabIndex = 0;
			this.grid1.TabStop = true;
			this.grid1.ToolTipText = "";
			// 
			// bnClose
			// 
			this.bnClose.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
			this.bnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.bnClose.Location = new System.Drawing.Point(844, 548);
			this.bnClose.Name = "bnClose";
			this.bnClose.Size = new System.Drawing.Size(75, 23);
			this.bnClose.TabIndex = 1;
			this.bnClose.Text = "Close";
			this.bnClose.UseVisualStyleBackColor = true;
			this.bnClose.Click += new System.EventHandler(bnClose_Click);
			// 
			// cbGuestStars
			// 
			this.cbGuestStars.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
			this.cbGuestStars.AutoSize = true;
			this.cbGuestStars.Location = new System.Drawing.Point(12, 552);
			this.cbGuestStars.Name = "cbGuestStars";
			this.cbGuestStars.Size = new System.Drawing.Size(119, 17);
			this.cbGuestStars.TabIndex = 4;
			this.cbGuestStars.Text = "Include &Guest Stars";
			this.cbGuestStars.UseVisualStyleBackColor = true;
			this.cbGuestStars.CheckedChanged += new System.EventHandler(cbGuestStars_CheckedChanged);
			// 
			// bnSave
			// 
			this.bnSave.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right));
			this.bnSave.Location = new System.Drawing.Point(763, 548);
			this.bnSave.Name = "bnSave";
			this.bnSave.Size = new System.Drawing.Size(75, 23);
			this.bnSave.TabIndex = 5;
			this.bnSave.Text = "&Save";
			this.bnSave.UseVisualStyleBackColor = true;
			this.bnSave.Click += new System.EventHandler(bnSave_Click);
			// 
			// rbName
			// 
			this.rbName.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
			this.rbName.AutoSize = true;
			this.rbName.Checked = true;
			this.rbName.Location = new System.Drawing.Point(199, 552);
			this.rbName.Name = "rbName";
			this.rbName.Size = new System.Drawing.Size(53, 17);
			this.rbName.TabIndex = 6;
			this.rbName.TabStop = true;
			this.rbName.Text = "Name";
			this.rbName.UseVisualStyleBackColor = true;
			this.rbName.CheckedChanged += new System.EventHandler(rbName_CheckedChanged);
			// 
			// rbTotals
			// 
			this.rbTotals.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
			this.rbTotals.AutoSize = true;
			this.rbTotals.Location = new System.Drawing.Point(258, 552);
			this.rbTotals.Name = "rbTotals";
			this.rbTotals.Size = new System.Drawing.Size(54, 17);
			this.rbTotals.TabIndex = 6;
			this.rbTotals.Text = "Totals";
			this.rbTotals.UseVisualStyleBackColor = true;
			this.rbTotals.CheckedChanged += new System.EventHandler(rbTotals_CheckedChanged);
			// 
			// rbCustom
			// 
			this.rbCustom.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
			this.rbCustom.AutoSize = true;
			this.rbCustom.Location = new System.Drawing.Point(318, 552);
			this.rbCustom.Name = "rbCustom";
			this.rbCustom.Size = new System.Drawing.Size(60, 17);
			this.rbCustom.TabIndex = 6;
			this.rbCustom.Text = "Custom";
			this.rbCustom.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(164, 554);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(29, 13);
			this.label1.TabIndex = 7;
			this.label1.Text = "Sort:";
			// 
			// ActorsGrid
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6, 13);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.bnClose;
			this.ClientSize = new System.Drawing.Size(931, 582);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.rbCustom);
			this.Controls.Add(this.rbTotals);
			this.Controls.Add(this.rbName);
			this.Controls.Add(this.bnSave);
			this.Controls.Add(this.cbGuestStars);
			this.Controls.Add(this.bnClose);
			this.Controls.Add(this.grid1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "ActorsGrid";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Text = "Actors Grid";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

#endregion
		private class CellClickEvent : SourceGrid.Cells.Controllers.ControllerBase
		{
			private string Who;
			private string Show;

			public CellClickEvent(string who, string show)
			{
				Who = who;
				Show = show;
			}

			public override void OnClick(SourceGrid.CellContext sender, EventArgs e)
			{
				TVDoc.SysOpen("http://www.imdb.com/find?s=nm&q="+Who);
			}
		}

		private class TopClickEvent : SourceGrid.Cells.Controllers.ControllerBase
		{
			private string Actor;
			private ActorsGrid G;

			public TopClickEvent(ActorsGrid g, string act)
			{
				G = g;
				Actor = act;
			}

			public override void OnClick(SourceGrid.CellContext sender, EventArgs e)
			{
				G.ActorToTop(Actor);
			}
		}

		private class SortRowsByCountEvent : SourceGrid.Cells.Controllers.ControllerBase
		{
			private ActorsGrid G;
			public SortRowsByCountEvent(ActorsGrid g)
			{
				G = g;
			}

			public override void OnClick(SourceGrid.CellContext sender, EventArgs e)
			{
				G.SortRowsByCount();
			}
		}

		private class SortColsByCountEvent : SourceGrid.Cells.Controllers.ControllerBase
		{
			private ActorsGrid G;
			public SortColsByCountEvent(ActorsGrid g)
			{
				G = g;
			}

			public override void OnClick(SourceGrid.CellContext sender, EventArgs e)
			{
				G.SortColsByCount();
			}
		}

		private class SideClickEvent : SourceGrid.Cells.Controllers.ControllerBase
		{
			private string Show;
			private ActorsGrid G;

			public SideClickEvent(ActorsGrid g, string show)
			{
				Show = show;
				G = g;
			}

			public override void OnClick(SourceGrid.CellContext sender, EventArgs e)
			{
				if (Show == null)
					G.DoSort();
				else
					G.ShowToTop(Show);
			}
		}

		private void BuildData()
		{
			// find actors that have been in more than one thing
			// Dictionary<String^, StringList ^> ^whoInWhat = gcnew Dictionary<String^, StringList ^>;
			TheTVDB db = mDoc.GetTVDB(true,"Actors");
			TheData = new DataArr(db.GetSeriesDict().Count);
			foreach (System.Collections.Generic.KeyValuePair<int, SeriesInfo > ser in db.GetSeriesDict())
			{
				SeriesInfo si = ser.Value;
				string actors = si.GetItem("Actors");
				if (!string.IsNullOrEmpty(actors))
					foreach (string act in actors.Split('|'))
					{
                        string aa = act.Trim();
						if (!string.IsNullOrEmpty(aa))
							TheData.Set(si.Name, aa, new DataArr.ArrData(true, true));
					}

				if (cbGuestStars.Checked)
				{
					foreach (System.Collections.Generic.KeyValuePair<int, Season > kvp in si.Seasons)
					{
						foreach (Episode ep in kvp.Value.Episodes)
						{
							string guest = ep.GetItem("GuestStars");

							if (!string.IsNullOrEmpty(guest))
								foreach (string g in guest.Split('|'))
								{
                                    string aa = g.Trim();
									if (!string.IsNullOrEmpty(aa))
										TheData.Set(si.Name, aa, new DataArr.ArrData(true, false));
								}
						}
					}
				}
			}

			db.Unlock("Actors");
			TheData.RemoveEmpties();
		}
		private void SortByName()
		{
			Internal++;
			rbName.Checked = true;
			Internal--;
			TheData.SortRows(false);
			TheData.SortCols(false);
		}
		private void SortByTotals()
		{
			Internal++;
			rbTotals.Checked = true;
			Internal--;
			TheData.SortRows(true);
			TheData.SortCols(true);
		}
		private void SortRowsByCount()
		{
			Internal++;
			rbCustom.Checked = true;
			Internal--;
			TheData.SortRows(true);
			FillGrid();
		}
		private void SortColsByCount()
		{
			Internal++;
			rbCustom.Checked = true;
			Internal--;
			TheData.SortCols(true);
			FillGrid();
		}
		private void ActorToTop(string a)
		{
			Internal++;
			rbCustom.Checked = true;
			Internal--;

			TheData.MoveColToTop(a);

			// also move the shows they've been in to the top, too
			int c = TheData.Cols.IndexOf(a);
			if (c != 0)
				return; // uh oh!
			int end = 0;
			for (int r =TheData.DataR-1;r>=end;r--)
			{
				DataArr.ArrData d = TheData.Data[r,0];
				if ((d!=null) && d.yes)
				{
					TheData.MoveRowToTop(TheData.Rows[r++]);
					end++;
				}
			}
			FillGrid();
		}
		private void ShowToTop(string s)
		{
			Internal++;
			rbCustom.Checked = true;
			Internal--;

			TheData.MoveRowToTop(s);

			// also move the actors in this show to the top, too
			int r = TheData.Rows.IndexOf(s);
			if (r != 0)
				return; // uh oh!
			int end = 0;
			for (int c =TheData.DataC-1;c>=end;c--)
			{
				DataArr.ArrData d = TheData.Data[0,c];
				if ((d!=null) && d.yes)
				{
					TheData.MoveColToTop(TheData.Cols[c++]);
					end++;
				}
			}

			FillGrid();
		}
		private void FillGrid()
		{
			SourceGrid.Cells.Views.Cell rowTitleModel = new SourceGrid.Cells.Views.Cell();
			rowTitleModel.BackColor = Color.SteelBlue;
			rowTitleModel.ForeColor = Color.White;
			rowTitleModel.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleLeft;

			SourceGrid.Cells.Views.Cell colTitleModel = new SourceGrid.Cells.Views.Cell();
			colTitleModel.ElementText = new RotatedText(-90.0f);
			colTitleModel.BackColor = Color.SteelBlue;
			colTitleModel.ForeColor = Color.White;
			colTitleModel.TextAlignment = DevAge.Drawing.ContentAlignment.BottomCenter;

			SourceGrid.Cells.Views.Cell topleftTitleModel = new SourceGrid.Cells.Views.Cell();
			topleftTitleModel.BackColor = Color.SteelBlue;
			topleftTitleModel.ForeColor = Color.White;
			topleftTitleModel.TextAlignment = DevAge.Drawing.ContentAlignment.BottomLeft;

			SourceGrid.Cells.Views.Cell yesModel = new SourceGrid.Cells.Views.Cell();
			yesModel.BackColor = Color.Green;
			yesModel.ForeColor = Color.Green;
			yesModel.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleLeft;

			SourceGrid.Cells.Views.Cell guestModel = new SourceGrid.Cells.Views.Cell();
			guestModel.BackColor = Color.LightGreen;
			guestModel.ForeColor = Color.LightGreen;
			guestModel.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleLeft;

			grid1.Columns.Clear();
			grid1.Rows.Clear();

			int rows = TheData.DataR + 2; // title and total
			int cols = TheData.DataC + 2;

			grid1.ColumnsCount = cols;
			grid1.RowsCount = rows;
			grid1.FixedColumns = 1;
			grid1.FixedRows = 1;
			grid1.Selection.EnableMultiSelection = false;

			for (int i =0;i<cols;i++)
			{
				grid1.Columns[i].AutoSizeMode = i ==0 ? SourceGrid.AutoSizeMode.Default : SourceGrid.AutoSizeMode.MinimumSize;
				if (i > 0)
					grid1.Columns[i].Width = 24;
			}

			grid1.Rows[0].AutoSizeMode = SourceGrid.AutoSizeMode.MinimumSize;
			grid1.Rows[0].Height = 100;

			SourceGrid.Cells.ColumnHeader h;
			h = new SourceGrid.Cells.ColumnHeader("Show");
			h.AutomaticSortEnabled = false;
			h.ResizeEnabled = false;

			grid1[0,0] = h;
			grid1[0,0].View = topleftTitleModel;
			grid1[0,0].AddController(new SideClickEvent(this, null)); // default sort

			SourceGrid.Cells.Views.Cell rotateView = new SourceGrid.Cells.Views.Cell();
			for (int c =0;c<TheData.DataC;c++)
			{
				h = new SourceGrid.Cells.ColumnHeader(TheData.Cols[c]); // "<A HREF=\"http://www.imdb.com/find?s=nm&q="+kvp->Key+"\">"+kvp->Key+"</a>");

				// h->AddController(sortableController);
				// h->SortComparer = gcnew SourceGrid::MultiColumnsComparer(c, 0); // TODO: remove?
				h.AutomaticSortEnabled = false;
				h.ResizeEnabled = false;
				grid1[0,c+1] = h;
				grid1[0,c+1].View = colTitleModel;
				grid1[0,c+1].AddController(new TopClickEvent(this, TheData.Cols[c]));
			}

			int totalCol = grid1.ColumnsCount - 1;
			h = new SourceGrid.Cells.ColumnHeader("Totals");
			h.AutomaticSortEnabled = false;
			//h->AddController(sortableController);
			// h->SortComparer = gcnew SourceGrid::MultiColumnsComparer(c, 0);
			h.ResizeEnabled = false;
			grid1.Columns[totalCol].Width = 48;
			grid1[0,totalCol] = h;
			grid1[0,totalCol].View = colTitleModel;
			grid1[0,totalCol].AddController(new SortRowsByCountEvent(this));

            SourceGrid.Cells.RowHeader rh = null;
			for (int r =0;r<TheData.DataR;r++)
			{
				rh = new SourceGrid.Cells.RowHeader(TheData.Rows[r]);
				rh.ResizeEnabled = false;

				grid1[r+1,0] = rh;
				grid1[r+1,0].AddController(new SideClickEvent(this, TheData.Rows[r]));
			}

			rh = new SourceGrid.Cells.RowHeader("Totals");
			rh.ResizeEnabled = false;
			grid1[TheData.DataR+1,0] = rh;
			grid1[TheData.DataR+1,0].AddController(new SortColsByCountEvent(this));

			for (int c =0;c<TheData.DataC;c++)
			{
				for (int r =0;r<TheData.DataR;r++)
				{
					DataArr.ArrData d = TheData.Data[r,c];
					if ((d != null) && (d.yes))
					{
						grid1[r+1,c+1] = new SourceGrid.Cells.Cell("Y");
						grid1[r+1,c+1].View = d.act ? yesModel : guestModel;
						grid1[r+1,c+1].AddController(new CellClickEvent(TheData.Cols[c], TheData.Rows[r]));
					}
					else
						grid1[r+1,c+1] = new SourceGrid.Cells.Cell("");
				}
			}

			for (int c =0;c<TheData.DataC;c++)
				grid1[rows-1,c+1] = new SourceGrid.Cells.Cell(TheData.ColScore(c));

			for (int r =0;r<TheData.DataR;r++)
				grid1[r+1,cols-1] = new SourceGrid.Cells.Cell(TheData.RowScore(r, null));

			grid1[TheData.DataR+1,TheData.DataC+1] = new SourceGrid.Cells.Cell("");

			grid1.AutoSizeCells();
		}
	private void bnClose_Click(object sender, System.EventArgs e)
			 {
				 this.Close();
			 }
	private void cbGuestStars_CheckedChanged(object sender, System.EventArgs e)
			 {
				 cbGuestStars.Update();
				 BuildData();
				 DoSort();
			 }
	private void bnSave_Click(object sender, System.EventArgs e)
			 {
				 saveFile.Filter = "PNG Files (*.png)|*.png|All Files (*.*)|*.*";
				 if (saveFile.ShowDialog() !=DialogResult.OK)
					 return;

				 SourceGrid.Exporter.Image image = new SourceGrid.Exporter.Image();
				 Bitmap b = image.Export(grid1, grid1.CompleteRange);
				 b.Save(saveFile.FileName, System.Drawing.Imaging.ImageFormat.Png);
			 }
			 private void DoSort()
			 {
				 if (rbTotals.Checked)
					 SortByTotals();
				 else
					 SortByName(); // will check name for us, too
				 FillGrid();

			 }
	private void rbName_CheckedChanged(object sender, System.EventArgs e)
			 {
				 if (Internal != 0)
					 return;
				 DoSort();
			 }
	private void rbTotals_CheckedChanged(object sender, System.EventArgs e)
			 {
				 if (Internal != 0)
					 return;
				 DoSort();
			 }
	}


}


