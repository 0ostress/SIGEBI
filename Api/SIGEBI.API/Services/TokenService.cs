using SIGEBI.Domain.Entities;

namespace SIGEBI.API.Services
{
    public class TokenService
    {
        public string GenerarToken(Usuario usuario)
        {
            return $"{usuario.Id}|{usuario.Email}|{usuario.Rol}";
        }
    }
}