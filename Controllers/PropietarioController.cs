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
            // if (!UsuarioAutenticado())
            // {
            //     return RedirectToAction("Index", "Login");
            // }

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
            // if (!UsuarioAutenticado())
            // {
            //     return RedirectToAction("Index", "Login");
            // }

            return View();
        }

        // Acción para procesar el formulario de creación
        [HttpPost]
        public IActionResult Insertar(Propietario propietario)
        {
            // if (!UsuarioAutenticado())
            // {
            //     return RedirectToAction("Index", "Login");
            // }

            if (ModelState.IsValid)
            {
                _propietarioRepository.Add(propietario);
                return RedirectToAction("Listar"); // Redirige a la lista de propietarios
            }
            return View(propietario);
        }

        // Acción para mostrar el formulario de edición
        public IActionResult Editar(int id)
        {
            // if (!UsuarioAutenticado())
            // {
            //     return RedirectToAction("Index", "Login");
            // }

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
            // if (!UsuarioAutenticado())
            // {
            //     return RedirectToAction("Index", "Login");
            // }

            if (id != propietario.IdPropietario)
            {
                return BadRequest(); // Retorna un error 400 si los IDs no coinciden
            }

            if (ModelState.IsValid)
            {
                _propietarioRepository.Update(propietario);
                return RedirectToAction("Listar"); // Redirige a la lista de propietarios
            }
            return View(propietario);
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
            _propietarioRepository.Delete(id);
            return RedirectToAction("Listar"); // Redirige a la lista de propietarios
        }
    }
}