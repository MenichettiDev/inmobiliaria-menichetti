using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_.Net_Core.Models
{
	public class Imagen
	{
		public int Id { get; set; }
		public int InmuebleId { get; set; }
		public string Url { get; set; } = "";
		public IFormFile? Archivo { get; set; } = null;
        //Este campo no se guarda en la base de datos, sino que se usa
        //transitoriamente para recibir el archivo subido desde el formulario web. 
	}
}