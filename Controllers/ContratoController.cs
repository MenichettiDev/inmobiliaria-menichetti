using Microsoft.AspNetCore.Mvc;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using Org.BouncyCastle.Tls;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace InmobiliariaApp.Controllers
{
    [Authorize]
    public class ContratoController : Controller
    {
        private readonly ContratoRepository _contratoRepository;
        private readonly InquilinoRepository _inquilinoRepo;
        private readonly InmuebleRepository _inmuebleRepo;
        private readonly UsuarioRepository _usuarioRepo;

        public ContratoController(ContratoRepository contratoRepository, InquilinoRepository inquilinoRepo, InmuebleRepository inmuebleRepo, UsuarioRepository usuarioRepo)
        {
            _usuarioRepo = usuarioRepo;
            _inquilinoRepo = inquilinoRepo;
            _inmuebleRepo = inmuebleRepo;
            _contratoRepository = contratoRepository;
        }

        // public IActionResult Listar(int? idInquilino, int? idInmueble, DateTime? fechaDesde, DateTime? fechaHasta, decimal? montoDesde, decimal? montoHasta, string? estado, int? activo, int? venceEnDias)
        // {
        //     activo = 1; // Solo activos
        //     ViewBag.Inquilinos = _inquilinoRepo.GetAll();
        //     ViewBag.Inmuebles = _inmuebleRepo.GetAll();

        //     var contratos = _contratoRepository.ObtenerFiltrados(idInquilino, idInmueble, fechaDesde, fechaHasta, montoDesde, montoHasta, estado, activo, venceEnDias);
        //     return View(contratos);
        // }

        // Acción para mostrar detalles de un contrato

        public IActionResult Listar(
            int? idInquilino,
            int? idInmueble,
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            decimal? montoDesde,
            decimal? montoHasta,
            string? estado,
            int? activo,
            int? venceEnDias,
            int page = 1,
            int pageSize = 10)
        {
            activo = 1; // Solo activos

            if (!Request.Query.ContainsKey("estado"))
                estado = "Vigente";

            ViewBag.Inquilinos = _inquilinoRepo.GetAll();
            ViewBag.Inmuebles = _inmuebleRepo.GetAll();

            // Obtener contratos paginados desde el repositorio
            var contratosPaginados = _contratoRepository.ObtenerContratosPaginados(
                idInquilino, idInmueble, fechaDesde, fechaHasta, montoDesde, montoHasta, estado, activo, venceEnDias, page, pageSize);

            // Total de registros para calcular el número de páginas
            var totalRegistros = _contratoRepository.ObtenerTotalContratos(
                idInquilino, idInmueble, fechaDesde, fechaHasta, montoDesde, montoHasta, estado, activo, venceEnDias);

            ViewBag.TotalPaginas = (int)Math.Ceiling(totalRegistros / (double)pageSize);
            ViewBag.PaginaActual = page;

            return View(contratosPaginados);
        }

        public IActionResult Detalles(int id)
        {
            var contrato = _contratoRepository.GetById(id);
            if (contrato == null)
            {
                return NotFound(); // Retorna un error 404 si no se encuentra el contrato
            }
            // Resolver nombres de auditoría
            ViewBag.CreadoPorNombre = ObtenerNombreUsuario(contrato.CreadoPor);
            ViewBag.ModificadoPorNombre = ObtenerNombreUsuario(contrato.ModificadoPor);
            ViewBag.EliminadoPorNombre = ObtenerNombreUsuario(contrato.EliminadoPor);

            return View(contrato);
        }

        // Acción para mostrar el formulario de creación
        public IActionResult Insertar(int? id)
        {
            var inquilinos = _inquilinoRepo.GetAll();
            ViewBag.Inquilinos = new SelectList(inquilinos, "IdInquilino", "NombreCompleto");
            var inmuebles = _inmuebleRepo.GetAll();
            ViewBag.IdSeleccionado = id;
            ViewBag.Inmuebles = new SelectList(inmuebles, "IdInmueble", "NombreInmueble", id);


            return View();
        }

        [HttpPost]
        public IActionResult Insertar(Contrato contrato)
        {
            try
            {
                if (contrato.FechaInicio.Date < DateTime.Now.Date)
                {
                    ViewBag.ErrorMessage = "La fecha de inicio no puede ser anterior al día de hoy.";
                }
                if (!ModelState.IsValid)
                {
                    // Si el modelo no es válido, volvemos a mostrar el formulario con los datos actuales
                    var inquilinos = _inquilinoRepo.GetAll();
                    ViewBag.Inquilinos = new SelectList(inquilinos, "IdInquilino", "NombreCompleto");
                    var inmuebles = _inmuebleRepo.GetAll();
                    ViewBag.Inmuebles = new SelectList(inmuebles, "IdInmueble", "NombreInmueble");

                    ViewBag.ErrorMessage = "Por favor complete correctamente todos los campos del formulario.";

                    return View(contrato);
                }

                //Claim para obtener el ID del usuario autenticado
                var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(idClaim))
                {
                    return Unauthorized(); // o manejarlo como prefieras
                }
                int userId = int.Parse(idClaim);

                _contratoRepository.Add(contrato, userId);
                TempData["SuccessMessage"] = "Contrato creado correctamente.";
                return RedirectToAction("Listar");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Ocurrió un error al crear el contrato: {ex.Message}";

                var inquilinos = _inquilinoRepo.GetAll();
                ViewBag.Inquilinos = new SelectList(inquilinos, "IdInquilino", "NombreCompleto");
                var inmuebles = _inmuebleRepo.GetAll();
                ViewBag.Inmuebles = new SelectList(inmuebles, "IdInmueble", "NombreInmueble");

                return View("Insertar", contrato);
            }
        }


        // GET: Renovar contrato - muestra el formulario con los datos actuales
        // public IActionResult Renovar(int id)
        // {
        //     var contrato = _contratoRepository.GetById(id);

        //     if (contrato == null)
        //     {
        //         return NotFound(); // 404 si no existe
        //     }


        //     return View(contrato);
        // }

        // [HttpPost]
        // public IActionResult Renovar(int id, Contrato contrato)
        // {
        //     try
        //     {
        //         if (id != contrato.IdContrato)
        //         {
        //             return BadRequest();
        //         }

        //         var nuevaFechaInicio = contrato.FechaInicio;
        //         var nuevaFechaFin = contrato.FechaFin;

        //         var contratoExistente = _contratoRepository.GetById(id);
        //         if (contratoExistente == null)
        //         {
        //             return NotFound();
        //         }

        //         var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //         if (string.IsNullOrEmpty(idClaim))
        //         {
        //             return Unauthorized();
        //         }

        //         int userId = int.Parse(idClaim);

        //         _contratoRepository.RenovarContrato(contratoExistente, nuevaFechaInicio, nuevaFechaFin, userId);

        //         TempData["SuccessMessage"] = "Contrato renovado correctamente.";
        //         return RedirectToAction("Listar");
        //     }
        //     catch (Exception ex)
        //     {
        //         ViewBag.ErrorMessage = $"Ocurrió un error al renovar el contrato: {ex.Message}";

        //         return View("Renovar", contrato);
        //     }
        // }



        // Acción para mostrar la vista de confirmación de eliminación

        [Authorize(Policy = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var contrato = _contratoRepository.GetById(id);
            if (contrato == null)
            {
                return NotFound(); // Retorna un error 404 si no se encuentra el contrato
            }
            return View(contrato);
        }

        // Acción para confirmar la terminación anticipada del contrato
        [Authorize(Policy = "Administrador")]
        [HttpPost, ActionName("FinalizarAnticipadamente")]
        public IActionResult FinalizarAnticipadamenteConfirmed(int id, DateTime fechaTerminacion)
        {
            try
            {
                var contrato = _contratoRepository.GetById(id);
                if (contrato == null)
                {
                    return NotFound(); // Si el contrato no se encuentra
                }

                //Claim para obtener el ID del usuario autenticado
                var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(idClaim))
                {
                    return Unauthorized(); 
                }
                int userId = int.Parse(idClaim);

                _contratoRepository.TerminarContratoAnticipadamente(id, fechaTerminacion, userId);

                TempData["SuccessMessage"] = "Contrato cancelado anticipadamente con éxito.";
                return RedirectToAction("Listar");
            }
            catch (Exception ex)
            {
                // Captura errores y los muestra en la vista
                ViewBag.ErrorMessage = $"Ocurrió un error al finalizar anticipadamente el contrato: {ex.Message}";

                var contrato = _contratoRepository.GetById(id); // Volver a cargar el contrato para mostrar la vista
                return View("Eliminar", contrato); // Mostramos la misma vista de confirmación con el error
            }
        }


        private string ObtenerNombreUsuario(int? idUsuario)
        {
            if (!idUsuario.HasValue) return "---";

            var usuario = _usuarioRepo.GetById(idUsuario.Value);
            return usuario != null ? $"{usuario.Email}" : "Desconocido";
        }


    }
}