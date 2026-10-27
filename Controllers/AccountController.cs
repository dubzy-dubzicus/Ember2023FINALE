using Microsoft.AspNetCore.Mvc;
using Ember2023.Classes;
using Ember2023.Classes.DBs;
using Ember2023.Classes.DBs.DBClasses;
using Ember2023.Auth;

namespace Ember2023.Controllers
{
    [ApiController]
    [Route("/acc")]
    public class AccountController : ControllerBase
    {
        [HttpGet("account/bulk")]
        public IActionResult GetAccountsBulk([FromQuery] List<long> id)
        {
            return Ok(PlayerDB.GetAccountsBulk(id));
        }
        
        [HttpGet("account/me")]
        public IActionResult GetAccountMe()
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null) 
            	return Unauthorized();

            var account = PlayerDB.GetAccountMe(id.Value);
            return account != null ? Ok(account) : NotFound();
        }
    }
}