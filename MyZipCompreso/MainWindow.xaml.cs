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
        string Tipo_de_compresion = "null";

        string Lz78= "LZ78";
        string Lz77= "LZ77";
        string Huffman= "Huffman";
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnCompress_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void rbLZ78_Checked(object sender, RoutedEventArgs e)
        {
            this.Tipo_de_compresion = this.Lz78;
        }

        private void rbLZ77_Checked(object sender, RoutedEventArgs e)
        {
            this.Tipo_de_compresion = this.Lz77;
        }

        private void rbHuffman_Checked(object sender, RoutedEventArgs e)
        {
            this.Tipo_de_compresion = this.Huffman;
        }


    }
}