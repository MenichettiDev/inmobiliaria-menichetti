using Microsoft.AspNetCore.Mvc;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace InmobiliariaApp.Controllers
{
    public class PagoController : Controller
    {
        private readonly PagoRepository _pagoRepository;
        private readonly ContratoRepository _contratoRepository;
        private readonly UsuarioRepository _usuarioRepo;

        public PagoController(PagoRepository pagoRepository, ContratoRepository contratoRepository, UsuarioRepository usuarioRepo)
        {
            _usuarioRepo = usuarioRepo;
            _pagoRepository = pagoRepository;
            _contratoRepository = contratoRepository;
        }


        // Listar con filtros y paginación
        public IActionResult Listar(int? idContrato, int? idInquilino, DateTime? desde, DateTime? hasta, decimal? importeMin, decimal? importeMax, string estado, int page = 1, int pageSize = 10)
        {
            ViewBag.Contratos = _contratoRepository.GetAll();
            ViewBag.EstadoSeleccionado = estado; // para mostrar el seleccionado en el HTML

            // Obtener los pagos paginados desde el repositorio
            var pagos = _pagoRepository.ObtenerFiltradosPaginados(idContrato, idInquilino, desde, hasta, importeMin, importeMax, estado, page, pageSize);

            // Total de registros para calcular el número de páginas
            var totalPagos = _pagoRepository.ObtenerTotalPagos(idContrato, idInquilino, desde, hasta, importeMin, importeMax, estado);

            ViewBag.TotalPaginas = (int)Math.Ceiling(totalPagos / (double)pageSize);
            ViewBag.PaginaActual = page;

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

            ViewBag.CreadoPorNombre = ObtenerNombreUsuario(pago.CreadoPor);
            ViewBag.ModificadoPorNombre = ObtenerNombreUsuario(pago.ModificadoPor);
            ViewBag.EliminadoPorNombre = ObtenerNombreUsuario(pago.EliminadoPor);

            return View(pago);
        }

        public IActionResult PagoCuota(int id)
        {
            var pago = _pagoRepository.GetById(id);

            if (pago == null)
            {
                return NotFound("Pago no encontrado.");
            }

            return View(pago);
        }

        [HttpPost]
        public IActionResult PagoCuota(int id, Pago pago)
        {
            if (!ModelState.IsValid)
            {
                return View("PagoCuota", pago); // vuelve al form con validaciones
            }

            pago.Estado = "Pagado";

            if (ModelState.IsValid)
            {

                _pagoRepository.Update(pago);

                ViewData["Mensaje"] = "Pago realizado con éxito.";
                ViewData["TipoMensaje"] = "success"; // o "error" según corresponda
                return RedirectToAction("Listar"); // Redirige a la lista de pagos
            }
            return View(pago);
        }

        public IActionResult Insertar()
        {
            var contratos = _contratoRepository.GetAll(); // método que deberías tener

            // Creamos una lista de objetos anónimos con Id y Descripción
            var items = contratos.Select(c => new
            {
                Id = c.IdContrato,
                Descripcion = $"#{c.IdContrato} - {c.Inmueble?.NombreInmueble} ({c.Inmueble?.Direccion})"
            });

            ViewBag.Contratos = new SelectList(items, "Id", "Descripcion");

            var pago = new Pago
            {
                FechaPago = DateTime.Today,
                Estado = "Pagado"
            };

            return View("Insertar", pago);
        }

        // Acción para procesar el formulario de creación
        [HttpPost]
        public IActionResult Insertar(Pago pago)
        {
            if (ModelState.IsValid)
            {
                pago.FechaPago = DateTime.Today;
                pago.Estado = "Pagado";

                _pagoRepository.Add(pago);
                return RedirectToAction("Listar");
            }

            // volver a cargar los contratos si falla el modelo
            var contratos = _contratoRepository.GetAll();
            ViewBag.Contratos = new SelectList(contratos, "IdContrato", "Descripcion");
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
        [Authorize(Policy = "Administrador")]
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
        [Authorize(Policy = "Administrador")]
        [HttpPost, ActionName("Eliminar")]
        public IActionResult DeleteConfirmed(int id)
        {

            _pagoRepository.Delete(id);
            return RedirectToAction("Listar"); // Redirige a la lista de pagos
        }

        private string ObtenerNombreUsuario(int? idUsuario)
        {
            if (!idUsuario.HasValue) return "---";

            var usuario = _usuarioRepo.GetById(idUsuario.Value);
            return usuario != null ? $"{usuario.Email}" : "Desconocido";
        }
    }
}