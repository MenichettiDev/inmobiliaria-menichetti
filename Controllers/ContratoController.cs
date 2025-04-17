using Microsoft.AspNetCore.Mvc;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories;

namespace InmobiliariaApp.Controllers
{
    public class ContratoController : Controller
    {
        private readonly ContratoRepository _contratoRepository;
        private readonly InquilinoRepository _inquilinoRepo;
        private readonly InmuebleRepository _inmuebleRepo;

        public ContratoController(ContratoRepository contratoRepository, InquilinoRepository inquilinoRepo, InmuebleRepository inmuebleRepo)
        {
            _inquilinoRepo = inquilinoRepo;
            _inmuebleRepo = inmuebleRepo;
            _contratoRepository = contratoRepository;
        }

        // Acción para listar todos los contratos
        // public IActionResult Listar()
        // {
        //     var contratos = _contratoRepository.GetAll();
        //     return View(contratos);
        // }

        public IActionResult Listar(int? idInquilino, int? idInmueble, DateTime? fechaDesde, DateTime? fechaHasta, decimal? montoDesde, decimal? montoHasta, string? estado, int? activo)
        {
            ViewBag.Inquilinos = _inquilinoRepo.GetAll();
            ViewBag.Inmuebles = _inmuebleRepo.GetAll();

            var contratos = _contratoRepository.ObtenerFiltrados(idInquilino, idInmueble, fechaDesde, fechaHasta, montoDesde, montoHasta, estado, activo);
            return View(contratos);
        }
        // Acción para mostrar detalles de un contrato
        public IActionResult Detalles(int id)
        {
            var contrato = _contratoRepository.GetById(id);
            if (contrato == null)
            {
                return NotFound(); // Retorna un error 404 si no se encuentra el contrato
            }
            return View(contrato);
        }

        // Acción para mostrar el formulario de creación
        public IActionResult Insertar()
        {
            return View();
        }

        // Acción para procesar el formulario de creación
        [HttpPost]
        public IActionResult Insertar(Contrato contrato)
        {
            if (ModelState.IsValid)
            {
                _contratoRepository.Add(contrato);
                return RedirectToAction("Index"); // Redirige a la lista de contratos
            }
            return View(contrato);
        }

        // Acción para mostrar el formulario de edición
        public IActionResult Editar(int id)
        {
            var contrato = _contratoRepository.GetById(id);
            if (contrato == null)
            {
                return NotFound(); // Retorna un error 404 si no se encuentra el contrato
            }
            return View(contrato);
        }

        // Acción para procesar el formulario de edición
        [HttpPost]
        public IActionResult Editar(int id, Contrato contrato)
        {
            if (id != contrato.IdContrato)
            {
                return BadRequest(); // Retorna un error 400 si los IDs no coinciden
            }

            if (ModelState.IsValid)
            {
                _contratoRepository.Update(contrato);
                return RedirectToAction("Listar"); // Redirige a la lista de contratos
            }
            return View(contrato);
        }

        // Acción para suspender el inmueble
        [HttpPost]
        public IActionResult BajaLogica(int id)
        {
            var inmueble = _contratoRepository.GetById(id);
            if (inmueble == null)
            {
                return NotFound(); // Retorna un error 404 si no se encuentra el inmueble
            }

            _contratoRepository.bajaLogica(inmueble);

            return RedirectToAction("Listar"); // Redirige a la lista de inmuebles
        }
        // Acción para activar el inmueble
        [HttpPost]
        public IActionResult AltaLogica(int id)
        {
            var inmueble = _contratoRepository.GetById(id);
            if (inmueble == null)
            {
                return NotFound(); // Retorna un error 404 si no se encuentra el inmueble
            }

            _contratoRepository.altaLogica(inmueble);

            return RedirectToAction("Listar"); // Redirige a la lista de inmuebles
        }

        // Acción para mostrar la vista de confirmación de eliminación
        public IActionResult Eliminar(int id)
        {
            var contrato = _contratoRepository.GetById(id);
            if (contrato == null)
            {
                return NotFound(); // Retorna un error 404 si no se encuentra el contrato
            }
            return View(contrato);
        }

        // Acción para confirmar la eliminación
        [HttpPost, ActionName("Eliminar")]
        public IActionResult DeleteConfirmed(int id)
        {
            _contratoRepository.Delete(id);
            return RedirectToAction("Listar"); // Redirige a la lista de contratos
        }
    }
}