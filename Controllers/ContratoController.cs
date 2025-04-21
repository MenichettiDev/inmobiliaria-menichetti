using Microsoft.AspNetCore.Mvc;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using Org.BouncyCastle.Tls;

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

        public IActionResult Listar(int? idInquilino, int? idInmueble, DateTime? fechaDesde, DateTime? fechaHasta, decimal? montoDesde, decimal? montoHasta, string? estado, int? activo, int? venceEnDias)
        {
            activo = 1; // Solo activos
            ViewBag.Inquilinos = _inquilinoRepo.GetAll();
            ViewBag.Inmuebles = _inmuebleRepo.GetAll();

            var contratos = _contratoRepository.ObtenerFiltrados(idInquilino, idInmueble, fechaDesde, fechaHasta, montoDesde, montoHasta, estado, activo, venceEnDias);
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

                _contratoRepository.Add(contrato);
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
        // GET: Renovar contrato - muestra el formulario con los datos actuales
        public IActionResult Renovar(int id)
        {
            var contrato = _contratoRepository.GetById(id);

            if (contrato == null)
            {
                return NotFound(); // 404 si no existe
            }


            return View(contrato);
        }

        [HttpPost]
        public IActionResult Renovar(int id, Contrato contrato)
        {
            try
            {
                if (id != contrato.IdContrato)
                {
                    return BadRequest(); // ID inconsistente
                }
                contrato = _contratoRepository.GetById(id); // Obtener el contrato existente
                if (contrato == null)
                {
                    return NotFound(); // 404 si no existe
                }
                // Crear un nuevo contrato basado en el original, con fechas nuevas
                _contratoRepository.RenovarContrato(contrato, contrato.FechaInicio, contrato.FechaFin);
                TempData["SuccessMessage"] = "Contrato renovado correctamente.";
                return RedirectToAction("Listar");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Ocurrió un error al renovar el contrato: {ex.Message}";

                return View("Renovar", contrato); // Retorna a la vista de renovación con el contrato actual
            }
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

        // Acción para confirmar la terminación anticipada del contrato
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

                _contratoRepository.TerminarContratoAnticipadamente(id, fechaTerminacion);

                TempData["SuccessMessage"] = "Contrato finalizado anticipadamente con éxito.";
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




        // Acción para suspender el contrato
        // [HttpPost]
        // public IActionResult BajaLogica(int id)
        // {
        //     var contrato = _contratoRepository.GetById(id);
        //     if (contrato == null)
        //     {
        //         return NotFound(); // Retorna un error 404 si no se encuentra el contrato
        //     }

        //     _contratoRepository.bajaLogica(contrato);

        //     return RedirectToAction("Listar"); // Redirige a la lista de contrato
        // }
        // // Acción para activar el contrato
        // [HttpPost]
        // public IActionResult AltaLogica(int id)
        // {
        //     var contrato = _contratoRepository.GetById(id);
        //     if (contrato == null)
        //     {
        //         return NotFound(); // Retorna un error 404 si no se encuentra el contrato
        //     }

        //     _contratoRepository.altaLogica(contrato);

        //     return RedirectToAction("Listar"); // Redirige a la lista de contrato
        // }


    }
}