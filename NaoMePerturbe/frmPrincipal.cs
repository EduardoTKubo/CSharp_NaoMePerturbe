using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using NaoMePerturbe.Classes;


namespace NaoMePerturbe
{
    public partial class frmPrincipal : Form
    {
        public frmPrincipal()
        {
            InitializeComponent();

            this.Text = "";
            this.Width = 87;
            this.Height = 81;

            timer2.Interval = 1000;
            timer2.Enabled = true;

            timer1.Interval = 45000;
            timer1.Enabled = true;

            dtPicker1.Value = DateTime.Now;
            clsVariaveis.GDtBase = DateTime.Now;

            //CarregaLista();

        }

        private async void CarregaLista()
        {
            listBox1.Items.Clear();

            clsVariaveis.GstrSQL = "select x.* from ( select top 5 * from [003_Arq_lido] order by id desc ) as x order by x.id ";
            DataTable dtArq = await Classes.clsBanco.ConsultaAsync(clsVariaveis.GstrSQL);
            if (dtArq.Rows.Count != 0)
            {
                foreach (DataRow item in dtArq.Rows)
                {
                    listBox1.Items.Add(Convert.ToDateTime(item["data"]).ToString("dd/MM/yyyy") + " " + item["hora"] + " | " + item["arquivo"]);
                }
            }
        }

        private void CentralizarForm()
        {
            Rectangle rect = Screen.PrimaryScreen.WorkingArea;
            this.Top = (rect.Height / 2) - (this.Height / 2);
            this.Left = (rect.Width / 2) - (this.Width / 2);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string strHor = DateTime.Now.ToString("HH");
            string strMin = DateTime.Now.ToString("mm");

            switch (strHor)
            {
                case "00":
                    clsVariaveis.GContador = 0;
                    clsVariaveis.GProcDia = false;

                    dtPicker1.Value = DateTime.Now;
                    clsVariaveis.GDtBase = DateTime.Now;

                    break;

                case "13":
                case "14":
                case "15":
                case "16":
                case "17":
                    if (clsVariaveis.GProcDia == false)
                    {
                        switch (strMin)
                        {
                            case "00":
                            case "15":
                            case "30":
                            case "45":
                                //P01_ConfereProcesso();
                                break;

                            default:
                                break;
                        }
                    }
                    break;

                default:
                    break;
            }
            clsVariaveis.GContador = 0;
        }


        private void timer2_Tick(object sender, EventArgs e)
        {
            clsVariaveis.GContador = clsVariaveis.GContador + 1;
            lblContador.Text = clsVariaveis.GContador.ToString();
            this.Refresh();
        }

        private void lblContador_DoubleClick(object sender, EventArgs e)
        {
            if (this.Width == 87)
            {
                this.Text = Application.ProductName.ToString() + " ".PadLeft(110) + Application.ProductVersion;
                this.Width = 581;
                this.Height = 446;
            }
            else
            {
                this.Text = "";
                this.Width = 87;
                this.Height = 81;
            }
            CentralizarForm();
        }

        private void dtPicker1_ValueChanged(object sender, EventArgs e)
        {
            //clsVariaveis.GDtBase = Convert.ToDateTime(dtPicker1.Value.ToString()).AddDays(-1);
            clsVariaveis.GDtBase = Convert.ToDateTime(dtPicker1.Value.ToString());
        }

        private void btnNow_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Tem certeza ?", "Processar agora ?", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                btnNow.Enabled = false;

                this.listBox1.Items.Add(DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " | Inicio");

