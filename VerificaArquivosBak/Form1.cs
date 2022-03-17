using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace VerificaArquivosBak
{
    


    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            dataGridView1.Width = this.Width - 10;
            dataGridView1.Height = statusStrip1.Location.Y - dataGridView1.Location.Y - 5;
            dataGridView1.ScrollBars = ScrollBars.Both;
        }

        private void ExibirArquivos()
        {
            String procurarEm = @"F:\Backup_Clientes";
            List<FileInfo> listaArqs = new List<FileInfo>();
            int filtraTamanhoMinimo = 30000; //em Kb
            string filtraExtensao = "*.BAK";

            try
            {
                filtraTamanhoMinimo = Int32.Parse(txtFiltroTamanhoArquivo.Text);
            }
            catch (FormatException  fex)
            {
                txtFiltroTamanhoArquivo.Text = filtraTamanhoMinimo.ToString();
            }


            toolStripStatusLabel1.Text = "Buscando arquivos...";
            this.Update();

            Directory.GetDirectories(procurarEm, "*", SearchOption.AllDirectories).ToList<String>().ForEach(
                nomeDir => Directory.GetFiles(nomeDir, filtraExtensao, SearchOption.AllDirectories).ToList<String>().ForEach(
                    nomeArquivo => listaArqs.Add(new FileInfo(nomeArquivo))
                )
            );

            var lista = from x in listaArqs
                        where (x.Length / 1024) > filtraTamanhoMinimo
                        select new
                        {
                            Diretório = x.DirectoryName,
                            Arquivo = x.Name,
                            Tamanho_Kb = String.Format("{0:0,0}", (x.Length / 1024 ))
                        };

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.DataSource = lista.ToArray();
            dataGridView1.AutoResizeColumns();

            toolStripStatusLabel2.Text =  String.Format("{0:0,0} Kb Encontrados", listaArqs.Sum(x => (x.Length/1024)) );
            toolStripStatusLabel1.Text = "Busca concluida!";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ExibirArquivos();
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            String nomeArquivo = "";

            DialogResult dialog = MessageBox.Show("Deseja mesmo excluir arquivo(s) selecionado(s)?", "Excluir", MessageBoxButtons.YesNo);
            if( dialog != DialogResult.Yes ) { return; }


            try
            {
                foreach (DataGridViewRow xRow in dataGridView1.SelectedRows)
                {
                    nomeArquivo = xRow.Cells["Diretório"].Value.ToString()
                                  + "\\"
                                  + xRow.Cells["Arquivo"].Value.ToString();

                    if(File.Exists(nomeArquivo))
                    {
                        File.Delete(nomeArquivo);
                    }
                }

                ExibirArquivos();
                MessageBox.Show("Itens excluidos!", "Excluir");
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message, "Excluir");
            }

            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            dataGridView1.Width = this.Width - 10;
            dataGridView1.Height = statusStrip1.Location.Y - dataGridView1.Location.Y - 5 ;
        }
    }


    
}
