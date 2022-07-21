using CG.Web.MegaApiClient;
using System.Threading.Tasks;

namespace src
{
    public partial class MegaSubidas : Form
    {
        private Thread t;
        private string cuenta = "cuenta@dominio.com";
        private string contra = "******************";
        private string nomCar = "carpeta";

        public MegaSubidas()
        {
            InitializeComponent();
            t = new Thread(subirArchivoAMega);
            t.Start();
        }

        private void subirArchivoAMega()
        {
            try
            {
                // Actualizar el label para informar al usuario.
                txtInfo.Invoke(new MethodInvoker(delegate { txtInfo.Text = "Conectando como " + cuenta; }));
                // Aumentar la barra de progreso.
                barraProgreso.Invoke(new MethodInvoker(delegate { barraProgreso.PerformStep(); }));
                // Instancia de un cliente para conectar con mega.
                MegaApiClient cliente = new MegaApiClient();
                // Inicio de sesión con el cliente, pasando el correo y la contraseña de la cuenta mega a la
                // que se sube el archivo.
                cliente.Login(cuenta, contra);
                // Aumentar la barra de progreso.
                barraProgreso.Invoke(new MethodInvoker(delegate { barraProgreso.PerformStep(); }));
                // Actualizar el label para informar al usuario.
                txtInfo.Invoke(new MethodInvoker(delegate { txtInfo.Text = "Obteniendo directorios..."; }));
                // Obtenemos los nodos (directorios/archivos) de la cuenta dentro de una variable.
                var nodos = cliente.GetNodes();
                // Actualizar el label para informar al usuario.
                txtInfo.Invoke(new MethodInvoker(delegate { txtInfo.Text = "Buscando carpeta " + nomCar + "..."; }));
                // Comprobar si existe algún nodo (directorio) que se llame "nomCar" (en mi caso quiero subir el
                // archivo a dicha carpeta).
                bool existe = cliente.GetNodes().Any(n => n.Name == nomCar);

                // Crear dos nodos.
                INode root;
                INode carpeta;

                // Si el directorio nomCar existe, se obtiene. Si no existe, se crea.
                if (existe == true)
                {
                    // Actualizar el label para informar al usuario.
                    txtInfo.Invoke(new MethodInvoker(delegate { txtInfo.Text = "Obteniendo la carpeta " + nomCar + "...."; }));
                    // Aumentar la barra de progreso.
                    barraProgreso.Invoke(new MethodInvoker(delegate { barraProgreso.PerformStep(); }));
                    // Obtenemos el directorio.
                    carpeta = nodos.Single(n => n.Name == nomCar);
                }
                else
                {
                    // Actualizar label para informar al usuario.
                    txtInfo.Invoke(new MethodInvoker(delegate { txtInfo.Text = "Creando carpeta " + nomCar + "..."; }));
                    // Aumentar la barra de progreso.
                    barraProgreso.Invoke(new MethodInvoker(delegate { barraProgreso.PerformStep(); }));
                    // Obtenemos el nodo raíz.
                    root = nodos.Single(n => n.Type == NodeType.Root);
                    // Creamos el directorio llamado "nomCar" en la raíz.
                    carpeta = cliente.CreateFolder(nomCar, root);
                }

                // Aumentar la barra de progreso.
                barraProgreso.Invoke(new MethodInvoker(delegate { barraProgreso.PerformStep(); }));
                // Actualizar label para informar al usuario.
                txtInfo.Invoke(new MethodInvoker(delegate { txtInfo.Text = "Subiendo archivo..."; }));
                // Aumentar la barra de progreso.
                barraProgreso.Invoke(new MethodInvoker(delegate { barraProgreso.PerformStep(); }));
                // Subimos el archivo al directorio "nomCar", pasando la ruta del archivo a subir
                // y el directorio de mega donde debe subirlo.
                INode archivo = cliente.UploadFile(@"ruta/del/archivo.png", carpeta);
                // Obtener el link de descarga del archivo subido por si se requiere para algo.
                Uri downloadUrl = cliente.GetDownloadLink(archivo);
                // Actualizar label para informar al usuario.
                txtInfo.Invoke(new MethodInvoker(delegate { txtInfo.Text = "Archivo subido con éxito."; }));
            }
            catch (Exception error)
            {
                // Mensaje en pantalla para informar al usuario del error.
                MessageBox.Show("Error al intentar subir el archivo. " + error.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // No se puede cerrar el form desde un subproceso ya que no es desde donde se ha creado.
            // Con esta llamada podemos cerrarlo.
            this.Invoke((MethodInvoker)delegate {this.Close();});
        }
    }
}