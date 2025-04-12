using Microsoft.AspNetCore.Mvc;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InmobiliariaApp.Controllers
{
    public class InmuebleController : Controller
    {
        private readonly InmuebleRepository _inmuebleRepository;
        private readonly PropietarioRepository _propietarioRepository;
        private readonly TipoInmuebleRepository _tipoInmuebleRepository;

        public InmuebleController(InmuebleRepository inmuebleRepository, PropietarioRepository propietarioRepository, TipoInmuebleRepository tipoInmuebleRepository)
        {
            _inmuebleRepository = inmuebleRepository;
            _propietarioRepository = propietarioRepository;
            _tipoInmuebleRepository = tipoInmuebleRepository;
        }

        // Acción para listar todos los inmuebles
        public IActionResult Listar()
        {
            var inmuebles = _inmuebleRepository.GetAll();
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
            Console.WriteLine("Insertar Inmueble: " + inmueble.ToString());
            if (ModelState.IsValid)
            {
                _inmuebleRepository.Add(inmueble);
                return RedirectToAction("Listar"); // Redirige a la lista de inmuebles
            }
            return View(inmueble);
        }

        // Acción para mostrar el formulario de edición
        public IActionResult Editar(int id)
        {
            var inmueble = _inmuebleRepository.GetById(id);
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
            if (id != inmueble.IdInmueble)
            {
                return BadRequest(); // Retorna un error 400 si los IDs no coinciden
            }

            if (ModelState.IsValid)
            {
                _inmuebleRepository.Update(inmueble);
                return RedirectToAction("Listar"); // Redirige a la lista de inmuebles
            }
            return View(inmueble);
        }

        // Acción para suspender el inmueble
        [HttpPost]
        public IActionResult BajaLogica(int id)
        {
            var inmueble = _inmuebleRepository.GetById(id);
            if (inmueble == null)
            {
                return NotFound(); // Retorna un error 404 si no se encuentra el inmueble
            }

            _inmuebleRepository.bajaLogica(inmueble);

            return RedirectToAction("Listar"); // Redirige a la lista de inmuebles
        }
        // Acción para activar el inmueble
        [HttpPost]
        public IActionResult AltaLogica(int id)
        {
            var inmueble = _inmuebleRepository.GetById(id);
            if (inmueble == null)
            {
                return NotFound(); // Retorna un error 404 si no se encuentra el inmueble
            }

            _inmuebleRepository.altaLogica(inmueble);

            return RedirectToAction("Listar"); // Redirige a la lista de inmuebles
        }

        // Acción para mostrar la vista de confirmación de eliminación
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
        public IActionResult DeleteConfirmed(int id)
        {
            _inmuebleRepository.Delete(id);
            return RedirectToAction("Listar"); // Redirige a la lista de inmuebles
        }
    }
}