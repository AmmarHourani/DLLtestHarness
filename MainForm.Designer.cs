namespace DllTestHarness
{
    partial class MainForm
    {
        /// <summary>Required designer variable.</summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>Clean up any resources being used.</summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblDll = new System.Windows.Forms.Label();
            this.txtDllPath = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnUnload = new System.Windows.Forms.Button();
            this.lblModuleStatus = new System.Windows.Forms.Label();
            this.grpResolve = new System.Windows.Forms.GroupBox();
            this.lblFn = new System.Windows.Forms.Label();
            this.txtFuncName = new System.Windows.Forms.TextBox();
            this.btnProbe = new System.Windows.Forms.Button();
            this.lblConv = new System.Windows.Forms.Label();
            this.cmbConvention = new System.Windows.Forms.ComboBox();
            this.lblSig = new System.Windows.Forms.Label();
            this.cmbSignature = new System.Windows.Forms.ComboBox();
            this.lblArg1 = new System.Windows.Forms.Label();
            this.txtArg1 = new System.Windows.Forms.TextBox();
            this.lblArg2 = new System.Windows.Forms.Label();
            this.txtArg2 = new System.Windows.Forms.TextBox();
            this.btnCall = new System.Windows.Forms.Button();
            this.lblStdNote = new System.Windows.Forms.Label();
            this.lblExports = new System.Windows.Forms.Label();
            this.lstExports = new System.Windows.Forms.ListView();
            this.colName = new System.Windows.Forms.ColumnHeader();
            this.colOrdinal = new System.Windows.Forms.ColumnHeader();
            this.colRva = new System.Windows.Forms.ColumnHeader();
            this.colHint = new System.Windows.Forms.ColumnHeader();
            this.lblOut = new System.Windows.Forms.Label();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.grpResolve.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblDll
            // 
            this.lblDll.AutoSize = true;
            this.lblDll.Location = new System.Drawing.Point(12, 18);
            this.lblDll.Name = "lblDll";
            this.lblDll.Size = new System.Drawing.Size(29, 15);
            this.lblDll.TabIndex = 0;
            this.lblDll.Text = "DLL:";
            // 
            // txtDllPath
            // 
            this.txtDllPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDllPath.Location = new System.Drawing.Point(70, 15);
            this.txtDllPath.Name = "txtDllPath";
            this.txtDllPath.Size = new System.Drawing.Size(480, 23);
            this.txtDllPath.TabIndex = 1;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(558, 14);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(130, 25);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(70, 48);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(160, 25);
            this.btnLoad.TabIndex = 3;
            this.btnLoad.Text = "Load (LoadLibrary)";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnUnload
            // 
            this.btnUnload.Location = new System.Drawing.Point(238, 48);
            this.btnUnload.Name = "btnUnload";
            this.btnUnload.Size = new System.Drawing.Size(160, 25);
            this.btnUnload.TabIndex = 4;
            this.btnUnload.Text = "Unload (FreeLibrary)";
            this.btnUnload.UseVisualStyleBackColor = true;
            this.btnUnload.Click += new System.EventHandler(this.btnUnload_Click);
            // 
            // lblModuleStatus
            // 
            this.lblModuleStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblModuleStatus.ForeColor = System.Drawing.Color.DimGray;
            this.lblModuleStatus.Location = new System.Drawing.Point(410, 52);
            this.lblModuleStatus.Name = "lblModuleStatus";
            this.lblModuleStatus.Size = new System.Drawing.Size(278, 18);
            this.lblModuleStatus.TabIndex = 5;
            this.lblModuleStatus.Text = "Not loaded";
            // 
            // grpResolve
            // 
            this.grpResolve.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpResolve.Controls.Add(this.lblFn);
            this.grpResolve.Controls.Add(this.txtFuncName);
            this.grpResolve.Controls.Add(this.btnProbe);
            this.grpResolve.Controls.Add(this.lblConv);
            this.grpResolve.Controls.Add(this.cmbConvention);
            this.grpResolve.Controls.Add(this.lblSig);
            this.grpResolve.Controls.Add(this.cmbSignature);
            this.grpResolve.Controls.Add(this.lblArg1);
            this.grpResolve.Controls.Add(this.txtArg1);
            this.grpResolve.Controls.Add(this.lblArg2);
            this.grpResolve.Controls.Add(this.txtArg2);
            this.grpResolve.Controls.Add(this.btnCall);
            this.grpResolve.Controls.Add(this.lblStdNote);
            this.grpResolve.Location = new System.Drawing.Point(12, 88);
            this.grpResolve.Name = "grpResolve";
            this.grpResolve.Size = new System.Drawing.Size(676, 210);
            this.grpResolve.TabIndex = 6;
            this.grpResolve.TabStop = false;
            this.grpResolve.Text = "Resolve && Call";
            // 
            // lblFn
            // 
            this.lblFn.AutoSize = true;
            this.lblFn.Location = new System.Drawing.Point(12, 28);
            this.lblFn.Name = "lblFn";
            this.lblFn.Size = new System.Drawing.Size(58, 15);
            this.lblFn.TabIndex = 0;
            this.lblFn.Text = "Function:";
            // 
            // txtFuncName
            // 
            this.txtFuncName.Location = new System.Drawing.Point(88, 25);
            this.txtFuncName.Name = "txtFuncName";
            this.txtFuncName.Size = new System.Drawing.Size(250, 23);
            this.txtFuncName.TabIndex = 1;
            // 
            // btnProbe
            // 
            this.btnProbe.Location = new System.Drawing.Point(348, 24);
            this.btnProbe.Name = "btnProbe";
            this.btnProbe.Size = new System.Drawing.Size(140, 25);
            this.btnProbe.TabIndex = 2;
            this.btnProbe.Text = "GetProcAddress";
            this.btnProbe.UseVisualStyleBackColor = true;
            this.btnProbe.Click += new System.EventHandler(this.btnProbe_Click);
            // 
            // lblConv
            // 
            this.lblConv.AutoSize = true;
            this.lblConv.Location = new System.Drawing.Point(12, 64);
            this.lblConv.Name = "lblConv";
            this.lblConv.Size = new System.Drawing.Size(75, 15);
            this.lblConv.TabIndex = 3;
            this.lblConv.Text = "Convention:";
            // 
            // cmbConvention
            // 
            this.cmbConvention.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbConvention.FormattingEnabled = true;
            this.cmbConvention.Location = new System.Drawing.Point(100, 61);
            this.cmbConvention.Name = "cmbConvention";
            this.cmbConvention.Size = new System.Drawing.Size(120, 23);
            this.cmbConvention.TabIndex = 4;
            // 
            // lblSig
            // 
            this.lblSig.AutoSize = true;
            this.lblSig.Location = new System.Drawing.Point(240, 64);
            this.lblSig.Name = "lblSig";
            this.lblSig.Size = new System.Drawing.Size(60, 15);
            this.lblSig.TabIndex = 5;
            this.lblSig.Text = "Signature:";
            // 
            // cmbSignature
            // 
            this.cmbSignature.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSignature.FormattingEnabled = true;
            this.cmbSignature.Location = new System.Drawing.Point(316, 61);
            this.cmbSignature.Name = "cmbSignature";
            this.cmbSignature.Size = new System.Drawing.Size(250, 23);
            this.cmbSignature.TabIndex = 6;
            this.cmbSignature.SelectedIndexChanged += new System.EventHandler(this.cmbSignature_SelectedIndexChanged);
            // 
            // lblArg1
            // 
            this.lblArg1.AutoSize = true;
            this.lblArg1.Location = new System.Drawing.Point(12, 104);
            this.lblArg1.Name = "lblArg1";
            this.lblArg1.Size = new System.Drawing.Size(38, 15);
            this.lblArg1.TabIndex = 7;
            this.lblArg1.Text = "Arg 1:";
            // 
            // txtArg1
            // 
            this.txtArg1.Location = new System.Drawing.Point(88, 101);
            this.txtArg1.Name = "txtArg1";
            this.txtArg1.Size = new System.Drawing.Size(200, 23);
            this.txtArg1.TabIndex = 8;
            // 
            // lblArg2
            // 
            this.lblArg2.AutoSize = true;
            this.lblArg2.Location = new System.Drawing.Point(300, 104);
            this.lblArg2.Name = "lblArg2";
            this.lblArg2.Size = new System.Drawing.Size(38, 15);
            this.lblArg2.TabIndex = 9;
            this.lblArg2.Text = "Arg 2:";
            // 
            // txtArg2
            // 
            this.txtArg2.Location = new System.Drawing.Point(360, 101);
            this.txtArg2.Name = "txtArg2";
            this.txtArg2.Size = new System.Drawing.Size(200, 23);
            this.txtArg2.TabIndex = 10;
            // 
            // btnCall
            // 
            this.btnCall.Location = new System.Drawing.Point(88, 150);
            this.btnCall.Name = "btnCall";
            this.btnCall.Size = new System.Drawing.Size(160, 36);
            this.btnCall.TabIndex = 11;
            this.btnCall.Text = "Call Function";
            this.btnCall.UseVisualStyleBackColor = true;
            this.btnCall.Click += new System.EventHandler(this.btnCall_Click);
            // 
            // lblStdNote
            // 
            this.lblStdNote.ForeColor = System.Drawing.Color.DimGray;
            this.lblStdNote.Location = new System.Drawing.Point(268, 144);
            this.lblStdNote.Name = "lblStdNote";
            this.lblStdNote.Size = new System.Drawing.Size(396, 50);
            this.lblStdNote.TabIndex = 12;
            this.lblStdNote.Text = "Note: std::string / std::wstring are not listed. They have no stable ABI layout " +
                "and cannot be marshalled across a raw pointer. Export such functions via a C shim " +
                "taking const char* / const wchar_t*.";
            // 
            // lblExports
            // 
            this.lblExports.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.lblExports.AutoSize = true;
            this.lblExports.Location = new System.Drawing.Point(12, 308);
            this.lblExports.Name = "lblExports";
            this.lblExports.Size = new System.Drawing.Size(115, 15);
            this.lblExports.TabIndex = 7;
            this.lblExports.Text = "Exported functions:";
            // 
            // lstExports
            // 
            this.lstExports.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstExports.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colOrdinal,
            this.colRva,
            this.colHint});
            this.lstExports.FullRowSelect = true;
            this.lstExports.GridLines = true;
            this.lstExports.HideSelection = false;
            this.lstExports.Location = new System.Drawing.Point(12, 326);
            this.lstExports.MultiSelect = false;
            this.lstExports.Name = "lstExports";
            this.lstExports.Size = new System.Drawing.Size(676, 150);
            this.lstExports.TabIndex = 8;
            this.lstExports.UseCompatibleStateImageBehavior = false;
            this.lstExports.View = System.Windows.Forms.View.Details;
            this.lstExports.SelectedIndexChanged += new System.EventHandler(this.lstExports_SelectedIndexChanged);
            // 
            // colName
            // 
            this.colName.Text = "Function";
            this.colName.Width = 280;
            // 
            // colOrdinal
            // 
            this.colOrdinal.Text = "Ordinal";
            this.colOrdinal.Width = 70;
            // 
            // colRva
            // 
            this.colRva.Text = "RVA";
            this.colRva.Width = 90;
            // 
            // colHint
            // 
            this.colHint.Text = "Parameters / Notes";
            this.colHint.Width = 220;
            // 
            // lblOut
            // 
            this.lblOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblOut.AutoSize = true;
            this.lblOut.Location = new System.Drawing.Point(12, 486);
            this.lblOut.Name = "lblOut";
            this.lblOut.Size = new System.Drawing.Size(50, 15);
            this.lblOut.TabIndex = 9;
            this.lblOut.Text = "Output:";
            // 
            // txtOutput
            // 
            this.txtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right))));
            this.txtOutput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.txtOutput.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtOutput.ForeColor = System.Drawing.Color.Gainsboro;
            this.txtOutput.Location = new System.Drawing.Point(12, 504);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ReadOnly = true;
            this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtOutput.Size = new System.Drawing.Size(676, 120);
            this.txtOutput.TabIndex = 10;
            this.txtOutput.WordWrap = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 636);
            this.Controls.Add(this.lblDll);
            this.Controls.Add(this.txtDllPath);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnUnload);
            this.Controls.Add(this.lblModuleStatus);
            this.Controls.Add(this.grpResolve);
            this.Controls.Add(this.lblExports);
            this.Controls.Add(this.lstExports);
            this.Controls.Add(this.lblOut);
            this.Controls.Add(this.txtOutput);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(640, 560);
            this.Name = "MainForm";
            this.Text = "DLL Test Harness";
            this.grpResolve.ResumeLayout(false);
            this.grpResolve.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblDll;
        private System.Windows.Forms.TextBox txtDllPath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnUnload;
        private System.Windows.Forms.Label lblModuleStatus;
        private System.Windows.Forms.GroupBox grpResolve;
        private System.Windows.Forms.Label lblFn;
        private System.Windows.Forms.TextBox txtFuncName;
        private System.Windows.Forms.Button btnProbe;
        private System.Windows.Forms.Label lblConv;
        private System.Windows.Forms.ComboBox cmbConvention;
        private System.Windows.Forms.Label lblSig;
        private System.Windows.Forms.ComboBox cmbSignature;
        private System.Windows.Forms.Label lblArg1;
        private System.Windows.Forms.TextBox txtArg1;
        private System.Windows.Forms.Label lblArg2;
        private System.Windows.Forms.TextBox txtArg2;
        private System.Windows.Forms.Button btnCall;
        private System.Windows.Forms.Label lblStdNote;
        private System.Windows.Forms.Label lblExports;
        private System.Windows.Forms.ListView lstExports;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colOrdinal;
        private System.Windows.Forms.ColumnHeader colRva;
        private System.Windows.Forms.ColumnHeader colHint;
        private System.Windows.Forms.Label lblOut;
        private System.Windows.Forms.TextBox txtOutput;
    }
}
