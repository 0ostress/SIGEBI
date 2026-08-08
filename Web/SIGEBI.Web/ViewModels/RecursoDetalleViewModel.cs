using SIGEBI.Business.DTOs;
using SIGEBI.Web.Services;

namespace SIGEBI.Web.ViewModels
{
    public class RecursoDetalleViewModel
    {
        public RecursoDTO Recurso { get; set; }
        public IEnumerable<ResenaDTO> Resenas { get; set; }
        public double PromedioEstrellas { get; set; }
    }
}