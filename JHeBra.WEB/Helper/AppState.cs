namespace JHeBra.WEB.Helper
{
    //Usamos inyeccion de dependencia, para que la clase este disponible donde se necesite y poder conservar los valores
    public class AppState
    {
        public string valorTextArea { get; set; }
        public string archivoSeleccionado { get; private set; }
        public List<string> Archivos { get; private set; } = new List<string>();

        public event Action OnCambioValorTexto;
        public event Action OnNombreArchivoSeleccionadoCambio;
        public event Action OnArchivoCambio;
        public void AsignarValorTexto(string newValue, string selectedFileName)
        {
            valorTextArea = newValue;
            archivoSeleccionado = selectedFileName;
            OnCambioValorTexto?.Invoke();
            OnNombreArchivoSeleccionadoCambio?.Invoke();
        }

        public void AsignarArchivo(List<string> nuevosArchivos)
        {
            Archivos = nuevosArchivos;
            OnArchivoCambio?.Invoke();
        }
        public void AsignarArchivo(string nuevosArchivos)
        {
            Archivos.Add(nuevosArchivos);
            OnArchivoCambio?.Invoke();
        }
    }
}
