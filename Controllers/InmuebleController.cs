using Microsoft.AspNetCore.Mvc;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using Inmobiliaria_.Net_Core.Models;
using Microsoft.AspNetCore.Authorization;

namespace InmobiliariaApp.Controllers
{
    public class InmuebleController : Controller
    {
        private readonly InmuebleRepository _inmuebleRepository;
        private readonly PropietarioRepository _propietarioRepository;
        private readonly TipoInmuebleRepository _tipoInmuebleRepository;
        private readonly UsuarioRepository _usuarioRepository;

        public InmuebleController(InmuebleRepository inmuebleRepository, PropietarioRepository propietarioRepository, TipoInmuebleRepository tipoInmuebleRepository, UsuarioRepository usuarioRepository)
        {
            _inmuebleRepository = inmuebleRepository;
            _propietarioRepository = propietarioRepository;
            _tipoInmuebleRepository = tipoInmuebleRepository;
            _usuarioRepository = usuarioRepository;
        }


        public IActionResult Listar(string? uso, int? ambientes, decimal? precioDesde, decimal? precioHasta, string estado, int? activo, int page = 1, int pageSize = 10)
        {
            if (!Request.Query.ContainsKey("activo"))
                activo = 1;
            if (!Request.Query.ContainsKey("estado"))
                estado = "Disponible";

            var inmuebles = _inmuebleRepository.ObtenerFiltrados(uso, ambientes, precioDesde, precioHasta, estado, activo, page, pageSize, out int totalItems);

            int totalPaginas = (int)Math.Ceiling((double)totalItems / pageSize);

            ViewBag.PaginaActual = page;
            ViewBag.TotalPaginas = totalPaginas;

            return View(inmuebles);
        }


        // Acción para mostrar detalles de un inmueble
        public IActionResult Detalles(int id)
        {

            var inmueble = _inmuebleRepository.GetById(id);
            if (inmueble == null)
            {
                return NotFound(); // Retorna un error 404 si no se encuentra el inmueble
            }

            return View(inmueble);
        }

        // Acción para mostrar el formulario de creación
        public IActionResult Insertar()
        {
            var propietarios = _propietarioRepository.GetAll(); // O el método que uses
            ViewData["Propietarios"] = new SelectList(propietarios, "IdPropietario", "NombreCompleto");
            var tiposInmueble = _tipoInmuebleRepository.GetAll(); // O el método que uses
            ViewData["TipoInmueble"] = new SelectList(tiposInmueble, "IdTipoInmueble", "Nombre");
            return View();
        }

        // Acción para procesar el formulario de creación
        [HttpPost]
        public IActionResult Insertar(Inmueble inmueble)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _inmuebleRepository.Add(inmueble);
                    TempData["SuccessMessage"] = "Inmueble creado correctamente.";
                    return RedirectToAction("Listar"); // Redirige a la lista de inmuebles
                }

                var propietarios = _propietarioRepository.GetAll(); // O el método que uses
                ViewData["Propietarios"] = new SelectList(propietarios, "IdPropietario", "NombreCompleto");
                var tiposInmueble = _tipoInmuebleRepository.GetAll(); // O el método que uses
                ViewData["TipoInmueble"] = new SelectList(tiposInmueble, "IdTipoInmueble", "Nombre");

                return View(inmueble);

            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Ocurrió un error al crear el Inmueble: {ex.Message}";

                var propietarios = _propietarioRepository.GetAll(); // O el método que uses
                ViewData["Propietarios"] = new SelectList(propietarios, "IdPropietario", "NombreCompleto");
                var tiposInmueble = _tipoInmuebleRepository.GetAll(); // O el método que uses
                ViewData["TipoInmueble"] = new SelectList(tiposInmueble, "IdTipoInmueble", "Nombre");

