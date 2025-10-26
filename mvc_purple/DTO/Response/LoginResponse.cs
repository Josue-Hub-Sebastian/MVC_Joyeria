using mvc_purple.Models;

namespace mvc_purple.DTO.Response
{
    public class LoginResponse
    {
        public string? Token { get; set; }
        public Cliente? Cliente { get; set; }
    }
}
