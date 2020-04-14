using System;
using System.Windows.Forms;

namespace Snake
{
    public delegate void setValue(int width, int height, int speed);

    public partial class other : Form
    {
        public event setValue setformValue;

        public other()
        {
            InitializeComponent();
        }

        private void other_FormClosing(object sender, FormClosingEventArgs e)
        {
            wirte();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            wirte();
            Close();
        }

        private void wirte()
        {
            setformValue(int.Parse(textBox1.Text), int.Parse(textBox2.Text), int.Parse(textBox3.Text));
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b' && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b' && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b' && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        public void other_Load(object sender, EventArgs e)
        {
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (int.Parse(textBox1.Text) < 10) textBox1.Text = "10";
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (int.Parse(textBox2.Text) < 6) textBox2.Text = "6";
        }

        private void textBox3_Leave(object sender, EventArgs e)
        {
            if (int.Parse(textBox3.Text) < 1) textBox3.Text = "1";
        }
    }
}
