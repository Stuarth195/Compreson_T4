using MyZipCompreso.Algoritmos;
using System.Text;
using System.Text.RegularExpressions;
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

       
        private void btnCompress_Click(object sender, RoutedEventArgs e)
        {
            
            if (this.Tipo_de_compresion == "null")
            {
                MessageBox.Show("Selecciona un algoritmo (LZ78).");
                return;
            }

            
            if (this.archivos_seleccionados_global == null || this.archivos_seleccionados_global.Length == 0)
            {
                MessageBox.Show("Primero selecciona archivos con el botón de Búsqueda (Browse).");
                return;
            }

            
            Microsoft.Win32.SaveFileDialog dialogo_guardar = new Microsoft.Win32.SaveFileDialog();
            dialogo_guardar.FileName = "paquete.myzip";
            dialogo_guardar.Filter = "Archivo MyZip|*.myzip";

            if (dialogo_guardar.ShowDialog() == true)
            {
                try
                {
                    Ordena_Compresor mi_ordenador = new Ordena_Compresor();

                    
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
            {
                // 1. Validaciones
                if (this.Tipo_de_compresion == "null")
                {
                    MessageBox.Show("Por favor, selecciona el algoritmo (ej. LZ78).");
                    return;
                }

                Microsoft.Win32.OpenFileDialog dialogo_abrir = new Microsoft.Win32.OpenFileDialog();
                dialogo_abrir.Filter = "Archivo MyZip|*.myzip";
                dialogo_abrir.Title = "Selecciona el archivo a descomprimir";

                if (dialogo_abrir.ShowDialog() == true)
                {
                    string ruta_archivo_zip = dialogo_abrir.FileName;

                    // Crear carpeta con el nombre del archivo + _EXTRAIDO
                    string carpeta_destino = System.IO.Path.Combine(
                        System.IO.Path.GetDirectoryName(ruta_archivo_zip),
                        System.IO.Path.GetFileNameWithoutExtension(ruta_archivo_zip) + "_EXTRAIDO"
                    );

                    try
                    {
                        if (!System.IO.Directory.Exists(carpeta_destino))
                        {
                            System.IO.Directory.CreateDirectory(carpeta_destino);
                        }

                        // === CAMBIO AQUÍ: Usamos la nueva clase separada ===
                        Ordena_Descompresor mi_desempaquetador = new Ordena_Descompresor();

                        mi_desempaquetador.Ejecutar_Desempaquetado(ruta_archivo_zip, carpeta_destino, this.Tipo_de_compresion);
                        // ===================================================

                        MessageBox.Show($"¡Descompresión exitosa!\nArchivos en: {carpeta_destino}");
                        System.Diagnostics.Process.Start("explorer.exe", carpeta_destino);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al descomprimir: " + ex.Message);
                    }
                }
            }
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
            
            if (lista_tuplas_archivos.Count == 0)
            {
                throw new Exception("No hay archivos cargados. Usa el botón de búsqueda primero.");
            }

            paquete_final_formateado = "";

            
            LZ78_Compresor compresor_lz78 = new LZ78_Compresor();

            List<Tuple<string, string>> lista_comprimida_temporal = new List<Tuple<string, string>>();

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

    public class Ordena_Descompresor
    {
        
        public void Ejecutar_Desempaquetado(string ruta_archivo_zip, string carpeta_destino, string algoritmo_seleccionado)
        {
            
            if (!System.IO.File.Exists(ruta_archivo_zip))
                throw new Exception("El archivo .myzip no existe.");

            string contenido_paquete_completo = System.IO.File.ReadAllText(ruta_archivo_zip);

            // 2. Regex para separar: ||nombre||_{contenido}_
            // Explicación: 
            // \|\|(.*?)       -> Captura el nombre
            // \|\|_\{(.*?)\}_ -> Captura el contenido comprimido
            string patron_regex = @"\|\|(.*?)\|\|_\{(.*?)\}_";

            MatchCollection coincidencias = Regex.Matches(contenido_paquete_completo, patron_regex, RegexOptions.Singleline);

            if (coincidencias.Count == 0)
            {
                throw new Exception("El archivo está vacío o no tiene el formato correcto de MyZip.");
            }

            
            LZ78_Descompresor lz78_descomp = new LZ78_Descompresor();

            
            foreach (Match match in coincidencias)
            {
                string nombre_original = match.Groups[1].Value;
                string contenido_comprimido = match.Groups[2].Value;
                string contenido_final_texto = "";

                
                if (algoritmo_seleccionado == "LZ78")
                {
                    lz78_descomp.LimpiarMemoria();
                    
                    lz78_descomp.DescomprimirDesdeCadena(contenido_comprimido);
                    contenido_final_texto = lz78_descomp.ObtenerResultadoString();
                }
                else if (algoritmo_seleccionado == "LZ77")
                {
                    contenido_final_texto = "PENDIENTE_LZ77";
                }
                else if (algoritmo_seleccionado == "Huffman")
                {
                    contenido_final_texto = "PENDIENTE_HUFFMAN";
                }
                else
                {
                    throw new Exception("Algoritmo desconocido.");
                }

                
                string ruta_final = System.IO.Path.Combine(carpeta_destino, nombre_original);
                System.IO.File.WriteAllText(ruta_final, contenido_final_texto);
            }
        }
    }
}

