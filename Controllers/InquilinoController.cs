using Microsoft.AspNetCore.Mvc;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories;

namespace InmobiliariaApp.Controllers
{
    public class InquilinoController : Controller
    {
        private readonly InquilinoRepository _inquilinoRepository;

        public InquilinoController(InquilinoRepository inquilinoRepository)
        {
            _inquilinoRepository = inquilinoRepository;
        }

        // Acción para listar todos los inquilinos
        public IActionResult Listar()
        {
            var inquilinos = _inquilinoRepository.GetAll();
            return View(inquilinos);
        }

        // Acción para mostrar detalles de un inquilino
        public IActionResult Detalles(int id)
        {
            var inquilino = _inquilinoRepository.GetById(id);
            if (inquilino == null)
            {
                return NotFound(); // Retorna un error 404 si no se encuentra el inquilino
            }
            return View(inquilino);
        }

        // Acción para mostrar el formulario de creación
        public IActionResult Insertar()
        {
            return View();
        }

        // Acción para procesar el formulario de creación
        [HttpPost]
        public IActionResult Insertar(Inquilino inquilino)
        {
            if (ModelState.IsValid)
            {
                _inquilinoRepository.Add(inquilino);
                return RedirectToAction("Listar"); // Redirige a la lista de inquilinos
            }
            return View(inquilino);
        }

        // Acción para mostrar el formulario de edición
        public IActionResult Editar(int id)
        {
            var inquilino = _inquilinoRepository.GetById(id);
            if (inquilino == null)
            {
                return NotFound(); // Retorna un error 404 si no se encuentra el inquilino
            }
            return View(inquilino);
        }

        // Acción para procesar el formulario de edición
        [HttpPost]
        public IActionResult Editar(int id, Inquilino inquilino)
        {
            if (id != inquilino.IdInquilino)
            {
                return BadRequest(); // Retorna un error 400 si los IDs no coinciden
            }

            if (ModelState.IsValid)
            {
                _inquilinoRepository.Update(inquilino);
                return RedirectToAction("Listar"); // Redirige a la lista de inquilinos
            }
            return View(inquilino);
        }

        // Acción para mostrar la vista de confirmación de eliminación
        public IActionResult Eliminar(int id)
        {
            var inquilino = _inquilinoRepository.GetById(id);
            if (inquilino == null)
            {
                return NotFound(); // Retorna un error 404 si no se encuentra el inquilino
            }
            return View(inquilino);
        }

        // Acción para confirmar la eliminación
        [HttpPost, ActionName("Eliminar")]
        public IActionResult DeleteConfirmed(int id)
        {
            _inquilinoRepository.Delete(id);
            return RedirectToAction("Listar"); // Redirige a la lista de inquilinos
        }
    }
}