#pragma once

#include "TVDoc.h"

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;
using namespace System::Collections::Generic;
using namespace SourceGrid;
using namespace Drawing::Imaging;

namespace TVRename {

	/// <summary>
	/// Summary for ActorsGrid
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public ref class ActorsGrid : public System::Windows::Forms::Form
	{
	private:
		ref class DataArr
		{
		public: ref class ArrData
				{
				public:
					bool yes;
					bool act; // false=>guest

					ArrData(bool set, bool isact)
					{
						yes = set;
						act = isact;
					}
					int Score()
					{
						return yes ? 1 : 0;
					}
				};

				int AllocR, AllocC;
				int DataR, DataC;
				StringList ^Rows;
				StringList ^Cols;
				cli::array<ArrData ^,2> ^Data;

				DataArr(int rowCountPreAlloc)
				{
					Rows = gcnew StringList();
					Cols = gcnew StringList();
					AllocR = rowCountPreAlloc;
					AllocC = rowCountPreAlloc*10;
					Data = gcnew cli::array<ArrData ^,2>(AllocR, AllocC);
					DataR = DataC = 0;
				}
				void SwapCols(int c1, int c2)
				{
					for (int r=0;r<DataR;r++)
					{
						ArrData ^t = Data[r,c2];
						Data[r,c2] = Data[r,c1];
						Data[r,c1] = t;
					}
					String ^t = Cols[c1];
					Cols[c1] = Cols[c2];
					Cols[c2] = t;
				}
				void SwapRows(int r1, int r2)
				{
					for (int c=0;c<DataC;c++)
					{
						ArrData ^t = Data[r2,c];
						Data[r2,c] = Data[r1,c];
						Data[r1,c] = t;
					}
					String ^t = Rows[r1];
					Rows[r1] = Rows[r2];
					Rows[r2] = t;
				}
				int RowScore(int r, array<bool> ^onlyCols)
				{
					int t = 0;
					for (int c=0;c<DataC;c++)
						if ((Data[r,c] != nullptr) && ((onlyCols == nullptr) || onlyCols[c]))
							t += Data[r,c]->Score();
					return t;
				}
				int ColScore(int c)
				{
					int t = 0;
					for (int r=0;r<DataR;r++)
						if (Data[r,c] != nullptr)
							t += Data[r,c]->Score();
					return t;
				}
				void Resize()
				{
					int newr = Rows->Count;
					int newc = Cols->Count;
					if ((newr > AllocR) || (newc > AllocC)) // need to enlarge array
					{
						if (newr > AllocR)
							AllocR = newr * 2;
						if (newc > AllocC)
							AllocC = newc * 2;
						cli::array<ArrData ^,2> ^newarr = gcnew cli::array<ArrData ^,2>(AllocR,AllocC);
						for (int r=0;r<DataR;r++)
							for (int c=0;c<DataC;c++)
								if ((r < newr) && (c < newc)) 
									newarr[r,c] = Data[r,c];
						Data = newarr;
					}
					DataR = newr;
					DataC = newc;
				}
				int AddRow(String ^name)
				{
					Rows->Add(name);
					Resize();
					return Rows->Count - 1;
				}
				int AddCol(String ^name)
				{
					Cols->Add(name);
					Resize();
					return Cols->Count - 1;
				}
				void RemoveEmpties()
				{
					array<bool> ^keepR = gcnew array<bool>(DataR);
					array<bool> ^keepC = gcnew array<bool>(DataC);
					// trim by actor 
					int countR = 0, countC = 0;

					for (int c=0;c<DataC;c++)
						if (ColScore(c) >= 2)
						{
							keepC[c] = true;
							countC++;
						}
						for (int r=0;r<DataR;r++)
							if (RowScore(r, keepC) >= 1)
							{
								keepR[r] = true;
								countR++;
							}

							cli::array<ArrData ^,2> ^newarr = gcnew cli::array<ArrData ^,2>(countR, countC);
							for (int r=0, newR=0;r<DataR;r++)
							{
								if (keepR[r])
								{
									for (int c=0, newC=0;c<DataC;c++)
										if (keepR[r] && keepC[c])
											newarr[newR,newC++] = Data[r,c];
									newR++;
								}
							}

							for (int r=0, newR=0;r<DataR;r++)
								if (keepR[r])
									Rows[newR++] = Rows[r];
							for (int c=0, newC=0;c<DataC;c++)
								if (keepC[c])
									Cols[newC++] = Cols[c];

							Rows->RemoveRange(countR, Rows->Count - countR);
							Cols->RemoveRange(countC, Cols->Count - countC);

							Data = newarr;
							AllocR = DataR = countR;
							AllocC = DataC = countC;
				}
				void Set(String ^row, String ^col, ArrData ^d)
				{
					int r = Rows->IndexOf(row);
					int c = Cols->IndexOf(col);
					if (r == -1)
						r = AddRow(row);
					if (c == -1)
						c = AddCol(col);
					Data[r,c] = d;
				}
				void SortCols(bool score) // false->name
				{
					for (int c2=0;c2<(DataC-1);c2++)
					{
						int topscore = 0;
						String ^topword = "";
						int maxat = -1;
						for (int c=c2;c<DataC;c++)
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
								if ((maxat == -1) || (Cols[c]->CompareTo(topword) < 0))
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
				void MoveColToTop(String ^col)
				{
					int n = Cols->IndexOf(col);
					if (n == -1)
						return;

					for (int r=0;r<DataR;r++)
					{
						DataArr::ArrData ^t = Data[r, n];
						for (int c=n;c>0;c--)
							Data[r, c] = Data[r, c-1];
						Data[r, 0] = t;
					}

					String ^t = Cols[n];
					for (int c=n;c>0;c--)
						Cols[c] = Cols[c-1];
					Cols[0] = t;
				}
				void MoveRowToTop(String ^row)
				{
					int n = Rows->IndexOf(row);
					if (n == -1)
						return;

					for (int c=0;c<DataC;c++)
					{
						DataArr::ArrData ^t = Data[n, c];
						for (int r=n;r>0;r--)
							Data[r,c] = Data[r-1,c];
						Data[0, c] = t;
					}

					String ^t = Rows[n];
					for (int r=n;r>0;r--)
						Rows[r] = Rows[r-1];
					Rows[0] = t;
				}
				void SortRows(bool score) // false->name
				{
					for (int r2=0;r2<(DataR-1);r2++)
					{
						int topscore = 0;
						int maxat = -1;
						String ^topword = "";
						for (int r=r2;r<DataR;r++)
						{
							if (score)
							{
								int sc = RowScore(r, nullptr);
								if ((maxat == -1) || (sc > topscore))
								{
									maxat = r;
									topscore = sc;
								}
							}
							else
							{
								if ((maxat == -1) || (Rows[r]->CompareTo(topword) < 0))
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
		};

	private:
		ref class RotatedText : DevAge::Drawing::VisualElements::TextGDI
		{
		public: 
			RotatedText(float angle)
			{
				Angle = angle;
			}

			float Angle;

			virtual void OnDraw(DevAge::Drawing::GraphicsCache ^graphics, RectangleF area) override
			{
				System::Drawing::Drawing2D::GraphicsState ^state = graphics->Graphics->Save();
				try
				{
					float width2 = area.Width / 2;
					float height2 = area.Height / 2;

					//For a better drawing use the clear type rendering
					graphics->Graphics->TextRenderingHint = System::Drawing::Text::TextRenderingHint::ClearTypeGridFit;

					//Move the origin to the center of the cell (for a more easy rotation)
					graphics->Graphics->TranslateTransform(area.X + width2, area.Y + height2);

					graphics->Graphics->RotateTransform(Angle);
					graphics->Graphics->TranslateTransform(-height2,0);//-(area.Y + height2));

					StringFormat->Alignment = StringAlignment::Near;
					StringFormat->LineAlignment = StringAlignment::Center;
					graphics->Graphics->DrawString(Value, Font, graphics->BrushsCache->GetBrush(ForeColor), 0, 0, StringFormat);
				}
				finally
				{
					graphics->Graphics->Restore(state);
				}
			}

			//TODO Implement Clone and MeasureContent
			//Here I should also implement MeasureContent (think also for a solution to allow rotated text with any kind of alignment)
		}; // RotatedText


	private:
		TVDoc ^mDoc;
		int Internal;
	private: System::Windows::Forms::RadioButton^  rbName;
	private: System::Windows::Forms::RadioButton^  rbTotals;
	private: System::Windows::Forms::RadioButton^  rbCustom;



	private: System::Windows::Forms::Label^  label1;
			 DataArr ^TheData;

	public:
		ActorsGrid(TVDoc ^doc)
		{
			Internal = 0;

			InitializeComponent();

			mDoc = doc;

			BuildData();
			DoSort();
		}

	protected:
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		~ActorsGrid()
		{
			if (components)
			{
				delete components;
			}
		}
	private: SourceGrid::Grid^  grid1;
	private: System::Windows::Forms::Button^  bnClose;
	private: System::Windows::Forms::CheckBox^  cbGuestStars;
	private: System::Windows::Forms::Button^  bnSave;
	private: System::Windows::Forms::SaveFileDialog^  saveFile;
	private: System::ComponentModel::IContainer^  components;

	protected: 

	private:
		/// <summary>
		/// Required designer variable.
		/// </summary>


#pragma region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		void InitializeComponent(void)
		{
			System::ComponentModel::ComponentResourceManager^  resources = (gcnew System::ComponentModel::ComponentResourceManager(ActorsGrid::typeid));
			this->grid1 = (gcnew SourceGrid::Grid());
			this->bnClose = (gcnew System::Windows::Forms::Button());
			this->cbGuestStars = (gcnew System::Windows::Forms::CheckBox());
			this->bnSave = (gcnew System::Windows::Forms::Button());
			this->saveFile = (gcnew System::Windows::Forms::SaveFileDialog());
			this->rbName = (gcnew System::Windows::Forms::RadioButton());
			this->rbTotals = (gcnew System::Windows::Forms::RadioButton());
			this->rbCustom = (gcnew System::Windows::Forms::RadioButton());
			this->label1 = (gcnew System::Windows::Forms::Label());
			this->SuspendLayout();
			// 
			// grid1
			// 
			this->grid1->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
				| System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->grid1->BackColor = System::Drawing::SystemColors::Window;
			this->grid1->Location = System::Drawing::Point(12, 12);
			this->grid1->Name = L"grid1";
			this->grid1->OptimizeMode = SourceGrid::CellOptimizeMode::ForRows;
			this->grid1->SelectionMode = SourceGrid::GridSelectionMode::Cell;
			this->grid1->Size = System::Drawing::Size(907, 522);
			this->grid1->TabIndex = 0;
			this->grid1->TabStop = true;
			this->grid1->ToolTipText = L"";
			// 
			// bnClose
			// 
			this->bnClose->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
			this->bnClose->DialogResult = System::Windows::Forms::DialogResult::Cancel;
			this->bnClose->Location = System::Drawing::Point(844, 548);
			this->bnClose->Name = L"bnClose";
			this->bnClose->Size = System::Drawing::Size(75, 23);
			this->bnClose->TabIndex = 1;
			this->bnClose->Text = L"Close";
			this->bnClose->UseVisualStyleBackColor = true;
			this->bnClose->Click += gcnew System::EventHandler(this, &ActorsGrid::bnClose_Click);
			// 
			// cbGuestStars
			// 
			this->cbGuestStars->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->cbGuestStars->AutoSize = true;
			this->cbGuestStars->Location = System::Drawing::Point(12, 552);
			this->cbGuestStars->Name = L"cbGuestStars";
			this->cbGuestStars->Size = System::Drawing::Size(119, 17);
			this->cbGuestStars->TabIndex = 4;
			this->cbGuestStars->Text = L"Include &Guest Stars";
			this->cbGuestStars->UseVisualStyleBackColor = true;
			this->cbGuestStars->CheckedChanged += gcnew System::EventHandler(this, &ActorsGrid::cbGuestStars_CheckedChanged);
			// 
			// bnSave
			// 
			this->bnSave->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
			this->bnSave->Location = System::Drawing::Point(763, 548);
			this->bnSave->Name = L"bnSave";
			this->bnSave->Size = System::Drawing::Size(75, 23);
			this->bnSave->TabIndex = 5;
			this->bnSave->Text = L"&Save";
			this->bnSave->UseVisualStyleBackColor = true;
			this->bnSave->Click += gcnew System::EventHandler(this, &ActorsGrid::bnSave_Click);
			// 
			// rbName
			// 
			this->rbName->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->rbName->AutoSize = true;
			this->rbName->Checked = true;
			this->rbName->Location = System::Drawing::Point(199, 552);
			this->rbName->Name = L"rbName";
			this->rbName->Size = System::Drawing::Size(53, 17);
			this->rbName->TabIndex = 6;
			this->rbName->TabStop = true;
			this->rbName->Text = L"Name";
			this->rbName->UseVisualStyleBackColor = true;
			this->rbName->CheckedChanged += gcnew System::EventHandler(this, &ActorsGrid::rbName_CheckedChanged);
			// 
			// rbTotals
			// 
			this->rbTotals->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->rbTotals->AutoSize = true;
			this->rbTotals->Location = System::Drawing::Point(258, 552);
			this->rbTotals->Name = L"rbTotals";
			this->rbTotals->Size = System::Drawing::Size(54, 17);
			this->rbTotals->TabIndex = 6;
			this->rbTotals->Text = L"Totals";
			this->rbTotals->UseVisualStyleBackColor = true;
			this->rbTotals->CheckedChanged += gcnew System::EventHandler(this, &ActorsGrid::rbTotals_CheckedChanged);
			// 
			// rbCustom
			// 
			this->rbCustom->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->rbCustom->AutoSize = true;
			this->rbCustom->Location = System::Drawing::Point(318, 552);
			this->rbCustom->Name = L"rbCustom";
			this->rbCustom->Size = System::Drawing::Size(60, 17);
			this->rbCustom->TabIndex = 6;
			this->rbCustom->Text = L"Custom";
			this->rbCustom->UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this->label1->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left));
			this->label1->AutoSize = true;
			this->label1->Location = System::Drawing::Point(164, 554);
			this->label1->Name = L"label1";
			this->label1->Size = System::Drawing::Size(29, 13);
			this->label1->TabIndex = 7;
			this->label1->Text = L"Sort:";
			// 
			// ActorsGrid
			// 
			this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->CancelButton = this->bnClose;
			this->ClientSize = System::Drawing::Size(931, 582);
			this->Controls->Add(this->label1);
			this->Controls->Add(this->rbCustom);
			this->Controls->Add(this->rbTotals);
			this->Controls->Add(this->rbName);
			this->Controls->Add(this->bnSave);
			this->Controls->Add(this->cbGuestStars);
			this->Controls->Add(this->bnClose);
			this->Controls->Add(this->grid1);
			this->Icon = (cli::safe_cast<System::Drawing::Icon^  >(resources->GetObject(L"$this.Icon")));
			this->Name = L"ActorsGrid";
			this->ShowInTaskbar = false;
			this->SizeGripStyle = System::Windows::Forms::SizeGripStyle::Show;
			this->Text = L"Actors Grid";
			this->ResumeLayout(false);
			this->PerformLayout();

		}
#pragma endregion

		ref class CellClickEvent : public SourceGrid::Cells::Controllers::ControllerBase
		{
		private:
			String ^Who, ^Show;

		public:
			CellClickEvent(String ^who, String ^show)
			{
				Who = who;
				Show = show;
			}

			virtual void OnClick(SourceGrid::CellContext sender, EventArgs ^e) override
			{
				TVDoc::SysOpen("http://www.imdb.com/find?s=nm&q="+Who);
			}
		};

		ref class TopClickEvent : public SourceGrid::Cells::Controllers::ControllerBase
		{
		private:
			String ^Actor;
			ActorsGrid ^G;

		public:
			TopClickEvent(ActorsGrid ^g, String ^act)
			{
				G = g;
				Actor = act;
			}

			virtual void OnClick(SourceGrid::CellContext sender, EventArgs ^e) override
			{
				G->ActorToTop(Actor);
			}
		};

		ref class SortRowsByCountEvent : public SourceGrid::Cells::Controllers::ControllerBase
		{
		private:
			ActorsGrid ^G;
		public:
			SortRowsByCountEvent(ActorsGrid ^g)
			{
				G = g;
			}

			virtual void OnClick(SourceGrid::CellContext sender, EventArgs ^e) override
			{
				G->SortRowsByCount();
			}
		};

		ref class SortColsByCountEvent : public SourceGrid::Cells::Controllers::ControllerBase
		{
		private:
			ActorsGrid ^G;
		public:
			SortColsByCountEvent(ActorsGrid ^g)
			{
				G = g;
			}

			virtual void OnClick(SourceGrid::CellContext sender, EventArgs ^e) override
			{
				G->SortColsByCount();
			}
		};

		ref class SideClickEvent : public SourceGrid::Cells::Controllers::ControllerBase
		{
		private:
			String ^Show;
			ActorsGrid ^G;

		public:
			SideClickEvent(ActorsGrid ^g, String ^show)
			{
				Show = show;
				G = g;
			}

			virtual void OnClick(SourceGrid::CellContext sender, EventArgs ^e) override
			{
				if (Show == nullptr)
					G->DoSort();
				else
				    G->ShowToTop(Show);
			}
		};

		void BuildData()
		{
			// find actors that have been in more than one thing
			// Dictionary<String^, StringList ^> ^whoInWhat = gcnew Dictionary<String^, StringList ^>;
			TheTVDB ^db = mDoc->GetTVDB(true,"Actors");
			TheData = gcnew DataArr(db->GetSeriesDict()->Count);
			for each (KeyValuePair<int, SeriesInfo ^> ^ser in db->GetSeriesDict())
			{
				SeriesInfo ^si = ser->Value;
				String ^actors = si->GetItem("Actors");
				if (!String::IsNullOrEmpty(actors))
					for each (String ^aa in actors->Split('|'))
					{
						aa = aa->Trim();
						if (!String::IsNullOrEmpty(aa))
							TheData->Set(si->Name, aa, gcnew DataArr::ArrData(true, true));
					}

				if (cbGuestStars->Checked)
				{
					for each (KeyValuePair<int, Season ^> ^kvp in si->Seasons)
					{
						for each (Episode ^ep in kvp->Value->Episodes)
						{
							String ^guest = ep->GetItem("GuestStars");

							if (!String::IsNullOrEmpty(guest))
								for each (String ^aa in guest->Split('|'))
								{
									aa = aa->Trim();
									if (!String::IsNullOrEmpty(aa))
										TheData->Set(si->Name, aa, gcnew DataArr::ArrData(true, false));
								}
						}
					}
				}
			}

			db->Unlock("Actors");
			TheData->RemoveEmpties();
		}
		void SortByName()
		{
			Internal++;
			rbName->Checked = true;
			Internal--;
			TheData->SortRows(false);
			TheData->SortCols(false);
		}
		void SortByTotals()
		{
			Internal++;
			rbTotals->Checked = true;
			Internal--;
			TheData->SortRows(true);
			TheData->SortCols(true);
		}
		void SortRowsByCount()
		{
			Internal++;
			rbCustom->Checked = true;
			Internal--;
			TheData->SortRows(true);
			FillGrid();
		}
		void SortColsByCount()
		{
			Internal++;
			rbCustom->Checked = true;
			Internal--;
			TheData->SortCols(true);
			FillGrid();
		}
		void ActorToTop(String ^a)
		{
			Internal++;
			rbCustom->Checked = true;
			Internal--;

			TheData->MoveColToTop(a);

			// also move the shows they've been in to the top, too
			int c = TheData->Cols->IndexOf(a);
			if (c != 0)
				return; // uh oh!
			int end = 0;
			for (int r=TheData->DataR-1;r>=end;r--)
			{
				DataArr::ArrData ^d = TheData->Data[r,0];
				if ((d!=nullptr) && d->yes)
				{
					TheData->MoveRowToTop(TheData->Rows[r++]);
					end++;
				}
			}
			FillGrid();
		}
		void ShowToTop(String ^s)
		{
			Internal++;
			rbCustom->Checked = true;
			Internal--;

			TheData->MoveRowToTop(s);

			// also move the actors in this show to the top, too
			int r = TheData->Rows->IndexOf(s);
			if (r != 0)
				return; // uh oh!
			int end = 0;
			for (int c=TheData->DataC-1;c>=end;c--)
			{
				DataArr::ArrData ^d = TheData->Data[0,c];
				if ((d!=nullptr) && d->yes)
				{
					TheData->MoveColToTop(TheData->Cols[c++]);
					end++;
				}
			}

			FillGrid();
		}
		void FillGrid()
		{
			SourceGrid::Cells::Views::Cell ^rowTitleModel = gcnew SourceGrid::Cells::Views::Cell();
			rowTitleModel->BackColor = Color::SteelBlue;
			rowTitleModel->ForeColor = Color::White;
			rowTitleModel->TextAlignment = DevAge::Drawing::ContentAlignment::MiddleLeft;

			SourceGrid::Cells::Views::Cell ^colTitleModel = gcnew SourceGrid::Cells::Views::Cell();
			colTitleModel->ElementText = gcnew RotatedText(-90.0);
			colTitleModel->BackColor = Color::SteelBlue;
			colTitleModel->ForeColor = Color::White;
			colTitleModel->TextAlignment = DevAge::Drawing::ContentAlignment::BottomCenter;

			SourceGrid::Cells::Views::Cell ^topleftTitleModel = gcnew SourceGrid::Cells::Views::Cell();
			topleftTitleModel->BackColor = Color::SteelBlue;
			topleftTitleModel->ForeColor = Color::White;
			topleftTitleModel->TextAlignment = DevAge::Drawing::ContentAlignment::BottomLeft;

			SourceGrid::Cells::Views::Cell ^yesModel = gcnew SourceGrid::Cells::Views::Cell();
			yesModel->BackColor = Color::Green;
			yesModel->ForeColor = Color::Green;
			yesModel->TextAlignment = DevAge::Drawing::ContentAlignment::MiddleLeft;

			SourceGrid::Cells::Views::Cell ^guestModel = gcnew SourceGrid::Cells::Views::Cell();
			guestModel->BackColor = Color::LightGreen;
			guestModel->ForeColor = Color::LightGreen;
			guestModel->TextAlignment = DevAge::Drawing::ContentAlignment::MiddleLeft;

			grid1->Columns->Clear();
			grid1->Rows->Clear();

			int rows = TheData->DataR + 2; // title and total
			int cols = TheData->DataC + 2;

			grid1->ColumnsCount = cols;
			grid1->RowsCount = rows;
			grid1->FixedColumns = 1;
			grid1->FixedRows = 1;
			grid1->Selection->EnableMultiSelection = false;

			for (int i=0;i<cols;i++)
			{
				grid1->Columns[i]->AutoSizeMode = i==0 ? SourceGrid::AutoSizeMode::Default : SourceGrid::AutoSizeMode::MinimumSize;
				if (i > 0)
					grid1->Columns[i]->Width = 24;
			}

			grid1->Rows[0]->AutoSizeMode = SourceGrid::AutoSizeMode::MinimumSize;
			grid1->Rows[0]->Height = 100;

			SourceGrid::Cells::ColumnHeader ^h;
			h = gcnew SourceGrid::Cells::ColumnHeader("Show");
			h->AutomaticSortEnabled = false;
			h->ResizeEnabled = false;

			grid1[0,0] = h;
			grid1[0,0]->View = topleftTitleModel;
			grid1[0,0]->AddController(gcnew SideClickEvent(this, nullptr)); // default sort

			SourceGrid::Cells::Views::Cell ^rotateView = gcnew SourceGrid::Cells::Views::Cell();
			for (int c=0;c<TheData->DataC;c++)
			{
				h = gcnew SourceGrid::Cells::ColumnHeader(TheData->Cols[c]); // "<A HREF=\"http://www.imdb.com/find?s=nm&q="+kvp->Key+"\">"+kvp->Key+"</a>");

				// h->AddController(sortableController);
				// h->SortComparer = gcnew SourceGrid::MultiColumnsComparer(c, 0); // TODO: remove?
				h->AutomaticSortEnabled = false;
				h->ResizeEnabled = false;
				grid1[0,c+1] = h;
				grid1[0,c+1]->View = colTitleModel;
				grid1[0,c+1]->AddController(gcnew TopClickEvent(this, TheData->Cols[c]));
			}

			int totalCol = grid1->ColumnsCount - 1;
			h = gcnew SourceGrid::Cells::ColumnHeader("Totals");
			h->AutomaticSortEnabled = false;
			//h->AddController(sortableController);
			// h->SortComparer = gcnew SourceGrid::MultiColumnsComparer(c, 0);
			h->ResizeEnabled = false;
			grid1->Columns[totalCol]->Width = 48;
			grid1[0,totalCol] = h;
			grid1[0,totalCol]->View = colTitleModel;
			grid1[0,totalCol]->AddController(gcnew SortRowsByCountEvent(this));

			for (int r=0;r<TheData->DataR;r++)
			{
				SourceGrid::Cells::RowHeader ^rh = gcnew SourceGrid::Cells::RowHeader(TheData->Rows[r]);
				rh->ResizeEnabled = false;

				grid1[r+1,0] = rh;
				grid1[r+1,0]->AddController(gcnew SideClickEvent(this, TheData->Rows[r]));
			}

			SourceGrid::Cells::RowHeader ^rh = gcnew SourceGrid::Cells::RowHeader("Totals");
			rh->ResizeEnabled = false;
			grid1[TheData->DataR+1,0] = rh;
			grid1[TheData->DataR+1,0]->AddController(gcnew SortColsByCountEvent(this));

			for (int c=0;c<TheData->DataC;c++)
			{
				for (int r=0;r<TheData->DataR;r++)
				{
					DataArr::ArrData ^d = TheData->Data[r,c];
					if ((d != nullptr) && (d->yes))
					{
						grid1[r+1,c+1] = gcnew SourceGrid::Cells::Cell("Y");
						grid1[r+1,c+1]->View = d->act ? yesModel : guestModel;
						grid1[r+1,c+1]->AddController(gcnew CellClickEvent(TheData->Cols[c], TheData->Rows[r]));
					}
					else
						grid1[r+1,c+1] = gcnew SourceGrid::Cells::Cell("");
				}
			}

			for (int c=0;c<TheData->DataC;c++)
				grid1[rows-1,c+1] = gcnew SourceGrid::Cells::Cell(TheData->ColScore(c));

			for (int r=0;r<TheData->DataR;r++)
				grid1[r+1,cols-1] = gcnew SourceGrid::Cells::Cell(TheData->RowScore(r, nullptr));

			grid1[TheData->DataR+1,TheData->DataC+1] = gcnew SourceGrid::Cells::Cell("");

			grid1->AutoSizeCells();
		}
	private: System::Void bnClose_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 this->Close();
			 }
	private: System::Void cbGuestStars_CheckedChanged(System::Object^  sender, System::EventArgs^  e) 
			 {
				 cbGuestStars->Update();
				 BuildData();
				 DoSort();
			 }
	private: System::Void bnSave_Click(System::Object^  sender, System::EventArgs^  e) 
			 {
				 saveFile->Filter = "PNG Files (*.png)|*.png|All Files (*.*)|*.*";
				 if (saveFile->ShowDialog() != ::DialogResult::OK)
					 return;

				 SourceGrid::Exporter::Image ^image = gcnew SourceGrid::Exporter::Image();
				 Bitmap ^b = image->Export(grid1, grid1->CompleteRange);
				 b->Save(saveFile->FileName, Imaging::ImageFormat::Png);
			 }
			 void DoSort()
			 {
				 if (rbTotals->Checked)
					 SortByTotals();
				 else
					 SortByName(); // will check name for us, too
				 FillGrid();

			 }
	private: System::Void rbName_CheckedChanged(System::Object^  sender, System::EventArgs^  e) 
			 {
				 if (Internal)
					 return;
				 DoSort();
			 }
	private: System::Void rbTotals_CheckedChanged(System::Object^  sender, System::EventArgs^  e) 
			 {
				 if (Internal)
					 return;
				 DoSort();
			 }
	};


}
