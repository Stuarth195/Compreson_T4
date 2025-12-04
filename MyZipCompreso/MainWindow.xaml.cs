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
using MyZipCompreso.Algoritmos;

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
        string[] archivos_seleccionados_global = null;
        public MainWindow()
        {
            InitializeComponent();
        }

        // 3. REEMPLAZA EL MÉTODO DEL BOTÓN COMPRESS (Comprimir)
        private void btnCompress_Click(object sender, RoutedEventArgs e)
        {
            // Validación: ¿Eligió algoritmo?
            if (this.Tipo_de_compresion == "null")
            {
                MessageBox.Show("Selecciona un algoritmo (LZ78).");
                return;
            }

            // Validación: ¿Usó el botón Browse antes?
            if (this.archivos_seleccionados_global == null || this.archivos_seleccionados_global.Length == 0)
            {
                MessageBox.Show("Primero selecciona archivos con el botón de Búsqueda (Browse).");
                return;
            }

            // Ahora solo preguntamos dónde guardar
            Microsoft.Win32.SaveFileDialog dialogo_guardar = new Microsoft.Win32.SaveFileDialog();
            dialogo_guardar.FileName = "paquete.myzip";
            dialogo_guardar.Filter = "Archivo MyZip|*.myzip";

            if (dialogo_guardar.ShowDialog() == true)
            {
                try
                {
                    Ordena_Compresor mi_ordenador = new Ordena_Compresor();

                    // Usamos la variable global que llenó el otro botón
                    mi_ordenador.Cargar_Archivos_Para_Procesar(this.archivos_seleccionados_global);
                    mi_ordenador.Ejecutar_Compresion_Y_Empaquetado(this.Tipo_de_compresion);
                    mi_ordenador.Guardar_Archivo_Final(dialogo_guardar.FileName);

                    MessageBox.Show("¡Archivo comprimido guardado con éxito!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
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

        private void btnDecompress_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Funcionalidad de descompresión pendiente de implementar.");
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialogo_abrir = new Microsoft.Win32.OpenFileDialog();
            dialogo_abrir.Multiselect = true;
            dialogo_abrir.Filter = "Archivos de texto|*.txt|Todos los archivos|*.*";
            dialogo_abrir.Title = "Selecciona los archivos para comprimir";

            if (dialogo_abrir.ShowDialog() == true)
            {
                // Guardamos las rutas en la memoria para usarlas después
                this.archivos_seleccionados_global = dialogo_abrir.FileNames;
                MessageBox.Show($"Seleccionaste {archivos_seleccionados_global.Length} archivo(s).");
            }
        }

        private void btnHuffman_Click(object sender, RoutedEventArgs e)
        {
            this.Tipo_de_compresion = this.Huffman;
        }

        private void btnLZ77_Click(object sender, RoutedEventArgs e)
        {
            this.Tipo_de_compresion = this.Lz77;
        }

        private void btnLZ78_Click(object sender, RoutedEventArgs e)
        {
            this.Tipo_de_compresion = this.Lz78;
        }
    }
}



namespace MyZipCompreso.Algoritmos
{
    public class Ordena_Compresor
    {
        private List<Tuple<string, string>> lista_tuplas_archivos = new List<Tuple<string, string>>();
        private string paquete_final_formateado = "";

        // ---------------- MÉTODOS PÚBLICOS ----------------

        public void Cargar_Archivos_Para_Procesar(string[] rutas_archivos_origen)
        {
            Cargar_Archivos_Aux(rutas_archivos_origen);
        }

        public void Ejecutar_Compresion_Y_Empaquetado(string algoritmo_seleccionado)
        {
            Ejecutar_Compresion_Y_Empaquetado_Aux(algoritmo_seleccionado);
        }

        public void Guardar_Archivo_Final(string ruta_destino_paquete)
        {
            Guardar_Archivo_Final_Aux(ruta_destino_paquete);
        }

        public void Limpiar_Memoria()
        {
            Limpiar_Memoria_Aux();
        }

        // ---------------- MÉTODOS PRIVADOS ----------------

        private void Cargar_Archivos_Aux(string[] rutas_entrada)
        {
            lista_tuplas_archivos.Clear();

            if (rutas_entrada == null || rutas_entrada.Length == 0)
            {
                throw new Exception("No se han recibido rutas de archivos para cargar.");
            }

            foreach (string ruta_actual in rutas_entrada)
            {
                if (System.IO.File.Exists(ruta_actual))
                {
                    string nombre_archivo_solo = System.IO.Path.GetFileName(ruta_actual);
                    string contenido_texto_plano = System.IO.File.ReadAllText(ruta_actual);

                    Tuple<string, string> tupla_archivo = new Tuple<string, string>(nombre_archivo_solo, contenido_texto_plano);
                    lista_tuplas_archivos.Add(tupla_archivo);
                }
            }
        }

        private void Ejecutar_Compresion_Y_Empaquetado_Aux(string tipo_algoritmo)
        {
            // Validación de seguridad
            if (lista_tuplas_archivos.Count == 0)
            {
                throw new Exception("No hay archivos cargados. Usa el botón de búsqueda primero.");
            }

            paquete_final_formateado = "";

            // Instancias de los compresores (Aquí tus compañeros instanciarán los suyos)
            LZ78_Compresor compresor_lz78 = new LZ78_Compresor();

            List<Tuple<string, string>> lista_comprimida_temporal = new List<Tuple<string, string>>();

            // 1. Fase de Compresión (Iteramos cada archivo cargado)
            foreach (Tuple<string, string> archivo_actual in lista_tuplas_archivos)
            {
                string nombre = archivo_actual.Item1;
                string contenido_original = archivo_actual.Item2;
                string contenido_resultado_compr = "";

                

                if (tipo_algoritmo == "LZ78")
                {
                    
                    compresor_lz78.LimpiarMemoria();
                    compresor_lz78.Comprimir(contenido_original);
                    contenido_resultado_compr = compresor_lz78.ObtenerResultadoString();
                }
                else if (tipo_algoritmo == "LZ77")
                {
               
                    contenido_resultado_compr = "PENDIENTE_IMPLEMENTACION_LZ77";
                }
                else if (tipo_algoritmo == "Huffman")
                {
                 

                    contenido_resultado_compr = "PENDIENTE_IMPLEMENTACION_HUFFMAN";
                }
                else
                {
                    throw new Exception("Algoritmo no reconocido o no seleccionado.");
                }

                
                lista_comprimida_temporal.Add(new Tuple<string, string>(nombre, contenido_resultado_compr));
            }

            // 2. Fase de Formateo Final (Empaquetado)
            foreach (Tuple<string, string> archivo_procesado in lista_comprimida_temporal)
            {
                string nombre_final = archivo_procesado.Item1;
                string data_final = archivo_procesado.Item2;

                paquete_final_formateado += "||" + nombre_final + "||";
                paquete_final_formateado += "_{" + data_final + "}_";
            }
        }

        private void Guardar_Archivo_Final_Aux(string ruta_guardado)
        {
            if (string.IsNullOrEmpty(paquete_final_formateado))
            {
                throw new Exception("Error: El paquete está vacío. Revisa si seleccionaste el algoritmo correcto.");
            }

            System.IO.File.WriteAllText(ruta_guardado, paquete_final_formateado);
        }

        private void Limpiar_Memoria_Aux()
        {
            lista_tuplas_archivos.Clear();
            paquete_final_formateado = "";
        }
    }
}