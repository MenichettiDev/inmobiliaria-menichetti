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
    [Authorize]
    public class ImagenesController : Controller
    {
        private readonly ImagenRepository _repositorio;
        private readonly IWebHostEnvironment _environment;

        public ImagenesController(ImagenRepository repositorio, IWebHostEnvironment environment)
        {
            _repositorio = repositorio;
            _environment = environment;
        }

        // Acción para mostrar todas las imágenes
        public IActionResult Listar()
        {
            var imagenes = _repositorio.ObtenerTodos();
            return View(imagenes);
        }

        // Acción para mostrar imágenes por inmueble
        public IActionResult PorInmueble(int id)
        {
            var imagenes = _repositorio.BuscarPorInmueble(id);
            return View(imagenes);
        }

        // Acción para mostrar detalles de una imagen
        public IActionResult Detalles(int id)
        {
            var imagen = _repositorio.ObtenerPorId(id);
            if (imagen == null)
                return NotFound();

            return View(imagen);
        }

        // Acción para mostrar formulario de subida de imágenes
        public IActionResult Subir(int id)
        {
            ViewBag.InmuebleId = id;
            return View();
        }

        // Acción para procesar la subida de imágenes
        [HttpPost]
        public async Task<IActionResult> Subir(int id, List<IFormFile> imagenes)
        {
            if (imagenes == null || imagenes.Count == 0)
                return BadRequest("No se recibieron archivos.");

            string ruta = Path.Combine(_environment.WebRootPath, "Uploads", "Inmuebles", id.ToString());
            if (!Directory.Exists(ruta))
                Directory.CreateDirectory(ruta);

            foreach (var file in imagenes)
            {
                if (file.Length > 0)
                {
                    var extension = Path.GetExtension(file.FileName);
                    var nombreArchivo = $"{Guid.NewGuid()}{extension}";
                    var rutaArchivo = Path.Combine(ruta, nombreArchivo);

                    using (var stream = new FileStream(rutaArchivo, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var imagen = new Imagen
                    {
                        InmuebleId = id,
                        Url = $"/Uploads/Inmuebles/{id}/{nombreArchivo}"
                    };

                    _repositorio.Alta(imagen);
                }
            }

            return RedirectToAction("PorInmueble", new { id });
        }

        // Acción para mostrar formulario de edición
        public IActionResult Editar(int id)
        {
            var imagen = _repositorio.ObtenerPorId(id);
            if (imagen == null)
                return NotFound();

            return View(imagen);
        }

        // Acción para procesar edición de imagen
        [HttpPost]
        public IActionResult Editar(int id, Imagen imagen)
        {
            if (id != imagen.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                _repositorio.Modificacion(imagen);
                return RedirectToAction("PorInmueble", new { id = imagen.InmuebleId });
            }

            return View(imagen);
        }

        // Acción para confirmar eliminación
        public IActionResult Eliminar(int id)
        {
            var imagen = _repositorio.ObtenerPorId(id);
            if (imagen == null)
                return NotFound();

            return View(imagen);
        }

        // Acción para procesar eliminación
        [HttpPost, ActionName("Eliminar")]
        [Authorize(Policy = "Administrador")]
        public IActionResult EliminarConfirmado(int id)
        {
            var imagen = _repositorio.ObtenerPorId(id);
            if (imagen == null)
                return NotFound();

            try
            {
                var rutaFisica = Path.Combine(_environment.WebRootPath, imagen.Url.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                if (System.IO.File.Exists(rutaFisica))
                {
                    System.IO.File.Delete(rutaFisica);
                }

                _repositorio.Baja(id);
                return RedirectToAction("PorInmueble", new { id = imagen.InmuebleId });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
