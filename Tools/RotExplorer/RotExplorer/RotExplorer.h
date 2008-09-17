#pragma once

#include <windows.h>
#include <uxtheme.h>
#include <ole2.h>
#include <shobjidl.h>
#include <thumbcache.h>

namespace RotExplorer {

	using namespace System;
	using namespace System::ComponentModel;
	using namespace System::Collections;
	using namespace System::Windows::Forms;
	using namespace System::Data;
	using namespace System::Drawing;
	using namespace System::Runtime::InteropServices;

	/// <summary>
	/// Summary for RotExplorer
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public ref class RotExplorer : public System::Windows::Forms::Form
	{
	public:

		RotExplorer(void)
		{
			InitializeComponent();

			HWND hWnd = (HWND)view->Handle.ToPointer();
			SetWindowTheme(hWnd, L"Explorer", NULL);
			::SendMessage(hWnd, LVM_SETEXTENDEDLISTVIEWSTYLE, 0, LVS_EX_DOUBLEBUFFER);

			Update();
		}

		void Update() {
			IRunningObjectTable* table;
			IBindCtx* ctx;
			Marshal::ThrowExceptionForHR(CreateBindCtx(0, &ctx));

			try {
				IEnumMoniker* enumMoniker;
				HRESULT hresult;
				
				Marshal::ThrowExceptionForHR(ctx->GetRunningObjectTable(&table));

				if(SUCCEEDED(hresult = table->EnumRunning(&enumMoniker))) {
					IMalloc* malloc = 0;

					view->Items->Clear();

					try {
						Marshal::ThrowExceptionForHR(CoGetMalloc(1, &malloc));

						IMoniker* moniker;
						while(S_OK == enumMoniker->Next(1, &moniker, NULL)) {
							ListViewItem^ item;

							{
								LPOLESTR name;
								moniker->GetDisplayName(ctx, NULL, &name);
								item = gcnew ListViewItem(Marshal::PtrToStringUni((IntPtr)name));
								malloc->Free(name);
							}

							{
								array<String^>^ names = gcnew array<String^>(7);
								names[0] = "None"; names[1] = "Composite"; names[2] = "File"; names[3] = "Anti";
								names[4] = "Item"; names[5] = "Pointer"; names[6] = "Class";
								
								DWORD mksys;
								if(SUCCEEDED(moniker->IsSystemMoniker(&mksys))) {
									if(mksys >= 0 && (int)mksys < names->Length) {
										item->SubItems->Add(gcnew ListViewItem::ListViewSubItem(item, names[mksys]));
									} else { 
										//Unknown type!
										item->SubItems->Add(gcnew ListViewItem::ListViewSubItem(item, "Unknown!", Color::Gray, Color::Transparent, view->Font));
									}
								} else {
									//Not a system moniker
									item->SubItems->Add(gcnew ListViewItem::ListViewSubItem(item, "Not system", SystemColors::GrayText, Color::Transparent, view->Font));
								}
							}

							IUnknown* unk = 0;
							try {
								if(SUCCEEDED(table->GetObject(moniker, &unk))) {
									String^ supported = "";

#define DETECT(cls) \
									{ \
										cls* pobj; \
										if(SUCCEEDED(unk->QueryInterface(IID_PPV_ARGS(&pobj)))) { \
											pobj->Release(); \
											supported = supported + "," #cls; \
										} \
									}

									DETECT(IDispatch);
									DETECT(IFileIsInUse);
									DETECT(IOleObject);
									DETECT(IOleWindow);
									DETECT(IOleDocument);
									DETECT(IOleLink);
									DETECT(IThumbnailProvider);
									DETECT(IMoniker);
									DETECT(IStream);
									DETECT(IPersist);
									DETECT(IRunnableObject);
									DETECT(IPersistFile);
									DETECT(IPersistFolder);
									DETECT(IPersistFolder2);
									DETECT(IPersistFolder3);
									DETECT(IPersistStream);
									DETECT(IPersistStorage);
									DETECT(ISynchronize);
									DETECT(IInitializeWithStream);
									DETECT(IInitializeWithItem);
									DETECT(IInitializeWithFile);
									DETECT(IPreviewHandler);
									DETECT(ITypeLib);
									DETECT(IPropertyBag);
									DETECT(IPropertyBag2);
									DETECT(IAccessibleObject);
									DETECT(IErrorInfo);
									DETECT(IFileDialog);
									DETECT(IFileOperation);
									DETECT(IFileOperationProgressSink);
									DETECT(IInternet);
									DETECT(IQueryCancelAutoPlay);
									DETECT(IEntity);
									DETECT(IErrorInfo);
#undef DETECT

									if(supported->Length > 0)
										supported = supported->Substring(1);

									item->SubItems->Add(gcnew ListViewItem::ListViewSubItem(item, supported));

									{
										IDispatch* disp;
										String^ desc = "[GetTypeInfo failed]";

										if(SUCCEEDED(unk->QueryInterface(IID_PPV_ARGS(&disp)))) {
											try {
												ITypeInfo* typeInfo;
												if(SUCCEEDED(disp->GetTypeInfo(0, 0, &typeInfo))) {
													desc = "";
													tagTYPEATTR* typeAttr = 0;
													try {
														if (SUCCEEDED(typeInfo->GetTypeAttr(&typeAttr))) {
															for(int memberID = 0; memberID < typeAttr->cFuncs; ++memberID) {
																BSTR names[2] = { 0 };
																UINT count;
																if(SUCCEEDED(typeInfo->GetNames(memberID, names, ARRAYSIZE(names), &count))) {
																	desc = desc + Marshal::PtrToStringUni((IntPtr)names[0]) + " ";
																}
															}
														}
													} finally {
														if(typeAttr)
															typeInfo->ReleaseTypeAttr(typeAttr);
														typeInfo->Release();
													}
												}
											} finally {
												disp->Release();
											}

											item->SubItems->Add(gcnew ListViewItem::ListViewSubItem(item, desc));
										} else {
											item->SubItems->Add(gcnew ListViewItem::ListViewSubItem());
										}
									}
								} else {
									item->SubItems->Add(gcnew ListViewItem::ListViewSubItem(item, "Can't GetObject"));
									//item->SubItems->Add(gcnew ListViewItem::ListViewSubItem());
								}
							} finally {
								if(unk)
									unk->Release();
							}

							view->Items->Add(item);
						}
					} finally {
						enumMoniker->Release();
						ctx->Release();
						if(malloc)
							malloc->Release();
					}
				} else {
					view->Items->Clear();
					view->Items->Add(gcnew ListViewItem(System::Runtime::InteropServices::Marshal::GetExceptionForHR(hresult)->Message));
				}
			} finally {
				table->Release();
			}
		}

		void refresh_Click(System::Object^  sender, System::EventArgs^ e) {
			Update();
		}

	protected:
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		~RotExplorer()
		{
			if (components)
			{
				delete components;
			}
		}
	private: System::Windows::Forms::ListView^  view;
	private: System::Windows::Forms::ColumnHeader^  monikerCol;
private: System::Windows::Forms::ColumnHeader^  monikerTypeCol;
private: System::Windows::Forms::ColumnHeader^  monikerSupportedInterfacesCol;
private: System::Windows::Forms::ColumnHeader^  dispatchNames;









	private: System::Windows::Forms::Button^  refresh;
	protected: 

	private:
		/// <summary>
		/// Required designer variable.
		/// </summary>
		System::ComponentModel::Container ^components;

#pragma region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		void InitializeComponent(void)
		{
			System::Windows::Forms::Label^  label1;
			this->view = (gcnew System::Windows::Forms::ListView());
			this->monikerCol = (gcnew System::Windows::Forms::ColumnHeader());
			this->monikerTypeCol = (gcnew System::Windows::Forms::ColumnHeader());
			this->monikerSupportedInterfacesCol = (gcnew System::Windows::Forms::ColumnHeader());
			this->dispatchNames = (gcnew System::Windows::Forms::ColumnHeader());
			this->refresh = (gcnew System::Windows::Forms::Button());
			label1 = (gcnew System::Windows::Forms::Label());
			this->SuspendLayout();
			// 
			// label1
			// 
			label1->AutoSize = true;
			label1->Location = System::Drawing::Point(12, 9);
			label1->Name = L"label1";
			label1->Size = System::Drawing::Size(122, 15);
			label1->TabIndex = 1;
			label1->Text = L"Running Object Table";
			// 
			// view
			// 
			this->view->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
				| System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->view->Columns->AddRange(gcnew cli::array< System::Windows::Forms::ColumnHeader^  >(4) {this->monikerCol, this->monikerTypeCol, 
				this->monikerSupportedInterfacesCol, this->dispatchNames});
			this->view->FullRowSelect = true;
			this->view->LabelEdit = true;
			this->view->Location = System::Drawing::Point(12, 27);
			this->view->Name = L"view";
			this->view->ShowItemToolTips = true;
			this->view->Size = System::Drawing::Size(805, 512);
			this->view->TabIndex = 0;
			this->view->UseCompatibleStateImageBehavior = false;
			this->view->View = System::Windows::Forms::View::Details;
			this->view->ItemActivate += gcnew System::EventHandler(this, &RotExplorer::view_ItemActivate);
			// 
			// monikerCol
			// 
			this->monikerCol->Text = L"Moniker Name";
			this->monikerCol->Width = 250;
			// 
			// monikerTypeCol
			// 
			this->monikerTypeCol->Text = L"Type";
			this->monikerTypeCol->Width = 65;
			// 
			// monikerSupportedInterfacesCol
			// 
			this->monikerSupportedInterfacesCol->Text = L"Interfaces";
			this->monikerSupportedInterfacesCol->Width = 200;
			// 
			// dispatchNames
			// 
			this->dispatchNames->Text = L"IDispatch Fields/Methods";
			this->dispatchNames->Width = 110;
			// 
			// refresh
			// 
			this->refresh->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
			this->refresh->Location = System::Drawing::Point(742, 545);
			this->refresh->Name = L"refresh";
			this->refresh->Size = System::Drawing::Size(75, 23);
			this->refresh->TabIndex = 2;
			this->refresh->Text = L"&Refresh";
			this->refresh->UseVisualStyleBackColor = true;
			this->refresh->Click += gcnew System::EventHandler(this, &RotExplorer::refresh_Click);
			// 
			// RotExplorer
			// 
			this->AutoScaleDimensions = System::Drawing::SizeF(7, 15);
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->ClientSize = System::Drawing::Size(829, 580);
			this->Controls->Add(this->refresh);
			this->Controls->Add(label1);
			this->Controls->Add(this->view);
			this->Font = (gcnew System::Drawing::Font(L"Segoe UI", 9, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point, 
				static_cast<System::Byte>(0)));
			this->Name = L"RotExplorer";
			this->Text = L"ROT Explorer";
			this->ResumeLayout(false);
			this->PerformLayout();

		}
#pragma endregion
private: System::Void view_ItemActivate(System::Object^  sender, System::EventArgs^  e) {
			 System::Text::StringBuilder^ sb = gcnew System::Text::StringBuilder();

			 for each(ListViewItem^ item in view->SelectedItems) {
				 sb->Append(item->Text);

				 for each(ListViewItem::ListViewSubItem^ subitem in item->SubItems) {
					 sb->Append(", ");
					 sb->Append(subitem->Text);
				 }
			 }

			 System::Windows::Forms::Clipboard::SetText(sb->ToString());
		 }
};
}

