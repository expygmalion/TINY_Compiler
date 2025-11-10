using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JASON_Compiler
{
    public partial class TinyProgLang : Form
    {
        public TinyProgLang()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Clear previous results before compiling
            dataGridView1.Rows.Clear();
            textBox2.Clear();

            string Code = textBox1.Text;
            TINY_Compiler.Start_Compiling(Code);
            PrintTokens();
            PrintErrors();
        }
        void PrintTokens()
        {
            for (int i = 0; i < TINY_Compiler.Jason_Scanner.Tokens.Count; i++)
            {
               dataGridView1.Rows.Add(TINY_Compiler.Jason_Scanner.Tokens.ElementAt(i).lex, TINY_Compiler.Jason_Scanner.Tokens.ElementAt(i).token_type);
            }
        }

        void PrintErrors()
        {
            for(int i=0; i<Errors.Error_List.Count; i++)
            {
                textBox2.Text += Errors.Error_List[i];
                textBox2.Text += "\r\n";
            }
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Clear all UI elements
            textBox1.Clear();
            textBox2.Clear();
            dataGridView1.Rows.Clear();

            // Clear underlying data structures
            TINY_Compiler.TokenStream.Clear();
            Errors.Error_List.Clear();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
