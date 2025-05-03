using Inmobiliaria_.Net_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using InmobiliariaApp.Repositories;
using InmobiliariaApp.Models;

namespace Inmobiliaria_.Net_Core.Controllers
{
    // [Authorize] comento porque sino no anda
    public class ImagenesController : Controller
    {
        private readonly ImagenRepository _imagenRepositorio;
        private readonly IWebHostEnvironment _environment;

        public ImagenesController(ImagenRepository repositorio, IWebHostEnvironment environment)
        {
            _imagenRepositorio = repositorio;
            _environment = environment;
        }

        // Acción para mostrar todas las imágenes
        public IActionResult Listar()
        {
            var imagenes = _imagenRepositorio.ObtenerTodos();
            return View(imagenes);
        }

        // Acción para mostrar imágenes por inmueble
        public IActionResult PorInmueble(int id)
        {
            var imagenes = _imagenRepositorio.BuscarPorInmueble(id);
            return View(imagenes);
        }

        // Acción para mostrar detalles de una imagen
        [Authorize]
        public IActionResult Detalles(int id)
        {
            var imagen = _imagenRepositorio.ObtenerPorId(id);
            if (imagen == null)
                return NotFound();

            return View(imagen);
        }

        // Acción para mostrar formulario de subida de imágenes
        [Authorize]
        public IActionResult Subir(int id)
        {
            ViewBag.InmuebleId = id;
            return View();
        }

        // Acción para procesar la subida de imágenes
        [HttpPost][Authorize]
        public async Task<IActionResult> Subir(int id, List<IFormFile> imagenes)
        {
            try
            {
                Console.WriteLine($"➡️ Subida iniciada para inmueble ID: {id}");

                if (imagenes == null || imagenes.Count == 0)
                {
                    Console.WriteLine("⚠️ No se recibieron archivos");
                    return BadRequest("No se recibieron archivos.");
                }

                Console.WriteLine($"📦 Cantidad de archivos recibidos: {imagenes.Count}");

                string ruta = Path.Combine(_environment.WebRootPath, "Uploads", "Inmuebles", id.ToString());
                Console.WriteLine($"📁 Ruta destino: {ruta}");

                if (!Directory.Exists(ruta))
                {
                    Directory.CreateDirectory(ruta);
                    Console.WriteLine("📂 Carpeta creada");
                }

                foreach (var file in imagenes)
                {
                    Console.WriteLine($"🖼 Procesando archivo: {file.FileName} ({file.Length} bytes)");

                    if (file.Length > 0)
                    {
                        var extension = Path.GetExtension(file.FileName);
                        var nombreArchivo = $"{Guid.NewGuid()}{extension}";
                        var rutaArchivo = Path.Combine(ruta, nombreArchivo);

                        Console.WriteLine($"📄 Guardando como: {rutaArchivo}");

                        using (var stream = new FileStream(rutaArchivo, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var imagen = new Imagen
                        {
                            InmuebleId = id,
                            Url = $"/Uploads/Inmuebles/{id}/{nombreArchivo}"
                        };

                        Console.WriteLine($"✅ Imagen guardada: {imagen.Url}");

                        _imagenRepositorio.Alta(imagen);
                    }
                    else
                    {
                        Console.WriteLine("⚠️ Archivo vacío ignorado");
                    }
                }

                var imagenesActuales = _imagenRepositorio.BuscarPorInmueble(id);
                Console.WriteLine($"📸 Total imágenes tras subir: {imagenesActuales.Count}");

                return Json(imagenesActuales);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en subida: {ex.Message}");
                return StatusCode(500, "Error interno del servidor: " + ex.Message);
            }
        }

        // Acción para mostrar formulario de edición
        [Authorize]
        public IActionResult Editar(int id)
        {
            var imagen = _imagenRepositorio.ObtenerPorId(id);
            if (imagen == null)
                return NotFound();

            return View(imagen);
        }

        // Acción para procesar edición de imagen
        [HttpPost][Authorize]
        public IActionResult Editar(int id, Imagen imagen)
        {
            if (id != imagen.IdImagen)
                return BadRequest();

            if (ModelState.IsValid)
            {
                _imagenRepositorio.Modificacion(imagen);
                return RedirectToAction("PorInmueble", new { id = imagen.InmuebleId });
            }

            return View(imagen);
        }

        // Acción para confirmar eliminación
        [Authorize(Policy = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var imagen = _imagenRepositorio.ObtenerPorId(id);
            if (imagen == null)
                return NotFound();

            return View(imagen);
        }

        // Acción para procesar eliminación
        [HttpPost, ActionName("Eliminar")][Authorize(Policy = "Administrador")]
        // [Authorize(Policy = "Administrador")] se comenta porque no anda
        public IActionResult EliminarConfirmado(int id)
        {
            var imagen = _imagenRepositorio.ObtenerPorId(id);
            if (imagen == null) return NotFound();

            try
            {
                var rutaFisica = Path.Combine(_environment.WebRootPath, imagen.Url.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                if (System.IO.File.Exists(rutaFisica))
                {
                    System.IO.File.Delete(rutaFisica);
                }

                _imagenRepositorio.Baja(id);


                // Devolver la nueva lista de imágenes del inmueble
                var imagenes = _imagenRepositorio.BuscarPorInmueble(imagen.InmuebleId);
                return Json(imagenes.Select(i => new { id = i.IdImagen, url = i.Url }));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
