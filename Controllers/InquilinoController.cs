using Microsoft.AspNetCore.Mvc;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories;
using Microsoft.AspNetCore.Authorization;

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
        public IActionResult Listar(string? dni, string? apellido, string? email, int page = 1, int pageSize = 10)
        {
            var inquilinos = _inquilinoRepository.ObtenerFiltrados(dni, apellido, email, page, pageSize, out int totalItems);

            int totalPaginas = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.PaginaActual = page;
            ViewBag.TotalPaginas = totalPaginas;

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
            try
            {
                if (ModelState.IsValid)
                {
                    _inquilinoRepository.Add(inquilino);
                    TempData["SuccessMessage"] = "Inquilino creado correctamente.";
                    return RedirectToAction("Listar"); // Redirige a la lista de inquilinos
                }
                return View(inquilino);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Ocurrió un error al crear el Inquilino: {ex.Message}";


                return View("Insertar", inquilino);
            }
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
            try
            {
                if (id != inquilino.IdInquilino)
                {
                    return BadRequest(); // Retorna un error 400 si los IDs no coinciden
                }

                if (ModelState.IsValid)
                {
                    _inquilinoRepository.Update(inquilino);
                    TempData["SuccessMessage"] = "Inquilino modificado correctamente.";

                    return RedirectToAction("Listar"); // Redirige a la lista de inquilinos
                }
                return View(inquilino);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Ocurrió un error al modificar el Inquilino: {ex.Message}";

                return View("Editar", inquilino);
            }
        }

        // Acción para mostrar la vista de confirmación de eliminación
        [Authorize(Policy = "Administrador")]
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
        [Authorize(Policy = "Administrador")]
        [HttpPost, ActionName("Eliminar")]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _inquilinoRepository.Delete(id);
                TempData["SuccessMessage"] = "Inquilino eliminado correctamente.";

                return RedirectToAction("Listar"); // Redirige a la lista de inquilinos
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Ocurrió un error al eliminar el Inquilino: {ex.Message}";

                var inquilino = _inquilinoRepository.GetById(id); // Volver a cargar el contrato para mostrar la vista
                return View("Eliminar", inquilino); // Mostramos la misma vista de confirmación con el error
            }
        }
    }
}