                return View("Insertar", inmueble);
            }
        }

        // Acción para mostrar el formulario de edición
        public IActionResult Editar(int id)
        {
            var inmueble = _inmuebleRepository.GetById(id);

            var propietarios = _propietarioRepository.GetAll(); // O el método que uses
            ViewData["Propietarios"] = new SelectList(propietarios, "IdPropietario", "NombreCompleto");

            var tiposInmueble = _tipoInmuebleRepository.GetAll(); // O el método que uses
            ViewData["TipoInmueble"] = new SelectList(tiposInmueble, "IdTipoInmueble", "Nombre");

            if (inmueble == null)
            {
                return NotFound(); // Retorna un error 404 si no se encuentra el inmueble
            }
            return View(inmueble);
        }

        // Acción para procesar el formulario de edición
        [HttpPost]
        public IActionResult Editar(int id, Inmueble inmueble)
        {

            try
            {
                if (id != inmueble.IdInmueble)
                {
                    return BadRequest(); // Retorna un error 400 si los IDs no coinciden
                }

                if (ModelState.IsValid)
                {
                    _inmuebleRepository.Update(inmueble);
                    TempData["SuccessMessage"] = "Inmueble modificado correctamente.";
                    return RedirectToAction("Listar"); // Redirige a la lista de inmuebles
                }
                return View(inmueble);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Ocurrió un error al editar el Inmueble: {ex.Message}";

                var propietarios = _propietarioRepository.GetAll(); // O el método que uses
                ViewData["Propietarios"] = new SelectList(propietarios, "IdPropietario", "NombreCompleto");
                var tiposInmueble = _tipoInmuebleRepository.GetAll(); // O el método que uses
                ViewData["TipoInmueble"] = new SelectList(tiposInmueble, "IdTipoInmueble", "Nombre");

                return View("Editar", inmueble);
            }
        }


        // Acción para mostrar la vista de confirmación de eliminación
        [Authorize(Policy = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var inmueble = _inmuebleRepository.GetById(id);
            if (inmueble == null)
            {
                return NotFound(); // Retorna un error 404 si no se encuentra el inmueble
            }
            return View(inmueble);
        }

        // Acción para confirmar la eliminación
        [HttpPost, ActionName("Delete")]
        [Authorize(Policy = "Administrador")]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _inmuebleRepository.Delete(id);
                TempData["SuccessMessage"] = "Inmueble eliminado correctamente.";
                return RedirectToAction("Listar");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Ocurrió un error al eliminar el inmueble: {ex.Message}";

                var inmueble = _inmuebleRepository.GetById(id); // Volver a cargar el contrato para mostrar la vista
                return View("Eliminar", inmueble); // Mostramos la misma vista de confirmación con el error
            }
        }


        // // Acción para suspender el inmueble
        [HttpPost]
        [Authorize(Policy = "Administrador")]
        public IActionResult BajaLogica(int id)
        {
            try
            {
                var inmueble = _inmuebleRepository.GetById(id);
                if (inmueble == null)
                {
                    return NotFound(); // Retorna un error 404 si no se encuentra el inmueble
                }

                _inmuebleRepository.bajaLogica(inmueble);
                TempData["SuccessMessage"] = "Inmueble dado de baja correctamente.";

                return RedirectToAction("Listar"); // Redirige a la lista de inmuebles
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Ocurrió un error al dar de baja el inmueble: {ex.Message}";

                var inmueble = _inmuebleRepository.GetById(id); // Volver a cargar el contrato para mostrar la vista
                return View("Eliminar", inmueble); // Mostramos la misma vista de confirmación con el error
            }
        }
        // Acción para activar el inmueble
        [HttpPost]
        [Authorize(Policy = "Administrador")]
        public IActionResult AltaLogica(int id)
        {
            try
            {
                var inmueble = _inmuebleRepository.GetById(id);
                if (inmueble == null)
                {
                    return NotFound(); // Retorna un error 404 si no se encuentra el inmueble
                }

                _inmuebleRepository.altaLogica(inmueble);
                TempData["SuccessMessage"] = "Inmueble reactivado correctamente.";

                return RedirectToAction("Listar"); // Redirige a la lista de inmuebles
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Ocurrió un error al reactivar el inmueble: {ex.Message}";

                var inmueble = _inmuebleRepository.GetById(id); // Volver a cargar el contrato para mostrar la vista
                return View("Eliminar", inmueble); // Mostramos la misma vista de confirmación con el error
            }
        }



        // GET: Inmueble/Imagenes/5
        public ActionResult Imagenes(int id, [FromServices] ImagenRepository _imagenRepository)
        {
            var inmueble = _inmuebleRepository.GetById(id);
            if (inmueble == null) return NotFound();
            inmueble.Imagenes = _imagenRepository.BuscarPorInmueble(id);
            return View(inmueble);
        }

        // POST: Inmueble/Portada
        [HttpPost]
        // [ValidateAntiForgeryToken]
        public ActionResult Portada(Imagen entidad, [FromServices] IWebHostEnvironment environment)
        {
            try
            {
                Console.WriteLine($"[DEBUG] Ingresó a Portada - InmuebleId: {entidad.InmuebleId}");

                // Recuperar el inmueble
                var inmueble = _inmuebleRepository.GetById(entidad.InmuebleId);
                if (inmueble != null)
                {
                    Console.WriteLine($"[DEBUG] Inmueble encontrado: ID = {inmueble.IdInmueble}");
                    if (inmueble.Portada != null)
                    {
                        string rutaEliminar = Path.Combine(environment.WebRootPath, "Uploads", "Inmuebles", Path.GetFileName(inmueble.Portada));
                        Console.WriteLine($"[DEBUG] Intentando eliminar portada existente en: {rutaEliminar}");

                        if (System.IO.File.Exists(rutaEliminar))
                        {
                            System.IO.File.Delete(rutaEliminar);
                            Console.WriteLine("[DEBUG] Imagen anterior eliminada correctamente.");
                        }
                        else
                        {
                            Console.WriteLine("[DEBUG] La imagen anterior no existía.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("[ERROR] Inmueble no encontrado.");
                }

                if (entidad.Archivo != null)
                {
                    Console.WriteLine($"[DEBUG] Archivo recibido: {entidad.Archivo.FileName}");

                    string wwwPath = environment.WebRootPath;
                    string path = Path.Combine(wwwPath, "Uploads", "Inmuebles");

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                        Console.WriteLine($"[DEBUG] Carpeta creada: {path}");
                    }

                    string fileName = "portada_" + entidad.InmuebleId + Path.GetExtension(entidad.Archivo.FileName);
                    string rutaFisicaCompleta = Path.Combine(path, fileName);
                    Console.WriteLine($"[DEBUG] Ruta física para guardar: {rutaFisicaCompleta}");

                    using (var stream = new FileStream(rutaFisicaCompleta, FileMode.Create))
                    {
                        entidad.Archivo.CopyTo(stream);
                        Console.WriteLine("[DEBUG] Archivo copiado exitosamente.");
                    }

                    entidad.Url = Path.Combine("/Uploads/Inmuebles", fileName);
                }
                else
                {
                    Console.WriteLine("[DEBUG] No se recibió archivo. Se eliminará la portada.");
                    entidad.Url = string.Empty;
                }

                _inmuebleRepository.ModificarPortada(entidad.InmuebleId, entidad.Url);
                Console.WriteLine("[DEBUG] Portada actualizada en base de datos.");

                TempData["Mensaje"] = "Portada actualizada correctamente";
                return RedirectToAction(nameof(Imagenes), new { id = entidad.InmuebleId });

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Excepción atrapada: {ex.Message}");
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Imagenes), new { id = entidad.InmuebleId });
            }
        }

        private string ObtenerNombreUsuario(int? idUsuario)
        {
            if (!idUsuario.HasValue) return "---";

            var usuario = _usuarioRepository.GetById(idUsuario.Value);
            return usuario != null ? $"{usuario.Email}" : "Desconocido";
        }
    }
}