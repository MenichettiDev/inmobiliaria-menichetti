using Microsoft.AspNetCore.Mvc;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories;

namespace InmobiliariaApp.Controllers
{
    public class PagoController : Controller
    {
        private readonly PagoRepository _pagoRepository;
        private readonly ContratoRepository _contratoRepository;

        public PagoController(PagoRepository pagoRepository, ContratoRepository contratoRepository)
        {
            _pagoRepository = pagoRepository;
            _contratoRepository = contratoRepository;
        }
        // Acción para listar todos los pagos
        // public IActionResult Listar()
        // {
        //     var pagos = _pagoRepository.GetAll();
        //     return View(pagos);
        // }

        //Listar con filtros
        public IActionResult Listar(int? idContrato, int? idInquilino, DateTime? desde, DateTime? hasta, decimal? importeMin, decimal? importeMax)
        {
            ViewBag.Contratos = _contratoRepository.GetAll(); // para el dropdown
            var pagos = _pagoRepository.ObtenerFiltrados(idContrato, idInquilino, desde, hasta, importeMin, importeMax);
            return View(pagos);
        }
        public IActionResult ListarDesde(int id)
        {
            var pagos = _pagoRepository.ObtenerPorContrato(id);
            ViewBag.IdContrato = id;
            return View("ListarDesde", pagos);
        }


        // Acción para mostrar detalles de un pago
        public IActionResult Detalles(int id)
        {
            var pago = _pagoRepository.GetById(id);
            if (pago == null)
            {
                return NotFound(); // Retorna un error 404 si no se encuentra el pago
            }
            return View(pago);
        }

        // Acción para mostrar el formulario de creación
        // GET: Pago/Insertar/5
        public IActionResult Insertar(int id) // este id es el IdContrato
        {
            var pago = new Pago
            {
                IdContrato = id,
                FechaPago = DateTime.Today
            };

            return View("Insertar", pago); // te aseguras de que el campo se llene
        }


        // Acción para procesar el formulario de creación
        [HttpPost]
        public IActionResult Insertar(Pago pago)
        {
            if (ModelState.IsValid)
            {
                _pagoRepository.Add(pago);
                return RedirectToAction("Listar"); // Redirige a la lista de pagos
            }
            return View(pago);
        }

        // Acción para mostrar el formulario de edición
        public IActionResult Editar(int id)
        {
            var pago = _pagoRepository.GetById(id);
            if (pago == null)
            {
                return NotFound(); // Retorna un error 404 si no se encuentra el pago
            }
            return View(pago);
        }

        // Acción para procesar el formulario de edición
        [HttpPost]
        public IActionResult Editar(int id, Pago pago)
        {
            if (id != pago.IdPago)
            {
                return BadRequest(); // Retorna un error 400 si los IDs no coinciden
            }

            if (ModelState.IsValid)
            {
                _pagoRepository.Update(pago);
                return RedirectToAction("Listar"); // Redirige a la lista de pagos
            }
            return View(pago);
        }

        // Acción para mostrar la vista de confirmación de eliminación
        public IActionResult Eliminar(int id)
        {
            var pago = _pagoRepository.GetById(id);
            if (pago == null)
            {
                return NotFound(); // Retorna un error 404 si no se encuentra el pago
            }
            return View(pago);
        }

        // Acción para confirmar la eliminación
        [HttpPost, ActionName("Eliminar")]
        public IActionResult DeleteConfirmed(int id)
        {
            _pagoRepository.Delete(id);
            return RedirectToAction("Listar"); // Redirige a la lista de pagos
        }
    }
}