using Microsoft.Win32;
using MyZipCompreso.Algoritmos;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyZipCompreso
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                txtFilePath.Text = openFileDialog.FileName;
            }
        }

        private void btnCompress_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtFilePath.Text) || txtFilePath.Text == "Seleccione un archivo...")
            {
                MessageBox.Show("Por favor seleccione un archivo primero.");
                return;
            }

            if (rbHuffman.IsChecked == true)
            {
                try
                {
                    string inputFile = txtFilePath.Text;
                    string outputFile = System.IO.Path.ChangeExtension(inputFile, ".myzip");

                    Stopwatch sw = new Stopwatch();
                    long memoryBefore = GC.GetTotalMemory(true);
                    
                    sw.Start();
                    Huffman_C huffman = new Huffman_C();
                    huffman.ComprimirADisco(inputFile, outputFile);
                    sw.Stop();

                    long memoryAfter = GC.GetTotalMemory(true);
                    long memoryUsed = memoryAfter - memoryBefore;

                    lblTime.Text = $"{sw.ElapsedMilliseconds} ms";
                    lblMemory.Text = $"{memoryUsed / 1024} KB";

                    long originalSize = new FileInfo(inputFile).Length;
                    long compressedSize = new FileInfo(outputFile).Length;
                    double compressionRate = (1.0 - ((double)compressedSize / originalSize)) * 100;
                    lblCompressionRate.Text = $"{compressionRate:F2} %";

                    MessageBox.Show($"Archivo comprimido exitosamente en: {outputFile}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al comprimir: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Algoritmo no implementado aún.");
            }
        }

        private void btnDecompress_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtFilePath.Text) || txtFilePath.Text == "Seleccione un archivo...")
            {
                MessageBox.Show("Por favor seleccione un archivo primero.");
                return;
            }

            if (rbHuffman.IsChecked == true)
            {
                try
                {
                    string inputFile = txtFilePath.Text;
                    // Assuming original was .txt for now, or we could append .decoded
                    string outputFile = inputFile + ".decoded.txt"; 

                    Stopwatch sw = new Stopwatch();
                    long memoryBefore = GC.GetTotalMemory(true);

                    sw.Start();
                    Huffman_Dc huffman = new Huffman_Dc();
                    huffman.DescomprimirDesdeDisco(inputFile, outputFile);
                    sw.Stop();

                    long memoryAfter = GC.GetTotalMemory(true);
                    long memoryUsed = memoryAfter - memoryBefore;

                    lblTime.Text = $"{sw.ElapsedMilliseconds} ms";
                    lblMemory.Text = $"{memoryUsed / 1024} KB";
                    lblCompressionRate.Text = "N/A"; // No compression rate for decompression

                    MessageBox.Show($"Archivo descomprimido exitosamente en: {outputFile}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al descomprimir: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Algoritmo no implementado aún.");
            }
        }
    }
}