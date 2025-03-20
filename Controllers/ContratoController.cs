using Microsoft.AspNetCore.Mvc;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories;

namespace InmobiliariaApp.Controllers
{
    public class ContratoController : Controller
    {
        private readonly ContratoRepository _contratoRepository;

        public ContratoController(ContratoRepository contratoRepository)
        {
            _contratoRepository = contratoRepository;
        }

        // Acción para listar todos los contratos
        public IActionResult Index()
        {
            var contratos = _contratoRepository.GetAll();
            return View(contratos);
        }

        // Acción para mostrar detalles de un contrato
        public IActionResult Details(int id)
        {
            var contrato = _contratoRepository.GetById(id);
            if (contrato == null)
            {
                return NotFound(); // Retorna un error 404 si no se encuentra el contrato
            }
            return View(contrato);
        }

        // Acción para mostrar el formulario de creación
        public IActionResult Create()
        {
            return View();
        }

        // Acción para procesar el formulario de creación
        [HttpPost]
        public IActionResult Create(Contrato contrato)
        {
            if (ModelState.IsValid)
            {
                _contratoRepository.Add(contrato);
                return RedirectToAction("Index"); // Redirige a la lista de contratos
            }
            return View(contrato);
        }

        // Acción para mostrar el formulario de edición
        public IActionResult Edit(int id)
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
        public IActionResult Edit(int id, Contrato contrato)
        {
            if (id != contrato.IdContrato)
            {
                return BadRequest(); // Retorna un error 400 si los IDs no coinciden
            }

            if (ModelState.IsValid)
            {
                _contratoRepository.Update(contrato);
                return RedirectToAction("Index"); // Redirige a la lista de contratos
            }
            return View(contrato);
        }

        // Acción para mostrar la vista de confirmación de eliminación
        public IActionResult Delete(int id)
        {
            var contrato = _contratoRepository.GetById(id);
            if (contrato == null)
            {
                return NotFound(); // Retorna un error 404 si no se encuentra el contrato
            }
            return View(contrato);
        }

        // Acción para confirmar la eliminación
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _contratoRepository.Delete(id);
            return RedirectToAction("Index"); // Redirige a la lista de contratos
        }
    }
}