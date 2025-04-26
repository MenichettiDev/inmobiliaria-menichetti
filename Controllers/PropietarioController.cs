using Microsoft.AspNetCore.Mvc;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace InmobiliariaApp.Controllers
{
    public class PropietarioController : Controller
    {
        private readonly PropietarioRepository _propietarioRepository;

        public PropietarioController(PropietarioRepository propietarioRepository)
        {
            _propietarioRepository = propietarioRepository;
        }


        // Acción para listar todos los propietarios
        public IActionResult Listar(string dni, string apellido, string nombre, int pagina = 1, int tamanioPagina = 10)
        {
            // Obtener el total de registros con los filtros
            int totalRegistros = _propietarioRepository.Contar(dni, apellido, nombre);

            // Calcular el offset para la paginación
            int offset = (pagina - 1) * tamanioPagina;

            // Obtener los propietarios paginados
            var propietarios = _propietarioRepository.Buscar(dni, apellido, nombre, offset, tamanioPagina);

            // Calcular el número total de páginas
            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling(totalRegistros / (double)tamanioPagina);
            ViewBag.Dni = dni;
            ViewBag.Apellido = apellido;
            ViewBag.Nombre = nombre;

            return View(propietarios);
        }


        // Acción para mostrar detalles de un propietario
        public IActionResult Detalles(int id)
        {

            var propietario = _propietarioRepository.GetById(id);
            if (propietario == null)
            {
                return NotFound(); // Retorna un error 404 si no se encuentra el propietario
            }
            return View(propietario);
        }

        // Acción para mostrar el formulario de creación
        public IActionResult Insertar()
        {

            return View();
        }

        // Acción para procesar el formulario de creación
        [HttpPost]
        public IActionResult Insertar(Propietario propietario)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    _propietarioRepository.Add(propietario);
                    TempData["SuccessMessage"] = "Propietario cargado correctamente.";
                    return RedirectToAction("Listar"); // Redirige a la lista de propietarios
                }
                return View(propietario);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Ocurrió un error al crear el Propietario: {ex.Message}";

                return View("Insertar", propietario);
            }
        }

        // Acción para mostrar el formulario de edición
        public IActionResult Editar(int id)
        {

            var propietario = _propietarioRepository.GetById(id);
            if (propietario == null)
            {
                return NotFound(); // Retorna un error 404 si no se encuentra el propietario
            }
            return View(propietario);
        }

        // Acción para procesar el formulario de edición
        [HttpPost]
        public IActionResult Editar(int id, Propietario propietario)
        {

            try
            {
                if (id != propietario.IdPropietario)
                {
                    return BadRequest(); // Retorna un error 400 si los IDs no coinciden
                }

                if (ModelState.IsValid)
                {
                    _propietarioRepository.Update(propietario);
                    TempData["SuccessMessage"] = "Propietario modificado correctamente.";
                    return RedirectToAction("Listar"); // Redirige a la lista de propietarios
                }
                return View(propietario);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Ocurrió un error al modificar el Propietario: {ex.Message}";

                return View("Editar", propietario);
            }
        }

        // Acción para mostrar la vista de confirmación de eliminación
        [Authorize(Policy = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var propietario = _propietarioRepository.GetById(id);
            if (propietario == null)
            {
                return NotFound(); // Retorna un error 404 si no se encuentra el propietario
            }
            return View(propietario);
        }

        // Acción para confirmar la eliminación
        [Authorize(Policy = "Administrador")]
        [HttpPost, ActionName("Eliminar")]
        public IActionResult EliminarConfirmed(int id)
        {
            try
            {
                _propietarioRepository.Delete(id);
                TempData["SuccessMessage"] = "Propietario eliminado correctamente.";
                return RedirectToAction("Listar"); // Redirige a la lista de propietarios
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Ocurrió un error al eliminar el propietario: {ex.Message}";

                var propietario = _propietarioRepository.GetById(id); // Volver a cargar el contrato para mostrar la vista
                return View("Eliminar", propietario?.IdPropietario); // Mostramos la misma vista de confirmación con el error
            }
        }
    }
}