                P01_Processar();
            }
        }

        private async void P01_Processar()
        {
            // verifica se ja rodou o processo do dia
            if (clsVariaveis.GProcDia == false)
            {
                timer2.Enabled = false;
                this.Cursor = Cursors.WaitCursor;
                this.Refresh();

                clsVariaveis.GStrResult = string.Empty;

                // Processo
                // download via sftp
                if (await clsFtp.P02_DownloadFTPAsync())
                {
                    // se OK , extrair arquivo ( .zip )
                    if (await clsExtrair.Proc_UnZipFileAsync(clsVariaveis.GStrCamArqZip, Application.StartupPath + @"\arquivo\"))
                    {
                        // se OK, importar para SQL
                        if (await clsArquivo.ImportarArquivoAsync(clsVariaveis.GStrCamArqZip.Replace(".zip", ".txt")))
                        {
                            clsVariaveis.GStrResult = "OK";
                        }
                    }
                }
                else
                {
                    clsVariaveis.GStrResult = "Arquivo nao disponibilizado";
                }

                if (clsVariaveis.GStrResult != "")
                {
                    this.listBox1.Items.Add(DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " | " + clsVariaveis.GStrResult.ToString());
                }

                timer2.Enabled = true;
                this.Cursor = Cursors.Default;

                if (btnNow.Enabled == false)
                {
                    btnNow.Enabled = true;
                }
            }
        }

        private async void btnCopy_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Tem certeza ?", "Copiar Atu_Procon_I para outros DBs ?", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                btnCopy.Enabled = false;
                this.Cursor = Cursors.WaitCursor;
                this.Refresh();

                clsVariaveis.GstrSQL = "select * from [Atu_Procon_1] where Obs is null";
                DataTable dt = await Classes.clsBanco.ConsultaAsync(clsVariaveis.GstrSQL);
                if (dt.Rows.Count != 0)
                {
                    for (int i = 1; i < 4; i++)
                    {
                        switch (i)
                        {
                            case 1:
                                bool booResp1 = await clsDatabase.CopyToDbAsync("10.0.032.53", @"Data Source=10.0.32.53;Initial Catalog=procon; User ID=sa; Password=Mudar@1q2w3e;");

                                this.listBox1.Items.Add(DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " | " + clsVariaveis.GStrResult);
                                if (clsVariaveis.IntIncluido != 0)
                                {
                                    this.listBox1.Items.Add(DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " | 10.0.032.53 | Incluidos : " + clsVariaveis.IntIncluido);
                                    this.listBox1.Items.Add(DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " | 10.0.032.53 | Total     : " + clsVariaveis.IntTotal);
                                }
                                break;

                            case 2:
                                bool booResp2 = await clsDatabase.CopyToDbAsync("DB01", @"Data Source=10.0.32.59;Initial Catalog=PROCON; User ID=sa; Password=SRV@admin2012;");

                                this.listBox1.Items.Add(DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " | " + clsVariaveis.GStrResult);
                                if (clsVariaveis.IntIncluido != 0)
                                {
                                    this.listBox1.Items.Add(DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " | DB01 | Incluidos : " + clsVariaveis.IntIncluido);
                                    this.listBox1.Items.Add(DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " | DB01 | Total     : " + clsVariaveis.IntTotal);
                                }
                                break;

                            case 3:
                                bool booResp3 = await clsDatabase.CopyToDbAsync("DB02", @"Data Source=10.0.32.54;Initial Catalog=PROCON; User ID=sa; Password=SRV@admin2012;");

                                this.listBox1.Items.Add(DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " | " + clsVariaveis.GStrResult);
                                if (clsVariaveis.IntIncluido != 0)
                                {
                                    this.listBox1.Items.Add(DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " | DB02 | Incluidos : " + clsVariaveis.IntIncluido);
                                    this.listBox1.Items.Add(DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " | DB02 | Total     : " + clsVariaveis.IntTotal);
                                }
                                break;
                        }                        
                    }
                }
                else
                {
                    this.listBox1.Items.Add(DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " | Não há registros a copiar");
                }
            }

            if (btnCopy.Enabled == false)
            {
                btnCopy.Enabled = true;
            }
            this.Cursor = Cursors.Default;
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                MessageBox.Show(listBox1.SelectedItem.ToString());
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 1:
                    string sEmail = "Flavio / Cassio" + System.Environment.NewLine + System.Environment.NewLine;
                    sEmail += "Atualizada a base de telefones bloqueados ( DoNotCall )." + System.Environment.NewLine + System.Environment.NewLine;
                    sEmail += "Favor realizar o cruzamento x bloqueio em suas bases de trabalho." + System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine;
                    sEmail += " ".PadLeft(30) + "Servidor : 10.0.32.53" + System.Environment.NewLine;
                    sEmail += " ".PadLeft(30) + "Database : procon" + System.Environment.NewLine;
                    sEmail += " ".PadLeft(30) + "Tabela   : DoNotCall" + System.Environment.NewLine;

                    txtEmail.Text = sEmail;

                    txtAssunto.Text = "Atualização Diária - NÃO ME PERTURBE - ref. " + DateTime.Now.ToString("dd/MM/yyyy"); ;

                    break;
            }
        }

        private async void btnEnviarEmail_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Tem certeza ?", "Enviar email agora ?", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                string sEmail = await clsEmail.EnviaEmailAsync(txtAssunto.Text ,txtEmail.Text);
                MessageBox.Show(sEmail, "email", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            }
        }
             
    }
}
