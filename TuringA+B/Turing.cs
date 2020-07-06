using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TuringA_B
{
    public partial class main : Form
    {
        public main()
        {
            InitializeComponent();
            InitializeForm();
            CreateMachines();
        }

        public void InitializeForm()
        {
            topPanel.BackColor = Color.FromArgb(33, 150, 243);
            pnlTitle.BackColor = Color.FromArgb(66, 165, 245);
            dtgvResult.AllowUserToAddRows = false;
            dtgvResult.AutoGenerateColumns = false;
            dtgvResult.Rows.Add();
            dtgvResult.MultiSelect = false;
            dtgvResult.AllowDrop = false;
            dtgvResult.AllowUserToDeleteRows = false;
            dtgvResult.AllowUserToOrderColumns = false;
            dtgvResult.AllowUserToResizeRows = false;
            dtgvResult.ColumnHeadersVisible = false;
            dtgvResult.RowHeadersVisible = false;
            dtgvResult.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dtgvResult.AllowUserToResizeColumns = false;
            dtgvResult.AllowUserToOrderColumns = false;
            dtgvResult.BackgroundColor = Color.White;
        }

        #region Create Machine Turing
        string cadena;
        char[] subStrings;
        int cont = 0;
        string estado = "";
        bool flag = false;
        Machine sumatoria;

        public void CreateMachines()
        {
            string cinta = "10BXYZS*=+-abc";
            string alphabet = "10*=+-aBc";

            sumatoria = new Machine(cinta.ToCharArray(), alphabet.ToCharArray(), "q0", "q3");

            sumatoria.addState("q0,+", "q0,X,1");
            sumatoria.addState("q0,=", "q3,=,1");
            sumatoria.addState("q0,1", "q1,X,1");
            sumatoria.addState("q1,=", "q1,=,1");
            sumatoria.addState("q1,+", "q1,+,1");
            sumatoria.addState("q1,1", "q1,1,1");
            sumatoria.addState("q1,»", "q2,1,-1");
            sumatoria.addState("q2,=", "q2,=,-1");
            sumatoria.addState("q2,+", "q2,+,-1");
            sumatoria.addState("q2,1", "q2,1,-1");
            sumatoria.addState("q2,X", "q0,X,1");
        }

        #endregion

        #region Events
        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            cadena = tbString.Text;
            subStrings = cadena.ToCharArray();
            int selectedCount = dtgvResult.Columns.Count;
            while (selectedCount > 1)
            {
                dtgvResult.Columns.RemoveAt(selectedCount-- - 1);
            }
            if (cadena.Length != 0)
            {
                for (int i = 0; i <= cadena.Length + 1; i++)
                {
                    if ((i == cadena.Length) || (i == cadena.Length + 1))
                    {
                        int cons = cadena.Length;
                        while (cons <= cadena.Length * 7)
                        {
                            this.dtgvResult.Columns.Add("blank" + cons, "»");
                            this.dtgvResult.Rows[0].Cells[cons].Value = "»";
                            cons++;
                        }
                    }
                    else
                    {
                        this.dtgvResult.Columns.Add("string" + i, subStrings[i].ToString());
                        this.dtgvResult.Rows[0].Cells[i].Value = subStrings[i].ToString();
                    }
                }
            }
            else
            {
                MessageBox.Show("Hãy nhập chuỗi W!");
            }
        }

        string[] temp;
        int i = 0;

        private void btnSum_Click(object sender, EventArgs e)
        {
            flag = false;
            while (!flag)
            {
                if (sumatoria.momement(sumatoria.getCurrent() + "," + this.dtgvResult.Rows[0].Cells[i].Value) != null)
                {
                    temp = sumatoria.momement(sumatoria.getCurrent() + "," + this.dtgvResult.Rows[0].Cells[i].Value).Split(',');
                    sumatoria.setCurrent(temp[0]);
                    dtgvResult.CurrentCell = dtgvResult.Rows[0].Cells[i];
                    dtgvResult.CurrentCell.Style.BackColor = Color.Tomato;
                    cont++;
                    estado = sumatoria.getCurrent();
                    this.dtgvResult.Rows[0].Cells[i].Value = temp[1];
                    if (temp[2] == "1")
                        i++;
                    else
                        i--;
                    if (sumatoria.itsEnd(sumatoria.getCurrent()))
                    {
                        flag = true;
                        i = 0;
                        sumatoria.setCurrent("q0");
                        cont = 0;
                    }
                }
                else
                {
                    MessageBox.Show("ERROR: Không thực hiện được hành động! Xuất hiện lỗi từ hệ thống!");
                    i = 0;
                    flag = true;
                    sumatoria.setCurrent("q0");
                }
            }
            int resultCount = dtgvResult.Columns.Count;
            int resultDec = 0;
            string resultBinary = "";
            bool checkEqual = false;
            for (int j = 0; j < resultCount; j++)
            {
                var value = this.dtgvResult.Rows[0].Cells[j].Value;
                if (value != null)
                {

                    if (value.ToString() == "=")
                        checkEqual = true;
                    if (checkEqual)
                    {
                        if (value.ToString().Equals("1"))
                        {
                            resultDec++;
                            resultBinary += 1;
                        }

                    }
                }
            }
            lbResultDEC.Text = resultDec.ToString();
            lbResultBinary.Text = resultBinary.ToString();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            this.tbString.Text = "";
            this.lbResultBinary.Text = "$$$";
            this.lbResultDEC.Text = "0";
            int selectedCount = dtgvResult.Columns.Count;
            while (selectedCount > 1)
            {
                dtgvResult.Columns.RemoveAt(selectedCount-- - 1);
            }
        }
        #endregion

        #region Move Form
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void topPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        #endregion
    }
}
