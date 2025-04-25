using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using inmobiliaria_menichetti.Models;
using InmobiliariaApp.Repositories;

namespace inmobiliaria_menichetti.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly InmuebleRepository _inmuebleRepository;
    private readonly PropietarioRepository _propietarioRepository;
    private readonly TipoInmuebleRepository _tipoInmuebleRepository;
    private readonly ContratoRepository _contratoRepository;

    public HomeController(ILogger<HomeController> logger, InmuebleRepository inmuebleRepository, PropietarioRepository propietarioRepository, TipoInmuebleRepository tipoInmuebleRepository, ContratoRepository contratoRepository)
    {
        _inmuebleRepository = inmuebleRepository;
        _propietarioRepository = propietarioRepository;
        _tipoInmuebleRepository = tipoInmuebleRepository;
        _contratoRepository = contratoRepository;
        _logger = logger;
    }



    public IActionResult Index(string? uso, int? ambientes, decimal? precioDesde, decimal? precioHasta,
    string estado, int? activo, DateTime? fechaDesde, DateTime? fechaHasta, int page = 1, int pageSize = 10)
    {
        if (!Request.Query.ContainsKey("activo"))
            activo = 1;

            if (!Request.Query.ContainsKey("estado"))
                estado = "Disponible";

        var inmuebles = _inmuebleRepository.IndexFiltrados(uso, ambientes, precioDesde, precioHasta,
            estado, activo, fechaDesde, fechaHasta, page, pageSize, out int totalItems);

        int totalPaginas = (int)Math.Ceiling((double)totalItems / pageSize);

        ViewBag.PaginaActual = page;
        ViewBag.TotalPaginas = totalPaginas;

        return View(inmuebles);
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult Restringido()
    {
        return View();
    }
}
