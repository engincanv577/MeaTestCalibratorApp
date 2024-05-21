namespace ExampleApplications
{
    partial class MainForm
    {
        /// <summary>
        ///Gerekli tasarımcı değişkeni.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///Kullanılan tüm kaynakları temizleyin.
        /// </summary>
        ///<param name="disposing">yönetilen kaynaklar dispose edilmeliyse doğru; aksi halde yanlış.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer üretilen kod

        /// <summary>
        /// Tasarımcı desteği için gerekli metot - bu metodun 
        ///içeriğini kod düzenleyici ile değiştirmeyin.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.DropDownList_COM_Ports = new Telerik.WinControls.UI.RadDropDownList();
            this.DropDownList_Baudrate = new Telerik.WinControls.UI.RadDropDownList();
            this.Label_COM = new Telerik.WinControls.UI.RadLabel();
            this.Label_Baudrate = new Telerik.WinControls.UI.RadLabel();
            this.MeaTestConnectionLabel = new Telerik.WinControls.UI.RadLabel();
            this.StartCommunicationErrorLabel = new System.Windows.Forms.Label();
            this.StartCommunicationButton = new Telerik.WinControls.UI.RadToggleButton();
            this.serialPortMeaTest = new System.IO.Ports.SerialPort(this.components);
            this.MeaTestOperStateSwitch = new Telerik.WinControls.UI.RadToggleSwitch();
            this.labelOperation = new System.Windows.Forms.Label();
            this.labelOperationCaution = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.DropDownList_COM_Ports)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DropDownList_Baudrate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Label_COM)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Label_Baudrate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MeaTestConnectionLabel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.StartCommunicationButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MeaTestOperStateSwitch)).BeginInit();
            this.SuspendLayout();
            // 
            // DropDownList_COM_Ports
            // 
            this.DropDownList_COM_Ports.DropDownAnimationEnabled = true;
            this.DropDownList_COM_Ports.Location = new System.Drawing.Point(90, 33);
            this.DropDownList_COM_Ports.Name = "DropDownList_COM_Ports";
            this.DropDownList_COM_Ports.Size = new System.Drawing.Size(107, 20);
            this.DropDownList_COM_Ports.TabIndex = 2;
            this.DropDownList_COM_Ports.SelectedIndexChanged += new Telerik.WinControls.UI.Data.PositionChangedEventHandler(this.DropDownList_COM_Ports_SelectedIndexChanged);
            this.DropDownList_COM_Ports.Click += new System.EventHandler(this.DropDownList_COM_Ports_Click);
            // 
            // DropDownList_Baudrate
            // 
            this.DropDownList_Baudrate.DefaultItemsCountInDropDown = 8;
            this.DropDownList_Baudrate.DropDownAnimationEnabled = true;
            this.DropDownList_Baudrate.Location = new System.Drawing.Point(90, 70);
            this.DropDownList_Baudrate.Name = "DropDownList_Baudrate";
            this.DropDownList_Baudrate.Size = new System.Drawing.Size(107, 20);
            this.DropDownList_Baudrate.TabIndex = 3;
            this.DropDownList_Baudrate.SelectedIndexChanged += new Telerik.WinControls.UI.Data.PositionChangedEventHandler(this.DropDownList_Baudrate_SelectedIndexChanged);
            this.DropDownList_Baudrate.Click += new System.EventHandler(this.DropDownList_Baudrate_Click);
            // 
            // Label_COM
            // 
            this.Label_COM.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Label_COM.Location = new System.Drawing.Point(12, 33);
            this.Label_COM.Name = "Label_COM";
            this.Label_COM.Size = new System.Drawing.Size(68, 21);
            this.Label_COM.TabIndex = 4;
            this.Label_COM.Text = "COM Port:";
            // 
            // Label_Baudrate
            // 
            this.Label_Baudrate.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Label_Baudrate.Location = new System.Drawing.Point(12, 69);
            this.Label_Baudrate.Name = "Label_Baudrate";
            this.Label_Baudrate.Size = new System.Drawing.Size(63, 21);
            this.Label_Baudrate.TabIndex = 5;
            this.Label_Baudrate.Text = "Baudrate:";
            // 
            // MeaTestConnectionLabel
            // 
            this.MeaTestConnectionLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.MeaTestConnectionLabel.Location = new System.Drawing.Point(57, 6);
            this.MeaTestConnectionLabel.Name = "MeaTestConnectionLabel";
            this.MeaTestConnectionLabel.Size = new System.Drawing.Size(188, 21);
            this.MeaTestConnectionLabel.TabIndex = 5;
            this.MeaTestConnectionLabel.Text = "MeaTest M133C Connection";
            // 
            // StartCommunicationErrorLabel
            // 
            this.StartCommunicationErrorLabel.AutoSize = true;
            this.StartCommunicationErrorLabel.ForeColor = System.Drawing.Color.Red;
            this.StartCommunicationErrorLabel.Location = new System.Drawing.Point(16, 108);
            this.StartCommunicationErrorLabel.Name = "StartCommunicationErrorLabel";
            this.StartCommunicationErrorLabel.Size = new System.Drawing.Size(0, 13);
            this.StartCommunicationErrorLabel.TabIndex = 6;
            // 
            // StartCommunicationButton
            // 
            this.StartCommunicationButton.Location = new System.Drawing.Point(203, 33);
            this.StartCommunicationButton.Name = "StartCommunicationButton";
            this.StartCommunicationButton.Size = new System.Drawing.Size(81, 57);
            this.StartCommunicationButton.TabIndex = 7;
            this.StartCommunicationButton.Text = "Connect";
            this.StartCommunicationButton.Click += new System.EventHandler(this.StartCommunicationButton_Click);
            // 
            // serialPortMeaTest
            // 
            this.serialPortMeaTest.ReadTimeout = 1000;
            // 
            // MeaTestOperStateSwitch
            // 
            this.MeaTestOperStateSwitch.Location = new System.Drawing.Point(12, 367);
            this.MeaTestOperStateSwitch.Name = "MeaTestOperStateSwitch";
            this.MeaTestOperStateSwitch.Size = new System.Drawing.Size(95, 27);
            this.MeaTestOperStateSwitch.TabIndex = 8;
            this.MeaTestOperStateSwitch.Value = false;
            this.MeaTestOperStateSwitch.ValueChanged += new System.EventHandler(this.MeaTestOperStateSwitch_ValueChanged);
            ((Telerik.WinControls.UI.RadToggleSwitchElement)(this.MeaTestOperStateSwitch.GetChildAt(0))).ThumbOffset = 0;
            ((Telerik.WinControls.UI.ToggleSwitchPartElement)(this.MeaTestOperStateSwitch.GetChildAt(0).GetChildAt(0))).BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(255)))), ((int)(((byte)(27)))));
            ((Telerik.WinControls.UI.ToggleSwitchPartElement)(this.MeaTestOperStateSwitch.GetChildAt(0).GetChildAt(0))).BackColor3 = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(255)))), ((int)(((byte)(27)))));
            ((Telerik.WinControls.UI.ToggleSwitchPartElement)(this.MeaTestOperStateSwitch.GetChildAt(0).GetChildAt(0))).BackColor4 = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(255)))), ((int)(((byte)(27)))));
            ((Telerik.WinControls.UI.ToggleSwitchPartElement)(this.MeaTestOperStateSwitch.GetChildAt(0).GetChildAt(0))).BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(255)))), ((int)(((byte)(27)))));
            // 
            // labelOperation
            // 
            this.labelOperation.AutoSize = true;
            this.labelOperation.Location = new System.Drawing.Point(25, 351);
            this.labelOperation.Name = "labelOperation";
            this.labelOperation.Size = new System.Drawing.Size(70, 13);
            this.labelOperation.TabIndex = 9;
            this.labelOperation.Text = "OPERATION";
            // 
            // labelOperationCaution
            // 
            this.labelOperationCaution.AutoSize = true;
            this.labelOperationCaution.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.labelOperationCaution.ForeColor = System.Drawing.Color.Red;
            this.labelOperationCaution.Location = new System.Drawing.Point(20, 397);
            this.labelOperationCaution.Name = "labelOperationCaution";
            this.labelOperationCaution.Size = new System.Drawing.Size(0, 13);
            this.labelOperationCaution.TabIndex = 10;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(795, 451);
            this.Controls.Add(this.labelOperationCaution);
            this.Controls.Add(this.labelOperation);
            this.Controls.Add(this.MeaTestOperStateSwitch);
            this.Controls.Add(this.StartCommunicationButton);
            this.Controls.Add(this.StartCommunicationErrorLabel);
            this.Controls.Add(this.MeaTestConnectionLabel);
            this.Controls.Add(this.Label_Baudrate);
            this.Controls.Add(this.Label_COM);
            this.Controls.Add(this.DropDownList_Baudrate);
            this.Controls.Add(this.DropDownList_COM_Ports);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.Text = "Firmware Update App";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.DropDownList_COM_Ports)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DropDownList_Baudrate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Label_COM)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Label_Baudrate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MeaTestConnectionLabel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.StartCommunicationButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MeaTestOperStateSwitch)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Telerik.WinControls.UI.RadDropDownList DropDownList_COM_Ports;
        private Telerik.WinControls.UI.RadDropDownList DropDownList_Baudrate;
        private Telerik.WinControls.UI.RadLabel Label_COM;
        private Telerik.WinControls.UI.RadLabel Label_Baudrate;
        private Telerik.WinControls.UI.RadLabel MeaTestConnectionLabel;
        private System.Windows.Forms.Label StartCommunicationErrorLabel;
        private Telerik.WinControls.UI.RadToggleButton StartCommunicationButton;
        private System.IO.Ports.SerialPort serialPortMeaTest;
        private Telerik.WinControls.UI.RadToggleSwitch MeaTestOperStateSwitch;
        private System.Windows.Forms.Label labelOperation;
        private System.Windows.Forms.Label labelOperationCaution;
    }
}

