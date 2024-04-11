using System;
using System.IO;
using System.Windows.Forms;

namespace MinfPatcher
{
    public partial class Form1 : Form
    {
        private string filePath;
        private const int offset1 = 0x4C;
        private const int offset2 = 0x50;
        private const int dataSize = 4;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                FileName = "Select Your Mtninf",
                Filter = "Mtninf files (*.mtninf)|*.mtninf",
                Title = "Open Mtninf"
            };

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = fileDialog.FileName;
                try
                {
                    // Read the selected file
                    byte[] fileBytes = File.ReadAllBytes(filePath);

                    // Check if the file is long enough to read from offsets
                    if (fileBytes.Length >= (offset2 + dataSize))
                    {
                        UpdateTextBoxes(fileBytes);
                    }
                    else
                    {
                        MessageBox.Show("File is too short to read bytes from the specified range.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error reading file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void UpdateTextBoxes(byte[] fileBytes)
        {
            // Extract bytes from offset1 to offset1 + dataSize
            byte[] extractedBytes1 = new byte[dataSize];
            Array.Copy(fileBytes, offset1, extractedBytes1, 0, dataSize);

            // Display the bytes in textBox1
            textBox1.TextChanged -= textBox1_TextChanged; // Remove event handler to prevent infinite loop
            textBox1.Text = BitConverter.ToInt32(extractedBytes1, 0).ToString();
            textBox1.TextChanged += textBox1_TextChanged;

            // Extract bytes from offset2 to offset2 + dataSize
            byte[] extractedBytes2 = new byte[dataSize];
            Array.Copy(fileBytes, offset2, extractedBytes2, 0, dataSize);

            // Display the bytes in textBox2
            textBox2.TextChanged -= textBox2_TextChanged; // Remove event handler to prevent infinite loop
            textBox2.Text = BitConverter.ToInt32(extractedBytes2, 0).ToString();
            textBox2.TextChanged += textBox2_TextChanged;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int intValue;
            if (int.TryParse(textBox1.Text, out intValue))
            {
                byte[] bytes = BitConverter.GetBytes(intValue);
                WriteBytesToFile(bytes, offset1);
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            int intValue;
            if (int.TryParse(textBox2.Text, out intValue))
            {
                byte[] bytes = BitConverter.GetBytes(intValue);
                WriteBytesToFile(bytes, offset2);
            }
        }

        private void WriteBytesToFile(byte[] bytes, int offset)
        {
            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Write))
                {
                    fileStream.Seek(offset, SeekOrigin.Begin);
                    fileStream.Write(bytes, 0, dataSize);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error writing file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
