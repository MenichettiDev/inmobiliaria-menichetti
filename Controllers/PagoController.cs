using Microsoft.AspNetCore.Mvc;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories;

namespace InmobiliariaApp.Controllers
{
    public class PagoController : Controller
    {
        private readonly PagoRepository _pagoRepository;

        public PagoController(PagoRepository pagoRepository)
        {
            _pagoRepository = pagoRepository;
        }

        // Acción para listar todos los pagos
        public IActionResult Listar()
        {
            var pagos = _pagoRepository.GetAll();
            return View(pagos);
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
        public IActionResult Insertar()
        {
            return View();
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