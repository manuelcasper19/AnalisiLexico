using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IO;

namespace JHeBra.API.Controllers
{
    [ApiController]
    [Route("/api/lexico")]
    public class LexicoController : ControllerBase
    {

        [HttpGet]
        public IActionResult GetArchivos()
        {
            //obtenemos ruta
            string rutaCarpeta = Path.Combine(Directory.GetCurrentDirectory(), "ArchivosJHeBra");
            try
            {
                //listamos todos los archivos con extensión .jhebra
                string[] archivos = Directory.GetFiles(rutaCarpeta, "*.jhebra");
                var nombresArchivos = archivos.Select(Path.GetFileName);
                return Ok(nombresArchivos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("archivo/{fileName}")]
        public IActionResult GetArchivo(string fileName)
        {
            string rutaCarpeta = Path.Combine(Directory.GetCurrentDirectory(), "ArchivosJHeBra");
            string rutaArchivo = Path.Combine(rutaCarpeta, fileName);
            try
            {
                //obtenemos todo el contenido del archivo
                string contenido = System.IO.File.ReadAllText(rutaArchivo);
                return Ok(contenido);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
          

        [HttpPost("sobrescribir/{fileName}")]
        public IActionResult SobrescribirArchivo([FromBody] string contenido, string fileName)
        {
    
            try
            {          
                string rutaArchivo = Path.Combine(Directory.GetCurrentDirectory(), "ArchivosJHeBra", fileName);
                System.IO.File.WriteAllText(rutaArchivo, contenido);

                //
                using (Process proceso = new Process())
                {
                    proceso.StartInfo.FileName = "JHeBra.exe";  //nombre de archivo a ejecutar
                    proceso.StartInfo.Arguments = rutaArchivo; //ruta de archivo .jhebra que llega como parametro
                    proceso.StartInfo.RedirectStandardOutput = true; //redirecciona la salida
                    proceso.StartInfo.UseShellExecute = false; //no se utilizara la powerShell
                    proceso.StartInfo.CreateNoWindow = true; //no se ejecutara consola para mostrar resultado de analisis

                    proceso.Start();
                    proceso.WaitForExit(); //esperamos que el proceso finalice para continuar
                }


                // Lee el resultado del análisis desde el archivo temporal.txt
                string rutaResultado = Path.Combine(Directory.GetCurrentDirectory(), "temporal.txt");
                string[] lineas = System.IO.File.ReadAllLines(rutaResultado);
                //creamos una lista con el contenido del archivo
                List<string> listaResultado = new List<string>(lineas);
              

                return Ok(listaResultado);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("archivo/crearnuevo")]
        public IActionResult CrearNuevoArchivoConFormato([FromBody] string nombreArchivo)
        {
            try
            {
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "ArchivosJHeBra");
                string[] files = Directory.GetFiles(folderPath, "*.jhebra");
                var fileNames = files.Select(Path.GetFileName);
                string nuevoNombreArchivo = $"{nombreArchivo}.jhebra";
                if(fileNames != null)
                {
                    foreach (string file in fileNames)
                    {
                        if (file == nuevoNombreArchivo)
                        {
                            return Conflict("El archivo ya existe");
                        }
                    }
                }

                // Generar el contenido con el formato deseado
                string contenido = $"_publico _clase {nombreArchivo} {{\n}}";

                // Generar un nombre único para el archivo
               

                // Obtener la ruta completa del archivo
                string rutaCompleta = Path.Combine(Directory.GetCurrentDirectory(), "ArchivosJHeBra", nuevoNombreArchivo);

                // Escribir el contenido en el nuevo archivo
                System.IO.File.WriteAllText(rutaCompleta, contenido);

                return Ok(nuevoNombreArchivo);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensaje = "Error al crear el archivo", Error = ex.Message });
            }
        }

        [HttpDelete("archivo/eliminar/{nombreArchivo}")]
        public IActionResult EliminarArchivo(string nombreArchivo)
        {
            try
            {
                string rutaArchivo = Path.Combine(Directory.GetCurrentDirectory(), "ArchivosJHeBra", nombreArchivo);

                if (System.IO.File.Exists(rutaArchivo))
                {
                    System.IO.File.Delete(rutaArchivo);
                    return Ok($"El archivo {nombreArchivo} ha sido eliminado.");
                }
                else
                {
                    return NotFound($"No se encontró el archivo {nombreArchivo}.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al eliminar el archivo: {ex.Message}");
            }
        }

    }


    }
